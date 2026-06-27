using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace TNet
{
	public class TcpProtocol : Player
	{
		public enum Stage
		{
			NotConnected = 0,
			Connecting = 1,
			Verifying = 2,
			Connected = 3
		}

		public Stage stage;

		public IPEndPoint tcpEndPoint;

		public long lastReceivedTime;

		public long timeoutTime = 10000L;

		private Queue<Buffer> mIn = new Queue<Buffer>();

		private Queue<Buffer> mOut = new Queue<Buffer>();

		private byte[] mTemp = new byte[8192];

		private Buffer mReceiveBuffer;

		private int mExpected;

		private int mOffset;

		private Socket mSocket;

		private bool mNoDelay;

		private IPEndPoint mFallback;

		private List<Socket> mConnecting = new List<Socket>();

		private static Buffer mBuffer;

		public bool isConnected
		{
			get
			{
				return stage == Stage.Connected;
			}
		}

		public bool isTryingToConnect
		{
			get
			{
				return mConnecting.size != 0;
			}
		}

		public bool noDelay
		{
			get
			{
				return mNoDelay;
			}
			set
			{
				if (mNoDelay != value)
				{
					mNoDelay = value;
					mSocket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.Debug, mNoDelay);
				}
			}
		}

		public string address
		{
			get
			{
				return (tcpEndPoint == null) ? "0.0.0.0:0" : tcpEndPoint.ToString();
			}
		}

		public void Connect(IPEndPoint externalIP)
		{
			Connect(externalIP, null);
		}

		public void Connect(IPEndPoint externalIP, IPEndPoint internalIP)
		{
			Disconnect();
			if (internalIP != null && Tools.GetSubnet(Tools.localAddress) == Tools.GetSubnet(internalIP.Address))
			{
				tcpEndPoint = internalIP;
				mFallback = externalIP;
			}
			else
			{
				tcpEndPoint = externalIP;
				mFallback = internalIP;
			}
			ConnectToTcpEndPoint();
		}

		private bool ConnectToTcpEndPoint()
		{
			if (tcpEndPoint != null)
			{
				stage = Stage.Connecting;
				try
				{
					lock (mConnecting)
					{
						mSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
						mConnecting.Add(mSocket);
					}
					IAsyncResult parameter = mSocket.BeginConnect(tcpEndPoint, OnConnectResult, mSocket);
					Thread thread = new Thread(CancelConnect);
					thread.Start(parameter);
					return true;
				}
				catch (Exception ex)
				{
					Error(ex.Message);
				}
			}
			else
			{
				Error("Unable to resolve the specified address");
			}
			return false;
		}

		private bool ConnectToFallback()
		{
			tcpEndPoint = mFallback;
			mFallback = null;
			return tcpEndPoint != null && ConnectToTcpEndPoint();
		}

		private void CancelConnect(object obj)
		{
			IAsyncResult asyncResult = (IAsyncResult)obj;
			if (asyncResult == null || asyncResult.AsyncWaitHandle.WaitOne(3000, true))
			{
				return;
			}
			try
			{
				Socket socket = (Socket)asyncResult.AsyncState;
				if (socket == null)
				{
					return;
				}
				socket.Close();
				lock (mConnecting)
				{
					if (mConnecting.size > 0 && mConnecting[mConnecting.size - 1] == socket)
					{
						mSocket = null;
						if (!ConnectToFallback())
						{
							Error("Unable to connect");
							Close(false);
						}
					}
					mConnecting.Remove(socket);
				}
			}
			catch (Exception)
			{
			}
		}

		private void OnConnectResult(IAsyncResult result)
		{
			Socket socket = (Socket)result.AsyncState;
			if (socket == null)
			{
				return;
			}
			if (mSocket != null && socket == mSocket)
			{
				bool flag = true;
				string error = "Failed to connect";
				try
				{
					socket.EndConnect(result);
				}
				catch (Exception ex)
				{
					if (socket == mSocket)
					{
						mSocket = null;
					}
					socket.Close();
					error = ex.Message;
					flag = false;
				}
				if (flag)
				{
					stage = Stage.Verifying;
					BinaryWriter binaryWriter = BeginSend(Packet.RequestID);
					binaryWriter.Write(12);
					binaryWriter.Write((!string.IsNullOrEmpty(name)) ? name : "Guest");
					binaryWriter.WriteObject(data);
					EndSend();
					StartReceiving();
				}
				else if (!ConnectToFallback())
				{
					Error(error);
					Close(false);
				}
			}
			lock (mConnecting)
			{
				mConnecting.Remove(socket);
			}
		}

		public void Disconnect()
		{
			Disconnect(false);
		}

		public void Disconnect(bool notify)
		{
			if (!isConnected)
			{
				return;
			}
			try
			{
				lock (mConnecting)
				{
					int num = mConnecting.size;
					while (num > 0)
					{
						Socket socket = mConnecting[--num];
						mConnecting.RemoveAt(num);
						if (socket != null)
						{
							socket.Close();
						}
					}
				}
				if (mSocket != null)
				{
					Close(notify || mSocket.Connected);
				}
			}
			catch (Exception)
			{
				lock (mConnecting)
				{
					mConnecting.Clear();
				}
				mSocket = null;
			}
		}

		public void Close(bool notify)
		{
			stage = Stage.NotConnected;
			name = "Guest";
			data = null;
			if (mReceiveBuffer != null)
			{
				mReceiveBuffer.Recycle();
				mReceiveBuffer = null;
			}
			if (mSocket == null)
			{
				return;
			}
			try
			{
				if (mSocket.Connected)
				{
					mSocket.Shutdown(SocketShutdown.Both);
				}
				mSocket.Close();
			}
			catch (Exception)
			{
			}
			mSocket = null;
			if (notify)
			{
				Buffer buffer = Buffer.Create();
				buffer.BeginPacket(Packet.Disconnect);
				buffer.EndTcpPacketWithOffset(4);
				lock (mIn)
				{
					mIn.Enqueue(buffer);
				}
			}
		}

		public void Release()
		{
			Close(false);
			Buffer.Recycle(mIn);
			Buffer.Recycle(mOut);
		}

		public BinaryWriter BeginSend(Packet type)
		{
			mBuffer = Buffer.Create(false);
			return mBuffer.BeginPacket(type);
		}

		public BinaryWriter BeginSend(byte packetID)
		{
			mBuffer = Buffer.Create(false);
			return mBuffer.BeginPacket(packetID);
		}

		public void EndSend()
		{
			mBuffer.EndPacket();
			SendTcpPacket(mBuffer);
			mBuffer = null;
		}

		public void SendTcpPacket(Buffer buffer)
		{
			buffer.MarkAsUsed();
			if (mSocket != null && mSocket.Connected)
			{
				buffer.BeginReading();
				lock (mOut)
				{
					mOut.Enqueue(buffer);
					if (mOut.Count == 1)
					{
						try
						{
							mSocket.BeginSend(buffer.buffer, buffer.position, buffer.size, SocketFlags.None, OnSend, buffer);
							return;
						}
						catch (Exception ex)
						{
							Error(ex.Message);
							Close(false);
							Release();
							return;
						}
					}
					return;
				}
			}
			buffer.Recycle();
		}

		private void OnSend(IAsyncResult result)
		{
			if (stage == Stage.NotConnected)
			{
				return;
			}
			int num;
			try
			{
				num = mSocket.EndSend(result);
			}
			catch (Exception ex)
			{
				num = 0;
				Close(true);
				Error(ex.Message);
				return;
			}
			lock (mOut)
			{
				mOut.Dequeue().Recycle();
				if (num > 0 && mSocket != null && mSocket.Connected)
				{
					Buffer buffer = ((mOut.Count != 0) ? mOut.Peek() : null);
					if (buffer != null)
					{
						try
						{
							mSocket.BeginSend(buffer.buffer, buffer.position, buffer.size, SocketFlags.None, OnSend, buffer);
							return;
						}
						catch (Exception ex2)
						{
							Error(ex2.Message);
							Close(false);
							return;
						}
					}
				}
				else
				{
					Close(true);
				}
			}
		}

		public void StartReceiving()
		{
			StartReceiving(null);
		}

		public void StartReceiving(Socket socket)
		{
			if (socket != null)
			{
				Close(false);
				mSocket = socket;
			}
			if (mSocket != null && mSocket.Connected)
			{
				stage = Stage.Verifying;
				lastReceivedTime = DateTime.UtcNow.Ticks / 10000;
				tcpEndPoint = (IPEndPoint)mSocket.RemoteEndPoint;
				try
				{
					mSocket.BeginReceive(mTemp, 0, mTemp.Length, SocketFlags.None, OnReceive, null);
				}
				catch (Exception ex)
				{
					Error(ex.Message);
					Disconnect(true);
				}
			}
		}

		public bool ReceivePacket(out Buffer buffer)
		{
			if (mIn.Count != 0)
			{
				lock (mIn)
				{
					buffer = mIn.Dequeue();
					return true;
				}
			}
			buffer = null;
			return false;
		}

		private void OnReceive(IAsyncResult result)
		{
			if (stage == Stage.NotConnected)
			{
				return;
			}
			int num = 0;
			try
			{
				num = mSocket.EndReceive(result);
			}
			catch (Exception ex)
			{
				Error(ex.Message);
				Disconnect(true);
				return;
			}
			lastReceivedTime = DateTime.UtcNow.Ticks / 10000;
			if (num == 0)
			{
				Close(true);
			}
			else if (ProcessBuffer(num))
			{
				if (stage != Stage.NotConnected)
				{
					try
					{
						mSocket.BeginReceive(mTemp, 0, mTemp.Length, SocketFlags.None, OnReceive, null);
					}
					catch (Exception ex2)
					{
						Error(ex2.Message);
						Close(false);
					}
				}
			}
			else
			{
				Close(true);
			}
		}

		private bool ProcessBuffer(int bytes)
		{
			if (mReceiveBuffer == null)
			{
				mReceiveBuffer = Buffer.Create();
				mReceiveBuffer.BeginWriting(false).Write(mTemp, 0, bytes);
			}
			else
			{
				mReceiveBuffer.BeginWriting(true).Write(mTemp, 0, bytes);
			}
			int num = mReceiveBuffer.size - mOffset;
			while (num >= 4)
			{
				if (mExpected == 0)
				{
					mExpected = mReceiveBuffer.PeekInt(mOffset);
					if (mExpected < 0 || mExpected > 16777216)
					{
						Close(true);
						return false;
					}
				}
				num -= 4;
				if (num == mExpected)
				{
					mReceiveBuffer.BeginReading(mOffset + 4);
					lock (mIn)
					{
						mIn.Enqueue(mReceiveBuffer);
					}
					mReceiveBuffer = null;
					mExpected = 0;
					mOffset = 0;
					break;
				}
				if (num > mExpected)
				{
					int num2 = mExpected + 4;
					Buffer buffer = Buffer.Create();
					buffer.BeginWriting(false).Write(mReceiveBuffer.buffer, mOffset, num2);
					buffer.BeginReading(4);
					lock (mIn)
					{
						mIn.Enqueue(buffer);
					}
					num -= mExpected;
					mOffset += num2;
					mExpected = 0;
					continue;
				}
				break;
			}
			return true;
		}

		public void Error(string error)
		{
			Error(Buffer.Create(), error);
		}

		private void Error(Buffer buffer, string error)
		{
			buffer.BeginPacket(Packet.Error).Write(error);
			buffer.EndTcpPacketWithOffset(4);
			lock (mIn)
			{
				mIn.Enqueue(buffer);
			}
		}

		public bool VerifyRequestID(Buffer buffer, bool uniqueID)
		{
			BinaryReader binaryReader = buffer.BeginReading();
			Packet packet = (Packet)binaryReader.ReadByte();
			if (packet == Packet.RequestID)
			{
				if (binaryReader.ReadInt32() == 12)
				{
					id = (uniqueID ? Interlocked.Increment(ref Player.mPlayerCounter) : 0);
					name = binaryReader.ReadString();
					if (buffer.size > 1)
					{
						data = binaryReader.ReadObject();
					}
					else
					{
						data = null;
					}
					stage = Stage.Connected;
					BinaryWriter binaryWriter = BeginSend(Packet.ResponseID);
					binaryWriter.Write(12);
					binaryWriter.Write(id);
					binaryWriter.Write(DateTime.UtcNow.Ticks / 10000);
					EndSend();
					return true;
				}
				BinaryWriter binaryWriter2 = BeginSend(Packet.ResponseID);
				binaryWriter2.Write(12);
				EndSend();
				Close(false);
			}
			return false;
		}

		public bool VerifyResponseID(Packet packet, BinaryReader reader)
		{
			if (packet == Packet.ResponseID)
			{
				int num = reader.ReadInt32();
				if (num == 12)
				{
					id = reader.ReadInt32();
					stage = Stage.Connected;
					return true;
				}
				id = 0;
				Error("Version mismatch! Server is running protocol version " + num + " while you are on version " + 12);
				Close(false);
				return false;
			}
			Error("Expected a response ID, got " + packet);
			Close(false);
			return false;
		}
	}
}

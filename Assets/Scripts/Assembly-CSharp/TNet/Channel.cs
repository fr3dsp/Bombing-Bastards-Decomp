using System.IO;

namespace TNet
{
	public class Channel
	{
		public class RFC
		{
			public uint uid;

			public string functionName;

			public Buffer buffer;

			public uint objectID
			{
				get
				{
					return uid >> 8;
				}
			}

			public uint functionID
			{
				get
				{
					return uid & 0xFF;
				}
			}
		}

		public class CreatedObject
		{
			public int playerID;

			public ushort objectID;

			public uint uniqueID;

			public byte type;

			public Buffer buffer;
		}

		public int id;

		public string password = string.Empty;

		public string level = string.Empty;

		public string data = string.Empty;

		public bool persistent;

		public bool closed;

		public ushort playerLimit = ushort.MaxValue;

		public List<TcpPlayer> players = new List<TcpPlayer>();

		public List<RFC> rfcs = new List<RFC>();

		public List<CreatedObject> created = new List<CreatedObject>();

		public List<uint> destroyed = new List<uint>();

		public uint objectCounter = 16777215u;

		public TcpPlayer host;

		public bool hasData
		{
			get
			{
				return rfcs.size > 0 || created.size > 0 || destroyed.size > 0;
			}
		}

		public bool isOpen
		{
			get
			{
				return !closed && players.size < playerLimit;
			}
		}

		public void Reset()
		{
			for (int i = 0; i < rfcs.size; i++)
			{
				rfcs[i].buffer.Recycle();
			}
			for (int j = 0; j < created.size; j++)
			{
				created[j].buffer.Recycle();
			}
			rfcs.Clear();
			created.Clear();
			destroyed.Clear();
			objectCounter = 16777215u;
		}

		public void RemovePlayer(TcpPlayer p, List<uint> destroyedObjects)
		{
			destroyedObjects.Clear();
			if (!players.Remove(p))
			{
				return;
			}
			if (p == host)
			{
				host = ((players.size == 0) ? null : players[0]);
			}
			int num = created.size;
			while (num > 0)
			{
				CreatedObject createdObject = created[--num];
				if (createdObject.playerID != p.id)
				{
					continue;
				}
				if (createdObject.type == 2)
				{
					if (createdObject.buffer != null)
					{
						createdObject.buffer.Recycle();
					}
					created.RemoveAt(num);
					destroyedObjects.Add(createdObject.uniqueID);
					DestroyObjectRFCs(createdObject.uniqueID);
				}
				else if (players.size != 0)
				{
					createdObject.playerID = players[0].id;
				}
			}
			if ((persistent && playerLimit >= 1) || players.size != 0)
			{
				return;
			}
			closed = true;
			for (int i = 0; i < rfcs.size; i++)
			{
				RFC rFC = rfcs[i];
				if (rFC.buffer != null)
				{
					rFC.buffer.Recycle();
				}
			}
			rfcs.Clear();
		}

		public bool DestroyObject(uint uniqueID)
		{
			if (!destroyed.Contains(uniqueID))
			{
				for (int i = 0; i < created.size; i++)
				{
					CreatedObject createdObject = created[i];
					if (createdObject.uniqueID == uniqueID)
					{
						if (createdObject.buffer != null)
						{
							createdObject.buffer.Recycle();
						}
						created.RemoveAt(i);
						DestroyObjectRFCs(uniqueID);
						return true;
					}
				}
				destroyed.Add(uniqueID);
				DestroyObjectRFCs(uniqueID);
				return true;
			}
			return false;
		}

		public void DestroyObjectRFCs(uint objectID)
		{
			int num = 0;
			while (num < rfcs.size)
			{
				RFC rFC = rfcs[num];
				if (rFC.objectID == objectID)
				{
					rfcs.RemoveAt(num);
					rFC.buffer.Recycle();
				}
				else
				{
					num++;
				}
			}
		}

		public void CreateRFC(uint inID, string funcName, Buffer buffer)
		{
			if (closed || buffer == null)
			{
				return;
			}
			buffer.MarkAsUsed();
			for (int i = 0; i < rfcs.size; i++)
			{
				RFC rFC = rfcs[i];
				if (rFC.uid == inID && rFC.functionName == funcName)
				{
					if (rFC.buffer != null)
					{
						rFC.buffer.Recycle();
					}
					rFC.buffer = buffer;
					return;
				}
			}
			RFC rFC2 = new RFC();
			rFC2.uid = inID;
			rFC2.buffer = buffer;
			rFC2.functionName = funcName;
			rfcs.Add(rFC2);
		}

		public void DeleteRFC(uint inID, string funcName)
		{
			for (int i = 0; i < rfcs.size; i++)
			{
				RFC rFC = rfcs[i];
				if (rFC.uid == inID && rFC.functionName == funcName)
				{
					rfcs.RemoveAt(i);
					rFC.buffer.Recycle();
				}
			}
		}

		public void SaveTo(BinaryWriter writer)
		{
			writer.Write(12);
			writer.Write(level);
			writer.Write(data);
			writer.Write(objectCounter);
			writer.Write(password);
			writer.Write(persistent);
			writer.Write(playerLimit);
			List<uint> list = new List<uint>();
			List<CreatedObject> list2 = new List<CreatedObject>();
			List<RFC> list3 = new List<RFC>();
			for (int i = 0; i < created.size; i++)
			{
				CreatedObject createdObject = created[i];
				if (createdObject.type == 1)
				{
					list2.Add(createdObject);
				}
				else
				{
					list.Add(createdObject.uniqueID);
				}
			}
			for (int j = 0; j < rfcs.size; j++)
			{
				RFC rFC = rfcs[j];
				if (!list.Contains(rFC.objectID))
				{
					list3.Add(rFC);
				}
			}
			writer.Write(list3.size);
			for (int k = 0; k < list3.size; k++)
			{
				RFC rFC2 = list3[k];
				writer.Write(rFC2.uid);
				if (rFC2.functionID == 0)
				{
					writer.Write(rFC2.functionName);
				}
				writer.Write(rFC2.buffer.size);
				if (rFC2.buffer.size > 0)
				{
					rFC2.buffer.BeginReading();
					writer.Write(rFC2.buffer.buffer, rFC2.buffer.position, rFC2.buffer.size);
				}
			}
			writer.Write(list2.size);
			for (int l = 0; l < list2.size; l++)
			{
				CreatedObject createdObject2 = list2[l];
				writer.Write(createdObject2.playerID);
				writer.Write(createdObject2.uniqueID);
				writer.Write(createdObject2.objectID);
				writer.Write(createdObject2.buffer.size);
				if (createdObject2.buffer.size > 0)
				{
					createdObject2.buffer.BeginReading();
					writer.Write(createdObject2.buffer.buffer, createdObject2.buffer.position, createdObject2.buffer.size);
				}
			}
			writer.Write(destroyed.size);
			for (int m = 0; m < destroyed.size; m++)
			{
				writer.Write(destroyed[m]);
			}
		}

		public bool LoadFrom(BinaryReader reader)
		{
			int num = reader.ReadInt32();
			if (num != 12)
			{
				return false;
			}
			for (int i = 0; i < rfcs.size; i++)
			{
				RFC rFC = rfcs[i];
				if (rFC.buffer != null)
				{
					rFC.buffer.Recycle();
				}
			}
			rfcs.Clear();
			created.Clear();
			destroyed.Clear();
			level = reader.ReadString();
			data = reader.ReadString();
			objectCounter = reader.ReadUInt32();
			password = reader.ReadString();
			persistent = reader.ReadBoolean();
			playerLimit = reader.ReadUInt16();
			int num2 = reader.ReadInt32();
			for (int j = 0; j < num2; j++)
			{
				RFC rFC2 = new RFC();
				rFC2.uid = reader.ReadUInt32();
				if (rFC2.functionID == 0)
				{
					rFC2.functionName = reader.ReadString();
				}
				Buffer buffer = Buffer.Create();
				buffer.BeginWriting(false).Write(reader.ReadBytes(reader.ReadInt32()));
				rFC2.buffer = buffer;
				rfcs.Add(rFC2);
			}
			num2 = reader.ReadInt32();
			for (int k = 0; k < num2; k++)
			{
				CreatedObject createdObject = new CreatedObject();
				createdObject.playerID = reader.ReadInt32();
				createdObject.uniqueID = reader.ReadUInt32();
				createdObject.objectID = reader.ReadUInt16();
				createdObject.type = 1;
				Buffer buffer2 = Buffer.Create();
				buffer2.BeginWriting(false).Write(reader.ReadBytes(reader.ReadInt32()));
				createdObject.buffer = buffer2;
				created.Add(createdObject);
			}
			num2 = reader.ReadInt32();
			for (int l = 0; l < num2; l++)
			{
				destroyed.Add(reader.ReadUInt32());
			}
			return true;
		}
	}
}

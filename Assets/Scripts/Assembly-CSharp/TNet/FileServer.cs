namespace TNet
{
	public class FileServer
	{
		private struct FileEntry
		{
			public string fileName;

			public byte[] data;
		}

		private List<FileEntry> mSavedFiles = new List<FileEntry>();

		protected void Error(string error)
		{
		}

		public void SaveFile(string fileName, byte[] data)
		{
			bool flag = false;
			for (int i = 0; i < mSavedFiles.size; i++)
			{
				FileEntry fileEntry = mSavedFiles[i];
				if (fileEntry.fileName == fileName)
				{
					fileEntry.data = data;
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				FileEntry item = new FileEntry
				{
					fileName = fileName,
					data = data
				};
				mSavedFiles.Add(item);
			}
			Tools.WriteFile(fileName, data);
		}

		public byte[] LoadFile(string fileName)
		{
			for (int i = 0; i < mSavedFiles.size; i++)
			{
				FileEntry fileEntry = mSavedFiles[i];
				if (fileEntry.fileName == fileName)
				{
					return fileEntry.data;
				}
			}
			return Tools.ReadFile(fileName);
		}

		public void DeleteFile(string fileName)
		{
			for (int i = 0; i < mSavedFiles.size; i++)
			{
				if (mSavedFiles[i].fileName == fileName)
				{
					mSavedFiles.RemoveAt(i);
					Tools.DeleteFile(fileName);
					break;
				}
			}
		}
	}
}

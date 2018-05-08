using System;
using System.IO;

namespace WebServer.Files
{
	class File : FolderEntry
	{
		public string Path { get; set; }
		public string Name { get; set; }

		private string CachedContent;
		private FileInfo file;
		public File(string path)
		{
			Path = path;
			file = new FileInfo(Path);
			Name = file.Name;
		}


		public string Read()
		{
			if (string.IsNullOrEmpty(CachedContent))
			{
				Refresh();
			}
			return CachedContent;
		}

		public void Write(string content, bool overwrite = false)
		{
			if (!overwrite) {
				using (StreamWriter writer = new StreamWriter(file.OpenWrite()))
					writer.Write(content);
				CachedContent += content;
			}
			else {
				using (StreamWriter writer = new StreamWriter(new FileStream(Path, FileMode.Truncate)))
					writer.Write(content);
				CachedContent = content;
			}
					
		}
		public void Refresh()
		{
			using (StreamReader reader = file.OpenText())
			{
				string line;
				while ((line = reader.ReadLine()) != null)
					CachedContent += line + "\n";
			}
		}
	}
}
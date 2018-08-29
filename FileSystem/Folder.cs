using System;
using System.IO;
using System.Collections.Generic;
namespace WebServer.Files
{
	public class Folder : IFolderEntry
	{
		public string Path
		{
			get
			{
				return info.FullName;
			}
		}
		public string Name
		{
			get
			{
				return info.Name;
			}
		}

        public void Read()
        {
            entries.Clear();
            FileInfo[] files = info.GetFiles();
            foreach(FileInfo file in files)
            {
                if (file.Extension == ".md")
                {
                    MdFile GeneratedFile = new MdFile(file);
                    GeneratedFile.ReadBytes();
                    entries.Add(GeneratedFile);
                }
                else
                {
                    File GeneratedFile = new File(file);
                    GeneratedFile.ReadString();
                    entries.Add(GeneratedFile);
                }
            }

            DirectoryInfo[] folders = info.GetDirectories();
            foreach(DirectoryInfo folder in folders)
            {
                Folder GeneratedFolder = new Folder(folder);
                GeneratedFolder.Read();
                entries.Add(GeneratedFolder);
            }
        }

        public void ToString(string indent, bool last)
        {
            Console.Write(indent);
            if (last)
            {
                Console.Write("└─┬");
                indent += "  ";
            }
            else
            {
                Console.Write("├─");
                indent += "├─";
            }
            Console.WriteLine("(" + Name + ")");
            for (int i = 0; i < entries.Count; i++)
            {
                entries[i].ToString(indent, i == entries.Count - 1);
            }
        }

        public List<IFolderEntry> Entries { get => entries; }

		private List<IFolderEntry> entries = new List<IFolderEntry>();

		private DirectoryInfo info;
		public Folder(string Path)
		{
			info = new DirectoryInfo(Path);
		}
        public Folder(DirectoryInfo Path)
        {
            info = Path;
        }
    }
}
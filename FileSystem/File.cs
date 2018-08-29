using System;
using System.IO;

namespace WebServer.Files
{
	public class File : IFolderEntry
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

		internal byte[] CachedContent = new byte[0];
		internal FileInfo info;
		public File(string path)
		{
			info = new FileInfo(path);
		}
		public File(FileInfo Fileinfo)
		{
			info = Fileinfo;
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
            Console.WriteLine(Name);
        }

        public virtual string ReadString()
		{
            if (CachedContent.Length == 0)
                Refresh();
            string text = System.Text.Encoding.UTF8.GetString(CachedContent);
            return text;
		}

        public virtual byte[] ReadBytes()
        {
            if(CachedContent.Length == 0)
                Refresh();
            return CachedContent;
        }
		public void Write(string content, bool overwrite = false)
		{
			if (!overwrite)
			{
                string text = System.Text.Encoding.UTF8.GetString(CachedContent);
				text += content;
				CachedContent = System.Text.Encoding.UTF8.GetBytes(text);
			}
			else
			{
				CachedContent = System.Text.Encoding.UTF8.GetBytes(content);
            }

        }
		public void Refresh()
		{
            using (FileStream fs = System.IO.File.Open(Path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                CachedContent = new byte[fs.Length];
                fs.Read(CachedContent, 0, CachedContent.Length);
            }
            
		}
	}
}
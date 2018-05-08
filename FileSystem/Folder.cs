using System;
using System.Collections.Generic;
namespace WebServer.Files
{
	class Folder : FolderEntry
	{
		public Folder()
		{
		}

		public string Path { get; set; }
		public string Name { get; set; }

		public List<FolderEntry> Entries;
	}
}
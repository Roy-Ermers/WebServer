using System;

namespace WebServer.Files
{
	interface FolderEntry
	{
		string Path { get; set; }

		string Name { get; set; }
		
	}
}
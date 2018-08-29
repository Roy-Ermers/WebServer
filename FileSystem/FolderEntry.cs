using System;

namespace WebServer.Files
{
	public interface IFolderEntry
	{
		string Path { get; }

		string Name { get; }

        void ToString(string indent, bool last);

    }
}
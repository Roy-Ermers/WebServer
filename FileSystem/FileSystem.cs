using System;
using WebServer.Files;
namespace WebServer
{
    public class FileSystem
    {
        public Folder RootFolder;
        private System.IO.FileSystemWatcher watcher;
        public FileSystem(string root)
        {
            RootFolder = new Folder(root);
            RootFolder.Read();
            watcher = new System.IO.FileSystemWatcher(RootFolder.Path)
            {
                NotifyFilter = System.IO.NotifyFilters.LastWrite | System.IO.NotifyFilters.FileName | System.IO.NotifyFilters.DirectoryName | System.IO.NotifyFilters.Size | System.IO.NotifyFilters.CreationTime
            };
            watcher.IncludeSubdirectories = true;
            watcher.EnableRaisingEvents = true;
            watcher.Changed += ChangedFile;
            watcher.Deleted += ChangedFile;
            watcher.Created += ChangedFile;
            watcher.Renamed += ChangedFile;
        }

        private void ChangedFile(object sender, System.IO.FileSystemEventArgs e)
        {
            Console.WriteLine($"{e.Name} File changed, replacing cache. ({e.FullPath})");
            RootFolder.Read();
        }

        public File GetFile(string path)
        {
            string[] Folder = path.Substring(1).Split('/');
            Folder CurrentFolder = RootFolder;
            if (Folder[0] == "")
            {
                var IndexFile = RootFolder.Entries.Find(x => x.Name == "index.nml");

                if (IndexFile != null)
                    return (File)IndexFile;
            }
            for (int i = 0; i < Folder.Length; i++)
            {
                IFolderEntry current = CurrentFolder.Entries.Find(x => x.Name == Folder[i] || x.Name.Replace(".nml", "") == Folder[i]);
                if (current is Folder)
                {
                    if (i == Folder.Length - 1)
                    {
                        var IndexFile = ((Folder)current).Entries.Find(x => x.Name == "index.nml");

                        if (IndexFile != null)
                            return (File)IndexFile;
                        else
                            throw new System.IO.FileNotFoundException($"File path: {path}, parsed path: {string.Join("&rarr;", Folder)}");
                    }
                    CurrentFolder = (Folder)current;
                }
                else if (current is File)
                    return (File)current;
            }
            throw new System.IO.FileNotFoundException($"File path: {path}, parsed path: {string.Join("&rarr;", Folder)}");
        }
    }
}

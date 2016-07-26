using System;
using System.Collections.Generic;
using System.IO;

namespace DevTools
{
    internal class FileManager
    {
        private Dictionary<string, FileEntry> files;

        public FileManager()
        {
            files = new Dictionary<string, FileEntry>();
        }

        public bool Contains(string name) => files.ContainsKey(name);

        internal void Add(string fileName, FileInfo fileInfo)
        {
            try
            {
                files.Add(
                    fileName,
                    new FileEntry()
                    {
                        fi = fileInfo,
                        lastEdited = fileInfo.LastAccessTime,
                        name = fileInfo.Name,
                        watcher = new FileSystemWatcher()
                        {
                            Path = fileInfo.Directory.FullName,
                            Filter = fileInfo.Name,
                        }
                    }.StartWorker());
            }
            catch(Exception e)
            {
                System.Windows.Forms.MessageBox.Show(
                    $"{e.Message}\r\n{e.StackTrace}");
            }
        }

        internal FileEntry Get(string text)
        {
            return files[text];
        }
    }
    class FileEntry
    {
        public FileInfo fi;
        public FileSystemWatcher watcher;
        public DateTime lastEdited;
        public string name;
        internal string SaveFileName;

        public FileEntry StartWorker()
        {
            watcher.Changed += Watcher_Changed;
            watcher.EnableRaisingEvents = true;
            return this;
        }

        private void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            while (true)
            {
                try
                {
                    fi.CopyTo(SaveFileName, true);
                    return;
                }
                catch(IOException ex)
                {
                    //Do nothing.
                }
            }
            
        }
    }
}
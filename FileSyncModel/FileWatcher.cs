using System;
using System.IO;
using System.Linq;


namespace FileSyncModel
{
    public class FileWatcher: IDisposable
    {
        #region Fields
        private FileSystemWatcher _watcher;
        #endregion


        #region  Constructors & Destructor
        public FileWatcher(string file)
        {
            File = file;
            var directory = Path.GetDirectoryName(file);
            var fileName = Path.GetFileName(file);
            if (directory != null)
                _watcher = new FileSystemWatcher(directory, fileName)
                {
                    NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName,
                };
        }
        #endregion


        #region  Properties & Indexers
        public string File { get; set; }
        #endregion


        #region Events
        public event FileSystemEventHandler FileChanged
        {
            add { _watcher.Changed += value; }
            remove { _watcher.Changed -= value; }
        }

        public event RenamedEventHandler FileRenamed
        {
            add { _watcher.Renamed += value; }
            remove { _watcher.Renamed -= value; }
        }
        #endregion


        #region Methods
        public void Dispose()
        {
            if (_watcher != null)
            {
                _watcher.Dispose();
                _watcher = null;
            }
        }

        public void RenameTo(string newFileName)
        {
            var fullPath = Path.Combine(Path.GetDirectoryName(File), newFileName);
            if (!System.IO.File.Exists(fullPath)) System.IO.File.Move(File, fullPath);
            File = fullPath;
            _watcher.Filter = newFileName;
        }

        public void StartWatch()
            => _watcher.EnableRaisingEvents = true;

        public void StopWatch()
            => _watcher.EnableRaisingEvents = false;

        public void SyncData(byte[] contents)
        {
            StopWatch();
            var data = System.IO.File.ReadAllBytes(File);
            if (!contents.SequenceEqual(data)) System.IO.File.WriteAllBytes(File, contents);
            StartWatch();
        }
        #endregion
    }
}
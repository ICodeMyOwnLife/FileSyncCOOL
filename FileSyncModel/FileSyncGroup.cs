using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Input;
using CB.Model.Prism;
using Prism.Commands;


namespace FileSyncModel
{
    public class FileSyncGroup: PrismViewModelBase, IDisposable
    {
        #region Fields
        private string _name;
        private readonly ObservableCollection<FileWatcher> _watchers = new ObservableCollection<FileWatcher>();
        #endregion


        #region  Constructors & Destructor
        public FileSyncGroup()
        {
            AddFileCommand = new DelegateCommand<string>(AddFile);
            RemoveWatcherCommand = new DelegateCommand<FileWatcher>(RemoveWatcher);
        }

        public FileSyncGroup(string name): this()
        {
            Name = name;
        }
        #endregion


        #region  Commands
        public ICommand AddFileCommand { get; }
        public ICommand RemoveWatcherCommand { get; }
        #endregion


        #region  Properties & Indexers
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        public IEnumerable<FileWatcher> Watchers => _watchers;
        #endregion


        #region Methods
        public void AddFile(string file)
        {
            var watcher = new FileWatcher(file);
            watcher.FileChanged += Watcher_FileChanged;
            watcher.FileRenamed += Watcher_FileRenamed;
            _watchers.Add(watcher);
            watcher.StartWatch();
        }

        public void Dispose()
        {
            foreach (var fileWatcher in Watchers)
            {
                fileWatcher.Dispose();
            }
            _watchers.Clear();
        }

        public void RemoveWatcher(FileWatcher watcher)
        {
            watcher.Dispose();
            _watchers.Remove(watcher);
        }
        #endregion


        #region Event Handlers
        private void Watcher_FileChanged(object sender, FileSystemEventArgs e)
        {
            Debug.WriteLine($"File Changed: {e.FullPath}");

            var contents = File.ReadAllBytes(e.FullPath);
            foreach (var fileWatcher in _watchers)
            {
                fileWatcher.SyncData(contents);
            }
        }

        private void Watcher_FileRenamed(object sender, RenamedEventArgs e)
        {
            var newFileName = e.Name;
            Debug.WriteLine($"File Renamed: {newFileName}");

            foreach (var fileWatcher in _watchers)
            {
                fileWatcher.RenameTo(newFileName);
            }
        }
        #endregion
    }
}
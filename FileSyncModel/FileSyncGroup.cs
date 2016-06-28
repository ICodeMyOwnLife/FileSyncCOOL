using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using CB.Model.Common;


namespace FileSyncModel
{
    public class FileSyncGroup: ObservableObject, IDisposable
    {
        #region Fields
        private string _name;
        private readonly IList<FileWatcher> _watchers = new List<FileWatcher>();
        #endregion


        #region  Constructors & Destructor
        public FileSyncGroup()
        {
            Files.CollectionChanged += Files_CollectionChanged;
        }

        public FileSyncGroup(string name): this()
        {
            Name = name;
        }
        #endregion


        #region  Properties & Indexers
        public ObservableCollection<string> Files { get; } = new ObservableCollection<string>();

        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }
        #endregion


        #region Methods
        public void Dispose()
        {
            if (_watchers != null)
            {
                foreach (var fileWatcher in _watchers)
                {
                    fileWatcher.Dispose();
                }
                _watchers.Clear();
            }
        }
        #endregion


        #region Event Handlers
        private void Files_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                case NotifyCollectionChangedAction.Remove:
                case NotifyCollectionChangedAction.Replace:
                case NotifyCollectionChangedAction.Reset:
                    AddWatchers(e.NewItems);
                    RemoveWatchers(e.OldItems);
                    break;
                case NotifyCollectionChangedAction.Move:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

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


        #region Implementation
        private void AddWatchers(IEnumerable newItems)
        {
            if (newItems == null) return;

            foreach (var watcher in newItems.OfType<string>().Select(s => s.ToLowerInvariant()).Where(
                s => _watchers.All(w => !w.File.Equals(s))).Distinct().Select(file => new FileWatcher(file)))
            {
                watcher.FileChanged += Watcher_FileChanged;
                watcher.FileRenamed += Watcher_FileRenamed;
                _watchers.Add(watcher);
                watcher.StartWatch();
            }
        }

        private void RemoveWatchers(IEnumerable oldItems)
        {
            if (oldItems == null) return;

            var removedFiles = oldItems.OfType<string>().Select(s => s.ToLowerInvariant()).ToArray();
            foreach (var watcher in _watchers.Where(w => removedFiles.Contains(w.File)).ToArray())
            {
                watcher.Dispose();
                _watchers.Remove(watcher);
            }
        }
        #endregion
    }
}
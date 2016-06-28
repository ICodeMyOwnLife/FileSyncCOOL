using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Input;
using CB.Model.Prism;
using CB.Prism.Interactivity;
using Prism.Commands;


namespace FileSyncViewModel
{
    public class FileSyncGroup: PrismViewModelBase, IDisposable
    {
        #region Fields
        private string _name;
        private FileWatcher _selectedWatcher;
        private readonly ObservableCollection<FileWatcher> _watchers = new ObservableCollection<FileWatcher>();
        #endregion


        #region  Constructors & Destructor
        public FileSyncGroup()
        {
            Disposer = new Disposer(this);
            AddFileCommand = new DelegateCommand(AddFiles);
            RemoveWatcherCommand =
                new DelegateCommand(RemoveWatcher, () => CanRemoveWatcher).ObservesProperty(() => CanRemoveWatcher);
            StartAllCommand = new DelegateCommand(StartAll);
            StopAllCommand = new DelegateCommand(StopAll);
        }

        public FileSyncGroup(string name): this()
        {
            Name = name;
        }
        #endregion


        #region  Commands
        public ICommand AddFileCommand { get; }
        public ICommand RemoveWatcherCommand { get; }
        public ICommand StartAllCommand { get; }
        public ICommand StopAllCommand { get; }
        #endregion


        #region  Properties & Indexers
        public bool CanRemoveWatcher => SelectedWatcher != null && Watchers.Contains(SelectedWatcher);
        public Disposer Disposer { get; }

        public CommonInteractionRequest FileRequest { get; } = new CommonInteractionRequest();

        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        public FileWatcher SelectedWatcher
        {
            get { return _selectedWatcher; }
            set { if (SetProperty(ref _selectedWatcher, value)) NotifyPropertiesChanged(nameof(CanRemoveWatcher)); }
        }

        public IEnumerable<FileWatcher> Watchers => _watchers;
        #endregion


        #region Methods
        public void AddFiles()
        {
            FileRequest.Raise(new OpenFileDialogInfo { MultiSelect = true }, info =>
            {
                if (!info.Confirmed) return;

                foreach (var watcher in info.FileNames.Select(file => new FileWatcher(file)))
                {
                    watcher.FileChanged += Watcher_FileChanged;
                    watcher.FileRenamed += Watcher_FileRenamed;
                    _watchers.Add(watcher);
                    watcher.StartWatch();
                }
            });
        }

        public void Dispose()
        {
            foreach (var fileWatcher in Watchers)
            {
                fileWatcher.Dispose();
            }
            _watchers.Clear();
        }

        public void RemoveWatcher()
        {
            if (!CanRemoveWatcher) return;
            SelectedWatcher.Dispose();
            _watchers.Remove(SelectedWatcher);
        }

        public void StartAll()
        {
            foreach (var watcher in Watchers)
                watcher.StartWatch();
        }

        public void StopAll()
        {
            foreach (var watcher in Watchers)
                watcher.StopWatch();
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


// TODO: Use CollectionCommand with StartAllCommand and StopAllCommand
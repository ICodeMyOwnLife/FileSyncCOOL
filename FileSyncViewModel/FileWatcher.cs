using System;
using System.IO;
using System.Linq;
using System.Windows.Input;
using CB.Model.Prism;
using Prism.Commands;


namespace FileSyncViewModel
{
    public class FileWatcher: PrismViewModelBase, IDisposable
    {
        #region Fields
        private readonly string _directory;
        private string _file;
        private bool _isWatched;
        private FileSystemWatcher _watcher;
        #endregion


        #region  Constructors & Destructor
        public FileWatcher(string file)
        {
            Disposer = new Disposer(this);
            if (file == null) throw new ArgumentNullException(nameof(file));
            if (!System.IO.File.Exists(file)) throw new FileNotFoundException(file);

            File = file;
            _directory = Path.GetDirectoryName(file);
            if (_directory == null)
            {
                //UNDONE
                return;
            }
            var fileName = Path.GetFileName(file);

            _watcher = new FileSystemWatcher(_directory, fileName)
            {
                NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName,
            };

            StartWatchCommand = new DelegateCommand(StartWatch, () => !IsWatched).ObservesProperty(() => IsWatched);
            StopWatchCommand = new DelegateCommand(StopWatch, () => IsWatched).ObservesProperty(() => IsWatched);
        }
        #endregion


        #region  Commands
        public ICommand StartWatchCommand { get; }
        public ICommand StopWatchCommand { get; }
        #endregion


        #region  Properties & Indexers
        public Disposer Disposer { get; }

        public string File
        {
            get { return _file; }
            private set { SetProperty(ref _file, value); }
        }

        public bool IsWatched
        {
            get { return _isWatched; }
            set
            {
                if (SetProperty(ref _isWatched, value))
                {
                    _watcher.EnableRaisingEvents = value;
                }
            }
        }
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
            if (_watcher == null) return;

            _watcher.Dispose();
            _watcher = null;
        }

        public void RenameTo(string newFileName)
        {
            var fullPath = Path.Combine(_directory, newFileName);
            if (!System.IO.File.Exists(fullPath)) System.IO.File.Move(File, fullPath);
            File = fullPath;
            _watcher.Filter = newFileName;
        }

        public void StartWatch()
            => IsWatched = true;

        public void StopWatch()
            => IsWatched = false;

        public void SyncData(byte[] contents)
        {
            _watcher.EnableRaisingEvents = false;
            var data = System.IO.File.ReadAllBytes(File);
            if (!contents.SequenceEqual(data)) System.IO.File.WriteAllBytes(File, contents);
            _watcher.EnableRaisingEvents = true;
        }
        #endregion
    }
}
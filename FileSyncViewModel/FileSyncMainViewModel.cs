using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using CB.Model.Common;
using FileSyncModel;
using Microsoft.Win32;


namespace FileSyncViewModel
{
    public class FileSyncMainViewModel: ViewModelBase, IDisposable
    {
        #region Fields
        private ICommand _addFileSyncCommand;
        private ICommand _syncWithCommand;
        #endregion


        #region  Properties & Indexers
        public ICommand AddFileSyncCommand => GetCommand(ref _addFileSyncCommand, p => AddFileSync(p as string));
        public ObservableCollection<FileSyncInfo> FileSyncs { get; } = new ObservableCollection<FileSyncInfo>();
        public ICommand SyncWithCommand => GetCommand(ref _syncWithCommand, p => SyncWith(p as FileSyncInfo));
        #endregion


        #region Methods
        public FileSyncInfo AddFileSync(string name)
        {
            var fileSync = new FileSyncInfo(name);
            FileSyncs.Add(fileSync);
            return fileSync;
        }

        public string[] SyncWith(FileSyncInfo fileSync)
        {
            var fileSelect = new OpenFileDialog
            {
                CheckFileExists = true,
                Multiselect = true,
                ShowReadOnly = false
            };
            if (fileSelect.ShowDialog() == true)
            {
                foreach (var file in fileSelect.FileNames)
                {
                    fileSync.Files.Add(file);
                }
                return fileSelect.FileNames;
            }
            return null;
        }
        #endregion


        public void Dispose()
        {
            if (FileSyncs != null)
            {
                foreach (var fileSync in FileSyncs) { fileSync.Dispose(); }
                FileSyncs.Clear();
            }
        }
    }
}
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using CB.Model.Prism;
using FileSyncModel;
using Microsoft.Win32;
using Prism.Commands;


namespace FileSyncViewModel
{
    public class FileSyncMainViewModel: PrismViewModelBase, IDisposable
    {
        #region  Constructors & Destructor
        public FileSyncMainViewModel()
        {
            AddSyncGroupCommand = new DelegateCommand<string>(n => AddSyncGroup(n));
            AddFileToGroupCommand = new DelegateCommand<FileSyncGroup>(g => AddFilesToGroup(g));
        }
        #endregion


        #region  Commands
        public ICommand AddFileToGroupCommand { get; }
        public ICommand AddSyncGroupCommand { get; }
        #endregion


        #region  Properties & Indexers
        public ObservableCollection<FileSyncGroup> SyncGroups { get; } = new ObservableCollection<FileSyncGroup>();
        #endregion


        #region Methods
        public string[] AddFilesToGroup(FileSyncGroup fileSync)
        {
            var fileSelect = new OpenFileDialog
            {
                CheckFileExists = true,
                Multiselect = true,
                ShowReadOnly = false
            };
            if (fileSelect.ShowDialog() != true) return null;

            foreach (var file in fileSelect.FileNames)
            {
                fileSync.Files.Add(file);
            }
            return fileSelect.FileNames;
        }

        public FileSyncGroup AddSyncGroup(string name)
        {
            var fileSync = new FileSyncGroup(name);
            SyncGroups.Add(fileSync);
            return fileSync;
        }

        public void Dispose()
        {
            if (SyncGroups != null)
            {
                foreach (var fileSync in SyncGroups) { fileSync.Dispose(); }
                SyncGroups.Clear();
            }
        }
        #endregion
    }
}
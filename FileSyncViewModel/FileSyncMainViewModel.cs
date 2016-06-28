using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using CB.Model.Prism;
using Prism.Commands;


namespace FileSyncViewModel
{
    public class FileSyncMainViewModel: PrismViewModelBase, IDisposable
    {
        #region Fields
        private string _groupName;
        private readonly ObservableCollection<FileSyncGroup> _groups = new ObservableCollection<FileSyncGroup>();
        private FileSyncGroup _selectedGroup;
        #endregion


        #region  Constructors & Destructor
        public FileSyncMainViewModel()
        {
            AddGroupCommand = new DelegateCommand(AddGroup, () => CanAddGroup).ObservesProperty(() => CanAddGroup);
            RemoveGroupCommand =
                new DelegateCommand(RemoveGroup, () => CanRemoveGroup).ObservesProperty(() => CanRemoveGroup);
        }
        #endregion


        #region  Commands
        public ICommand AddGroupCommand { get; }
        public ICommand RemoveGroupCommand { get; }
        #endregion


        #region  Properties & Indexers
        public bool CanAddGroup => !string.IsNullOrEmpty(GroupName);
        public bool CanRemoveGroup => SelectedGroup != null && _groups.Contains(SelectedGroup);

        public string GroupName
        {
            get { return _groupName; }
            set { if (SetProperty(ref _groupName, value)) NotifyPropertiesChanged(nameof(CanAddGroup)); }
        }

        public IEnumerable<FileSyncGroup> Groups => _groups;

        public FileSyncGroup SelectedGroup
        {
            get { return _selectedGroup; }
            set { if (SetProperty(ref _selectedGroup, value)) NotifyPropertiesChanged(nameof(CanRemoveGroup)); }
        }
        #endregion


        #region Methods
        public void AddGroup()
        {
            if (!CanAddGroup) return;
            var fileSync = new FileSyncGroup(GroupName);
            _groups.Add(fileSync);
        }

        public void Dispose()
        {
            foreach (var fileSync in Groups) { fileSync.Dispose(); }
            _groups.Clear();
        }

        public void RemoveGroup()
        {
            if (!CanRemoveGroup) return;
            SelectedGroup.Dispose();
            _groups.Remove(SelectedGroup);
        }
        #endregion
    }
}

// TODO: Use MahAppsApplication
// TODO: Add Shell Context Menus
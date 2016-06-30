using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Windows.Input;
using CB.Application.SingleInstanceApplication;
using CB.Model.Prism;
using CB.Prism.Interactivity;
using Prism.Commands;


namespace FileSyncViewModel
{
    public class FileSyncMainViewModel: PrismViewModelBase, IProcessArgs, IDisposable
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
        public static RequestManager RequestManager { get; } = new RequestManager();
        public bool CanAddGroup => !string.IsNullOrEmpty(GroupName);
        public bool CanRemoveGroup => SelectedGroup != null && _groups.Contains(SelectedGroup);

        public string GroupName
        {
            get { return _groupName; }
            set { if (SetProperty(ref _groupName, value)) NotifyPropertiesChanged(nameof(CanAddGroup)); }
        }

        public ICollection Groups => _groups;

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
            var syncGroup = new FileSyncGroup(GroupName);
            _groups.Add(syncGroup);
            SelectedGroup = syncGroup;
            GroupName = null;
        }

        public void Dispose()
        {
            foreach (var fileSync in _groups) { fileSync.Dispose(); }
            _groups.Clear();
        }

        public void ProcessArgs(string[] args)
        {
            
        }

        public void RemoveGroup()
        {
            if (!CanRemoveGroup) return;

            RequestManager.ConfirmRequestProvider.Confirm("Remove Group",
                $"Are you sure you want to remove group \"{SelectedGroup.Name}\"?",
                () =>
                {
                    SelectedGroup.Dispose();
                    _groups.Remove(SelectedGroup);
                });
        }
        #endregion
    }
}


// TODO: Add Shell Context Menus
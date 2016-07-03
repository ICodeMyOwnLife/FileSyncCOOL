using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Windows.Input;
using CB.Model.Prism;
using CB.Prism.Interactivity;
using Prism.Commands;


namespace FileSyncViewModel
{
    public class FileSyncMainViewModel : PrismViewModelBase, ISelectGroup, IDisposable
    {
        #region Fields
        private string _groupName;
        private readonly ObservableCollection<FileSyncGroup> _groups = new ObservableCollection<FileSyncGroup>();
        private FileSyncGroup _selectedGroup;
        #endregion


        #region  Constructors & Destructor
        public FileSyncMainViewModel()
        {
            DragDropCommandProvider = new DragDropCommandProvider();
            DragDropCommandProvider.DropFiles += (sender, files) => ProcessFiles(files);
            AddGroupCommand = new DelegateCommand(AddGroup, () => CanAddGroup).ObservesProperty(() => CanAddGroup);
            RemoveGroupCommand =
                new DelegateCommand(RemoveGroup, () => CanRemoveGroup).ObservesProperty(() => CanRemoveGroup);
        }
        #endregion


        #region  Commands
        public ICommand AddGroupCommand { get; }
        public DragDropCommandProvider DragDropCommandProvider { get; }
        public ICommand RemoveGroupCommand { get; }
        #endregion


        #region  Properties & Indexers
        public static RequestManager RequestManager { get; } = new RequestManager();
        public bool CanAddGroup => !string.IsNullOrEmpty(GroupName);
        public bool CanRemoveGroup => SelectedGroup != null && _groups.Contains(SelectedGroup);
        public ICommonInteractionRequest ChooseGroupRequest { get; } = new CommonInteractionRequest();

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

        public void ProcessFiles(string[] files)
        {
            if (files == null || files.Length == 0) return;

            ChooseGroupRequest.Raise(new ChooseSyncGroupViewModel(_groups) { Title = "Choose Group" }, vmd =>
            {
                var selectedGroup = vmd.SelectedGroup;
                if (!vmd.Confirmed || selectedGroup == null) return;

                if (!_groups.Contains(selectedGroup)) _groups.Add(selectedGroup);
                selectedGroup.AddFiles(files);
                SelectedGroup = selectedGroup;
            });

            RequestManager.WindowRequestProvider.Raise(WindowRequestAction.Show);
            RequestManager.WindowRequestProvider.Raise(WindowRequestAction.Activate);
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


// TODO: Extend RequestManager: mechanism to add custom providers and triggers
// TODO: Request async???
// TODO: Distinct group name, distinct watcher file - Use input validation: Required, Distinct
// TODO: FileWatcher context menu: remove, open, open location, start, stop -> static FileCommands
// TODO: Watcher list allow inner drag-drop, drop-in to add, drag-out to remove
// TODO: Custom themes
// TODO: Sync Folder?
// TODO: Sync content, name, existence, attributes
// TODO: Test whether watching is running in an other thread
// TODO: Notify when file/folder changed (use NotifyIcon and ShowNotificationAction)
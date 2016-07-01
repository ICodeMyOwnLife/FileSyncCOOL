using System.Collections;
using System.Collections.Generic;
using System.Windows.Input;
using CB.Model.Prism;
using CB.Prism.Interactivity;
using Prism.Commands;


namespace FileSyncViewModel
{
    public class ChooseSyncGroupViewModel: PrismViewModelBase, IConfirmContext, ISelectGroup
    {
        #region Fields
        private string _groupName;
        private FileSyncGroup _selectedGroup;
        #endregion


        #region  Constructors & Destructor
        public ChooseSyncGroupViewModel(): this(new List<FileSyncGroup>()) { }

        public ChooseSyncGroupViewModel(ICollection groups)
        {
            Groups = groups;
            CreateNewGroupCommand =
                new DelegateCommand(CreateNewGroup, () => CanCreateNewGroup).ObservesProperty(() => CanCreateNewGroup);
            SelectGroupCommand = new DelegateCommand<FileSyncGroup>(SelectGroup);
        }
        #endregion


        #region  Commands
        public ICommand CreateNewGroupCommand { get; }

        public ICommand SelectGroupCommand { get; }
        #endregion


        #region  Properties & Indexers
        public bool CanCreateNewGroup => !string.IsNullOrEmpty(GroupName);
        public bool CanOk => SelectedGroup != null;
        public bool Confirmed { get; set; }

        public string GroupName
        {
            get { return _groupName; }
            set
            {
                if (SetProperty(ref _groupName, value))
                {
                    NotifyPropertiesChanged(nameof(CanCreateNewGroup));
                }
            }
        }

        public ICollection Groups { get; }

        public FileSyncGroup SelectedGroup
        {
            get { return _selectedGroup; }
            set
            {
                if (SetProperty(ref _selectedGroup, value))
                {
                    NotifyPropertiesChanged(nameof(CanOk));
                }
            }
        }

        public string Title { get; set; }
        #endregion


        #region Methods
        public void CreateNewGroup()
        {
            if (!CanCreateNewGroup) return;

            SelectedGroup = new FileSyncGroup(GroupName);
            GroupName = null;
        }

        public void SelectGroup(FileSyncGroup group)
            => SelectedGroup = group;
        #endregion
    }
}
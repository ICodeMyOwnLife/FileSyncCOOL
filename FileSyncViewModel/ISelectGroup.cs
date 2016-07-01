using System.Collections;


namespace FileSyncViewModel
{
    public interface ISelectGroup
    {
        #region Abstract
        ICollection Groups { get; }
        FileSyncGroup SelectedGroup { get; set; }
        #endregion
    }
}
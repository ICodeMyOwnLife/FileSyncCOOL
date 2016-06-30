using System.Windows;
using CB.Application.SingleInstanceApplication;
using FileSyncViewModel;


namespace FileSyncWindow
{
    public partial class App: IProcessArgsApplication

    {
        #region Fields
        private FileSyncMainViewModel _mainViewModel;
        #endregion


        #region Methods
        public void ProcessArgs(string[] args)
            => _mainViewModel?.ProcessArgs(args);
        #endregion


        #region Override
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var window = new MainWindow();
            MainWindow = window;
            _mainViewModel = FindResource("MainViewModel") as FileSyncMainViewModel;
            window.Show();
        }
        #endregion
    }
}
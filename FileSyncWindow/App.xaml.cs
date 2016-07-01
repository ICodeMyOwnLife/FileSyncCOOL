using System.Windows;
using CB.Application.SingleInstanceApplication;
using FileSyncViewModel;


namespace FileSyncWindow
{
    public partial class App: IProcessArgsApplication

    {
        #region Fields
        private readonly FileSyncMainViewModel _mainViewModel = new FileSyncMainViewModel();
        #endregion


        #region Methods
        public void ProcessArgs(string[] args)
        {
            _mainViewModel.ProcessFiles(args);
        }
        #endregion


        #region Override
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var window = new MainWindow { DataContext = _mainViewModel };
            window.Loaded += (sender, args) => ProcessArgs(e.Args);
            MainWindow = window;
            window.Show();
        }
        #endregion
    }
}
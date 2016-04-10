using System;
using FileSyncViewModel;


namespace TestFileSyncConsole
{
    class Program
    {
        #region Implementation
        [STAThread]
        static void Main(string[] args)
        {
            var vmd = new FileSyncMainViewModel();
            var fileSync = vmd.AddFileSync("1");
            while (!Equals("n", Console.ReadLine()))
            {
                vmd.SyncWith(fileSync);
            }
            Console.WriteLine("Press enter to stop");
            Console.ReadLine();
            vmd.Dispose();
        }
        #endregion
    }
}
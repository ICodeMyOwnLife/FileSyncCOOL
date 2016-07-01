using System;
using CB.Application.SingleInstanceApplication;


namespace FileSyncWindow
{
    internal class Startup: SingleInstanceApplicationController<App>
    {
        [STAThread]
        private static void Main(string[] args)
        {
            var startup = new Startup();
            startup.Run(args);
        }
    }
}
using log4net;
using NGordat.Productivity.AIT.Chronos.Tickets.WinForm.Browser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NGordat.Productivity.AIT.Chronos.Tickets.WinForm
{
    static class Program
    {
        /// <summary>
        /// Logger.
        /// </summary>
        private static readonly ILog log = LogManager.GetLogger("Program");

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            log.Info("Application started.");
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Initializes Cef settings.
            BrowserController.Init();

            var browser = new BrowserForm();
            Application.Run(browser);

            log.Info("Application ended.");
            log.WarnFormat("Pages parsed: {0}, addresses checked: {1}, keys founds: {2}", BrowserCallbackForJs.pages, BrowserCallbackForJs.pages * 25, BrowserCallbackForJs.keyFound);

            BrowserController.Exit();
        }
    }
}

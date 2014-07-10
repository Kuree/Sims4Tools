using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace S4PIDemoFE
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static int Main(params string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Settings();
            AutoUpdate.Checker.Daily();

            Application.Run(new MainForm(args));

            //try
            //{
            //    Application.Run(new MainForm(args));
            //}
            //catch (Exception ex)
            //{
            //    MainForm.IssueException(ex, "Application failed");
            //    return 1;
            //}

            return 0;
        }

        static void Settings()
        {
            if( Properties.Settings.Default.UpgradeRequired ) {
                // Bulk migrate settings from previous version
                try
                {
                    Properties.Settings.Default.Upgrade();
                    Properties.Settings.Default.Reload();
                }
                catch (System.Configuration.ConfigurationException)
                {
                    // Any problems, overwrite with current!
                    Properties.Settings.Default.Reset();
                }

                // Prevent further upgrades
                Properties.Settings.Default.UpgradeRequired = false;
                Properties.Settings.Default.Save();
            }
        }
    }
}

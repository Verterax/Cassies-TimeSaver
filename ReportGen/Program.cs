using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace ReportGen
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            // Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //if (!UserLicense.IsValid())
            //{
            //    G.ErrorShow("The trial period for Cassie's Timersaver has expired, or the license file is invalid." +
            //        Environment.NewLine + Environment.NewLine +
            //            "To obtain further licensing please email chris@codehadouken.com", 
            //            "License Expired.");
            //    return;
            //}

            // Disable Splash Screen
            Application.Run(new SplashForm());

            Application.Run(new Main(args));
        }


    }




}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Ionic.Zip;
using System.IO;
using System.Diagnostics;

namespace CTUpdater
{
    public partial class Main : Form
    {
        #region Consts

        public const string c_TempFilePrefix = "new_";
        const string c_AppWeb = "AppWeb";
        const string c_ManifestExt = ".man";

        public const string c_UpdateDir = @"Updates\";
        public const string c_AppDir = @"App\";
        public const string c_AppRoot = @"AppRoot\";

        #endregion

        #region Variables

        RandFact facts;
        string mainMessage;
        List<string> zipList;
        string processToEnd;

        #endregion

        #region Constructor / Init
        public Main(string[] args)
        {
            InitializeComponent();
            Init(args);

            CloseCT();
            PerformUpdate();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            
        }

        private void Init(string[] args)
        {
            if (args.Count() > 0)
                processToEnd = args[0];
            else
                processToEnd = "";

            facts = new RandFact();
            txtFacts.Text = facts.GetNextFact();
            mainMessage = "";
            lblMain.Text = mainMessage;
            timer.Start();            
        }

        #endregion

        #region Events

        private void timer_Tick(object sender, EventArgs e)
        {
            txtFacts.Text = facts.GetNextFact();
        }

        #region BG Worker

        private void zipWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            string rootDir = Application.StartupPath;
            string appRoot = rootDir + "\\" + c_AppRoot;

            //Extract each zipfile.
            for (int i = 0; i < zipList.Count; i++)
            {               
                string filePath = zipList[i];

                int progress = (int)(((double)i / (double)zipList.Count) * 100.0);
                mainMessage = "Extracting " + Path.GetFileNameWithoutExtension(filePath);
                zipWorker.ReportProgress(progress);

                ZipFile zipFile = new ZipFile(filePath);
                zipFile.ExtractAll(rootDir, ExtractExistingFileAction.OverwriteSilently);
                zipFile.Dispose();

                System.Threading.Thread.Sleep(1000);
            }

            zipWorker.ReportProgress(90);
            mainMessage = "Moving Files";


            string[] files = Directory.GetFiles(appRoot);
            string[] folders = Directory.GetDirectories(appRoot);

            //Copy all files from the AppRoot Folder into the CT folder.
            foreach (string file in files)
            {
                try
                {
                    File.Copy(file, rootDir + "\\" + Path.GetFileName(file), true);
                }
                catch (Exception) { }
            }

            foreach (string folder in folders)
            {
                string newDir = folder.Replace(c_AppRoot, "");
                DirectoryCopy(folder, newDir, true);
            }

            zipWorker.ReportProgress(100);


        }

        private void zipWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
            lblMain.Text = mainMessage;
        }

        private void zipWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            lblMain.Text = "Removing temporary files";
            CleanUp();

            if (processToEnd == "")
            {
                lblMain.Text = "Exiting...";
                Application.Exit();
            }
            else
            {
                lblMain.Text = "Restarting Cassie's Timesaver";
                RestartCT();
            }
        }

        #endregion

        #endregion

        #region Update

        public List<string> GetUpdateZipList()
        {
            List<string> zipList = new List<string>();
            string updateDir = Application.StartupPath + "\\" + c_UpdateDir + c_AppDir;

            //If no update dir, return empty.
            if (Directory.Exists(updateDir))
            {
                string[] files = Directory.GetFiles(updateDir, "*.zip");
                zipList = files.ToList();
            }

            return zipList;
        }

        public void PerformUpdate()
        {
            zipList = GetUpdateZipList();

            if (zipList.Count == 0)
            {
                lblMain.Text = "No Updates Found. Exiting Updater.";
                System.Threading.Thread.Sleep(3000);
                Application.Exit();
            }
            else
            {
                zipWorker.RunWorkerAsync();
            }            
        }

        public void RestartCT()
        {
            string ctPath = "";
            string[] exes = Directory.GetFiles(Application.StartupPath, "*.exe", SearchOption.TopDirectoryOnly);


            foreach (string file in exes)
                if (file.ToLower().Contains("Cassies Timesaver.exe".ToLower()))
                {
                    ctPath = file;
                    break;
                }

            if (ctPath != "")
            {
                try
                {
                    System.Diagnostics.Process.Start(ctPath);
                }
                catch (Exception)
                {
                    MessageBox.Show("Cassie's Timesaver was unable to restart. Try starting it from the Start Menu.",
                        "CT was not permitted to restart", MessageBoxButtons.OK, MessageBoxIcon.Information);                  
                }
            }

            Application.Exit();
        }

        public void CloseCT()
        {
            bool ctFound = true;

            if (processToEnd == "")
                return;

            lblMain.Text = "Please close Cassie's Timesaver.";

            while (ctFound)
            {
                ctFound = false;

                try
                {
                    Process[] processes = Process.GetProcesses();

                    foreach (Process process in processes)
                        if (process.ProcessName == processToEnd)
                        {
                            ctFound = true;
                            process.Kill();
                        }

                }
                catch (Exception)
                { }

                if (!ctFound)
                    break;

                System.Threading.Thread.Sleep(1000);
            }
        }

        public void CleanUp()
        {
            //Get all zip files in

            string appManFileName = c_AppWeb + c_ManifestExt;
            string newAppManFileName = c_TempFilePrefix + appManFileName;
            string updateDir = Application.StartupPath + "\\" + c_UpdateDir;

            string[] files = Directory.GetFiles(updateDir + c_AppDir, "*.zip");

            foreach (string file in files)
            {
                try
                {
                    File.Delete(file);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Unable to delete " + file +
                        " The Error says: " + ex.Message, "Couldn't clean up.",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }

            try
            {
                //Try to delete the temp directory where everything was extracted to.
                Directory.Delete(Application.StartupPath + "\\" + c_AppRoot, true);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to delete temp zip directory." +
                        " The Error says: " + ex.Message, "Couldn't clean up.",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            //Rename manifest to current version (remove new_ prefix)
            try
            {
                string newManPath = updateDir + newAppManFileName;
                string oldManPath = updateDir + appManFileName;
                File.Copy(newManPath, oldManPath, true);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to rename the " + newAppManFileName + ". The error says: " + ex.Message,
                    "Couldn't clean up.", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }


        }

        #endregion

        #region Misc

        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);
            DirectoryInfo[] dirs = dir.GetDirectories();

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            // If the destination directory doesn't exist, create it. 
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, true);
            }

            // If copying subdirectories, copy them and their contents to new location. 
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }

        #endregion

    }


  
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;
using System.ComponentModel;
using Ionic.Zip;

namespace ReportGen
{
    //Checks for updates.
    public class UpdateManager
    {

        #region Consts

        const string c_AppWeb = "AppWeb";
        const string c_DocWeb = "DocWeb";
        const string c_ManifestExt = ".man";
        const string c_NewFilePrefix = "new_";
        const string c_ZipExt = ".zip";

        const string c_UpdaterExe = "CTUpdater.exe";

        const string c_UpdatesURL = "http://www.axiomcomputing.com/ct/update/";
        const string c_AppURL = "app/";
        const string c_DocURL = "doc/";

        const string c_AppDir = @"App\";
        const string c_DocDir = @"Doc\";

        const string c_DocRoot = @"DocRoot\";
    

        #endregion

        #region Variables

        public static string bwMessage;

        #endregion

        #region Init / Load

        public UpdateManager()
        {
            Init();
        }

        private void Init()
        {
            bwMessage = "";
        }

        #endregion

        #region Check for Updates

        #region Download Manifests

        public static DownloaderArgs GetAppManArgs()
        {
            string updateFolder = G.GetUpdateFolder();

            string appManFileName = GetAppManFileName();
            string appManPath = GetAppManFilePath(true); //true, Save as new
            string appManURL = GetAppManURL();

            return new DownloaderArgs(appManURL, appManFileName, appManPath, DLReason.Manifest);
        }

        public static DownloaderArgs GetDocManArgs()
        {
            string updateFolder = G.GetUpdateFolder();

            string docManFileName = GetDocManFileName();
            string docManPath = GetDocManFilePath(true); //true, Save as new
            string docManURL = GetDocManURL();

            return new DownloaderArgs(docManURL, docManFileName, docManPath, DLReason.Manifest);
        }
        #endregion

        #region Download Update Zips

        /// <summary>
        /// Return a list of args to changed files to download
        /// </summary>
        /// <returns></returns>
        public static List<DownloaderArgs> DetermineUpdates(Dictionary<string, string> license)
        {
            #region Local Variables
            List<DownloaderArgs> dlArgs = new List<DownloaderArgs>();

            //get old app manifest.
            WebManifest oldAppMan = new WebManifest(GetAppManFilePath(false));
            Dictionary<int, WebPackage> oldAppPackages = new Dictionary<int, WebPackage>();
            WebManifest newAppMan = new WebManifest(GetAppManFilePath(true));

            WebManifest oldDocMan = new WebManifest(GetDocManFilePath(false));
            Dictionary<int, WebPackage> oldDocPackages = new Dictionary<int, WebPackage>();
            WebManifest newDocMan = new WebManifest(GetDocManFilePath(true));

            DownloaderArgs arg = null;
            string downloadURL = "";
            string fileName = "";
            string saveTo = "";

            #endregion

            #region Throw Error on no new Manifests
            if (newAppMan == null)
                throw new Exception("Unable to open new app manifest file.");
            if (newDocMan == null)
                throw new Exception("Unable to open new doc manifest file.");
            #endregion

            #region Populate Package Dictionaries
            if (oldAppMan.Packages != null)
                foreach (WebPackage pkg in oldAppMan.Packages)
                    oldAppPackages.Add(pkg.PID, pkg);

            if (oldDocMan.Packages != null)
                foreach (WebPackage pkg in oldDocMan.Packages)
                    oldDocPackages.Add(pkg.PID, pkg);
            #endregion

            #region Add App Files

            //Update all files to newest version
            if (newAppMan.Packages != null)
            {
                foreach (WebPackage pkg in newAppMan.Packages)
                {
                    fileName = pkg.Name + c_ZipExt;
                    saveTo = G.GetUpdateFolder() + c_AppDir + fileName;
                    downloadURL = c_UpdatesURL + c_AppURL + fileName;

                    arg = new DownloaderArgs(downloadURL, fileName, saveTo, DLReason.AppFile);

                    if (oldAppMan == null)
                        dlArgs.Add(arg);
                    else
                    {
                        //If this is an old package, check version.
                        if (oldAppPackages.ContainsKey(pkg.PID))
                        {
                            WebPackage oldPackage = oldAppPackages[pkg.PID];
                            WebPackage newPackage = pkg;

                            //Add to list since newer.
                            if (newPackage.Vers > oldPackage.Vers)
                                dlArgs.Add(arg);
                        }
                        else //It's new, add to download list.
                        {
                            dlArgs.Add(arg);
                        }
                    }
                }
            }

            #endregion

            #region Add Doc Files

            //Update all files to newest version
            if (newDocMan.Packages != null)
            {
                foreach (WebPackage pkg in newDocMan.Packages)
                {
                    fileName = pkg.Name + c_ZipExt;
                    saveTo = G.GetUpdateFolder() + c_DocDir + fileName;
                    downloadURL = c_UpdatesURL + c_DocURL + fileName;

                    arg = new DownloaderArgs(downloadURL, fileName, saveTo, DLReason.DocFile);

                    int[] permissions = UserLicense.GetDocPermissions();

                    if (oldDocMan == null)
                        dlArgs.Add(arg);
                    else
                    {
                        //If this is an old package, check version.
                        if (oldDocPackages.ContainsKey(pkg.PID))
                        {
                            WebPackage oldPackage = oldDocPackages[pkg.PID];
                            WebPackage newPackage = pkg;

                            //Add to list since newer.
                            if (newPackage.Vers > oldPackage.Vers)
                                dlArgs.Add(arg);
                        }
                        else //It's new, add to download list.
                        {
                            dlArgs.Add(arg);
                        }
                    }
                }
            }

            #endregion

            return dlArgs;
        }


        #endregion

        #region Report Updates Available

        /// <summary>
        ///  Returns true if any zip files exist in the App update folder.
        /// </summary>
        /// <returns></returns>
        public static bool HaveAppUpdates()
        {
            string appUpdatesFolderPath = G.GetUpdateFolder() + c_AppDir;

            if (!Directory.Exists(appUpdatesFolderPath))
                return false;

            string[] files = Directory.GetFiles(appUpdatesFolderPath, "*.zip", SearchOption.AllDirectories);

            if (files.Count() > 0)
                return true;
            else
                return false;

        }

        /// <summary>
        /// Returns true if any zip files exist in the Doc update folder.
        /// </summary>
        /// <returns></returns>
        public static bool HaveDocUpdates()
        {
            string docUpdatesFolderPath = G.GetUpdateFolder() + c_DocDir;

            if (!Directory.Exists(docUpdatesFolderPath))
                return false;

            string[] files = Directory.GetFiles(docUpdatesFolderPath, "*.zip", SearchOption.AllDirectories);

            if (files.Count() > 0)
                return true;
            else
                return false;
        }

        public static List<string> DocPackageNamesToUpdate()
        {
            string docUpdatesFolderPath = G.GetUpdateFolder() + c_DocDir;
            List<string> fileNames = new List<string>();

            if (!Directory.Exists(docUpdatesFolderPath))
                return fileNames;

            string[] files = Directory.GetFiles(docUpdatesFolderPath, "*.zip", SearchOption.AllDirectories);
            string fileName = "";

            foreach (string file in files)
            {
                fileName = Path.GetFileNameWithoutExtension(file);
                fileNames.Add(fileName);
            }

            return fileNames;
        }

        #endregion

        #endregion

        #region Do Updates

        public static int UpdateDocPackages(BackgroundWorker bw)
        {
            //Unzip packages.
            string rootDir = Application.StartupPath;
            string docRoot = rootDir + "\\" + c_DocRoot;
            string updateDir = G.GetUpdateFolder();
            string updateDocsDir = updateDir + c_DocDir;

            string userInputDir = G.GetInputRoot();
            string userTemplateDir = G.GetTemplatesRoot();

            int packagesUpdated = 0;
            int failedPackages = 0;
            bool errorOccured = false;
       
            //Get zips to unzip.
            List<string> zipList = Directory.GetFiles(updateDocsDir, "*.zip", SearchOption.TopDirectoryOnly).ToList(); ;
            List<string> packageFolderList = null;

            //Extract each zipfile.
            for (int i = 0; i < zipList.Count; i++)
            {
                errorOccured = false;

                string filePath = zipList[i];
                string packageName = Path.GetFileNameWithoutExtension(filePath);

                int progress = (int)(((double)i / (double)zipList.Count) * 100.0);
                bwMessage = "Extracting " + Path.GetFileNameWithoutExtension(filePath);
                bw.ReportProgress(progress);

                try
                {
                    ZipFile zipFile = new ZipFile(filePath);
                    zipFile.ExtractAll(rootDir, ExtractExistingFileAction.OverwriteSilently);
                    zipFile.Dispose();
                }
                catch (Exception ex)
                {
                    G.ErrorShow("Unable to unzip the package. The error says: " + ex.Message, "Package Update Error.");
                    errorOccured = true;
                    continue;
                }

                if (Directory.Exists(docRoot))
                    packageFolderList = Directory.GetDirectories(docRoot, "*.*", SearchOption.TopDirectoryOnly).ToList();
                else
                    continue;

                //For Sub Dir (Template, Input)
                foreach (string subDir in packageFolderList)
                {
                    string topDirName = subDir.Replace(docRoot, "");
                    //string packageDir = subDir + "\\" + packageName + "\\";

                    switch (topDirName)
                    {
                        case "Input":
                            //Move folder to Template/[PackageName]/                        
                            try { G.DirectoryCopy(subDir, userInputDir, true); }
                            catch (Exception) { errorOccured = true; }
                            break;

                        case "Template":
                            //Move folder to Input/[PackageName]/
                            try { G.DirectoryCopy(subDir, userTemplateDir, true); }
                            catch (Exception) { errorOccured = true; }
                            break;

                        default:
                            G.ErrorShow("Unexpected folder ( " + topDirName + " ) in Package " + packageName, "Package Error.");
                            break;
                    }
                }

                //Delete the DocRoot folder created from the unzipping process.
                try
                {
                    if (Directory.Exists(docRoot))
                        Directory.Delete(docRoot, true);
                }
                catch (Exception) { }

                if (!errorOccured)
                {
                    try { File.Delete(filePath); }
                    catch (Exception ex)
                    {
                        G.ErrorShow("Unable to delete the package zip. The error says: " + ex.Message,
                            "Unable to cleanup package update.");
                    }

                    packagesUpdated++;
                }
                else
                { 
                    failedPackages++; 
                }
            }

            //All packages successfully applied, rename Manifest new to old.
            if (failedPackages == 0)
            {
                string oldDocManifestPath = GetDocManFilePath(false);
                string newDocManifestPath = GetDocManFilePath(true);
                File.Copy(newDocManifestPath, oldDocManifestPath, true);
            }

            return packagesUpdated;                 
        }

        public static bool UpdateAppPackages(bool restart)
        {
            //Start updater
            string updaterPath = Application.StartupPath + "\\" + c_UpdaterExe;
            string ctProcessName = Process.GetCurrentProcess().ProcessName;
            ProcessStartInfo updateStart = null;

            if (restart) //Pass this process name to restart it after update.
                updateStart = new ProcessStartInfo(updaterPath, ctProcessName); 
            else
                updateStart = new ProcessStartInfo(updaterPath);

            try
            {
                Process.Start(updateStart);
                return true;
            }
            catch (Exception ex)
            {
                G.ErrorShow("Unable to start the updater. The error says, " + ex.Message, "Getting nowhere fast.");
            }

            return false;
        }

        #endregion

        #region Get Update Name and Paths

        #region App

        public static string GetAppManFileName()
        {
            return c_AppWeb + c_ManifestExt;
        }

        public static string GetAppManFilePath(bool isNew)
        {
            string updateFolder = G.GetUpdateFolder();

            if (isNew)
                return updateFolder + c_NewFilePrefix + GetAppManFileName();
            else
                return updateFolder + GetAppManFileName();
        }

        public static string GetAppManURL()
        {
            return c_UpdatesURL + c_AppURL + GetAppManFileName();
        }

        #endregion

        #region Doc


        public static string GetDocManFileName()
        {
            return c_DocWeb + c_ManifestExt;
        }

        public static string GetDocManFilePath(bool isNew)
        {
            string updateFolder = G.GetUpdateFolder();

            if (isNew)
                return updateFolder + c_NewFilePrefix + GetDocManFileName();
            else
                return updateFolder + GetDocManFileName();
        }

        public static string GetDocManURL()
        {
            return c_UpdatesURL + c_DocURL + GetDocManFileName();
        }


        #endregion

        #endregion

        #region Ensure Folders

        /// <summary>
        /// Ensures the existence of the update download folders.
        /// </summary>
        public static void EnsureUpdateFolders()
        {
            string updateDir = G.GetUpdateFolder();

            //Create Update Dir
            if (!Directory.Exists(updateDir))
                Directory.CreateDirectory(updateDir);

            //Create App Dir
            if (!Directory.Exists(updateDir + c_AppDir))
                Directory.CreateDirectory(updateDir + c_AppDir);

            //Create Doc Dir
            if (!Directory.Exists(updateDir + c_DocDir))
                Directory.CreateDirectory(updateDir + c_DocDir);
        }

        #endregion

        #region Update Updater

        /// <summary>
        /// Rename any files in the root dir beginning with new_
        /// </summary>
        public static void UpdateUpdater()
        {
            string rootDir = Application.StartupPath;
            SwapOldForNew(rootDir);
        }

        public static void SwapOldForNew(string rootDir)
        {
            string[] files = Directory.GetFiles(rootDir, "*.*");

            foreach (string file in files)
            {
                string fileName = Path.GetFileName(file);
                string oldName = "";
                string oldPath = "";

                if (fileName.StartsWith(c_NewFilePrefix))
                {
                    oldName = fileName.Replace(c_NewFilePrefix, "");
                    oldPath = rootDir + "\\" + oldName;

                    try
                    {
                        if (File.Exists(oldPath))
                            File.Delete(oldPath);

                        File.Move(file, oldPath);
                    }
                    catch (Exception ex)
                    {
                        G.ErrorShow("Unable to rename" + c_NewFilePrefix + oldName + 
                            "The error says, " + ex.Message, "Trouble.");
                    }
                }
            }
        }

        #endregion
    }
}

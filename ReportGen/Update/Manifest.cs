using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using Ionic.Zip;

namespace ReportGen
{

    #region Shared
    [Serializable]
    public class FileStats
    {
        public string FileName;
        public DateTime DateModified;

        #region Load / Init
        public FileStats()
        {
            Init();
        }

        public FileStats(string fileName, DateTime dateModified)
        {
            this.FileName = fileName;
            this.DateModified = dateModified;
        }

        public void Init()
        {
            this.FileName = "";
            this.DateModified = DateTime.MinValue;
        }

        #endregion
    }
    #endregion

    #region Local Manifest

    //Usable for Updating CT, or Updating Book/Template Packages
    [Serializable]
    public class Manifest
    {
        public string ProgName;
        public double Vers;

        [XmlIgnore]
        public DateTime Updated
        {
            get { return GetLastUpdated().DateModified; }
        }
        public string URL;
        public List<Package> Packages;

        public const double c_IncBy = .001;
        public const string c_Created = "Created: ";
        public const string c_Deleted = "Deleted: ";

        #region Load / Init

        public Manifest()
        {
            this.Init();
        }

        public Manifest(string filePath)
        {
            if (File.Exists(filePath))
            {
                this.Init();
                this.Deserialize(filePath);
            }
        }

        public void Init()
        {
            this.ProgName = "";
            this.Vers = 0.0;
            this.URL = "";
            this.Packages = new List<Package>();
        }

        #endregion

        #region Serialization

        public void Deserialize(string filePath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Manifest));
            using (FileStream stream = new FileStream(filePath, FileMode.Open))
            {
                object obj = serializer.Deserialize(stream);
                Manifest man = (Manifest)obj;

                this.Packages = man.Packages;
                this.ProgName = man.ProgName;
                this.URL = man.URL;
                this.Vers = man.Vers;
            }

        }

        public void Serialize(string savePath)
        {
            //Everytime we serialize, increment the version.
            Type typeLayout = typeof(Manifest);
            XmlSerializer manifestSerializer = new XmlSerializer(typeLayout);
            using (FileStream stream = new FileStream(savePath, FileMode.Create))
            {
                manifestSerializer.Serialize(stream, this);
            }
        }

        #endregion

        #region Misc

        private FileStats GetLastUpdated()
        {
            FileStats lastUpdated = new FileStats();

            foreach (Package pkg in Packages)
                if (pkg.Updated > lastUpdated.DateModified)
                    lastUpdated = pkg.Contents.GetMostRecent();

            return lastUpdated;
        }

        public Dictionary<string, Package> GetPackagesDictionary()
        {
            Dictionary<string, Package> dic = new Dictionary<string, Package>();

            foreach (Package pkg in Packages)
                dic.Add(pkg.Name, pkg);

            return dic;
        }

        public List<string> CreateNewPkgZips(ref Manifest oldManifest, string updatesDir)
        {
            List<string> zipActions = new List<string>();
            Dictionary<string, Package> dicNewMain = this.GetPackagesDictionary();
            Dictionary<string, Package> dicOldMain = null;

            //If oldManifest does not exist.
            //Create all packages.
            if (oldManifest == null)
            {
                foreach (string pkgName in dicNewMain.Keys)
                {
                    Package package = dicNewMain[pkgName];              
                    package.Zip(updatesDir);
                    zipActions.Add(c_Created + pkgName);
                }
                return zipActions;
            }
            else //
            {
                dicOldMain = oldManifest.GetPackagesDictionary();
            }

                //If there are updates, zip affected packages and update .man file.
                if (this.Updated > oldManifest.Updated)
                {
                    //Add new zips
                    foreach (string pkgName in dicNewMain.Keys)
                    {
                        //If new Package is not in the old manifest, zip it.
                        if (!dicOldMain.ContainsKey(pkgName))
                        {
                            dicNewMain[pkgName].Zip(updatesDir);
                            zipActions.Add(c_Created + pkgName);
                        }
                        else if (dicNewMain[pkgName].Updated > dicOldMain[pkgName].Updated)
                        {
                            dicNewMain[pkgName].Zip(updatesDir);
                            zipActions.Add(c_Created + pkgName);


                            //Increment Version number
                            int pkgID = dicNewMain[pkgName].PID;
                            double pkgVers = 0.0;

                            foreach (Package pkg in oldManifest.Packages)
                            {
                                if (pkg.PID == pkgID)
                                {
                                    pkg.Vers += c_IncBy; //Increment the old pkg Vers.
                                    pkgVers = pkg.Vers; //Store version to pass to new manifest in next loop.
                                    break;
                                }

                            } 

                            foreach (Package pkg in this.Packages)
                            {
                                if (pkg.PID == pkgID)
                                {
                                    pkg.Vers = pkgVers; //Set version in new Manifest Package.
                                    break;
                                }
                            }


                                                       
                        }                     
                    }

                    //Remove old zips
                    foreach (string pkgName in dicOldMain.Keys)
                    {
                        //If old name not in new manifest, delete the zip.
                        if (!dicNewMain.ContainsKey(pkgName))
                        {
                            dicOldMain[pkgName].TryDeleteZip(updatesDir);
                            zipActions.Add(c_Deleted + pkgName);
                        }                        
                    }

                    //Finally, increment entire Manifest Version.
                    oldManifest.Vers += c_IncBy;
                    this.Vers = oldManifest.Vers;
                }               

            return zipActions;
        }


        #endregion

    }

    //Expresses a CT program component, or Book Package.
    [Serializable]
    public class Package
    {
        public int PID;
        public string Name;
        public double Vers;
        public string SaveName;
        public DirStruct Contents;

        [XmlIgnore]
        public DateTime Updated
        {
            get { return Contents.GetMostRecent().DateModified; }
        }

        public const double c_IncBy = .001;

        #region Load Init

        public Package()
        {
            Init();
        }

        public Package(
            int pid,
            string name,
            double vers,
            string savename,
            DirStruct contents)
        {
            this.PID = pid;
            this.Name = name;
            this.Vers = vers;
            this.SaveName = savename;
            this.Contents = contents;
        }

        public void Init()
        {
            PID = -1;
            Name = "";
            Vers = 1.0;
            SaveName = "";
            Contents = new DirStruct();
        }

        #endregion

        #region Misc

        //Zips the contents of this package to the updatesDir location
        public bool Zip(string updatesDir)
        {
            bool success = false;
            string savePath = updatesDir + SaveName + ".zip";

            //Workaround for Zip's stupid prevention of saving.
            if (File.Exists(savePath))
                File.Delete(savePath);

            string zipRoot = string.Empty;
            string subDir = this.Contents.Root;
            string appRootStr = "Debug";
            string docRootStr = "Cassies Timesaver";

            if (this.Contents.Root.Contains(appRootStr))
            {
                int remTo = subDir.IndexOf(appRootStr) + appRootStr.Length;
                zipRoot = @"AppRoot" + this.Contents.Root.Remove(0, remTo);
            }

            using (ZipFile zip = new ZipFile(savePath, System.Console.Out))
            {
                //Zip the Input Dir and Template Dir of this Package name.
                if (Contents.Dirs.Count > 0)
                {
                    foreach (DirStruct dir in Contents.Dirs)
                    {
                        if (dir.Root.Contains(docRootStr))
                        {
                            int remTo = subDir.IndexOf(docRootStr) + docRootStr.Length;
                            zipRoot = @"DocRoot" + dir.Root.Remove(0, remTo);
                        }

                        zip.AddItem(dir.Root, zipRoot);
                    }
                }
               

                foreach (FileStats file in Contents.Files)
                {
                    string filePath = Contents.Root + file.FileName;
                    string zipPath = (zipRoot + file.FileName).Replace('\\', '/');

                    if (!zip.EntryFileNames.Contains(zipPath))
                        zip.AddItem(filePath, zipRoot);
                }

                //Remove illegal files.
                foreach (ZipEntry entry in zip.Entries)
                    if (entry.FileName.StartsWith("~"))
                    {
                        zip.RemoveEntry(entry);
                    }

                zip.Save(savePath);
            }
            
            return success;
        }

        //Tries to delete an update package zip in the updatesDir
        public bool TryDeleteZip(string updatesDir)
        {
            string deletePath = Contents.Root + SaveName + ".zip";

            if (File.Exists(deletePath))
            {
                File.Delete(deletePath);
                return true;
            }

            return false;
        }

        #endregion
    }

    [Serializable]
    public class DirStruct
    {
        #region Variables
        public string Root;
        public List<FileStats> Files;
        public List<DirStruct> Dirs;
        #endregion

        #region Load/Init
        public DirStruct()
        {
            Init();
        }

        public DirStruct(string currentDir, string[] extensions)
        {
            Init();

            DirStruct rootDir = PopulateDirStruct(currentDir, extensions);

            this.Root = currentDir;
            this.Dirs = rootDir.Dirs;
            this.Files = rootDir.Files;
        }

        public DirStruct PopulateDirStruct(string currentDir, string[] extensions)
        {
            DirStruct thisDir = new DirStruct();
            thisDir.Root = currentDir;

            DirectoryInfo dir = new DirectoryInfo(currentDir);
            DirectoryInfo[] subDirs = dir.GetDirectories();
            List<FileInfo> files = new List<FileInfo>();

            //Discover all files of each extension in this directory.
            for (int i = 0; i < extensions.Length; i++)
            {
                FileInfo[] foundFiles = dir.GetFiles("*." + extensions[i], SearchOption.TopDirectoryOnly);
                files.AddRange(foundFiles);
            }

            //Add all found files to DirStruct object.
            foreach (FileInfo file in files)
            {
                FileStats fileStat = new FileStats(file.Name, file.LastWriteTime);
                thisDir.Files.Add(fileStat);
            }
            
            //Recurse into subDirs
            foreach (DirectoryInfo subDir in subDirs)
            {
                DirStruct subDirStruct = PopulateDirStruct(subDir.FullName, extensions);
                thisDir.Dirs.Add(subDirStruct);
            }

            return thisDir;

        }

        public void Init()
        {
            this.Files = new List<FileStats>();
            this.Dirs = new List<DirStruct>();
        }

        #endregion

        #region Misc

        public FileStats GetMostRecent()
        {
            FileStats mostRecent = new FileStats();

            foreach (FileStats file in this.Files)
                if (file.DateModified > mostRecent.DateModified)
                    mostRecent = file;

            foreach (DirStruct subDir in this.Dirs)
            {
                FileStats subMostRecent = subDir.GetMostRecent();

                if (subMostRecent.DateModified > mostRecent.DateModified)
                    mostRecent = subMostRecent;
            }

            return mostRecent;
        }

        #endregion

    }

    #endregion

    #region Web Manifest

    [Serializable]
    public class WebManifest
    {
        public double Vers;
        public DateTime Updated;
        public string URL;
        public List<WebPackage> Packages;

        #region Load Init

        public WebManifest()
        {
            Init();
        }

        public WebManifest(Manifest localManifest)
        {
            Init();
            PopulateFromLocalManifest(localManifest);
        }

        public WebManifest(string filePath)
        {
            if (File.Exists(filePath))
                Deserialize(filePath);
        }


        public void PopulateFromLocalManifest(Manifest localManifest)
        {
            this.Vers = localManifest.Vers;
            this.Updated = localManifest.Updated;
            this.URL = localManifest.URL;

            foreach (Package pkg in localManifest.Packages)
            {
                WebPackage webPack = new WebPackage(pkg);
                this.Packages.Add(webPack);
            }
        }



        private void Init()
        {
            Vers = 0;
            Updated = DateTime.MinValue;
            Packages = new List<WebPackage>();
            URL = "";
        }

        #region Serialization

        public void Serialize(string savePath)
        {
            Type typeLayout = typeof(WebManifest);
            XmlSerializer manifestSerializer = new XmlSerializer(typeLayout);
            using (FileStream stream = new FileStream(savePath, FileMode.Create))
            {
                manifestSerializer.Serialize(stream, this);
            }
        }

        public void Deserialize(string filePath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(WebManifest));
            using (FileStream stream = new FileStream(filePath, FileMode.Open))
            {
                object obj = serializer.Deserialize(stream);
                WebManifest man = (WebManifest)obj;

                this.Packages = man.Packages;
                this.Vers = man.Vers;
                this.Updated = man.Updated;
            }

        }

        #endregion

        #endregion
    }

    [Serializable]
    public class WebPackage
    {
        public int PID;
        public double Vers;
        public DateTime Updated;
        public string Name;

        #region Load Init

        public WebPackage()
        {
            Init();
        }

        public WebPackage(Package localPack)
        {
            this.PID = localPack.PID;
            this.Vers = localPack.Vers;
            this.Updated = localPack.Updated;
            this.Name = localPack.Name;
        }

        private void Init()
        {
            PID = -1;
            Vers = 0;
            Updated = DateTime.MinValue;
            Name = string.Empty;
        }

        #endregion
    }

    #endregion


}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using Ionic.Zip;
using System.Net;
using System.Net.FtpClient;
using ReportGen;

namespace OnBuild
{
    class Program
    {

        /// <summary>
        /// Remember to Define new Template Packages in GetNamesIds() <--!!
        /// </summary>

        public const string c_ProgName = "Cassie's TimeSaver";


        public const string c_NewFilePrefix = "new_";

        public const string c_App = "App";
        public const string c_Doc = "Doc";
        public const string c_Web = "Web";
        public const string c_ManifestFileExt = "man";


        public const string c_UpdateURL = "/ct/update/";

        public const string c_CTBuildPath = @"C:\Users\Christopher\Documents\Visual Studio 2010\Projects\ReportGen\ReportGen\bin\Debug\";
        public const string c_CTUpdaterBuildPath = @"C:\Users\Christopher\Documents\Visual Studio 2010\Projects\ReportGen\CTUpdater\bin\Debug\";
        public const string c_CTUpdatesPath = @"C:\Users\Christopher\Documents\Visual Studio 2010\Projects\ReportGen\ReportGen\Updates\";
        public const string c_CTDocPath = @"C:\Users\Christopher\Documents\Cassies Timesaver\";

        public const string c_CTUpdaterEXE = "CTUpdater.exe";

        //CT Application Directory Folders
        public const string c_Images = @"Images\";
        public const string c_Documentation = @"Documentation\";
        public const string c_Updates = @"Updates\";

       
        //CT User Documents Folders
        public const string c_Input = @"Input\";
        public const string c_Output = @"Output\";
        public const string c_Template = @"Template\";
        

        static void Main(string[] args)
        {

            EnsureUpdateFolders();

            bool foundChanges = false;

            while (true)
            {
               Console.WriteLine("Press ESC to Exit or ENTER to run Cassie's Timesaver modernization.");
               ConsoleKeyInfo response = Console.ReadKey();

                if (response.Key == ConsoleKey.Escape)
                {
                    Console.WriteLine("Skipping Modernization.");
                    return;
                }
                else if (response.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine("Running CT Modernization.");
                    List<string> mainChanges = ModernizeApplication();
                    List<string> tempChanges = ModernizeTemplates();

                    Console.WriteLine();
                    Console.WriteLine();

                    if (mainChanges.Count == 0)
                        Console.WriteLine("No changes in Primary CT Application.");
                    else
                    {
                        foundChanges = true;
                        Console.WriteLine("Updated the following Primary Application Packages: ");
                        foreach (string changeStr in mainChanges)
                            Console.WriteLine(changeStr);
                    }

                    Console.WriteLine();
                    Console.WriteLine();

                    if (tempChanges.Count == 0)
                        Console.WriteLine("No changes in CT Template Packages.");
                    else
                    {
                        foundChanges = true;
                        Console.WriteLine("Updated the following Template Packages: ");
                        foreach (string changeStr in tempChanges)
                            Console.WriteLine(changeStr);
                    }


                    Console.WriteLine();
                    Console.WriteLine();

                    if (!foundChanges)
                    {
                        Console.WriteLine("No changes. Update All Files? ESC = No | ENTER = Yes");

                        while (true)
                        {
                            response = Console.ReadKey();

                            if (response.Key == ConsoleKey.Escape)
                                return;
                            else
                            {
                                //Upload All Update Files to the server.

                                Console.WriteLine("Write this function.");
                                Console.Read();
                                return;
                            }
                        }
                    }


                    while (foundChanges)
                    {
                        Console.WriteLine("Would you like to push all files to the Update Server?");
                        Console.WriteLine("ESC = NO | ENTER = YES");
                        response = Console.ReadKey();


                        if (response.Key == ConsoleKey.Escape)
                        {
                            return;
                        }
                        else if (response.Key == ConsoleKey.Enter)
                        {
                            Console.WriteLine("Pushing Changes to " + c_UpdateURL);
                            string ErrCode = PushChangesToUpdateServer(mainChanges, tempChanges);

                            if (ErrCode != "")
                            {
                                Console.WriteLine("Error uploading changes to Update Server. The message says: ");
                                Console.WriteLine();
                                Console.WriteLine(ErrCode);
                            }
                            else
                            {
                                Console.WriteLine("Upload success at " + DateTime.Now.ToLocalTime());
                                Console.Read();
                            }


                            return;
                        }

                    }                
                }
            }
        }


        #region Manage Updates

        public static List<string> GetZipFileNames(List<string> changeMessages, string inDir)
        {
            List<string> fileNames = new List<string>();

            foreach (string message in changeMessages)
            {
                string name = message.Remove(0, message.IndexOf(' ') + 1);
                fileNames.Add(inDir + name.Replace(' ', '_') + ".zip");
            }

            return fileNames;

        }

        public static void UploadFiles(List<string> changesList, string subUpdate)
        {

            int buffLen = 4096;
            byte[] buff = new byte[buffLen];

            Console.WriteLine("Uploading changes to Web App Directory.");
            string subDir = c_CTUpdatesPath + subUpdate + "\\";
            string manifestPath = subDir + subUpdate + c_Web + "." + c_ManifestFileExt;
            List<string> subFilesToUpload = GetZipFileNames(changesList, subDir);
            //Add App Web Manifest.
            subFilesToUpload.Add(manifestPath);

            foreach (string filePath in subFilesToUpload)
            {
                string webPath = "ftp://ftp.axiomcomputing.com/" + subUpdate.ToLower() + "/" + Path.GetFileName(filePath);
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(webPath);
                request.Method = WebRequestMethods.Ftp.UploadFile;
                request.Credentials = new NetworkCredential("updater", "brA*e*+Y2fREn56h");

                FileInfo uploadFile = new FileInfo(filePath);
                FileStream fStream = uploadFile.OpenRead();
                int contentLen = fStream.Read(buff, 0, buffLen);
                long fileLength = fStream.Length;
                long progress = 0;


                request.ContentLength = contentLen;

                Stream upStream = request.GetRequestStream();

                while (contentLen != 0)
                {
                    upStream.Write(buff, 0, contentLen);
                    contentLen = fStream.Read(buff, 0, buffLen);
                    progress += contentLen;
                    Console.WriteLine("Sending " + Path.GetFileName(filePath) + " " +
                       progress + " of " + fileLength);
                }

                upStream.Close();
                fStream.Dispose();

                FtpWebResponse response = (FtpWebResponse)request.GetResponse();

                Console.WriteLine("Uploaded File Status: {0}", response.StatusDescription);
                response.Close();
            }
        }

        public static string PushChangesToUpdateServer(List<string> mainChanges, List<string> tempChanges)
        {
            string ErrCode = "";
            

            //Push Main Changes
            #region App Section
            if (mainChanges.Count > 0)
            {
                UploadFiles(mainChanges, c_App);
                Console.WriteLine("Finished Uploading App Files.");
            }
            #endregion

            //Push Template Changes
            #region Doc Section
            if (tempChanges.Count > 0)
            {
                UploadFiles(tempChanges, c_Doc);
                Console.WriteLine("Finished Uploading Doc Files.");
            }

            #endregion

            return ErrCode;
        }


        public static List<string> ModernizeApplication()
        {
            Manifest manOldMain = null;
            Manifest manNewMain = GetMainManifest();
            string manPath = c_CTUpdatesPath + c_App + "\\" + c_App + "." + c_ManifestFileExt;
            string webManPath = c_CTUpdatesPath + c_App + "\\" + c_App + c_Web + "." + c_ManifestFileExt; 

            if (File.Exists(manPath)) 
                manOldMain = new Manifest(manPath);

            List<string> mainChanges = 
                manNewMain.CreateNewPkgZips(ref manOldMain, c_CTUpdatesPath + c_App.ToLower() + "\\");

            if (mainChanges.Count > 0)
            {

                //Save local manifest.
                manNewMain.Serialize(manPath);

                //Save web manifest
                WebManifest webMan = new WebManifest(manNewMain);
                webMan.Serialize(webManPath);

            }

            //Copy the web manifest to the Primary CT bin\updates\ folder so it gets wrapped up in the install package.
            if (File.Exists(webManPath))
            {
                string primaryUpdatesBinPath = c_CTBuildPath + c_Updates + Path.GetFileName(webManPath);
                File.Copy(webManPath, primaryUpdatesBinPath, true);
            }

            return mainChanges;
        }

        public static List<string> ModernizeTemplates()
        {
            Manifest manOldTemp = null;
            Manifest manNewTemp = GetTemplatesManifest();
            string manPath = c_CTUpdatesPath + c_Doc + "\\" + c_Doc + "." + c_ManifestFileExt;
            string webManPath = c_CTUpdatesPath + c_Doc + "\\" + c_Doc + c_Web + "." + c_ManifestFileExt; 

            if (File.Exists(manPath)) 
                manOldTemp = new Manifest(manPath);

            List<string> tempChanges = 
                manNewTemp.CreateNewPkgZips(ref manOldTemp, c_CTUpdatesPath + c_Doc + "\\");

            if (tempChanges.Count > 0)
            {
                manNewTemp.Serialize(manPath);
                WebManifest webMan = new WebManifest(manNewTemp);
                webMan.Serialize(webManPath);
             }
                
            return tempChanges;
        }

        #endregion

        #region Get Manifests

        static Manifest GetMainManifest()
        {           
            Package Executable = GetExecutable();
            Package Libraries = GetLibraries();
            Package Images = GetImages();
            Package Documentation = GetDocumentation();

            Manifest ctCurMan = new Manifest();
            ctCurMan.ProgName = c_ProgName;
            ctCurMan.URL = c_UpdateURL + c_App + "/";
            ctCurMan.Packages.Add(Executable);
            ctCurMan.Packages.Add(Libraries);
            ctCurMan.Packages.Add(Images);
            ctCurMan.Packages.Add(Documentation);

            return ctCurMan;
        }

        /// <summary>
        /// The official list of Template Names and their ID numbers.
        /// </summary>
        /// <returns></returns>
        static Dictionary<string, int> GetNamesIds()
        {
            Dictionary<string, int> NameId = new Dictionary<string, int>();

            NameId.Add("Psych IEP", 100);

            return NameId;
        }

        /// <summary>
        /// Gets the Manifest of the Template Packages from my HDD.
        /// </summary>
        /// <returns></returns>
        static Manifest GetTemplatesManifest()
        {
            Manifest tempMan = new Manifest();
            string makeManifestDir = c_CTDocPath + c_Input;
            Dictionary<string, int> nameIDs = GetNamesIds();
            DirectoryInfo inputDir = new DirectoryInfo(makeManifestDir);
            DirectoryInfo[] packageDirs = inputDir.GetDirectories();

            int undefinedPackageID = 1000; //Any packages without a NameID gets a temp package ID.

            foreach (DirectoryInfo dir in packageDirs)
            {
                Package package = new Package();

                //Designate pre-defined Package names.
                if (nameIDs.ContainsKey(dir.Name))
                    package.PID = nameIDs[dir.Name];
                else
                    package.PID = undefinedPackageID++;

                package.Name = dir.Name;
                package.SaveName = dir.Name.Replace(' ', '_'); //Replace Filename spaces.
                package.Contents = PopulateTemplateContents(dir.Name); //Gets directory structure.                 
                tempMan.Packages.Add(package);

            }


            return tempMan;
        }

        #endregion

        #region Get CT Packages

        /// <summary>
        /// Returns a DirStruct containing the named directory structure of Input and Template
        /// </summary>
        /// <param name="templateName"></param>
        /// <returns></returns>
        static DirStruct PopulateTemplateContents(string templateName)
        {
            DirStruct root = new DirStruct();
            string[] inputTypes = new string[] {"pag", "bok"};
            string[] templateTypes = new string[] {"docx"};
            root.Root = c_CTDocPath;

            //Add input Dir for templateName
            DirStruct inputDir = new DirStruct(c_CTDocPath + c_Input + templateName + "\\", inputTypes);
            root.Dirs.Add(inputDir);

            //Add template Dir for templateName
            DirStruct templateDir = new DirStruct(c_CTDocPath + c_Template + templateName + "\\", templateTypes);
            root.Dirs.Add(templateDir);

            //No files should be added in root.Files.
            //It should only contain the folders Template and Input, then each will contain the folder templateName.

            return root;
        }

        //0
        static Package GetExecutable()
        {
            string[] ignoreString = new string[] { "vshost" };
            FileInfo[] updater = GetAllofTypeIn("exe", ignoreString, c_CTUpdaterBuildPath);
            FileInfo[] primary = GetAllofTypeIn("exe", ignoreString, c_CTBuildPath);
            List<FileInfo> tempFiles = new List<FileInfo>();

            //Copy updater exe to primary output path.
            foreach (FileInfo file in updater)
            {
                string newName = c_NewFilePrefix + file.Name;
                string newPath = c_CTBuildPath + newName;

                if (File.Exists(newPath))
                    File.Delete(newPath);

                File.Copy(file.FullName, newPath, true);
                FileInfo newFile = new FileInfo(newPath);
                tempFiles.Add(newFile);
            }

            foreach (FileInfo file in primary)
                tempFiles.Add(file);

            FileInfo[] files = tempFiles.ToArray();

            
            Package package = new Package();

            package.PID = 0;
            package.Name = "Executable";
            package.SaveName = package.Name;
            package.Contents.Root = c_CTBuildPath;


            foreach (FileInfo file in files)
            {
                FileStats stats = new FileStats(file.Name, file.LastWriteTime);

                //Ignore CTUpdate.exe (new_CTUpdater.exe is the proper exe to add.)
                if (file.Name == c_CTUpdaterEXE)
                {
                    Console.WriteLine("Ignored " + c_CTUpdaterEXE);
                    continue;
                }

                package.Contents.Files.Add(stats);                
            }

            return package;
        }

        //1
        static Package GetLibraries()
        {
            FileInfo[] files = GetAllofTypeIn("dll", c_CTBuildPath);

            Package package = new Package();

            package.PID = 1;
            package.Name = "Libraries";
            package.SaveName = package.Name;
            package.Contents.Root = c_CTBuildPath;

            foreach (FileInfo file in files)
            {
                FileStats stats = new FileStats(file.Name, file.LastWriteTime);

                package.Contents.Files.Add(stats);
            }

            return package;
        }

        //2
        static Package GetImages()
        {
            Package package = new Package();
            string[] types = new string[] { "ico", "jpg" };
            string imagesDir = c_CTBuildPath + c_Images;

            foreach (string type in types)
            {
                FileInfo[] files = GetAllofTypeIn(type, imagesDir);

                foreach (FileInfo file in files)
                {
                    FileStats stats = new FileStats(file.Name, file.LastWriteTime);
                    package.Contents.Files.Add(stats);
                }
            }

            package.PID = 2;
            package.Name = "Images";
            package.SaveName = package.Name;
            package.Contents.Root = imagesDir;

            return package;
        }

        //3
        static Package GetDocumentation()
        {
            Package package = new Package();
            string documentationDir = c_CTBuildPath + c_Documentation;
            string[] ignoreString = new string[] { "~$" };
            FileInfo[] files = GetAllofTypeIn("docx", ignoreString, documentationDir);
     
            package.PID = 3;
            package.Name = "Documentation";
            package.SaveName = package.Name;
            package.Contents.Root = documentationDir;

            foreach (FileInfo file in files)
            {
                FileStats stats = new FileStats(file.Name, file.LastWriteTime);
                package.Contents.Files.Add(stats);
            }

            return package;
        }

        #endregion

        #region Get Template Packages

        

        #endregion

        #region Misc

        static void EnsureUpdateFolders()
        {
            if (!Directory.Exists(c_CTUpdatesPath))
                Directory.CreateDirectory(c_CTUpdatesPath);

            if (!Directory.Exists(c_CTUpdatesPath + c_App + "\\"))
                Directory.CreateDirectory(c_CTUpdatesPath + c_App + "\\");

            if (!Directory.Exists(c_CTUpdatesPath + c_Doc + "\\"))
                Directory.CreateDirectory(c_CTUpdatesPath + c_Doc + "\\");
        }

        static FileInfo[] GetAllofTypeIn(string fileType, string directory)
        {

            fileType = fileType.TrimStart('.');

            if (Directory.Exists(directory))
            {
                DirectoryInfo dirInfo = new DirectoryInfo(directory);
                FileInfo[] files = dirInfo.GetFiles("*." + fileType);
                return files;
            }

            return null;
        }

        static FileInfo[] GetAllofTypeIn(string fileType, string[] ignoreString, string directory)
        {

            if (Directory.Exists(directory))
            {
                bool ignore = false;
                DirectoryInfo dirInfo = new DirectoryInfo(directory);
                FileInfo[] files = dirInfo.GetFiles("*." + fileType);
                List<FileInfo> fileList = new List<FileInfo>();

                foreach (FileInfo file in files)
                {
                    ignore = false;

                    foreach (string ignoreStr in ignoreString)
                    {
                        if (file.Name.Contains(ignoreStr))
                        {
                            ignore = true;
                            break;
                        }
                    }

                    if (!ignore)
                        fileList.Add(file);                  
                }

                return fileList.ToArray();
            }

            return null;
        }


        #endregion

    }
}

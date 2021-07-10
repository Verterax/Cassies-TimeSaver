using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Data;
using System.Drawing;
using System.Data.SqlServerCe;
using System.Windows.Forms;
using System.IO;
using System.Reflection;

namespace ReportGen
{
    public static class G
    {
        #region Consts

        public const string c_ProgName = "Cassies TimeSaver";
        public const string c_CTManual = "Welcome to CT.docx";
        public const string c_IOTZipFile = "IOT Folders.zip";
        public const string c_Globals = "Globals";
        public const string c_Page_Hierarchy = @"Input\";
        public const string c_UpdateFolder = @"Updates\";
        public const string c_Templates_Folder_Name = @"Templates\";
        public const string c_Output_Folder_Name = @"Output\";
        public const string c_DocumentationFolder = @"Documentation\";
        public const string c_BookFolder_Name = @"Pages\Books\";

        public const string c_ImagesFolder = @"Images\";
        public const string c_defaultDir = @"C:\";
        public const string c_SelectFrom = "SELECT * FROM ";
        public const string c_NoErr = "0";
        public const string c_Cancel = "Cancel";
        public const string c_ErrCode = "Err0r";
        public const float c_ZoomStep = 0.5f;
        
        public const string c_DataSource = "Data Source = ";
        public const string c_MIN_DATE = "10/10/1950";
        
        public const string c_EmptyRead = "EMPTY";
        public const string c_DefaultFont = "Microsoft Sans Serif";

        public const string c_PageExt = ".pag";
        public const string c_BookExt = ".bok";
        public const string c_TemplateExt = ".docx";

        public const string c_defaultPageIconPath = "ReportGen.Images.page_icon1.png";
        public const string c_defaultBookIconPath = "ReportGen.Images.col_icon1.png";
        public const string c_defaultTemplateIconPath = "ReportGen.Images.docx_icon.png";
        public const string c_defaultFolderIconPath = "ReportGen.Images.emptyFolder_icon.jpg";
        public const string c_defaultImportIcon = "ReportGen.Images.downArrow_icon.png";
        //public const string c_defaultLoadBookIcon = "ReportGen.Images.openFile_Icon.png";
        public const string c_defaultOpenFileIcon = "ReportGen.Images.openFile_Icon.png";
        public const string c_defaultDesignerIcon = "ReportGen.Images.rulerCompass_icon.png";
        public const string c_defaultDocGenIcon = "ReportGen.Images.docGen_icon.png";
        public const string c_defaultSelectIcon = "ReportGen.Images.selectDoc_icon.png";
        public const string c_defaultBlueArrowIcon = "ReportGen.Images.downArrowBlue_icon.png";
        public const string c_defaultSaveIcon = "ReportGen.Images.save_icon.png";
        public const string c_defaultNewBookIcon = "ReportGen.Images.newBook_icon.png";
        public const string c_defaultExitIcon = "ReportGen.Images.exit_icon.png";

        #endregion

        #region Static Variables

        public static int tierCount = 0;
        public static Image folderIcon;
        public static Image pageIcon;
        public static Image bookIcon;
        public static Image docxIcon;
        public static Image importIcon;
        public static Image loadBookIcon;
        public static Image loadPageIcon;
        public static Image designerIcon;
        public static Image docGenIcon;
        public static Image selectIcon;
        public static Image newBookIcon;
        public static Image saveIcon;       
        public static Image exitIcon;
        public static Hashtable iconsInfo;


        #endregion

        #region Init

        public static void Init()
        {
            PopulateIconVariables();
        }

        public static void PopulateIconVariables()
        {
            iconsInfo = RegisteredFileType.GetFileTypeAndIcon();
            folderIcon = GetFolderIcon();
            pageIcon = GetPageIcon();
            bookIcon = GetBookIcon();
            docxIcon = GetTemplateIcon();
            importIcon = GetDownArrowBlueIcon();
            loadPageIcon = GetDownArrowOrangeIcon();
            loadBookIcon = GetLoadBookIcon();
            designerIcon = GetCompassRulerIcon();
            selectIcon = GetSelectIcon();
            docGenIcon = GetDocGenIcon();
            newBookIcon = GetNewBookIcon();
            saveIcon = GetSaveIcon();
            exitIcon = GetExitIcon();
        }



        #endregion

        #region Remembered Files / Directories

        public static string DefaultDir
        {
            set {  Properties.Settings.Default.defaultDir = value;
            Properties.Settings.Default.Save();}
            get {  return Properties.Settings.Default.defaultDir; }
        }

        public static string PagesDir
        {
            set
            {
                Properties.Settings.Default.formFuncDir = value;
                Properties.Settings.Default.Save();
            }
            get 
            {
                if (Properties.Settings.Default.formFuncDir == "null")
                    return GetInputRoot();
                else
                    return Properties.Settings.Default.formFuncDir; 
            }
        }



        public static string TemplatesDir
        {
            set
            {
                Properties.Settings.Default.templatesDir = value;
                Properties.Settings.Default.Save();
            }
            get {
                if (Properties.Settings.Default.templatesDir == "null")
                    return GetTemplatesRoot();
                else
                    return Properties.Settings.Default.templatesDir; }
        }

        public static string DocReaderDir
        {
            set
            {
                Properties.Settings.Default.docReaderDir = value;
                Properties.Settings.Default.Save();
            }
            get { return Properties.Settings.Default.docReaderDir; }
        }

        public static string DocMergeDir
        {
            set
            {
                Properties.Settings.Default.docMergeTemplates = value;
                Properties.Settings.Default.Save();
            }
            get { return Properties.Settings.Default.docMergeTemplates; }
        }

        public static string OutputDir
        {
            set
            {
                Properties.Settings.Default.outputFolder = value;
                Properties.Settings.Default.Save();
            }
            get {
                if (Properties.Settings.Default.outputFolder == "null")
                    return GetOutputRoot();
                else
                    return Properties.Settings.Default.outputFolder;}
        }

        public static string LastTemplatePath
        {
            get
            {
                if (Properties.Settings.Default.lastTemplatePath == c_defaultDir)
                    return G.GetTemplatesRoot();
                else
                    return Properties.Settings.Default.lastTemplatePath;
            }
            set
            {
                Properties.Settings.Default.lastTemplatePath = value;
                Properties.Settings.Default.Save();
            }
            
        }

        public static string LastBookPath
        {
            get
            {
                if (Properties.Settings.Default.lastBookPath == c_defaultDir)
                    return G.GetInputRoot();
                else
                    return Properties.Settings.Default.lastBookPath; 
            }
            set
            {
                Properties.Settings.Default.lastBookPath = value;
                Properties.Settings.Default.Save();
            }
        }
        
        

        #endregion

        #region Dialog Boxes

        public static bool ErrorShow(string mainError, string shortError)
        {
            if (mainError != string.Empty && mainError != "")
            {
                MessageBox.Show(mainError, shortError, MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return true;
            }

            return false;
        }

        public static void MessageShow(string mainMessage, string shortMessage)
        {
            MessageBox.Show(mainMessage, shortMessage, MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        public static DialogResult YesNoQuestion(string longQuestion, string shortQuestion)
        {
            return MessageBox.Show(longQuestion, shortQuestion, MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Question, MessageBoxDefaultButton.Button2); 
        }

        public static string SelectFillFromFile()
        {
            string filePath = string.Empty;

            OpenFileDialog openDlg = new OpenFileDialog();

            openDlg.Title = "Select File to Fill From";
            openDlg.InitialDirectory = DocReaderDir;
            openDlg.Filter = "Any File (*.*) | *.*";
            openDlg.FileName = "";

            DialogResult result = openDlg.ShowDialog();

            if (result != DialogResult.Cancel)
            {
                filePath = openDlg.FileName;
                DocReaderDir = G.GetDir(filePath);
            }

            return filePath;
        }

        public static string SelectFNFSavePath(FormAndFuncs ff)
        {
            string filePath = string.Empty;

            SaveFileDialog saveDlg = new SaveFileDialog();

            if (ff != null)
            {
                string fileName = G.CleanFileName(ff.Text);
                if (fileName != string.Empty)
                    saveDlg.FileName = fileName;
                else
                    saveDlg.FileName = "New Page";
            }

            saveDlg.AddExtension = true;
            saveDlg.DefaultExt = c_PageExt.TrimStart('.');
            saveDlg.Filter = "(*" + c_PageExt + ") | *" + c_PageExt;
            saveDlg.OverwritePrompt = true;
            saveDlg.Title = "Save Input Page As...";
            saveDlg.ValidateNames = true;

            DialogResult result = saveDlg.ShowDialog();

            if (result != DialogResult.Cancel)
            {
                filePath = saveDlg.FileName;               
            }

            return filePath;
        }

        public static string SelectFNFLoadPath()
        {
            string filePath = string.Empty;
            string initialDir = G.PagesDir;
            OpenFileDialog fnfOpen = new OpenFileDialog();

            if (initialDir == c_defaultDir)
                initialDir = G.GetInputRoot();

            fnfOpen.InitialDirectory = initialDir;
            fnfOpen.Title = "Select an Input Page File.";
            fnfOpen.FileName = "";
            fnfOpen.Filter = "Input Page (*" + c_PageExt + ")|*" + c_PageExt;
            fnfOpen.CheckFileExists = true;
            fnfOpen.ValidateNames = true;

            DialogResult result = fnfOpen.ShowDialog();

            if (result != DialogResult.Cancel)
            {
                filePath = fnfOpen.FileName;
                G.PagesDir = G.GetDir(filePath);
            }

            return filePath;
        }

        public static string SelectTemplatePath()
        {
            string filePath = string.Empty;
            string initialDir = G.TemplatesDir;
            OpenFileDialog templateSelect = new OpenFileDialog();

            if (initialDir == c_defaultDir)
                initialDir = G.GetTemplatesRoot();

            templateSelect.InitialDirectory = initialDir;
            templateSelect.Title = "Select a .docx Template File";
            templateSelect.FileName = "";
            templateSelect.Filter = "DOCX Template (*" + c_TemplateExt + ")|*" + c_TemplateExt;
            templateSelect.CheckFileExists = true;
            templateSelect.ValidateNames = true;

            DialogResult result = templateSelect.ShowDialog();

            if (result != DialogResult.Cancel)
            {
                filePath = templateSelect.FileName;
                G.TemplatesDir = G.GetDir(filePath);
            }

            return filePath;
        }

        public static string SelectPageCollectionSavePath()
        {
            string filePath = string.Empty;

            SaveFileDialog saveDlg = new SaveFileDialog();
            saveDlg.AddExtension = true;
            saveDlg.DefaultExt = c_BookExt.TrimStart('.');
            saveDlg.Filter = "(*" + c_BookExt + ") | *" + c_BookExt;
            saveDlg.OverwritePrompt = true;
            saveDlg.Title = "Save Page Collection As...";
            saveDlg.ValidateNames = true;
            saveDlg.InitialDirectory = G.GetDir(LastBookPath);

            DialogResult result = saveDlg.ShowDialog();

            if (result != DialogResult.Cancel)
            {
                filePath = saveDlg.FileName;
                LastBookPath = filePath;
            }
            else filePath = string.Empty;

            return filePath;
        }

        public static string SelectBookLoadPath()
        {
            string filePath = G.GetBookFolder();

            OpenFileDialog openDlg = new OpenFileDialog();
            openDlg.AddExtension = true;
            openDlg.DefaultExt = c_BookExt.TrimStart('.');
            openDlg.Filter = "(*" + c_BookExt + ") | *" + c_BookExt;
            openDlg.CheckFileExists = true;
            openDlg.Title = "Select Input Book";
            openDlg.ValidateNames = true;
            openDlg.InitialDirectory = GetDir(LastBookPath);

            DialogResult result = openDlg.ShowDialog();

            if (result != DialogResult.Cancel)
            {
                filePath = openDlg.FileName;
                LastBookPath = filePath;
            }
            else filePath = string.Empty;

            return filePath;
        }

        #endregion

        #region File/Directory Info/Manip
        /// <summary>
        /// Returns directory of the .exe
        /// </summary>
        /// <returns></returns>
        public static string GetRoot()
        {
            Process process = Process.GetCurrentProcess();
            return GetDir(process.Modules[0].FileName);
        }

        /// <summary>
        /// Returns the path of the IOT Zipfile.
        /// </summary>
        /// <returns></returns>
        public static string GetIOTZipfilePath()
        {
            return GetRoot() + G.c_IOTZipFile;
        }

        public static string GetUserDocsPath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) +
                '\\' + G.c_ProgName.Replace("'", "") + '\\';
        }

        public static string CleanFileName(string fileName)
        {
            string newName = Path.GetInvalidFileNameChars().Aggregate(fileName, (current, c) => current.Replace(c.ToString(), string.Empty));
            return newName;
        }

        /// <summary>
        /// Returns the directory portion of file path.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string GetDir(string filePath)
        {
            if (filePath == null)
                return string.Empty;

            //Then it's already the path, send it back.
            if (filePath.Trim().EndsWith(@"\"))
                return filePath;

            //Determine where the last backslash is.
            int position = filePath.LastIndexOf(@"\");
            //If there is no backslash then it's the fileName or invalid, return String.Empty
            if (position == -1)
            {
                return string.Empty;
            }
            else
            {
                //Return filepath without the file name.
                return filePath.Substring(0, position + 1);
            }
        }

        public static string GetFileName(string filePath)
        {
            if (filePath == null ||
                filePath == string.Empty)
                return string.Empty;

            string fileDir = GetDir(filePath);
            string fileName = filePath.Replace(fileDir, string.Empty);
            return fileName;
        }

        /// <summary>
        /// Returns the running dir of the FAF Tabs
        /// </summary>
        /// <returns></returns>
        public static string GetInputRoot()
        {
            string tabsDir = G.GetUserDocsPath() + G.c_Page_Hierarchy;
            return tabsDir;
        }

        public static string GetTemplatesRoot()
        {
            string templatesDir = G.GetUserDocsPath() + G.c_Templates_Folder_Name;
            return templatesDir;
        }

        public static string GetOutputRoot()
        {
            string outputDir = G.GetUserDocsPath() + G.c_Output_Folder_Name;
            return outputDir;
        }

        public static string GetBookFolder()
        {
            string booksDir = G.GetUserDocsPath() + G.c_BookFolder_Name;
            return booksDir;
        }

        public static string GetUpdateFolder()
        {
            return GetRoot() + c_UpdateFolder;
        }

        public static string GetFilePrefix(string filePath)
        {
            string fileName = GetFileName(filePath);

            int indexOfDot = fileName.LastIndexOf('.');

            if (indexOfDot > -1)
                fileName = fileName.Remove(indexOfDot);
            else
                fileName = string.Empty;

            return fileName;
        }

        public static string GetFileExt(string filePath)
        {

            if (filePath.Contains('.'))
            {
                int indexOfdot = filePath.IndexOf('.');
                return (filePath.Substring(indexOfdot,
                     filePath.Length - indexOfdot));
            }
            else
            {
                return null;
            }

        }

        public static string GetDirName(string thisDir)
        {
            string reverseDir = ReverseStr(thisDir);
            reverseDir = reverseDir.TrimStart('\\');

            int firstBS = reverseDir.IndexOf('\\');

            if (firstBS < 0)
                return string.Empty;

            //int secondBS = reverseDir.IndexOf('\\', firstBS + 1);

            //if (secondBS < 0)
            //    return string.Empty;

            string dirName = reverseDir.Substring(0, firstBS);

            return ReverseStr(dirName);
        }

        public static string RecursiveFileSearch(string rootDir, string fileName)
        {
            // Returns, if found, the full path of the file; otherwise returns null.
            var response = Path.Combine(rootDir, fileName);

            if (File.Exists(response))
                return response;

            // Recursion.
            var directories = Directory.GetDirectories(rootDir);

            for (var i = 0; i < directories.Length; i++)
            {
                try
                {
                    response = RecursiveFileSearch(directories[i], fileName);
                }
                catch { response = ""; }

                if (response != null && response != "") 
                    return response;
            }

            // { file was not found }
            return string.Empty;
        }

        /// <summary>
        /// Copy a directory to another directory.
        /// </summary>
        /// <param name="sourceDirName"></param>
        /// <param name="destDirName"></param>
        /// <param name="copySubDirs"></param>
        public static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
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


        public static void DropDown_Closing(object sender, ToolStripDropDownClosingEventArgs e)
        {
            if (e.CloseReason == ToolStripDropDownCloseReason.ItemClicked)
            {
                e.Cancel = true;
            }
        }

        /// <summary>
        /// Returns a ToolStripMenuItem containing the subfolders and specific files within.
        /// </summary>
        /// <param name="thisDir">The root to recurse into.</param>
        /// <param name="fileExt">The file extension to create icons for</param>
        /// <param name="fileIcon">The Icon image to associate with a given file TooStrip item.</param>
        /// <returns></returns>
        public static ToolStripMenuItem RecursePopulateTSHierarchy(
            string thisDir,
            List<string> fileExts, 
            List<Image> fileIcons)
        {
            //List<string> folderNames = new List<string>();
            //List<string> pageFiles = new List<string>();
            ToolStripMenuItem rootDirectory = new ToolStripMenuItem();
            rootDirectory.DropDown.Closing += new ToolStripDropDownClosingEventHandler(DropDown_Closing);
            rootDirectory.Text = G.GetDirName(thisDir);

            IEnumerable<string> theseDirectoryFolders = 
                System.IO.Directory.EnumerateDirectories(thisDir);

            IEnumerable<string> theseDirectoryFiles =
                System.IO.Directory.EnumerateFiles(thisDir);

            //Populate this directory's directories
            foreach (string subFolder in theseDirectoryFolders)
            {
                ToolStripMenuItem newFolderItem = RecursePopulateTSHierarchy(subFolder, fileExts, fileIcons);
                newFolderItem.Image = G.folderIcon;
                rootDirectory.DropDownItems.Add(newFolderItem);
            }

            //Populate the directories files
            foreach (string filePath in theseDirectoryFiles)
            {
                for (int fileTypeIndex = 0; fileTypeIndex < fileExts.Count; fileTypeIndex++)
                {
                    string fileExt = fileExts[fileTypeIndex];
                    Image fileIcon = fileIcons[fileTypeIndex];

                    if (filePath.EndsWith(fileExt))
                    {
                        ToolStripMenuItem newFileItem = new ToolStripMenuItem();
                        newFileItem.Text = GetFilePrefix(filePath);
                        newFileItem.Image = fileIcon;
                        newFileItem.Tag = filePath;
                        rootDirectory.DropDownItems.Add(newFileItem);
                    }
                }
            }

            return rootDirectory;
        }

        #endregion

        #region Ensure Folders

        #endregion

        #region Text Manipuation

        public static string TagWrap(string tagToWrap)
        {
            return RPNCalc.c_TagO + tagToWrap + RPNCalc.c_TagClo;
        }
        public static string TagCloWrap(string tagToWrap)
        {
            return TagWrap('/' + tagToWrap);
        }


        public static string TagUnwrap(string tagToUnwrap)
        {
            //tagToUnwrap = tagToUnwrap.TrimStart('<');
            //tagToUnwrap = tagToUnwrap.TrimEnd('>');
            if (tagToUnwrap.StartsWith(InfixCalc.c_TagO))
                return tagToUnwrap.Substring(1, tagToUnwrap.Length - 2);
            else
                return tagToUnwrap;
        }

        public static string TagGetPrefix(string tag, string removeSuffix)
        {
            string prefix = string.Empty;
            removeSuffix = removeSuffix.Replace("\\", "");
            prefix = tag.Replace(removeSuffix, "");
            prefix = TagUnwrap(prefix);
            return prefix;
        }

        public static string Tagify(string inputTag)
        {
            string tagName = string.Empty;
            bool letterFound = false;
            bool validChar = false;

            foreach (char c in inputTag)
            {
                validChar = false;

                if (!letterFound)
                    if (!IsNum(c))
                        letterFound = true;
                    else 
                        continue;

                if (c == '_') validChar = true;
                if (IsNum(c)) validChar = true;
                if (IsAlpha(c)) validChar = true;

                if (validChar) tagName += c;               
            }

            return tagName;
        }

        public static bool IsNum(char c)
        {
            if (c > 47 && c < 58 || c == '.')
                return true;
            else
                return false;
        }

        public static bool IsAlpha(char c)
        {
            if (
                (c > 64 && c < 91) //Lower case
                    ||
                (c > 96 && c < 123) //Upper Case
               )
                return true;
            else
                return false;
        }

        public static string ReverseStr(string strToRev)
        {
            char[] rev = strToRev.Reverse().ToArray();
            return new string(rev);
        }

        public static string GetSpacePadding(int len, int maxLen)
        {
            string space = string.Empty;
            for (int i = len; i < maxLen; i++)
                space += ' ';

            return space;
        }

        public static string NL
        {
            get { return Environment.NewLine; }
        }

        #endregion 

        #region SQL Functions

        //public static int SQLBoolToBit(bool input)
        //{
        //    if (input)
        //        return 1;
        //    else
        //        return 0;
        //}

        //public static int GetRowCount(string tableName)
        //{
        //    string dbPath = GetDBPath();
        //    int rowCount = 0;
        //    SqlCeConnection conn = new SqlCeConnection(dbPath);
        //    conn.Open();
        //    SqlCeCommand cmd = new SqlCeCommand(c_SelectFrom + tableName, conn);
        //    SqlCeDataReader reader = cmd.ExecuteReader();

        //    while (reader.HasRows)
        //        rowCount++;

        //    conn.Close();

        //    return rowCount;
        //}

        //public static DataTable GetTable(string FilePath, string TableName)
        //{
        //    SqlCeConnection conn = new SqlCeConnection(c_DataSource + FilePath);
        //    DataTable returnTable = new DataTable();
        //    try
        //    {
        //        //Connect to the DB and Load the data into a Table.
        //        conn.Open();
        //        SqlCeDataAdapter da = new SqlCeDataAdapter(c_SelectFrom + TableName, conn);
        //        da.Fill(returnTable);
        //    }
        //    catch (Exception ex)
        //    {
        //        conn.Close();
        //        throw new Exception(ex.ToString());
        //    }

        //    conn.Close();
        //    return returnTable;
        //}

        //public static string PopulateStudents(ref List<StudentID> Students)
        //{
        //        string dbPath = GetDBPath();
        //        SqlCeConnection conn = new SqlCeConnection(c_DataSource + dbPath);

        //    try
        //    {
        //        conn.Open();
        //        SqlCeCommand cmd = new SqlCeCommand(c_SelectFrom + c_StudentInfo, conn);
        //        SqlCeDataReader reader = cmd.ExecuteReader();
        //        int index = -1;

        //        while (reader.HasRows)
        //        {
        //            index++;
        //            StudentID newID = new StudentID();
        //            newID.Index = index;
        //            newID.ID = (int)reader.GetValue(0);
        //            newID.FName = (string)reader.GetValue(1);
        //            newID.MI = (string)reader.GetValue(2);
        //            newID.LName = (string)reader.GetValue(3);

        //            Students.Add(newID);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        conn.Close();
        //        return ex.Message;
        //    }

        //    conn.Close();
        //    return "0";
        //}


        #endregion

        #region TagNameConflicts

        public static List<string> GetTagNameConflicts(
            ref TabControl leftPane,
            ref TabControl rightPane,
            FormAndFuncs newFormFunc)
        {
            List<string> fieldConflicts = new List<string>();
            List<string> actionConflicts = new List<string>();

            List<TabControl> tabPanes = new List<TabControl>();
            tabPanes.Add(leftPane);
            tabPanes.Add(rightPane);

            foreach (TabControl pane in tabPanes)
                foreach (FormAndFuncs formFunc in pane.Controls)
                    formFunc.GetConflicts(newFormFunc, ref fieldConflicts, ref actionConflicts);

            foreach (string conflict in actionConflicts)
                fieldConflicts.Add(conflict);

            return fieldConflicts;
        }

        public static string GetConflictString(
            ref List<string> conflictList,
            string filePath = "")
        {
            string fileName = "this Form & Function";

            if (filePath != "")          
                fileName = G.GetFileName(filePath);

            string conflictString = string.Empty;
            string csLine = string.Empty;
            string space = string.Empty;
            string word = string.Empty;
            int colCount = 0;
            const int tabSize = 18;
            int conflictCount = conflictList.Count;

            conflictString = "Duplicate Tag Names are not allowed. " +
                "Opening " + fileName + " has been prevented due to the " +
                "following " + conflictCount + " naming conflicts.\n\n";

            foreach (string conflict in conflictList)
            {
                space = G.GetSpacePadding(conflict.Length, tabSize);
                word = conflict + space + "\t";
                csLine += word;

                colCount++;
                if (colCount == 3)
                {
                    colCount = 0;
                    conflictString += csLine + Environment.NewLine;
                    csLine = string.Empty;
                }
            }

            if (csLine != string.Empty)
                conflictString += csLine + Environment.NewLine;

            conflictString += "\n\n";
            conflictString += "In order to load this Form & Function alongside " +
                "others, both must have entirely unique Tag Names.";

            return conflictString;
        }

        #endregion

        #region Image Functions

        public static void SwapImg(ref PictureBox pic)
        {
            Image FG = pic.Image;
            Image BG = pic.BackgroundImage;

            Image temp = FG;
            pic.Image = BG;
            pic.BackgroundImage = temp;
        }

        #endregion

        #region Calculations

        public static Point GetControlCenter(Control ctrl)
        {
           return new Point(ctrl.Width / 2, ctrl.Height / 2);
        }

        public static void AutoAdjustComboBoxDropDownSize(Control sender)
        {
            const int maxItems = 8;

            ComboBox box = (ComboBox)sender;
            int width = box.DropDownWidth;
            int newWidth = 0;
            int newHeight = 0;
            int itemsCount = box.Items.Count;
            Graphics g = box.CreateGraphics();
            Font font = box.Font;
            int vertScrollbarWidth =
                (box.Items.Count > box.MaxDropDownItems) ?
                SystemInformation.VerticalScrollBarWidth : 0;

            foreach (string s in box.Items)
            {
                newWidth = (int)g.MeasureString(s, font).Width
                    + vertScrollbarWidth;

                if (newWidth > width)
                    width = newWidth;
            }

            if (itemsCount > maxItems)
                newHeight = box.Height * maxItems;
            else
                newHeight = box.Height * itemsCount;


            if (width == 0)
                box.DropDownWidth = 50;
            else
                box.DropDownWidth = width;


            if (newHeight == 0)
                box.DropDownHeight = 50;
            else
                box.DropDownHeight = newHeight;
        }
 

        #endregion

        #region Drawing Functions

        public static Bitmap GetControlImage(Control ctrl)
        {
            if (ctrl.Width < 1 || ctrl.Height < 1)
                return null;

            Bitmap bm = new Bitmap(ctrl.Width, ctrl.Height);
            ctrl.DrawToBitmap(bm, new Rectangle(0, 0, ctrl.Width, ctrl.Height));
            return bm;
        }

        #endregion

        #region Get Icons

        #region Icons

        public static Image GetPageIcon()
        {
            return GetLocalIcon(c_PageExt, c_defaultPageIconPath);
        }

        public static Image GetBookIcon()
        {
            return GetLocalIcon(c_PageExt, c_defaultBookIconPath);
        }

        public static Image GetFolderIcon()
        {
            return GetImageResource(c_defaultFolderIconPath);
        }

        public static Image GetTemplateIcon()
        {
            return GetLocalIcon(c_TemplateExt, c_defaultTemplateIconPath);
        }

        public static Image GetDownArrowOrangeIcon()
        {
            return GetImageResource(c_defaultImportIcon);      
        }

        public static Image GetDownArrowBlueIcon()
        {
            return GetImageResource(c_defaultBlueArrowIcon);
        }

        public static Image GetCompassRulerIcon()
        {
            return GetImageResource(c_defaultDesignerIcon);
        }

        public static Image GetDocGenIcon()
        {
            return GetImageResource(c_defaultDocGenIcon);
        }
        public static Image GetSelectIcon()
        {
            return GetImageResource(c_defaultSelectIcon);
        }

        public static Image GetNewBookIcon()
        {
            return GetImageResource(c_defaultNewBookIcon);
        }

        public static Image GetSaveIcon()
        {
            return GetImageResource(c_defaultSaveIcon);
        }

        public static Image GetLoadBookIcon()
        {
            return GetImageResource(c_defaultOpenFileIcon);
        }

        public static Image GetExitIcon()
        {
            return GetImageResource(c_defaultExitIcon);
        }

        public static Image GetBlueDownArrowIcon()
        {
            return GetImageResource(c_defaultBlueArrowIcon);
        }


        #endregion

        public static Image GetImageResource(string resourceName)
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            Stream stream = asm.GetManifestResourceStream(resourceName);
            return Image.FromStream(stream);
        }

        public static Image GetLocalIcon(string fileType, string fallBackIconPath)
        {
            object objName = iconsInfo[fileType];
            if (objName == null)
                return GetImageResource(fallBackIconPath);
            else
            {
                try
                {
                    string fileAndParam = (iconsInfo[fileType]).ToString();

                    if (string.IsNullOrEmpty(fileAndParam))
                        return GetImageResource(fallBackIconPath);

                    Icon icon = RegisteredFileType.ExtractIconFromFile(fileAndParam, true);

                    if (icon == null)
                        return GetImageResource(fallBackIconPath);

                    return icon.ToBitmap();
                }
                catch
                { }

            }

            return GetImageResource(fallBackIconPath);
        }

        #endregion

        #region Open Using..

        public static bool OpenInNotepad(string filePath)
        {
            try
            {
                ProcessStartInfo notepad = new ProcessStartInfo();
                notepad.FileName = "notepad.exe";
                notepad.Arguments = filePath;
                Process.Start(notepad);
            }
            catch (Exception) { return false; }
            return true;

        }

        #endregion

    }
}

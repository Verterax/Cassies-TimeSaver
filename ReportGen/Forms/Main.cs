using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Ionic.Zip;

namespace ReportGen
{
   
    public partial class Main : Form
    {
        #region Consts
        const string c_Vers = "Cassie's Time Saver Vers. 1.37";
        const string c_sufTS = "TS";
        const int c_ScrollChange = 40;
        #endregion

        #region Variables


        //private UserLicense userProfile;
        private List<string> progArgs;
        private List<DownloaderArgs> dlArgs;

        private int statusMessageCountdown;     
        private bool statusMessageOn;
        int threeTabs;
        DateTimePicker lastPicker;

        string p_curTempPath;
        string p_curBookPath;

        string CurrentTemplateFilePath
        {
            set
            {
                if (value == "" || value == "null" || value == null)
                {
                    p_curTempPath = value;
                    this.SetTemplateMessage("None");
                }
                else if (value != "null")
                {
                    p_curTempPath = value;
                    G.LastTemplatePath = value;
                    this.SetTemplateMessage(G.GetFilePrefix(value));
                }
            }
            get
            {
                if (p_curTempPath == null)
                    return "";
                else
                    return p_curTempPath;
            }
        }
        string CurrentBookPath
        {
            set
            {
                if (value != "null" && value != string.Empty)
                {
                    p_curBookPath = value;
                    this.Text = G.c_ProgName + " - " + CurrentBookName;

                }
            }
            get { return p_curBookPath; }
        }
        public string CurrentBookName 
        {
            get { return Path.GetFileNameWithoutExtension(CurrentBookPath); }
        }

        FFBook loadBook;
        Image loadImage;
        PictureBox imageBox;
        string loadError;

        FormAndFuncs pageToMove;
        FormAndFuncs lastPageClicked;

        ToolStripMenuItem pageHierarchy; // Variables must be loaded populated by the bg thread
        ToolStripMenuItem templateHierarchy; //before being picked up by the ui thread.

        FileSystemWatcher pageFileWatcher;
        FileSystemWatcher docxFileWatcher;
        bool templatesWatcherLock;
        bool pageWatcherLock; //Lock required to prevent concurrent and conflicting threads.
        //Which would crash the program.

        bool forceClose; //To bypass the save request.
        bool runUpdaterOnExit;

        #endregion

        private void AutoGoto()
        {
            //string dir = @"C:\Users\Christopher\Documents\Visual Studio 2010\Projects\ReportGen\ReportGen\bin\Debug\Tabs\";
            //LoadFormFunc(dir + "WIAT-3.xml");
            //LoadFormFunc(dir + "StudentInfo.xml");
            //LoadFormFunc(dir + "WJ3.xml");

            //tabDesignerToolStripMenuItem.PerformClick();
            //string path = @"C:\Users\Christopher\Documents\Visual Studio 2010\Projects\ReportGen\ReportGen\bin\Debug\Tabs\Testing.xml";
            //this.LoadFormFunc(path);

           // string thisDir = G.GetRoot();
          //  string folderName = G.GetDirName(thisDir);
           // Console.WriteLine("The: " + folderName);
            //pageDesignerToolStripMenuItem.PerformClick();
            //MessageBox.Show(G.GetUserDocsPath(), "");
        }

        #region Initialize/Load

        public Main(string[] args)
        {
            InitializeComponent();
            
            progArgs = new List<string>();
            foreach (string arg in args)
                progArgs.Add(arg);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Text = G.c_ProgName + " - Untitled";

            Init();
            ShowGeneratingWindow("", false); //Hide loading window.
            //this.AutoGoto();
        }

        private void Init()
        {

            //MessageBox.Show(Environment.GetFolderPath(
            //Environment.SpecialFolder.CommonApplicationData));            
            G.Init();

            EnsureIOTFolders();
            SetDropDownMenuIcons();
            AddTabControlMouseEvents(ref leftPane);
            AddTabControlMouseEvents(ref rightPane);
            AddFormEvents();
            AddPageFileSystemWatcher();
            AddTemplateFileSystemWatcher();
            ShowProgressBar(false);

            loadBook = null;
            pageToMove = null;  
            loadError = string.Empty;
            statusMessageOn = true; //Set true to clear on start.
            statusMessageCountdown = 0;
            threeTabs = 0; //For intertabbing in DateTimePickers.
            lastPicker = null; //For tracking last picker for intertabbing.
            dlArgs = new List<DownloaderArgs>(); //Holds a list of files needing downloading.
           
            bgRefreshPageHierarchy.RunWorkerAsync();
            bgRefreshTemplateHierarchy.RunWorkerAsync();

            CurrentTemplateFilePath = "";//G.LastTemplatePath;
            //CurrentPlacemarker = 0;

            ticker.Start();


            //Try load any arguments passed to the program.
            LoadCommandLineArgs();

            try
            {
                loadImage = new Bitmap(G.GetRoot() + G.c_ImagesFolder + "Space Cat.jpg");
                imageBox = new PictureBox();
                imageBox.Size = loadImage.Size;
                imageBox.BackgroundImage = loadImage;
            }
            catch (Exception) { imageBox = null; }

            UpdateManager.UpdateUpdater();

            if (UpdateManager.HaveDocUpdates())
                DoDocUpdates(false);
            else
                CheckForUpdates();
        }

        #region Start-up Methods

        public void SetDropDownMenuIcons()
        {
            //File
            newProjectToolStripMenuItem.Image = G.newBookIcon;
            saveProjectToolStripMenuItem.Image = G.saveIcon;
            openProjectToolStripMenuItem.Image = G.loadBookIcon;
            exitToolStripMenuItem.Image = G.exitIcon;


            //Input
            pageDesignerToolStripMenuItem.Image = G.designerIcon;
            selectFileToolStripMenuItem.Image = G.importIcon;
            openPagesFolderToolStripMenuItem.Image = G.folderIcon;

            //Template
            openTemplatesFolderToolStripMenuItem.Image = G.folderIcon;
            selectTemplateFileToolStripMenuItem.Image = G.selectIcon;

            //Output
            openOutputFolderToolStripMenuItem.Image = G.folderIcon;
            generateDocumentToolStripMenuItem1.Image = G.docGenIcon;

        }

        /// <summary>
        /// Ensures the existence of CT's required forlders.
        /// </summary>
        private void EnsureIOTFolders()
        {
            List<string> mandatoryDirectories = new List<string>();

            string programFilesFolder = G.GetUserDocsPath();

            if (!Directory.Exists(programFilesFolder))
            {
                string IOTZipPath = G.GetIOTZipfilePath();

                try
                {
                    string documentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    ZipFile zipFile = new ZipFile(IOTZipPath);
                    zipFile.ExtractAll(documentsFolder, ExtractExistingFileAction.OverwriteSilently);
                    zipFile.Dispose();

                    // Create input folder
                    System.IO.Directory.CreateDirectory(G.GetInputRoot());

                }
                catch (Exception)
                {
                    MessageBox.Show("Cassie's Timesaver was unable to find the required folder of: " +
                        programFilesFolder + ". " + Environment.NewLine + "To overcome this problem, ensure that " +
                        " the parent folder is not write protected. Or try to run Cassie's Timesaver " +
                        "with Administrative privileges.", "Required Folder Unavailable.");
                    Environment.Exit(3);
                }

            }

            //mandatoryDirectories.Add(G.GetTemplatesRoot());
            //mandatoryDirectories.Add(G.GetInputRoot());
            //mandatoryDirectories.Add(G.GetOutputRoot());

            //foreach (string dir in mandatoryDirectories)
            //{
            //    if (!System.IO.Directory.Exists(dir))
            //    {
            //        try
            //        {
            //            System.IO.Directory.CreateDirectory(dir);
            //        }
            //        catch (Exception) 
            //        {
            //            MessageBox.Show("Cassie's Timesaver was unable to find the required folder of: " +
            //                dir + ". " + Environment.NewLine + "To overcome this problem, ensure that " +
            //                " the parent folder is not write protected. Or try to run Cassie's Timesaver " +
            //                "with Administrative privileges.", "Required Folder Unavailable.");
            //            Environment.Exit(3);
            //        }
            //    }
            //}


        }

        private void LoadCommandLineArgs()
        {
            List<string> cantOpen = new List<string>();

            foreach (string arg in progArgs)
            {
                if (System.IO.File.Exists(arg))
                {
                    string ext = G.GetFileExt(arg);

                    if (ext == G.c_PageExt)
                    {
                        LoadPage(arg);
                    }
                    else if (ext == G.c_BookExt)
                    {
                        LoadFFBook(arg);
                    }
                    else
                        cantOpen.Add(arg);
                }
            }

            if (cantOpen.Count > 0)
            {
                string ErrMsg = "Couldn't open: " + Environment.NewLine;
                foreach (string fail in cantOpen)
                    ErrMsg += G.GetFileName(fail) + Environment.NewLine;

                ErrMsg += "CT can only open " + G.c_PageExt + " and " + G.c_BookExt + " file types.";



                G.ErrorShow(ErrMsg, "Unusable File Type.");

            }
        }

        #endregion

        #endregion

        #region Menu Items

        #region File

        private void newProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewFFCol();
        }

        private void saveProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFFBook();
        }

        private void openProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadFFBook();
        }
     
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //if (MessageBox.Show("Are you sure you want to exit?"
            //    , "Exit program?", MessageBoxButtons.YesNoCancel,
            //        MessageBoxIcon.Question)
            //            == DialogResult.Yes)
            this.Close();
        }

        #endregion

        #region Templates

        private void selectTemplateFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string filePath = G.SelectTemplatePath();

            if (filePath == string.Empty ||
                filePath == "null")
            {
                filePath = "";
            }

            CurrentTemplateFilePath = filePath;
        }

        private void openTemplatesFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string rootTemplatesFolder = G.GetTemplatesRoot();

            if (System.IO.Directory.Exists(rootTemplatesFolder))
                Process.Start(rootTemplatesFolder);
        }

        private void SelectTemplatePath(string templatePath)
        {
            if (System.IO.File.Exists(templatePath))
                CurrentTemplateFilePath = templatePath;
            else
                G.ErrorShow("Unable to use " + templatePath + " the file does not exist.",
                    "It was here a second ago...");
        }

        private void SelectTemplateWithinHierarchy(object sender, EventArgs e)
        {
            if (sender != null)
            {
                if (sender is ToolStripMenuItem)
                {
                    string templatePath = string.Empty;
                    ToolStripMenuItem templateMenuItem = (ToolStripMenuItem)sender;

                    if (templateMenuItem.Tag is string)
                        CurrentTemplateFilePath = (string)templateMenuItem.Tag;
                }
            }       
        }

        #endregion

        #region Input

        private void selectFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string filePath = G.SelectFNFLoadPath();

            if (filePath != string.Empty)
                LoadPage(filePath);
            else
                return;
        }

        private void SelectPageOrColWithinHierarchy(object sender, EventArgs e)
        {
            ToolStripMenuItem itemClicked = (ToolStripMenuItem)sender;
            string filePath = (string)itemClicked.Tag;
            string fileExt = G.GetFileExt(filePath);

            if (fileExt == G.c_PageExt)
                LoadPage(filePath);
            else if (fileExt == G.c_BookExt)
                LoadFFBook(filePath);


            //try
            //{
            //}
            //catch (Exception ex)
            //{
            //    string fileName = G.GetFileName(filePath);
            //    G.ErrorShow("The file: " + fileName + " unfortunately can not be opened. " +
            //        "It has file structure errors. The error says: " + ex.Message, "File reading error.");
            //}

            //Console.WriteLine(filePath);
        }

        private void openPagesFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string rootPagesFolder = G.GetInputRoot();

            if (System.IO.Directory.Exists(rootPagesFolder))
                Process.Start(rootPagesFolder);
        }

        private void tabDesignerToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            Form designer = new Page_Designer(ref rightPane, ref leftPane);
            this.Hide();
            DialogResult result = designer.ShowDialog();
            this.Show();
            bgRefreshPageHierarchy.RunWorkerAsync();
        }

        #endregion

        #region Output

        private void generateDocumentToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            string ErrCode = RequestDocGen(CurrentTemplateFilePath);
             

            if (ErrCode != G.c_NoErr)
                G.ErrorShow(ErrCode, "Something less than perfect.");
        }        

        private void openOutputFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string outputFolder = G.OutputDir;

            if (System.IO.Directory.Exists(outputFolder))
                Process.Start(outputFolder);
        }

        #endregion

        #region Options


        private void setOutputFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void automaticallyOpenGeneratedDocumentsToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        #endregion

        #region Help

        private void cTManualToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string documentationPath = G.GetRoot() + G.c_DocumentationFolder + G.c_CTManual;

            if (System.IO.File.Exists(documentationPath))
            {
                string ErrCode = DocGen.OpenDocProgram(documentationPath);

                if (ErrCode != G.c_NoErr)
                {
                    G.ErrorShow("Unable to open CT Manual documentation. " + Environment.NewLine +
                        "A suitable program is not available to open the Manual. A program capable of opening .docx files " +
                    "is highly recommended when using this program.",
                        "Cannot Help You.");
                }
            }
            else
            {
                G.ErrorShow("The File named '" + G.c_CTManual + "' could not be found and is missing. Please try to obtain another online at www.axiomcomputing.com.",
                    "Cannot Help You.");
            }


        }

        private void aboutToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("For help and information call me, (Christopher Caldwell) @ (951) 310-2533, or email me: chris@codehadouken.com" +
                Environment.NewLine + Environment.NewLine +
                "All Rights Reserved, Axiom Computing.", "Well hello.",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }


        private void checkForUpdateToolStripMenuItem_Click(object sender, EventArgs e)
        {       
            this.CheckForUpdates();
            checkForUpdateToolStripMenuItem.Enabled = false;

        }

        #endregion

        #region Update

        private void updateCTToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DoAppUpdates(true, true);
        }

        private void updatePackageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DoDocUpdates(true);
        }

        #endregion

        private void CloseDropDownMenus()
        {
            inputToolStripMenuItem.HideDropDown();
        }

        #endregion

        #region Buttons

        private void btnPageRight_Click(object sender, EventArgs e)
        {
            TurnPageRight();
        }

        private void btnPageLeft_Click(object sender, EventArgs e)
        {
            TurnPageLeft();
        }

        private void btnLeftPaneLeft_Click(object sender, EventArgs e)
        {
            if (leftPane.Controls.Count > 0)
                if (leftPane.SelectedIndex - 1 >= 0)
                    leftPane.SelectedIndex--;
        }

        private void btnLeftPaneRight_Click(object sender, EventArgs e)
        {
            if (leftPane.Controls.Count > 0)
                if (leftPane.SelectedIndex + 1 <= leftPane.Controls.Count - 1)
                    leftPane.SelectedIndex++;
        }

        private void btnRightPaneLeft_Click(object sender, EventArgs e)
        {
            if (rightPane.Controls.Count > 0)
                if (rightPane.SelectedIndex - 1 >= 0)
                    rightPane.SelectedIndex--;
        }

        private void btnRightPaneRight_Click(object sender, EventArgs e)
        {
            if (rightPane.Controls.Count > 0)
                if (rightPane.SelectedIndex + 1 <= rightPane.Controls.Count - 1)
                    rightPane.SelectedIndex++;
        }

        private void btnSwap_Click(object sender, EventArgs e)
        {
            Control[] leftTemp = new Control[leftPane.Controls.Count];
            Control[] rightTemp = new Control[rightPane.Controls.Count];

            leftPane.Controls.CopyTo(leftTemp, 0);
            rightPane.Controls.CopyTo(rightTemp, 0);
            leftPane.Controls.Clear();
            rightPane.Controls.Clear();
            rightPane.Controls.AddRange(leftTemp);
            leftPane.Controls.AddRange(rightTemp);          
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            bgDocGen.CancelAsync();
        }

        #endregion

        #region Pop-Up messages.

        private void ShowNoPageSelectedError()
        {
            G.MessageShow("No page is selected. Click on a page and try again.",
                    "Select a page");
        }

        #endregion //Message Shows.

        #region Context Menu

        #region Form Func

        #region File
        private void readFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string fileToFillFrom = G.SelectFillFromFile();

            if (fileToFillFrom != string.Empty)
                lastPageClicked.FillFromFile(fileToFillFrom);
            
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string saveFFAs = G.SelectFNFSavePath(lastPageClicked);

            if (saveFFAs != string.Empty)
                lastPageClicked.Serialize(saveFFAs);
        }
        #endregion


        private void editInDesignerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Open in designer
            if (lastPageClicked == null)
            {
                ShowNoPageSelectedError();
                return;
            }

            bool fromLeft = true;

            if (rightPane.Controls.Contains(lastPageClicked))
                fromLeft = false;

            int fromIndex = 0;

            if (fromLeft)
                fromIndex = leftPane.SelectedIndex;
            else
                fromIndex = rightPane.SelectedIndex;

            if (fromIndex == -1)
                fromIndex = 0;

            RemoveFFEvents(ref lastPageClicked);

            Page_Designer designer = new Page_Designer(
                ref rightPane,
                ref leftPane,
                ref lastPageClicked,
                fromLeft,
                fromIndex);

            this.Hide();
            designer.ShowDialog();
            this.Show();

            this.BringToFront();
            
        }

        #region Clear

        private void clearPageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lastPageClicked != null)
                lastPageClicked.ClearFields();
        }

        private void clearThisSideToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lastPageClicked != null)
            {
                if (lastPageClicked.Parent != null)
                {
                    TabControl pane = (TabControl)lastPageClicked.Parent;

                    string paneName = GetPaneName(pane);
                    string question = string.Format("Clear all Fields on the {0} side?", paneName);

                    DialogResult result = G.YesNoQuestion(question,
                        "Clear " + paneName + " Fields?");

                    if (result == DialogResult.Yes)
                    {
                        foreach (TabPage page in pane.TabPages)
                        {
                            if (page is FormAndFuncs)
                            {
                                FormAndFuncs ff = (FormAndFuncs)page;

                                ff.ClearFields();
                            }
                        }

                        ShowMessage(paneName + " side Pages cleared!", true, 3000);
                    }
                }
            }
        }

        private void clearEntireBookToolStripMenuItem_Click(object sender, EventArgs e)
        {

            string question = "Are you sure you'd like to clear all Fields in all Input Pages?";
           DialogResult result = G.YesNoQuestion(question,
                        "Clear All Page Fields?");

           if (result == DialogResult.Yes)
           {
               List<TabControl> panes = new List<TabControl>();
               panes.Add(leftPane);
               panes.Add(rightPane);

               foreach (TabControl pane in panes)
               {
                   foreach (TabPage page in pane.TabPages)
                   {
                       if (page is FormAndFuncs)
                       {
                           FormAndFuncs ff = (FormAndFuncs)page;

                           ff.ClearFields();
                       }
                   }
               }
           }
        }

        #endregion

        private void removePageToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (lastPageClicked == null)
            {
                ShowNoPageSelectedError();
                return;
            }

            //string tabLabel = lastPageClicked.Text;
            //DialogResult response = G.YesNoQuestion("Remove " + tabLabel + " ? " +
            //    G.NL + G.NL + "Any unsaved changes to it will be lost.",
            //    "Remove " + tabLabel + "?");

            //if (response != DialogResult.Cancel)
            //{
                this.RemoveFFTab(lastPageClicked);
            //}

        }

        #endregion

        #endregion

        #region Background Workers

        #region Load Book

        private void bgLoadBook_DoWork(object sender, DoWorkEventArgs e)
        {
            string filePath = (string)e.Argument;
            string filePrefix = G.GetFilePrefix(filePath);
            string fileDir = G.GetDir(filePath);

            if (System.IO.File.Exists(filePath))
            {
                //try
                //{
                    loadBook = new FFBook(filePath);

                    if (loadBook != null)
                    {
                        //Remove current pages
                        RemoveAllPages();  

                        foreach (FormAndFuncs ff in loadBook.leftPane)
                            InsertFFTab(ff, ref leftPane);
                        if (leftPane.Controls.Count > 0)
                            leftPane.SelectedIndex = 0;

                        foreach (FormAndFuncs ff in loadBook.rightPane)
                            InsertFFTab(ff, ref rightPane);
                        if (rightPane.Controls.Count > 0)
                            rightPane.SelectedIndex = 0;
                    }



                //    loadError = G.c_NoErr;
                //}
                //catch (Exception ex)
                //{
                //    loadError = "Unable to load " + filePrefix + Environment.NewLine +
                //       "The error says. " + ex.Message;
                //    return;
                //}
            }
            else
            {
                loadError = "Unable to load " + filePath + ". " +
                    Environment.NewLine + "That document Template was not found at " +
                    fileDir;
            }

            e.Result = filePath;

        }

        private void bgLoadBook_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

            if (loadBook != null)
            {
                string loadTemplatePath = loadBook.lastTemplateFilePath;
                string loadPath = (string)e.Result;
                string filePrefix = G.GetFilePrefix(loadPath);
               
                if (loadTemplatePath == string.Empty)
                    this.CurrentTemplateFilePath = null;
                else //Check if template exists at path.
                {
                    if (System.IO.File.Exists(loadTemplatePath))
                        this.CurrentTemplateFilePath = loadTemplatePath; //Found template where expected.
                    else//Try to find one with the same name in the template hierarchy.
                    {
                        string templatePrefix = G.GetFilePrefix(loadTemplatePath);

                        string newFoundTemplatePath =
                            RecurseSearchTSForFileByName(templateToolStripMenuItem,
                            templatePrefix);

                        if (newFoundTemplatePath != string.Empty)
                            this.CurrentTemplateFilePath = newFoundTemplatePath; //Found new template
                        else
                        {
                            this.CurrentTemplateFilePath = string.Empty; //Found no template by that name.
                            this.SetTemplateMessage("None. Couldn't find: " + templatePrefix);
                        }
                    }
                }

                ShowFFBookLoading(false);

                CurrentBookPath = loadPath;
                ShowMessage("Loaded " + filePrefix + "!", true, 3000);
            }
            else
            {
                G.ErrorShow(loadError, "Unable to Generate Document.");
            }
        }

        #endregion

        #region Document Generation

        private void bgDocGen_DoWork(object sender, DoWorkEventArgs e)
        {
            //Doc Gen
            DocGenArgs args = (DocGenArgs)e.Argument;
            args.errCode = DocGen.GenCustom(ref args, bgDocGen);
            
            e.Result = args;
        }

        private void bgDocGen_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            UpdateProgress();
        }

        public void UpdateProgress()
        {
            if (DocGen.progress1 <= 100)
                if (pbr1.Value != DocGen.progress1)
                    pbr1.Value = DocGen.progress1;

            if (DocGen.progress2 <= 100)
                if (pbr2.Value != DocGen.progress2)
                    pbr2.Value = DocGen.progress2;

            if (lblMainMsg.Text != DocGen.msg1)
                lblMainMsg.Text = DocGen.msg1;

            if (lblSecondMsg.Text != DocGen.msg2)
                lblSecondMsg.Text = DocGen.msg2;
        }
       
        private void bgDocGen_RunWorkerCompleted
            
            (object sender, RunWorkerCompletedEventArgs e)
        {
            DocGenArgs args = (DocGenArgs)e.Result;
            string result = args.errCode;

            EnableEditing(true);

            //Hide Generating window
            this.ShowGeneratingWindow("", false);

          //  System.Media.SoundPlayer player = new System.Media.SoundPlayer(@"C:\Program Files\Bethesda Softworks\Oblivion\LauncherMusic.wav");
           // player.Play();

            if (result == G.c_Cancel)
                return;

            if (result != G.c_NoErr)
            {
                MessageBox.Show(result, "Document Generation Error.", MessageBoxButtons.OK);
            }

            //Success!
            result = DocGen.OpenDocProgram(args.newDocFilePath);
            if (result != G.c_NoErr)
                G.MessageShow("Unable to open new document. " + args.newDocFilePath + Environment.NewLine +
                    result, "Cannot open new file.");
        }

        #endregion

        #region Refresh Page Hierarchy

        private void bgRefreshPageHierarchy_DoWork(object sender, DoWorkEventArgs e)
        {
            string pagesDir = G.GetInputRoot();
            List<string> fileExts = new List<string>();
            fileExts.Add(G.c_PageExt);
            fileExts.Add(G.c_BookExt);

            List<Image> fileIcons = new List<Image>();
            fileIcons.Add(G.pageIcon);
            fileIcons.Add(G.bookIcon);

            pageHierarchy = G.RecursePopulateTSHierarchy(pagesDir, fileExts, fileIcons);
        }

        public delegate void DelegateRefreshPageHierarchy();
        private void bgRefreshPageHierarchy_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

            if (this.InvokeRequired)
            {
                this.Invoke(new DelegateRefreshPageHierarchy(RefreshPagesHierarchy));
                pageWatcherLock = false;
                return;
            }

            RefreshPagesHierarchy();
            pageWatcherLock = false;
        }

        #endregion

        #region Refresh Templates Hierarchy

        private void bgRefreshTemplateHierarchy_DoWork(object sender, DoWorkEventArgs e)
        {
            string templatesDir = G.GetTemplatesRoot();
            List<string> templateExts = new List<string>();
            templateExts.Add(G.c_TemplateExt);

            List<Image> templateIcons = new List<Image>();
            templateIcons.Add(G.docxIcon);

            templateHierarchy = G.RecursePopulateTSHierarchy(templatesDir, templateExts, templateIcons);
        }

        public delegate void DelegateRefreshTemplatesHierarchy();
        private void bgRefreshTemplateHierarchy_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new DelegateRefreshTemplatesHierarchy(RefreshTemplatesHierarchy));
                templatesWatcherLock = false;
                return;
            }

            RefreshTemplatesHierarchy();
            templatesWatcherLock = false;
        }

        #endregion

        #region Download Files

        private void bgDownloader_DoWork(object sender, DoWorkEventArgs e)
        {
            if (sender == null)
            {
                e.Result = null;
                return;
            }

            DownloaderArgs args = (DownloaderArgs)e.Argument;
            args.ErrCode = WebData.Download(args.URL, args.FileName, args.SaveTo, bgDownloader);

            e.Result = args;
        }

        private void bgDownloader_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            tsPbr.Value = e.ProgressPercentage;
        }

        private void bgDownloader_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //Check to see if more files exist to download.

            DownloaderArgs result = (DownloaderArgs)e.Result;

            if (result.ErrCode == G.c_NoErr)
                PostDownloadAction(result);
            else
            {
                ShowMessage("", false, 0);
                tsPbr.Visible = false;
            }
            
        }

        #endregion

        #region Doc Update (Unzip Doc Updates)

        private void bgDocUpdate_DoWork(object sender, DoWorkEventArgs e)
        {   
            int packagesUpdated = UpdateManager.UpdateDocPackages(bgDocUpdate);
            e.Result = packagesUpdated;
        }

        private void bgDocUpdate_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            tsPbr.Value = e.ProgressPercentage;
        }

        private void bgDocUpdate_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            int packagesUpdated = (int)e.Result;

            if (packagesUpdated > 0)
            {
                G.MessageShow(packagesUpdated + " packages updated!", "Success!");
                NotifyUpdatesReady(false);
            }
            else
                G.MessageShow("No packages were updated. Please make sure that all Template Documents are closed in all programs before attempting to update.",
                    "Template Package Update Failure");
        }

        #endregion


        #endregion

        #region Events

        #region CT Closing / Exit
        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            //If not forcing close.
            if (!forceClose)
            {
                DialogResult result = AskSaveFirst(); //Ask if save first.
                if (result == DialogResult.Cancel)
                {
                    e.Cancel = true; //If cancel, prevent closing.
                    return;
                }
            }

            //Release image.
            if (imageBox != null)
            {
                imageBox.Dispose();
            }

            //If updates pending, launch updater.
            if (runUpdaterOnExit)
            {
                runUpdaterOnExit = false;
                DoAppUpdates(false, false);
            }
        }
        #endregion
        #region Drop Down Menu Items



        #endregion

        #region FormFunc Events

        private void Field_KeyPress(object sender, KeyPressEventArgs e)
        {
            DateTimePicker picker = null;

            if (sender is DateTimePicker)
            {
                picker = (DateTimePicker)sender;

                if (e.KeyChar == 9) //Tab pressed.
                {
                    if (picker == lastPicker) //same picker as last time.
                    {
                        if (threeTabs >= 3)
                        {
                            threeTabs = 0;
                            lastPicker = null;
                        }
                        else
                        {
                            threeTabs++;
                            e.KeyChar = (char)Keys.Right;
                        }

                    }
                    else //new picker.
                    {
                        threeTabs = 0;
                        lastPicker = picker;
                    }
                }
            }
        }

        private void FormFunc_MouseDown(object sender, MouseEventArgs e)
        {

            FormAndFuncs lastPage = GetControlFF((Control)sender);

            if (lastPage != null)
                lastPageClicked = lastPage;

            if (e.Button == MouseButtons.Left)
            {
                pageToMove = lastPage;
            }

            if (e.Button == MouseButtons.Right)
            {
            }
        }

        private void FormFunc_MouseUp(object sender, MouseEventArgs e)
        {       
    
            if (e.Button == MouseButtons.Left)
            {
                TryPageMove();              
            }

            if (e.Button == MouseButtons.Right)
            {
            }
        }

        private void Form_MouseWheel(object sender, MouseEventArgs e)
        {

            Point p = Cursor.Position;
            FormAndFuncs ff = null;

            if (leftPane.ClientRectangle.Contains(leftPane.PointToClient(p)))
            {
                if (leftPane.TabPages.Count > 0)
                {
                    if (leftPane.SelectedIndex == -1)
                        return;

                    ff = (FormAndFuncs)leftPane.TabPages[leftPane.SelectedIndex];
                }
            }
            else if (rightPane.ClientRectangle.Contains(rightPane.PointToClient(p)))
            {
                if (rightPane.SelectedIndex == -1)
                    return;

                ff = (FormAndFuncs)rightPane.TabPages[rightPane.SelectedIndex];
            }

            if (ff == null)
                return;

            if (e.Delta > 0)
            {
                if (ff.VerticalScroll.Value - c_ScrollChange < 0)
                    ff.VerticalScroll.Value = 0;
                else
                    ff.VerticalScroll.Value -= c_ScrollChange;
            }
            else
                ff.VerticalScroll.Value += c_ScrollChange;


            ff.PerformLayout();

        }
  
        private void PagePane_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        private void Pane_ControlAdded(object sender, ControlEventArgs e)
        {
            FormAndFuncs formFunc = (FormAndFuncs)e.Control;
            AddFFEvents(ref formFunc);
        }

        private void Pane_ControlRemoved(object sender, ControlEventArgs e)
        {
            FormAndFuncs formFunc = (FormAndFuncs)e.Control;
            RemoveFFEvents(ref formFunc);
        }

        private void PagePane_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] fileNames = (string[])e.Data.GetData(DataFormats.FileDrop);
                List<string> Errors = new List<string>();
                List<FormAndFuncs> loadFFs = new List<FormAndFuncs>();

                for (int i = 0; i < fileNames.Count(); i++)
                {
                    string fileName = fileNames[i];
                    string fileExt = Path.GetExtension(fileName);

                    if (fileExt == G.c_PageExt)
                    {
                        FormAndFuncs loadFF = new FormAndFuncs(fileName);

                        if (loadFF == null)
                            Errors.Add("Unable to load page: " + Path.GetFileName(fileName) + Environment.NewLine);
                        else
                            loadFFs.Add(loadFF);
                    }
                    else if (fileExt != G.c_PageExt && sender is FormAndFuncs)
                    {
                        FormAndFuncs formFunc = (FormAndFuncs)sender;
                        string textToRead = FormAndFuncs.ReadDocumentFile(fileName);
                        formFunc.FillFromFile(fileName);
                    }
                }

                if (loadFFs.Count > 0)
                {
                    TabControl addTo = null;

                    if (sender is TabControl)
                        addTo = (TabControl)sender;
                    else
                        addTo = (TabControl)((FormAndFuncs)sender).Parent;

                    if (addTo != null)
                    {
                        foreach (FormAndFuncs ff in loadFFs)
                        {
                            List<string> conflicts = G.GetTagNameConflicts(
                                ref leftPane,
                                ref rightPane,
                                ff);

                            if (conflicts.Count > 0)
                            {
                                string conflictString = G.GetConflictString(
                                    ref conflicts, "");
                                G.MessageShow(conflictString, "Naming Conflicts Exist.");
                            }
                            else
                            {
                                InsertFFTab(ff, ref addTo);
                                addTo.SelectedIndex = addTo.TabPages.Count - 1;
                            }
                        }                                                
                    }                   
                }

                if (Errors.Count > 0)
                {
                    string errString = string.Empty;

                    foreach (string err in Errors)
                        errString += err;
                    errString += Environment.NewLine +
                        "The file format of these files is not usable by " + G.c_ProgName;
                    G.ErrorShow(errString, "Page Loading Errors.");
                }


            }

        }

        private void AddFFEvents(ref FormAndFuncs ffTab)
        {
            ffTab.ContextMenuStrip = ctxFormFunc;
            RecurseAddFFEvents(ffTab);

            TabControl pane = (TabControl)ffTab.Parent;

            if (pane != null)
            {
                //int indexOfAddedItem = pane.Controls.IndexOf(ffTab);
                //pane.SelectedIndex = indexOfAddedItem;
            }
        }

        private void RecurseAddFFEvents(Control rootCtrl)
        {
            rootCtrl.MouseDown += new MouseEventHandler(FormFunc_MouseDown);
            rootCtrl.MouseUp += new MouseEventHandler(FormFunc_MouseUp);
            rootCtrl.DragDrop += new DragEventHandler(PagePane_DragDrop);
            rootCtrl.DragEnter += new DragEventHandler(PagePane_DragEnter);

            if (rootCtrl is ComboBox)
            {
                //Technically not an event.
                //Since combobox items don't change,
                //Set size once, instead of on each dropdown.
                G.AutoAdjustComboBoxDropDownSize(rootCtrl);           
            }

            if (rootCtrl is DateTimePicker)
            {
                rootCtrl.KeyPress += new KeyPressEventHandler(Field_KeyPress);
            }

            if (rootCtrl.HasChildren)
                foreach (Control child in rootCtrl.Controls)
                    RecurseAddFFEvents(child);
        }

        private void RemoveFFEvents(ref FormAndFuncs ffTab)
        {
            RecurseRemoveFFEvents(ffTab);
        }

        private void RecurseRemoveFFEvents(Control rootCtrl)
        {
            rootCtrl.MouseDown -= FormFunc_MouseDown;
            rootCtrl.DragDrop -= PagePane_DragDrop;
            rootCtrl.DragEnter -= PagePane_DragEnter;

            if (rootCtrl.HasChildren)
                foreach (Control child in rootCtrl.Controls)
                    RecurseRemoveFFEvents(child);
        }

        private void AddTabControlMouseEvents(ref TabControl tabPane)
        {
            tabPane.MouseDown += new MouseEventHandler(TabControl_MouseDown);
            tabPane.MouseUp += new MouseEventHandler(TabControl_MouseUp);
            tabPane.ControlAdded += new ControlEventHandler(Pane_ControlAdded);
            tabPane.ControlRemoved += new ControlEventHandler(Pane_ControlRemoved);
            tabPane.DragEnter += new DragEventHandler(PagePane_DragEnter);
            tabPane.DragDrop += new DragEventHandler(PagePane_DragDrop);
        }

        private void AddFormEvents()
        {
            this.MouseWheel +=new MouseEventHandler(Form_MouseWheel);
        }

        #endregion

        #region FileSystem Watchers

        #region Page Files

        private void AddPageFileSystemWatcher()
        {
            string pagesPath = G.GetInputRoot();
            pageFileWatcher = new FileSystemWatcher(pagesPath);
            pageFileWatcher.EnableRaisingEvents = true;
            pageFileWatcher.IncludeSubdirectories = true;

            pageFileWatcher.Created += new FileSystemEventHandler(PageFileChangeDetected);
            //watcher.Changed += new FileSystemEventHandler(FileChangeDetected);
            pageFileWatcher.Deleted += new FileSystemEventHandler(PageFileChangeDetected);
            pageFileWatcher.Renamed += new RenamedEventHandler(PageFileChangeDetected);
        }

        private void PageFileChangeDetected(object sender, FileSystemEventArgs e)
        {
            RunPageChangedWorker();
        }

        private void PageFileChangeDetected(object sender, RenamedEventHandler e)
        {
            RunPageChangedWorker();
        }

        private void RunPageChangedWorker()
        {
            if (!pageWatcherLock)
            {
                pageWatcherLock = true;
                System.Threading.Thread.Sleep(500);
                bgRefreshPageHierarchy.RunWorkerAsync();
            }
        }

        #endregion

        #region DocX Templates

        private void AddTemplateFileSystemWatcher()
        {
            string templatePath = G.GetTemplatesRoot();
            docxFileWatcher = new FileSystemWatcher(templatePath);
            docxFileWatcher.EnableRaisingEvents = true;
            docxFileWatcher.IncludeSubdirectories = true;

            docxFileWatcher.Created += new FileSystemEventHandler(TemplatesFileChangeDetected);
            docxFileWatcher.Deleted += new FileSystemEventHandler(TemplatesFileChangeDetected);
            docxFileWatcher.Renamed += new RenamedEventHandler(TemplatesFileChangeDetected);
        }

        private void TemplatesFileChangeDetected(object sender, FileSystemEventArgs e)
        {
            RunTemplatesChangedWorker();
        }

        private void TemplatesFileChangeDetected(object sender, RenamedEventHandler e)
        {
            RunTemplatesChangedWorker();
        }

        private void RunTemplatesChangedWorker()
        {
            if (!templatesWatcherLock)
            {
                templatesWatcherLock = true;
                System.Threading.Thread.Sleep(500);
                bgRefreshTemplateHierarchy.RunWorkerAsync();
            }
        }

        #endregion

        #endregion

        #endregion
     
        #region Save / Load / New

        #region LoadFormFunc

        private void LoadPage(string filePath)
        {
            FormAndFuncs newFormFuncs = null;

            try
            {
                newFormFuncs = new FormAndFuncs(filePath);
            }
            catch (Exception ex)
            {
                string fileName = G.GetFileName(filePath);
                G.ErrorShow("Sorry. The file: " + fileName +
                " has an unreadable file structure, and cannot be opened.\n\n" +
                "Error: " + ex.Message, "Sorry baby...");
                return;
            }

            //Verify that loaded tagNames don't conflict with new tagNames
            List<string> conflicts = G.GetTagNameConflicts(
                ref leftPane,
                ref rightPane,
                newFormFuncs);

            if (conflicts.Count > 0)
            {
                string conflictString = G.GetConflictString(
                    ref conflicts, filePath);
                G.MessageShow(conflictString, "Naming Conflicts Exist.");
                return;
            }

            if (leftPane.TabPages.Count == 0)
                InsertFFTab(newFormFuncs, ref leftPane);
            else
            {
                InsertFFTab(newFormFuncs, ref rightPane);
                rightPane.SelectedIndex = rightPane.TabPages.IndexOf(newFormFuncs);
            }
        }

        private void PopulateTagsListFromFormFunc(
            ref List<string> fillWithTagNames,
            ref FormAndFuncs formFuncs)
        {
            fillWithTagNames.Clear();

            Fields fields = new Fields();
            fields.PopulateFromFormFunc(formFuncs);
            fields.PopulateFromActionList(formFuncs.actions);

            foreach (string key in fields.Keys)
                fillWithTagNames.Add(key);
        }

        /// <summary>
        /// Return a list of conflicts.
        /// </summary>
        /// <param name="currentList"></param>
        /// <param name="newList"></param>
        /// <returns></returns>
        private List<string> CompareLists(
            ref List<string> currentList,
            ref List<string> newList)
        {
            List<string> conflicts = new List<string>();

            foreach (string tagName in newList)
            {
                if (currentList.Contains(tagName))
                    conflicts.Add(tagName);
            }

            return conflicts;
        }


        #endregion

        #region Form Collections

        private void SaveFFBook(string savePath = "")
        {
            string errCode = G.c_NoErr;

            if (savePath == "")
                savePath = G.SelectPageCollectionSavePath();

            if (savePath == string.Empty) //If no path given.
                return;

            string currentDocPath = CurrentTemplateFilePath;

            FFBook saveCol = new FFBook(leftPane, rightPane, currentDocPath);

            errCode = saveCol.Serialize(savePath);

            if (errCode != G.c_NoErr)
                G.ErrorShow(errCode, "Unable to save.");
            else
            {
                string fileName = G.GetFilePrefix(savePath);
                CurrentBookPath = savePath;
                ShowMessage("Successfully saved: '" + fileName + "'", true, 3000);
            }
        }

        private void LoadFFBook(string loadPath = "")
        {
            AskSaveFirst();

            CloseDropDownMenus();

            if (loadPath == string.Empty)
                loadPath = G.SelectBookLoadPath();

            if (loadPath == string.Empty)
                return;

            string filePrefix = G.GetFilePrefix(loadPath);
            string loadTemplatePath = string.Empty;
            FFBook ffBook = null;

            try
            {
                ShowFFBookLoading(true);
                ffBook = new FFBook(loadPath);
                loadTemplatePath = ffBook.lastTemplateFilePath;
            }
            catch (Exception ex)
            {
                G.ErrorShow("Unable to load " + filePrefix + Environment.NewLine +
                   "The error says. " + ex.Message, "Computers... Sheesh.");
                ShowFFBookLoading(false);
                return;
            }

            //Remove current pages
            RemoveAllPages();

            if (loadTemplatePath == string.Empty)
                this.CurrentTemplateFilePath = null;
            else //Check if template exists at path.
            {
                if (System.IO.File.Exists(loadTemplatePath))
                    this.CurrentTemplateFilePath = loadTemplatePath; //Found template where expected.
                else//Try to find one with the same name in the template hierarchy.
                {
                    string templatePrefix = G.GetFilePrefix(loadTemplatePath);

                    string newFoundTemplatePath =
                        RecurseSearchTSForFileByName(templateToolStripMenuItem,
                        templatePrefix);

                    if (newFoundTemplatePath != string.Empty)
                        this.CurrentTemplateFilePath = newFoundTemplatePath; //Found new template
                    else
                    {
                        this.CurrentTemplateFilePath = string.Empty; //Found no template by that name.
                        this.SetTemplateMessage("None. Couldn't find: " + templatePrefix);
                    }
                }
            }

            foreach (FormAndFuncs ff in ffBook.leftPane)
                InsertFFTab(ff, ref leftPane);
            if (leftPane.Controls.Count > 0)
                leftPane.SelectedIndex = 0;

            foreach (FormAndFuncs ff in ffBook.rightPane)
                InsertFFTab(ff, ref rightPane);
            if (rightPane.Controls.Count > 0)
                rightPane.SelectedIndex = 0;

            ShowFFBookLoading(false);
            CurrentBookPath = loadPath;
            ShowMessage("Loaded " + filePrefix + "!", true, 3000);

        }

        private void NewFFCol(bool ignorePrompt = false)
        {
            AskSaveFirst();
            RemoveAllPages();
            CurrentTemplateFilePath = string.Empty;
        }

        private DialogResult AskSaveFirst()
        {
            DialogResult result = DialogResult.No;

            if (ProjectContainsPages())
                if (CurrentBookName == string.Empty || CurrentBookName == null)
                {
                    result = G.YesNoQuestion("Save changes before discarding these pages?", "Save before poof?");
                }
                else
                {
                    result = G.YesNoQuestion("Save changes to " + CurrentBookName + "?", "Save before poof?");
                }
                    

            if (result == DialogResult.Yes)
                SaveFFBook();

            return result;
        }

        #endregion

        #endregion

        #region ToolStrip File Hierarchies

        #region Page / Collection
        private void RefreshPagesHierarchy()
        {
            if (pageHierarchy == null)
                return;

            ToolStripItem keepManualSelect = selectFileToolStripMenuItem;
            ToolStripMenuItem keepDesignerItem = pageDesignerToolStripMenuItem;
            ToolStripMenuItem keepOpenFolder = openPagesFolderToolStripMenuItem;
            inputToolStripMenuItem.DropDownItems.Clear();
            //re-add manual file select toolstrip menu item.
            inputToolStripMenuItem.DropDownItems.Add(keepManualSelect);
            inputToolStripMenuItem.DropDownItems.Add(keepDesignerItem);

            Console.WriteLine("Count of items: " + pageHierarchy.DropDownItems.Count);

            //Add each item in the pageHierarchy to pagesToolstripMenuItem
            while (pageHierarchy.DropDownItems.Count > 0)
            {
                ToolStripMenuItem item = (ToolStripMenuItem)pageHierarchy.DropDownItems[0];
                RecurseAddPagesMenuItemClickEvent(item);
                inputToolStripMenuItem.DropDownItems.Add(item);
            }

            inputToolStripMenuItem.DropDownItems.Add(keepOpenFolder);

        }

        private void RecurseAddPagesMenuItemClickEvent(ToolStripMenuItem root)
        {
            if (root.Tag is string)
                root.Click += new EventHandler(SelectPageOrColWithinHierarchy);
            else if (root.HasDropDownItems)
                foreach (ToolStripMenuItem item in root.DropDownItems)
                    RecurseAddPagesMenuItemClickEvent(item);

        }

        #endregion

        #region Template Hierarchy

        private void RefreshTemplatesHierarchy()
        {
            if (templateHierarchy == null)
                return;

            ToolStripItem keepManualSelect = selectTemplateFileToolStripMenuItem;
            ToolStripMenuItem keepOpenFolder = openTemplatesFolderToolStripMenuItem;
            templateToolStripMenuItem.DropDownItems.Clear();
            //re-add manual file select toolstrip menu item.
            templateToolStripMenuItem.DropDownItems.Add(keepManualSelect);

            Console.WriteLine("Count of items: " + templateHierarchy.DropDownItems.Count);

            //Add each item in the pageHierarchy to pagesToolstripMenuItem
            while (templateHierarchy.DropDownItems.Count > 0)
            {
                ToolStripMenuItem item = (ToolStripMenuItem)templateHierarchy.DropDownItems[0];
                RecurseAddTemplatesMenuItemClickEvent(item);
                templateToolStripMenuItem.DropDownItems.Add(item);
            }

            templateToolStripMenuItem.DropDownItems.Add(keepOpenFolder);
        }

        private void RecurseAddTemplatesMenuItemClickEvent(ToolStripMenuItem root)
        {
            if (root.Tag is string)
                root.Click += new EventHandler(SelectTemplateWithinHierarchy);
            else if (root.HasDropDownItems)
                foreach (ToolStripMenuItem item in root.DropDownItems)
                    RecurseAddTemplatesMenuItemClickEvent(item);

        }

        private string RecurseSearchTSForFileByName(ToolStripMenuItem root, string filePrefix)
        {
            if (root.Tag is string)
            {
                string fileAtTS = G.GetFilePrefix((string)root.Tag);

                if (fileAtTS == filePrefix)
                    return (string)root.Tag;
            }
            else if (root.HasDropDownItems)
                foreach (ToolStripMenuItem item in root.DropDownItems)
                    RecurseSearchTSForFileByName(item, filePrefix);

            return string.Empty;
        }

        #endregion

        #endregion

        #region Mouse Tab Moving

        private void PageMove(
            ref FormAndFuncs movePage,
            ref TabControl fromPane,
            ref TabControl toPane,
            int moveToIndex)
        {
            FormAndFuncs hoverToPage = GetHoverFF();

            int oldIndex = fromPane.TabPages.IndexOf(movePage);
            if (oldIndex == -1 ||
                moveToIndex == -1)
            {
                Console.WriteLine("No can move.");
                return;
            }

            int fromIndex= fromPane.TabPages.IndexOf(pageToMove);

            //Don't move page to own location... pointless
            if (moveToIndex == fromIndex &&
                hoverToPage == movePage)
                return;

            //Remove old page.
            fromPane.Controls.Remove(movePage);
            //if moving cross pane, select next closest tab in old Pane 
            if (fromPane != toPane)
                if (fromPane.TabPages.Count >= oldIndex)
                {
                    if (oldIndex == 0)
                        fromPane.SelectedIndex = oldIndex;
                    else
                        fromPane.SelectedIndex = oldIndex - 1;
                }
                else
                    fromPane.SelectedIndex = fromPane.TabPages.Count - 1;

            List<TabPage> otherTabs = new List<TabPage>();
            int toPanePageCount = toPane.TabPages.Count;

            //Store pages in list.
            for (int i = 0; i < toPanePageCount; i++)
                otherTabs.Add(toPane.TabPages[i]);
            toPane.TabPages.Clear();

            //Add pages back along with movePage.
            for (int i = 0; i < toPanePageCount + 1; i++)
            {
                if (i == moveToIndex)
                    toPane.TabPages.Add(pageToMove);
                else
                {
                    if (otherTabs.Count > 0)
                    {
                        toPane.TabPages.Add(otherTabs[0]);
                        otherTabs.RemoveAt(0);
                    }
                }
            }


            toPane.SelectedIndex = moveToIndex;

            pageToMove = null;
        }

        private void TryPageMove()
        {

            TabControl fromPane = null;
            TabControl toPane = GetHoverTabControl();
            FormAndFuncs hoverPage = GetHoverFF();

            if (pageToMove == null || toPane == null)
                return;
            else if (hoverPage == pageToMove)
                return;

            int moveToIndex = toPane.SelectedIndex;

            if (pageToMove.Parent != null)
                fromPane = (TabControl)pageToMove.Parent;

            if (fromPane == null || toPane == null)
                return;

            PageMove(
                ref pageToMove,
                ref fromPane,
               ref toPane,
               moveToIndex + 1);
        }

        private void TabControl_MouseDown(object sender, MouseEventArgs e)
        {
            TabControl thisPane = (TabControl)sender;
            int currentIndex = GetHoverTabIndex(thisPane);
            if (currentIndex == -1)
                return;

            pageToMove = (FormAndFuncs)thisPane.TabPages[currentIndex];
            pageToMove.Tag = currentIndex;
            
            if (pageToMove != null)
                Console.WriteLine(pageToMove.Name);
            else
                Console.WriteLine("NULL");
        }

        private void TabControl_MouseUp(object sender, MouseEventArgs e)
        {
            TabControl fromPane = (TabControl)sender;
            TabControl toPane = GetHoverTabControl();

            //If we know where we're going.
            if (toPane != null && pageToMove != null)
            {

            int fromIndex = -1;
            int toIndex = GetHoverTabIndex(toPane);

                fromIndex = (int)pageToMove.Tag;
                fromPane = (TabControl)pageToMove.Parent;

                //If not same tab and same pane.
                if (fromIndex != toIndex
                    || fromPane != toPane)
                {
                    if (fromPane != toPane
                        && toIndex == -1)
                        toIndex = 0;

                    PageMove(
                        ref pageToMove,
                        ref fromPane,
                        ref toPane,
                        toIndex);
                }
            }

            //The Tab was move, or not.
            pageToMove = null;
        }

        private int GetHoverTabIndex(TabControl pane)
        {
            for (int i = 0; i < pane.TabPages.Count; i++)
                if (pane.GetTabRect(i).Contains(pane.PointToClient(Cursor.Position)))
                {
                    return i;
                }

            if (pane.SelectedIndex == -1)
                return 0;
            else
                return pane.SelectedIndex;
        }

        private TabControl GetHoverTabControl()
        {
            foreach (Control ctrl in this.Controls)
            {
                if (ctrl is TabControl)
                {
                    TabControl thisPane = (TabControl)ctrl;
                    if (thisPane.ClientRectangle.Contains(thisPane.PointToClient(Cursor.Position)))
                    {
                        return thisPane;
                    }
                }
            }

            return null;
        }

        private FormAndFuncs GetHoverFF()
        {
            TabControl pane = GetHoverTabControl();

            if (pane == null) return null;

            int selectedIndex = (pane.SelectedIndex) == -1 ? 0 : pane.SelectedIndex;

            if (pane.TabPages.Count == 0)
                return null;

            FormAndFuncs ff = (FormAndFuncs)pane.TabPages[selectedIndex];

            return ff;
        }

        #endregion //Mouse Tab Moving.

        #region FFTab Handler

        public delegate void InsertDelegate(FormAndFuncs tabToInsert,
            ref TabControl intoPane);

        public void InsertFFTab(
            FormAndFuncs tabToInsert, 
            ref TabControl intoPane)
        {
            //Bind Context Menu
            tabToInsert.ContextMenuStrip = ctxFormFunc;

            //Insert into left or right Pane
            intoPane.Controls.Add(tabToInsert);
        }

        public bool RemoveFFTab(FormAndFuncs tabToRemove)
        {
            List<TabControl> tabPanes = GetTabPanes();
            //RemoveFFEvents(ref tabToRemove);

            foreach (TabControl pane in tabPanes)
            {
                if (pane.Contains(tabToRemove))
                {
                    pane.Controls.Remove(tabToRemove);
                    return true;
                }
            }

            tabToRemove.Dispose();
            return false;
        }

        public void RemoveAllPages()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(RemoveAllPages));
            }
            else
            {
                RemovePagesFromPane(ref leftPane);
                RemovePagesFromPane(ref rightPane);
                CurrentTemplateFilePath = string.Empty;
                CurrentBookPath = string.Empty;
                this.Text = G.c_ProgName + " - Untitled";
            }
        }

        public void RemovePagesFromPane(ref TabControl pagePane)
        {
            while (pagePane.Controls.Count > 0)
            {
                Control page = pagePane.Controls[0];

                if (page is FormAndFuncs)
                {
                    FormAndFuncs removeFF = (FormAndFuncs)page;
                    RemoveFFTab(removeFF);
                }
                else
                    pagePane.Controls.Remove(page);

                page.Dispose();
            }
        }

        public void TurnPageRight()
        {
            if (leftPane.Controls.Count > 0)
            {
                TabPage turnPage = (TabPage)leftPane.Controls[leftPane.Controls.Count - 1];
                rightPane.TabPages.Insert(0, turnPage);
            }

            CenterPageFocus();

        }

        public void TurnPageLeft()
        {
            if (rightPane.Controls.Count > 0)
            {
                rightPane.SelectedIndex = 0;
                TabPage turnPage = rightPane.SelectedTab;
                leftPane.Controls.Add(turnPage);               
            }

            CenterPageFocus();

        }

        private void CenterPageFocus()
        {
            if (leftPane.Controls.Count > 0)
                leftPane.SelectedIndex = leftPane.Controls.Count - 1;
            if (rightPane.Controls.Count > 0)
                rightPane.SelectedIndex = 0;

        }

        #endregion

        #region Generate Documents


        //Call upon start and finish of doc merge.
        private void EnableEditing(bool enable)
        {
            tsMenu.Enabled = enable;
            btnPageLeft.Enabled = enable;
            btnLeftPaneLeft.Enabled = enable;
            btnLeftPaneRight.Enabled = enable;
            btnPageRight.Enabled = enable;
            btnRightPaneLeft.Enabled = enable;
            btnRightPaneRight.Enabled = enable;
            btnSwap.Enabled = enable;
            leftPane.Enabled = enable;
            rightPane.Enabled = enable;
        }

        private string RequestDocGen(string docToGen)
        {
            string Err = G.c_NoErr;

            if (docToGen == string.Empty)
                return "You must target a Template document before attempting to generate. Please select a " + DocGen.c_docxExt +
                    " template from the Template Menu.";

            if (System.IO.File.Exists(docToGen))
            {
                string ext = System.IO.Path.GetExtension(docToGen);

                if (ext != DocGen.c_docxExt)
                {
                    Err = G.c_ProgName + " uses the " + DocGen.c_docxExt + " file format " +
                        " for document generation, not the ." + ext + " format." + Environment.NewLine;

                    if (ext == ".doc")
                    {
                        Err += Environment.NewLine +
                            "An old .doc file template can be converted using Microsoft Word 2007 or higher, " +
                            "or by using a current version of Apache OpenOffice.";
                    }

                    return Err;
                }

                this.ShowGeneratingWindow("Generating Custom Doc.", true);
                List<FormAndFuncs> formAndFuncs = GetAllFormFuncs();
                Fields fields = new Fields(formAndFuncs);

                DocGenArgs args = new DocGenArgs(
                    docToGen, formAndFuncs, fields);

                EnableEditing(false);
                bgDocGen.RunWorkerAsync(args);
            }
            else
            {
                if (docToGen == string.Empty)
                    Err = "No document template is selected. Try selecting a .docx template.";
                else
                    Err = "The template: " + docToGen + " does not exist. Try re-selecting the template at it's current location, or select a different template.";
            }



            return Err;
        }

        #endregion

        #region Loading Windows

        private void ShowFFBookLoading(bool show)
        {
            //try
            //{
                if (show)
                {
                    if (imageBox != null)
                    {
                        this.Controls.Add(imageBox);
                        imageBox.BringToFront();
                        imageBox.Refresh();
                    }
                }
                else
                {
                    this.Controls.Remove(imageBox);
                }
            //}
            //catch (Exception) { }


            //if (show)
            //{
            //    this.panGenerating.Size = new Size(423, 92);

            //    this.lblGenerating.Text = "Loading Input Page Book";
            //    this.panGenerating.Left = (this.Width / 2) - (panGenerating.Width / 2);
            //    this.panGenerating.Top = (this.Height / 2) - (panGenerating.Height / 2);
            //    this.BringToFront();
            //    this.panGenerating.Show();
            //    this.panGenerating.Refresh();
            //}
            //else
            //{
            //    this.panGenerating.Hide();
            //}
        }

        private void ShowGeneratingWindow(string message, bool show = false)
        {
            if (show)
            {

                this.panGenerating.Size = new Size(423, 242);

                this.lblGenerating.Text = message;
                this.panGenerating.Left = (this.Width / 2) - (panGenerating.Width / 2);
                this.panGenerating.Top = (this.Height / 2) - (panGenerating.Height / 2);
                this.BringToFront();
                this.panGenerating.Show();
            }
            else
            {
                this.panGenerating.Hide();
            }

            this.Update();
        }

        #endregion
        
        #region Ticker

        private void ticker_Tick(object sender, EventArgs e)
        {
            if (statusMessageOn)
            {
                if (statusMessageCountdown > 0)
                    statusMessageCountdown -= ticker.Interval;
                else
                { //time expired.
                    ShowMessage("", false, 0);
                }
            }
        }

        #endregion

        #region Display Minor Messages

        private void ShowMessage(string displayMessage, bool turnOn, int durationMS)
        {
            statusLabel.Text = displayMessage;
            statusMessageOn = turnOn;
            statusMessageCountdown = durationMS;
            statusLabel.Invalidate();
        }

        private void SetTemplateMessage(string templateMessage)
        {
            statusCurrentTemplate.Text = "Document Template: ( " + templateMessage + " )";
        }

        private void ShowProgressBar(bool show)
        {
            tsPbr.Value = 0;
            tsPbr.Visible = show;
        }

        #endregion

        #region Misc

        //Use to keep View/Add tests open.
        private bool recurseCheckContainsMouse(ref ToolStripMenuItem parent, Point mouseLoc)
        {
            foreach (ToolStripMenuItem child in parent.DropDownItems)
            {
                if (child.HasDropDownItems)
                    recurseCheckContainsMouse(ref parent, mouseLoc);

                if (child.ContentRectangle.Contains(mouseLoc))
                    return true;
                else
                    return false;
            }

            return false;
        }

        public List<FormAndFuncs> GetAllFormFuncs()
        {
            List<TabControl> tabPanes = GetTabPanes();
            List<FormAndFuncs> formFuncs = new List<FormAndFuncs>();

            foreach (TabControl pane in tabPanes)
                foreach (FormAndFuncs formFunc in pane.Controls)
                    formFuncs.Add(formFunc);

            return formFuncs;
        }

        public List<TabControl> GetTabPanes()
        {
            List<TabControl> tabPanes = new List<TabControl>();
            tabPanes.Add(leftPane);
            tabPanes.Add(rightPane);
            return tabPanes;
        }

        private FormAndFuncs GetControlFF(Control child)
        {
            if (child is FormAndFuncs)
                return (FormAndFuncs)child;
            else
            {
                if (child.Parent != null)
                    return GetControlFF(child.Parent);
                else
                    return null;
            }
                
        }

        private bool ProjectContainsPages()
        {
            if (leftPane.Controls.Count > 0)
                return true;
            else if (rightPane.Controls.Count > 0)
                return true;
            else
                return false;
        }

        private string GetPaneName(TabControl pane)
        {
            if (pane == null) return "Unknown";
            if (pane == leftPane)
                return "Left";
            else if (pane == rightPane)
                return "Right";
            else
                return "Unknown";
        }

        #endregion

        #region CT Updating

        #region Specific Actions
        /// <summary>
        /// Downloads the manifest files from web.
        /// </summary>
        private void CheckForUpdates()
        {
            //Ensure Update folder exists.
            UpdateManager.EnsureUpdateFolders();

            DownloaderArgs appArgs = UpdateManager.GetAppManArgs();
            DownloaderArgs docArgs = UpdateManager.GetDocManArgs();

            dlArgs.Add(appArgs);
            dlArgs.Add(docArgs);

            DownloadNextFile();
        }

        /// <summary>
        /// Compare old manifest to new manifest, download newer files.
        /// </summary>
        private void DetermineUpdates()
        {
            //Comapre to old manifest and add dlArgs to the update list, kick off downloader.
            dlArgs = UpdateManager.DetermineUpdates(UserLicense.values);
            DownloadNextFile();
        }


        #endregion

        /// <summary>
        /// Downloads the next file in dlArgs list.
        /// </summary>
        private void DownloadNextFile()
        {
            if (dlArgs.Count > 0)
            {
                DownloaderArgs args = dlArgs[0];
                dlArgs.RemoveAt(0);
                ShowProgressBar(true);
                ShowMessage("Downloading " + args.FileName, true, 9999999);
                bgDownloader.RunWorkerAsync(args);
            }
            else
                NotifyUpdatesReady(false); //No files to download. Notify Update Status.
            
        }

        private void PostDownloadAction(DownloaderArgs lastArgs)
        {

            if (dlArgs.Count > 0)
            {
                DownloadNextFile();
            }
            else
            {
                //Download Finished. Examine Reason token to determine next action.
                ShowProgressBar(false);

                if (lastArgs.Reason == DLReason.Manifest)
                    DetermineUpdates();
                if (lastArgs.Reason == DLReason.DocFile ||
                    lastArgs.Reason == DLReason.AppFile)
                    NotifyUpdatesReady(false);
            }
            
        }

        /// <summary>
        /// When update zips are available notify the user what to do next.
        /// </summary>
        private bool NotifyUpdatesReady(bool queryUpdateNow)
        {
            bool haveAppUpdates = UpdateManager.HaveAppUpdates();
            bool haveDocUpdates = UpdateManager.HaveDocUpdates();
            bool haveUpdates = haveAppUpdates || haveDocUpdates;

            runUpdaterOnExit = haveAppUpdates; //If have App updates, run updater on close.


            ShowProgressBar(false);

            if (haveAppUpdates && haveDocUpdates)
            {
                updateCTToolStripMenuItem.Enabled = true;
                updatePackageToolStripMenuItem.Enabled = true;
                ShowMessage("New CT and Template Updates.", true, 9000);
            }
            else if (haveAppUpdates)
            {
                updateCTToolStripMenuItem.Enabled = true;
                ShowMessage("New CT Updates.", true, 9000);
            }
            else if (haveDocUpdates)
            {
                updatePackageToolStripMenuItem.Enabled = true;
                ShowMessage("New Template Updates.", true, 9000);
            }
            else
            {
                ShowMessage("CT is up to date!", true, 5000);
            }


            //Return having any updates.
            if (haveAppUpdates || haveDocUpdates)
                return true;
            else
                return false;
        }

        private void DoDocUpdates(bool query)
        {
            string longQuestion = "Before performing the package update, please make sure " +
                "all Document Templates are not open in any other programs. Otherwise the Package Update will fail."
                + Environment.NewLine + Environment.NewLine +
                "The following package(s) will be updated: " + Environment.NewLine;

            List<string> packages = UpdateManager.DocPackageNamesToUpdate();

            foreach (string packageName in packages)
            {
                longQuestion += packageName + Environment.NewLine;
            }

            longQuestion += Environment.NewLine + Environment.NewLine + "Update Packages now?";
            DialogResult result = G.YesNoQuestion(longQuestion, "Ready to Update Packages?");


            if (result != DialogResult.Yes)
                return;
                    
            //Update Docs on background worker.
            bgDocUpdate.RunWorkerAsync();            
        }

        private void DoAppUpdates(bool query, bool restart)
        {
            if (query)
            {
                DialogResult result = G.YesNoQuestion("Unsaved work will be lost. Save Book before updating?",
                    "Save work?");

                if (result == DialogResult.Cancel)
                    return;

                if (result == DialogResult.Yes)
                    SaveFFBook();
            }

            //Close if updater started successfully.
            if (UpdateManager.UpdateAppPackages(restart))
            {
                forceClose = true;
                Application.Exit();
            }
        }


        #endregion


       

    }
}

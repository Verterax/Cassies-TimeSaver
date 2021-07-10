namespace ReportGen
{
    partial class Main
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.tsMenu = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.inputToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pageDesignerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openPagesFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.templateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectTemplateFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openTemplatesFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.generateDocumentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.generateDocumentToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.openOutputFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cTManualToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.installUpdateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.checkForUpdateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.updateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.updateCTToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.updatePackageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panGenerating = new System.Windows.Forms.Panel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lblSecondMsg = new System.Windows.Forms.Label();
            this.lblMainMsg = new System.Windows.Forms.Label();
            this.picLoading = new System.Windows.Forms.PictureBox();
            this.pbr2 = new System.Windows.Forms.ProgressBar();
            this.pbr1 = new System.Windows.Forms.ProgressBar();
            this.lblGenerating = new System.Windows.Forms.Label();
            this.bgDocGen = new System.ComponentModel.BackgroundWorker();
            this.rightPane = new System.Windows.Forms.TabControl();
            this.leftPane = new System.Windows.Forms.TabControl();
            this.dlgFileOpen = new System.Windows.Forms.OpenFileDialog();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.statusCurrentTemplate = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsPbr = new System.Windows.Forms.ToolStripProgressBar();
            this.ctxFormFunc = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.fileToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.readFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editInDesignerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearPageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearThisSideToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearEntireBookToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removePageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bgRefreshPageHierarchy = new System.ComponentModel.BackgroundWorker();
            this.bgRefreshTemplateHierarchy = new System.ComponentModel.BackgroundWorker();
            this.ticker = new System.Windows.Forms.Timer(this.components);
            this.btnPageRight = new System.Windows.Forms.Button();
            this.btnPageLeft = new System.Windows.Forms.Button();
            this.btnRightPaneLeft = new System.Windows.Forms.Button();
            this.btnRightPaneRight = new System.Windows.Forms.Button();
            this.btnLeftPaneRight = new System.Windows.Forms.Button();
            this.btnLeftPaneLeft = new System.Windows.Forms.Button();
            this.btnSwap = new System.Windows.Forms.Button();
            this.bgLoadBook = new System.ComponentModel.BackgroundWorker();
            this.bgDownloader = new System.ComponentModel.BackgroundWorker();
            this.bgDocUpdate = new System.ComponentModel.BackgroundWorker();
            this.tsMenu.SuspendLayout();
            this.panGenerating.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picLoading)).BeginInit();
            this.statusStrip.SuspendLayout();
            this.ctxFormFunc.SuspendLayout();
            this.SuspendLayout();
            // 
            // tsMenu
            // 
            this.tsMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.inputToolStripMenuItem,
            this.templateToolStripMenuItem,
            this.generateDocumentToolStripMenuItem,
            this.aboutToolStripMenuItem,
            this.updateToolStripMenuItem});
            this.tsMenu.Location = new System.Drawing.Point(0, 0);
            this.tsMenu.Name = "tsMenu";
            this.tsMenu.Size = new System.Drawing.Size(1008, 24);
            this.tsMenu.TabIndex = 0;
            this.tsMenu.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newProjectToolStripMenuItem,
            this.saveProjectToolStripMenuItem,
            this.openProjectToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // newProjectToolStripMenuItem
            // 
            this.newProjectToolStripMenuItem.Name = "newProjectToolStripMenuItem";
            this.newProjectToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
            this.newProjectToolStripMenuItem.Text = "New Book";
            this.newProjectToolStripMenuItem.Click += new System.EventHandler(this.newProjectToolStripMenuItem_Click);
            // 
            // saveProjectToolStripMenuItem
            // 
            this.saveProjectToolStripMenuItem.Name = "saveProjectToolStripMenuItem";
            this.saveProjectToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
            this.saveProjectToolStripMenuItem.Text = "Save Book";
            this.saveProjectToolStripMenuItem.Click += new System.EventHandler(this.saveProjectToolStripMenuItem_Click);
            // 
            // openProjectToolStripMenuItem
            // 
            this.openProjectToolStripMenuItem.Name = "openProjectToolStripMenuItem";
            this.openProjectToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
            this.openProjectToolStripMenuItem.Text = "Open Book";
            this.openProjectToolStripMenuItem.Click += new System.EventHandler(this.openProjectToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // inputToolStripMenuItem
            // 
            this.inputToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.pageDesignerToolStripMenuItem,
            this.selectFileToolStripMenuItem,
            this.openPagesFolderToolStripMenuItem});
            this.inputToolStripMenuItem.Name = "inputToolStripMenuItem";
            this.inputToolStripMenuItem.Size = new System.Drawing.Size(47, 20);
            this.inputToolStripMenuItem.Text = "Input";
            // 
            // pageDesignerToolStripMenuItem
            // 
            this.pageDesignerToolStripMenuItem.Name = "pageDesignerToolStripMenuItem";
            this.pageDesignerToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.pageDesignerToolStripMenuItem.Text = "Open Designer";
            this.pageDesignerToolStripMenuItem.Click += new System.EventHandler(this.tabDesignerToolStripMenuItem_Click_1);
            // 
            // selectFileToolStripMenuItem
            // 
            this.selectFileToolStripMenuItem.Name = "selectFileToolStripMenuItem";
            this.selectFileToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.selectFileToolStripMenuItem.Text = "Import Page";
            this.selectFileToolStripMenuItem.Click += new System.EventHandler(this.selectFileToolStripMenuItem_Click);
            // 
            // openPagesFolderToolStripMenuItem
            // 
            this.openPagesFolderToolStripMenuItem.Name = "openPagesFolderToolStripMenuItem";
            this.openPagesFolderToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.openPagesFolderToolStripMenuItem.Text = "Folder";
            this.openPagesFolderToolStripMenuItem.Click += new System.EventHandler(this.openPagesFolderToolStripMenuItem_Click);
            // 
            // templateToolStripMenuItem
            // 
            this.templateToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.selectTemplateFileToolStripMenuItem,
            this.openTemplatesFolderToolStripMenuItem});
            this.templateToolStripMenuItem.Name = "templateToolStripMenuItem";
            this.templateToolStripMenuItem.Size = new System.Drawing.Size(69, 20);
            this.templateToolStripMenuItem.Text = "Template";
            // 
            // selectTemplateFileToolStripMenuItem
            // 
            this.selectTemplateFileToolStripMenuItem.Name = "selectTemplateFileToolStripMenuItem";
            this.selectTemplateFileToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.selectTemplateFileToolStripMenuItem.Text = "Select";
            this.selectTemplateFileToolStripMenuItem.Click += new System.EventHandler(this.selectTemplateFileToolStripMenuItem_Click);
            // 
            // openTemplatesFolderToolStripMenuItem
            // 
            this.openTemplatesFolderToolStripMenuItem.Name = "openTemplatesFolderToolStripMenuItem";
            this.openTemplatesFolderToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.openTemplatesFolderToolStripMenuItem.Text = "Folder";
            this.openTemplatesFolderToolStripMenuItem.Click += new System.EventHandler(this.openTemplatesFolderToolStripMenuItem_Click);
            // 
            // generateDocumentToolStripMenuItem
            // 
            this.generateDocumentToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.generateDocumentToolStripMenuItem1,
            this.openOutputFolderToolStripMenuItem});
            this.generateDocumentToolStripMenuItem.Name = "generateDocumentToolStripMenuItem";
            this.generateDocumentToolStripMenuItem.Size = new System.Drawing.Size(57, 20);
            this.generateDocumentToolStripMenuItem.Text = "Output";
            // 
            // generateDocumentToolStripMenuItem1
            // 
            this.generateDocumentToolStripMenuItem1.Name = "generateDocumentToolStripMenuItem1";
            this.generateDocumentToolStripMenuItem1.Size = new System.Drawing.Size(180, 22);
            this.generateDocumentToolStripMenuItem1.Text = "Generate Document";
            this.generateDocumentToolStripMenuItem1.Click += new System.EventHandler(this.generateDocumentToolStripMenuItem1_Click);
            // 
            // openOutputFolderToolStripMenuItem
            // 
            this.openOutputFolderToolStripMenuItem.Name = "openOutputFolderToolStripMenuItem";
            this.openOutputFolderToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.openOutputFolderToolStripMenuItem.Text = "Folder";
            this.openOutputFolderToolStripMenuItem.Click += new System.EventHandler(this.openOutputFolderToolStripMenuItem_Click);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cTManualToolStripMenuItem,
            this.aboutToolStripMenuItem1,
            this.installUpdateToolStripMenuItem,
            this.checkForUpdateToolStripMenuItem});
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.aboutToolStripMenuItem.Text = "Help";
            // 
            // cTManualToolStripMenuItem
            // 
            this.cTManualToolStripMenuItem.Name = "cTManualToolStripMenuItem";
            this.cTManualToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.cTManualToolStripMenuItem.Text = "CT Manual";
            this.cTManualToolStripMenuItem.Click += new System.EventHandler(this.cTManualToolStripMenuItem_Click);
            // 
            // aboutToolStripMenuItem1
            // 
            this.aboutToolStripMenuItem1.Name = "aboutToolStripMenuItem1";
            this.aboutToolStripMenuItem1.Size = new System.Drawing.Size(166, 22);
            this.aboutToolStripMenuItem1.Text = "About";
            this.aboutToolStripMenuItem1.Click += new System.EventHandler(this.aboutToolStripMenuItem1_Click);
            // 
            // installUpdateToolStripMenuItem
            // 
            this.installUpdateToolStripMenuItem.Enabled = false;
            this.installUpdateToolStripMenuItem.Name = "installUpdateToolStripMenuItem";
            this.installUpdateToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.installUpdateToolStripMenuItem.Text = "Install Update";
            this.installUpdateToolStripMenuItem.Visible = false;
            // 
            // checkForUpdateToolStripMenuItem
            // 
            this.checkForUpdateToolStripMenuItem.Name = "checkForUpdateToolStripMenuItem";
            this.checkForUpdateToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.checkForUpdateToolStripMenuItem.Text = "Check for Update";
            this.checkForUpdateToolStripMenuItem.Visible = false;
            this.checkForUpdateToolStripMenuItem.Click += new System.EventHandler(this.checkForUpdateToolStripMenuItem_Click);
            // 
            // updateToolStripMenuItem
            // 
            this.updateToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.updateCTToolStripMenuItem,
            this.updatePackageToolStripMenuItem});
            this.updateToolStripMenuItem.Enabled = false;
            this.updateToolStripMenuItem.Name = "updateToolStripMenuItem";
            this.updateToolStripMenuItem.Size = new System.Drawing.Size(57, 20);
            this.updateToolStripMenuItem.Text = "Update";
            this.updateToolStripMenuItem.Visible = false;
            // 
            // updateCTToolStripMenuItem
            // 
            this.updateCTToolStripMenuItem.Enabled = false;
            this.updateCTToolStripMenuItem.Name = "updateCTToolStripMenuItem";
            this.updateCTToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
            this.updateCTToolStripMenuItem.Text = "Update CT";
            this.updateCTToolStripMenuItem.Click += new System.EventHandler(this.updateCTToolStripMenuItem_Click);
            // 
            // updatePackageToolStripMenuItem
            // 
            this.updatePackageToolStripMenuItem.Enabled = false;
            this.updatePackageToolStripMenuItem.Name = "updatePackageToolStripMenuItem";
            this.updatePackageToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
            this.updatePackageToolStripMenuItem.Text = "Update Package";
            this.updatePackageToolStripMenuItem.Click += new System.EventHandler(this.updatePackageToolStripMenuItem_Click);
            // 
            // panGenerating
            // 
            this.panGenerating.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panGenerating.Controls.Add(this.btnCancel);
            this.panGenerating.Controls.Add(this.lblSecondMsg);
            this.panGenerating.Controls.Add(this.lblMainMsg);
            this.panGenerating.Controls.Add(this.picLoading);
            this.panGenerating.Controls.Add(this.pbr2);
            this.panGenerating.Controls.Add(this.pbr1);
            this.panGenerating.Controls.Add(this.lblGenerating);
            this.panGenerating.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panGenerating.Location = new System.Drawing.Point(1106, 319);
            this.panGenerating.Name = "panGenerating";
            this.panGenerating.Size = new System.Drawing.Size(423, 242);
            this.panGenerating.TabIndex = 116;
            // 
            // btnCancel
            // 
            this.btnCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.Location = new System.Drawing.Point(156, 196);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(117, 33);
            this.btnCancel.TabIndex = 117;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // lblSecondMsg
            // 
            this.lblSecondMsg.AutoSize = true;
            this.lblSecondMsg.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSecondMsg.Location = new System.Drawing.Point(100, 164);
            this.lblSecondMsg.Name = "lblSecondMsg";
            this.lblSecondMsg.Size = new System.Drawing.Size(71, 16);
            this.lblSecondMsg.TabIndex = 116;
            this.lblSecondMsg.Text = "Main Task";
            // 
            // lblMainMsg
            // 
            this.lblMainMsg.AutoSize = true;
            this.lblMainMsg.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMainMsg.Location = new System.Drawing.Point(100, 87);
            this.lblMainMsg.Name = "lblMainMsg";
            this.lblMainMsg.Size = new System.Drawing.Size(71, 16);
            this.lblMainMsg.TabIndex = 115;
            this.lblMainMsg.Text = "Main Task";
            // 
            // picLoading
            // 
            this.picLoading.Image = ((System.Drawing.Image)(resources.GetObject("picLoading.Image")));
            this.picLoading.Location = new System.Drawing.Point(103, 59);
            this.picLoading.Name = "picLoading";
            this.picLoading.Size = new System.Drawing.Size(223, 21);
            this.picLoading.TabIndex = 113;
            this.picLoading.TabStop = false;
            // 
            // pbr2
            // 
            this.pbr2.Location = new System.Drawing.Point(73, 138);
            this.pbr2.Name = "pbr2";
            this.pbr2.Size = new System.Drawing.Size(281, 23);
            this.pbr2.TabIndex = 114;
            // 
            // pbr1
            // 
            this.pbr1.Location = new System.Drawing.Point(73, 109);
            this.pbr1.Name = "pbr1";
            this.pbr1.Size = new System.Drawing.Size(281, 23);
            this.pbr1.TabIndex = 113;
            // 
            // lblGenerating
            // 
            this.lblGenerating.AutoSize = true;
            this.lblGenerating.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblGenerating.Location = new System.Drawing.Point(56, 17);
            this.lblGenerating.Name = "lblGenerating";
            this.lblGenerating.Size = new System.Drawing.Size(314, 29);
            this.lblGenerating.TabIndex = 112;
            this.lblGenerating.Text = "Generating your document...";
            // 
            // bgDocGen
            // 
            this.bgDocGen.WorkerReportsProgress = true;
            this.bgDocGen.WorkerSupportsCancellation = true;
            this.bgDocGen.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgDocGen_DoWork);
            this.bgDocGen.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.bgDocGen_ProgressChanged);
            this.bgDocGen.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgDocGen_RunWorkerCompleted);
            // 
            // rightPane
            // 
            this.rightPane.AllowDrop = true;
            this.rightPane.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.rightPane.Location = new System.Drawing.Point(508, 59);
            this.rightPane.Name = "rightPane";
            this.rightPane.SelectedIndex = 0;
            this.rightPane.Size = new System.Drawing.Size(496, 642);
            this.rightPane.TabIndex = 6;
            // 
            // leftPane
            // 
            this.leftPane.AllowDrop = true;
            this.leftPane.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.leftPane.Location = new System.Drawing.Point(6, 59);
            this.leftPane.Name = "leftPane";
            this.leftPane.SelectedIndex = 0;
            this.leftPane.Size = new System.Drawing.Size(496, 642);
            this.leftPane.TabIndex = 6;
            // 
            // dlgFileOpen
            // 
            this.dlgFileOpen.FileName = "openFileDialog1";
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusCurrentTemplate,
            this.statusLabel,
            this.tsPbr});
            this.statusStrip.Location = new System.Drawing.Point(0, 708);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.statusStrip.Size = new System.Drawing.Size(1008, 22);
            this.statusStrip.TabIndex = 129;
            this.statusStrip.Text = "statusStrip1";
            // 
            // statusCurrentTemplate
            // 
            this.statusCurrentTemplate.ActiveLinkColor = System.Drawing.Color.Magenta;
            this.statusCurrentTemplate.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.statusCurrentTemplate.ForeColor = System.Drawing.Color.Navy;
            this.statusCurrentTemplate.Name = "statusCurrentTemplate";
            this.statusCurrentTemplate.Size = new System.Drawing.Size(153, 17);
            this.statusCurrentTemplate.Text = "Template:  ( I\'maTemplate )";
            // 
            // statusLabel
            // 
            this.statusLabel.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.statusLabel.ForeColor = System.Drawing.Color.OrangeRed;
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(70, 17);
            this.statusLabel.Text = "Status Label";
            // 
            // tsPbr
            // 
            this.tsPbr.Name = "tsPbr";
            this.tsPbr.Size = new System.Drawing.Size(100, 16);
            // 
            // ctxFormFunc
            // 
            this.ctxFormFunc.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem1,
            this.editInDesignerToolStripMenuItem,
            this.cToolStripMenuItem,
            this.removePageToolStripMenuItem});
            this.ctxFormFunc.Name = "ctxFormFunc";
            this.ctxFormFunc.Size = new System.Drawing.Size(157, 92);
            // 
            // fileToolStripMenuItem1
            // 
            this.fileToolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.readFileToolStripMenuItem,
            this.saveAsToolStripMenuItem});
            this.fileToolStripMenuItem1.Name = "fileToolStripMenuItem1";
            this.fileToolStripMenuItem1.Size = new System.Drawing.Size(156, 22);
            this.fileToolStripMenuItem1.Text = "File";
            // 
            // readFileToolStripMenuItem
            // 
            this.readFileToolStripMenuItem.Name = "readFileToolStripMenuItem";
            this.readFileToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this.readFileToolStripMenuItem.Text = "Fill From File";
            this.readFileToolStripMenuItem.Click += new System.EventHandler(this.readFileToolStripMenuItem_Click);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this.saveAsToolStripMenuItem.Text = "SaveAs";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
            // 
            // editInDesignerToolStripMenuItem
            // 
            this.editInDesignerToolStripMenuItem.Name = "editInDesignerToolStripMenuItem";
            this.editInDesignerToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.editInDesignerToolStripMenuItem.Text = "Edit in Designer";
            this.editInDesignerToolStripMenuItem.Click += new System.EventHandler(this.editInDesignerToolStripMenuItem_Click);
            // 
            // cToolStripMenuItem
            // 
            this.cToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.clearPageToolStripMenuItem,
            this.clearThisSideToolStripMenuItem,
            this.clearEntireBookToolStripMenuItem});
            this.cToolStripMenuItem.Name = "cToolStripMenuItem";
            this.cToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.cToolStripMenuItem.Text = "Clear Fields";
            // 
            // clearPageToolStripMenuItem
            // 
            this.clearPageToolStripMenuItem.Name = "clearPageToolStripMenuItem";
            this.clearPageToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.clearPageToolStripMenuItem.Text = "Clear Page";
            this.clearPageToolStripMenuItem.Click += new System.EventHandler(this.clearPageToolStripMenuItem_Click);
            // 
            // clearThisSideToolStripMenuItem
            // 
            this.clearThisSideToolStripMenuItem.Name = "clearThisSideToolStripMenuItem";
            this.clearThisSideToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.clearThisSideToolStripMenuItem.Text = "Clear This Side";
            this.clearThisSideToolStripMenuItem.Click += new System.EventHandler(this.clearThisSideToolStripMenuItem_Click);
            // 
            // clearEntireBookToolStripMenuItem
            // 
            this.clearEntireBookToolStripMenuItem.Name = "clearEntireBookToolStripMenuItem";
            this.clearEntireBookToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.clearEntireBookToolStripMenuItem.Text = "Clear Entire Book";
            this.clearEntireBookToolStripMenuItem.Click += new System.EventHandler(this.clearEntireBookToolStripMenuItem_Click);
            // 
            // removePageToolStripMenuItem
            // 
            this.removePageToolStripMenuItem.Name = "removePageToolStripMenuItem";
            this.removePageToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.removePageToolStripMenuItem.Text = "Remove Page";
            this.removePageToolStripMenuItem.Click += new System.EventHandler(this.removePageToolStripMenuItem_Click);
            // 
            // bgRefreshPageHierarchy
            // 
            this.bgRefreshPageHierarchy.WorkerSupportsCancellation = true;
            this.bgRefreshPageHierarchy.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgRefreshPageHierarchy_DoWork);
            this.bgRefreshPageHierarchy.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgRefreshPageHierarchy_RunWorkerCompleted);
            // 
            // bgRefreshTemplateHierarchy
            // 
            this.bgRefreshTemplateHierarchy.WorkerSupportsCancellation = true;
            this.bgRefreshTemplateHierarchy.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgRefreshTemplateHierarchy_DoWork);
            this.bgRefreshTemplateHierarchy.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgRefreshTemplateHierarchy_RunWorkerCompleted);
            // 
            // ticker
            // 
            this.ticker.Tick += new System.EventHandler(this.ticker_Tick);
            // 
            // btnPageRight
            // 
            this.btnPageRight.BackColor = System.Drawing.Color.SteelBlue;
            this.btnPageRight.Location = new System.Drawing.Point(533, 27);
            this.btnPageRight.Name = "btnPageRight";
            this.btnPageRight.Size = new System.Drawing.Size(212, 32);
            this.btnPageRight.TabIndex = 130;
            this.btnPageRight.Text = "-->>";
            this.btnPageRight.UseVisualStyleBackColor = false;
            this.btnPageRight.Click += new System.EventHandler(this.btnPageRight_Click);
            // 
            // btnPageLeft
            // 
            this.btnPageLeft.BackColor = System.Drawing.Color.SteelBlue;
            this.btnPageLeft.Location = new System.Drawing.Point(265, 27);
            this.btnPageLeft.Name = "btnPageLeft";
            this.btnPageLeft.Size = new System.Drawing.Size(212, 32);
            this.btnPageLeft.TabIndex = 131;
            this.btnPageLeft.Text = "<<--";
            this.btnPageLeft.UseVisualStyleBackColor = false;
            this.btnPageLeft.Click += new System.EventHandler(this.btnPageLeft_Click);
            // 
            // btnRightPaneLeft
            // 
            this.btnRightPaneLeft.BackColor = System.Drawing.Color.LightSteelBlue;
            this.btnRightPaneLeft.Location = new System.Drawing.Point(751, 27);
            this.btnRightPaneLeft.Name = "btnRightPaneLeft";
            this.btnRightPaneLeft.Size = new System.Drawing.Size(126, 32);
            this.btnRightPaneLeft.TabIndex = 132;
            this.btnRightPaneLeft.Text = "<...";
            this.btnRightPaneLeft.UseVisualStyleBackColor = false;
            this.btnRightPaneLeft.Click += new System.EventHandler(this.btnRightPaneLeft_Click);
            // 
            // btnRightPaneRight
            // 
            this.btnRightPaneRight.BackColor = System.Drawing.Color.LightSteelBlue;
            this.btnRightPaneRight.Location = new System.Drawing.Point(878, 27);
            this.btnRightPaneRight.Name = "btnRightPaneRight";
            this.btnRightPaneRight.Size = new System.Drawing.Size(126, 32);
            this.btnRightPaneRight.TabIndex = 132;
            this.btnRightPaneRight.Text = "...>";
            this.btnRightPaneRight.UseVisualStyleBackColor = false;
            this.btnRightPaneRight.Click += new System.EventHandler(this.btnRightPaneRight_Click);
            // 
            // btnLeftPaneRight
            // 
            this.btnLeftPaneRight.BackColor = System.Drawing.Color.LightSteelBlue;
            this.btnLeftPaneRight.Location = new System.Drawing.Point(133, 27);
            this.btnLeftPaneRight.Name = "btnLeftPaneRight";
            this.btnLeftPaneRight.Size = new System.Drawing.Size(126, 32);
            this.btnLeftPaneRight.TabIndex = 134;
            this.btnLeftPaneRight.Text = "...>";
            this.btnLeftPaneRight.UseVisualStyleBackColor = false;
            this.btnLeftPaneRight.Click += new System.EventHandler(this.btnLeftPaneRight_Click);
            // 
            // btnLeftPaneLeft
            // 
            this.btnLeftPaneLeft.BackColor = System.Drawing.Color.LightSteelBlue;
            this.btnLeftPaneLeft.Location = new System.Drawing.Point(6, 27);
            this.btnLeftPaneLeft.Name = "btnLeftPaneLeft";
            this.btnLeftPaneLeft.Size = new System.Drawing.Size(126, 32);
            this.btnLeftPaneLeft.TabIndex = 133;
            this.btnLeftPaneLeft.Text = "<...";
            this.btnLeftPaneLeft.UseVisualStyleBackColor = false;
            this.btnLeftPaneLeft.Click += new System.EventHandler(this.btnLeftPaneLeft_Click);
            // 
            // btnSwap
            // 
            this.btnSwap.BackColor = System.Drawing.Color.RoyalBlue;
            this.btnSwap.Location = new System.Drawing.Point(483, 27);
            this.btnSwap.Name = "btnSwap";
            this.btnSwap.Size = new System.Drawing.Size(44, 32);
            this.btnSwap.TabIndex = 135;
            this.btnSwap.Text = "-><-";
            this.btnSwap.UseVisualStyleBackColor = false;
            this.btnSwap.Click += new System.EventHandler(this.btnSwap_Click);
            // 
            // bgLoadBook
            // 
            this.bgLoadBook.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgLoadBook_DoWork);
            this.bgLoadBook.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgLoadBook_RunWorkerCompleted);
            // 
            // bgDownloader
            // 
            this.bgDownloader.WorkerReportsProgress = true;
            this.bgDownloader.WorkerSupportsCancellation = true;
            this.bgDownloader.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgDownloader_DoWork);
            this.bgDownloader.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.bgDownloader_ProgressChanged);
            this.bgDownloader.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgDownloader_RunWorkerCompleted);
            // 
            // bgDocUpdate
            // 
            this.bgDocUpdate.WorkerReportsProgress = true;
            this.bgDocUpdate.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgDocUpdate_DoWork);
            this.bgDocUpdate.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.bgDocUpdate_ProgressChanged);
            this.bgDocUpdate.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgDocUpdate_RunWorkerCompleted);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightSteelBlue;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(1008, 730);
            this.Controls.Add(this.btnSwap);
            this.Controls.Add(this.btnLeftPaneRight);
            this.Controls.Add(this.btnLeftPaneLeft);
            this.Controls.Add(this.btnRightPaneRight);
            this.Controls.Add(this.btnRightPaneLeft);
            this.Controls.Add(this.btnPageLeft);
            this.Controls.Add(this.btnPageRight);
            this.Controls.Add(this.panGenerating);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.leftPane);
            this.Controls.Add(this.rightPane);
            this.Controls.Add(this.tsMenu);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.tsMenu;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(2000, 2000);
            this.MinimumSize = new System.Drawing.Size(1024, 768);
            this.Name = "Main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Cassie\'s TimeSaver";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Main_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.tsMenu.ResumeLayout(false);
            this.tsMenu.PerformLayout();
            this.panGenerating.ResumeLayout(false);
            this.panGenerating.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picLoading)).EndInit();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ctxFormFunc.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

       }

        #endregion

        private System.Windows.Forms.MenuStrip tsMenu;
        private System.Windows.Forms.Panel panGenerating;
        private System.Windows.Forms.PictureBox picLoading;
        private System.Windows.Forms.Label lblGenerating;
        private System.ComponentModel.BackgroundWorker bgDocGen;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.TabControl rightPane;
        private System.Windows.Forms.TabControl leftPane;
        private System.Windows.Forms.OpenFileDialog dlgFileOpen;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ContextMenuStrip ctxFormFunc;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem readFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editInDesignerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removePageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem inputToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem selectFileToolStripMenuItem;
        private System.ComponentModel.BackgroundWorker bgRefreshPageHierarchy;
        private System.Windows.Forms.ToolStripMenuItem templateToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem selectTemplateFileToolStripMenuItem;
        private System.ComponentModel.BackgroundWorker bgRefreshTemplateHierarchy;
        private System.Windows.Forms.ToolStripStatusLabel statusCurrentTemplate;
        private System.Windows.Forms.ToolStripMenuItem pageDesignerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem generateDocumentToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem generateDocumentToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem openOutputFolderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openTemplatesFolderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newProjectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveProjectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openProjectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openPagesFolderToolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel statusLabel;
        private System.Windows.Forms.Timer ticker;
        private System.Windows.Forms.Button btnPageRight;
        private System.Windows.Forms.Button btnPageLeft;
        private System.Windows.Forms.Button btnRightPaneLeft;
        private System.Windows.Forms.Button btnRightPaneRight;
        private System.Windows.Forms.Button btnLeftPaneRight;
        private System.Windows.Forms.Button btnLeftPaneLeft;
        private System.Windows.Forms.Button btnSwap;
        private System.Windows.Forms.ProgressBar pbr2;
        private System.Windows.Forms.ProgressBar pbr1;
        private System.Windows.Forms.Label lblSecondMsg;
        private System.Windows.Forms.Label lblMainMsg;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.ComponentModel.BackgroundWorker bgLoadBook;
        private System.Windows.Forms.ToolStripMenuItem clearPageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearEntireBookToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearThisSideToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cTManualToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem installUpdateToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem checkForUpdateToolStripMenuItem;
        private System.ComponentModel.BackgroundWorker bgDownloader;
        private System.Windows.Forms.ToolStripProgressBar tsPbr;
        private System.Windows.Forms.ToolStripMenuItem updateToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem updateCTToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem updatePackageToolStripMenuItem;
        private System.ComponentModel.BackgroundWorker bgDocUpdate;
    }
}


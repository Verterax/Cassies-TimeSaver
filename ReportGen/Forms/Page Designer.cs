using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
//using Helper;
using System.Text.RegularExpressions;

namespace ReportGen
{
    public partial class Page_Designer : Form
    {

        #region Consts

        //Button dimensions, padding, of offsets
        //Action Consts
        private const int c_xRootOffset = 5;
        private const int c_yRootOffset = 50;
        private const int c_xyPadding = 5;
        private const int c_minCtrlSize = 10;

        private const int c_ffOffsetX = 13; //13
        private const int c_ffOffsetY = 55; //55
        private const int c_FFScrollMargin = 50;

        //Visual Editing Consts
        private const string c_labelPrefix = "__lbl";
        private const int c_ScrollChange = 40;
        private const int c_repasteDelay = 50;

        //Layers of undo / redo
        private const int c_layersOfUndoRedo = 50;

        #endregion

        #region Variables

        public FormAndFuncs backupFF;

        #region Layout Variables

        
        bool resizing;
        
        List<Control> selectedControls;
        TabControl bufferPane;
        bool ignoreLabelChange;
        bool tagnameChanged;
        bool labelChanged;
        bool comboItemsChanged;
        bool calandarDropped;

        //Mouse Rubberband
        //bool rubberbanding;
        bool rubberbanding;
        bool mouseDown;
        bool mouseMoving;
        bool skipOnce;
        Point ptMouseDownAt;
        Point ptOldMouse;
        Control rbLanding;

        //Editing Cut, Copy, Paste
        bool copyEnabled;
        bool pasteEnabled;
        Point pasteLocation;
        DateTime lastKeyActionAt;

        //Control dragging.
        bool newControl;
        bool draggingControls;
        Point dragStart;
        Point lastMouseLoc;
        Point controlGrabLoc;


        //Key Presses
        bool keyDownPressed = false;
        bool keyUpPressed = false;
        bool keyLeftPressed = false;
        bool keyRightPressed = false;

        //Mouse buttons
        bool mouseLeftIsDown;
        bool mouseRightIsDown;
        

        #endregion

        #region Action Variables

        Dictionary<string, Action> actionList;
        public bool actionChanged;

        #endregion //Actions

        #region DocReader Variables

        Dictionary<string, FlatRegexField> regexList;
        bool regexChanged;
        #endregion //DocReader

        #region Misc Variables

        //Debug
        private string viewPos = "checkBox1";


        //Reference to main page's panes.
        private bool changesMade;
        private bool fromLeft;
        private int fromIndex;

        public bool editMode;
        public bool statusMessageOn;
        private bool debug = false;
        public int statusMessageCountdown; //Time in ms
        private string lastOpenFilePath;
        TabControl rightPane;
        TabControl leftPane;

        Stack<FormAndFuncs> undoStack;
        Stack<FormAndFuncs> redoStack;

        #endregion //Misc

        #endregion //Variables

        #region Load/Constructors

        public Page_Designer(ref TabControl rightPane, ref TabControl leftPane)
        {
            InitializeComponent();
            InitializeLogic();
            backupFF = null;
            this.rightPane = rightPane;
            this.leftPane = leftPane;
            editMode = false;

            NewFormFunc();
        }

        //For Editing from Main.
        public Page_Designer(
            ref TabControl rightPane, 
            ref TabControl leftPane,
            ref FormAndFuncs editFF,
            bool fromLeft,
            int fromIndex)
        {
            InitializeComponent();
            InitializeLogic();

            editMode = true;
            this.fromLeft = fromLeft;
            this.fromIndex = fromIndex;
            this.rightPane = rightPane;
            this.leftPane = leftPane;

            Console.WriteLine(editFF.Controls.Count + " controls in " + editFF.Name);

            backupFF = new FormAndFuncs(editFF);

            InsertFormFunc(ref editFF);
        }


        private void InitializeLogic()
        {
            btnNewAction.BackColor = LogicButton.c_ActionColor;
            actionList = new Dictionary<string, Action>();
            regexList = new Dictionary<string, FlatRegexField>();
            selectedControls = new List<Control>();
            lastOpenFilePath = string.Empty;
            regexChanged = true; 
            actionChanged = true;

            ptMouseDownAt = new Point();
            ptOldMouse = new Point();
            rbLanding = null;

            //Undo redo stack
            undoStack = new Stack<FormAndFuncs>(c_layersOfUndoRedo);
            redoStack = new Stack<FormAndFuncs>(c_layersOfUndoRedo);
            bufferPane = new TabControl();
            this.Controls.Add(bufferPane);
            bufferPane.Location = new Point(-1000, -1000);

            AddDesignerEvents();
            AddLabelPositions();
            InitSendToBack();
            RecurseAddKeyPressEvent(this);
            RecurseAddMouseButtonEvents(this);

            //Set repaste delay to overcome double keypress bug.
            lastKeyActionAt = DateTime.Now;

            //Populate DocReader
            InvalidateRegexList();
            InvalidateActionList();
            RefreshSelectedProperties();

            fromIndex = 0; //Set index to place page in if sent to Workspace.
            statusLabel.Text = ""; //Clear status Label.            

            //start the ticker.
            ticker.Start();

            if (cboFields.Items.Count > 0)
                cboFields.SelectedIndex = 0;

            //this.DoubleBuffered = true;
            this.BringToFront();

        }

        private void Tab_Designer_Load(object sender, EventArgs e)
        {
            AutoGoto();
        }

        private void AutoGoto()
        {
            //string loadText = FormAndFuncs.ReadDocumentFile(txtFilePath.Text);
            //rtbRawDoc.Text = loadText;
            //newTabToolStripMenuItem.PerformClick();

            
            
            //panDragDropToolbox.MouseCaptureChanged += delegate(object sender, EventArgs e)
            //{
            //    Console.WriteLine("Mouse Captured");
            //};

            //ControlMover.Init(lblContents);

        }

        private void InitSendToBack()
        {
            //paneLabel.SendToBack();
            complexPane.SendToBack();
        }

        private void AddLabelPositions()
        {
            string[] names = ControlData.GetLocationNames();
            foreach (string posName in names)
                cboLabelPosition.Items.Add(posName);
        }
       
        #endregion

        #region Menu Items

        #region File

        private void saveTabToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFormFunc();        
        }

        private void saveAsToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            lastOpenFilePath = string.Empty;
            SaveFormFunc();
        }

        private void loadTabToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadFormFunc();            
        }

        private void revertToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lastOpenFilePath == string.Empty)
            {             
                InsertFormFunc(ref backupFF);
                backupFF = new FormAndFuncs(backupFF);
            }
            else
            {
                DialogResult result = DialogResult.No;

                if (changesMade)
                    result = G.YesNoQuestion("Revert to last saved file version?", "Re-load file?");
                else
                    result = DialogResult.Yes;
                
                if (result == DialogResult.Yes)
                    LoadFormFunc(lastOpenFilePath, true);
            }
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion //File

        #region Edit

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Undo();
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Redo();
        }

        private void copyToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            CopySelectedControls();
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CutSelectedControls();
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PasteSuccession();
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormAndFuncs ff = GetCurrentFormFunc();

            if (ff == null)
                return;

            ClearSelections();
            foreach (Control ctrl in ff.Controls)
            {
                if (!IsAutoLabelName(ctrl.Name))
                    AddToSelection(ctrl);
            }

        }

        #endregion

        #region Send to Workspace

        private void sendToWorkspaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormAndFuncs formFunc = GetCurrentFormFunc();

            if (formFunc != null)
            {
                List<string> conflicts = G.GetTagNameConflicts(
                    ref leftPane,
                    ref rightPane,
                    formFunc);

                if (conflicts.Count > 0)
                {
                    string conflictString = G.GetConflictString(
                        ref conflicts);

                    G.ErrorShow(conflictString, "TagName Conflict.");
                    return;
                }

                //Must set scroll to empty before sending.
                //Point oldLoc = formFunc.AutoScrollPosition;
                //oldLoc = new Point(oldLoc.X, oldLoc.Y);
                formFunc.AutoScrollPosition = new Point(0, 0);         

                backupFF = new FormAndFuncs(formFunc);              

                if (fromLeft)
                    leftPane.TabPages.Insert(fromIndex, backupFF);
                else
                    rightPane.TabPages.Insert(fromIndex, backupFF);

                DisassociateFormFunc();
                this.Close();              
            }
            else
            {
                G.MessageShow("Looks like there's no Form & Function to send.", "Ain't no.");
            }

        }

        #endregion

        private void listButtonsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int count = 0;
            foreach (Control ctrl in logicPane.Controls)
            {
                if (ctrl is LogicButton)
                {
                    LogicButton button = (LogicButton)ctrl;
                    Console.WriteLine("Type: " + button.type + "\tText: " + button.Text + " Vis: " + button.Visible);
                    count++;
                }
            }

            Console.WriteLine("Count of buttons = " + count);
        }

        private void newTabToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewFormFunc();
        }

        #endregion //Menu Items
   
        #region Events

        #region On-Close Events

        private void Tab_Designer_FormClosing(object sender, FormClosingEventArgs e)
        {
            FormAndFuncs currentFF = GetCurrentFormFunc();

            if (currentFF != null)
            {
                if (editMode)
                {
                    DialogResult result = DialogResult.Yes;

                    if (changesMade)
                        result = G.YesNoQuestion(
                            "Would you like to keep these changes? Press 'No' to rollback page changes.", 
                            "Keep changes?");
                    else
                        result = DialogResult.No;

                    if (result == DialogResult.Yes)
                    {
                        SendFFToWorkspace(currentFF);
                    }
                    else if (result == DialogResult.No)
                    {
                        if (fromLeft) //Back where it came from.
                            leftPane.TabPages.Insert(fromIndex, backupFF);
                        else
                            rightPane.TabPages.Insert(fromIndex, backupFF);

                        DisassociateFormFunc();
                    }
                    else
                        e.Cancel = true;
                }
                else
                    if (changesMade)
                    {
                        DialogResult result = G.YesNoQuestion("Would you like to save your changes before exiting the page editor?", "Save changes?");

                        if (result == DialogResult.Cancel)
                            e.Cancel = true;
                        else if (result == DialogResult.Yes)
                        {
                            bool didSave = SaveFormFunc();

                            if (!didSave)
                                e.Cancel = true;
                        }
                    }
            }
        }

        //Don't just close, dispose.
        private void Tab_Designer_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Dispose();
        }

        #endregion

        #region Button Clicks

        #region Visual Designer Buttons

        #region Zoom Control

        private void btnZoomIn_Click(object sender, EventArgs e)
        {
            Zoom(true);
        }

        private void btnZoomOut_Click(object sender, EventArgs e)
        {
            Zoom(false);
        }

        #endregion

        #region Control Font and Colors
        private void btnCtrlFont_Click(object sender, EventArgs e)
        {
            FontDialog fontDlg = new FontDialog();
            if (selectedControls.Count == 1)
                fontDlg.Font = selectedControls[0].Font;

            DialogResult result = fontDlg.ShowDialog();

            if (result != DialogResult.Cancel)
                foreach (Control ctrl in selectedControls)
                {
                    if (ctrl is DateTimePicker)
                    {
                        DateTimePicker picker = (DateTimePicker)ctrl;
                        picker.CalendarFont = fontDlg.Font;
                        picker.Font = fontDlg.Font;
                    }
                    else
                        ctrl.Font = fontDlg.Font;
                }

            InvalidateControls(); //Ctrl Font Change
        }

        private void btnCtrlFgCol_Click(object sender, EventArgs e)
        {
            ColorDialog colorDlg = new ColorDialog();
            if (selectedControls.Count == 1)
                colorDlg.Color = selectedControls[0].ForeColor;

            DialogResult result = colorDlg.ShowDialog();

            if (result != DialogResult.Cancel)
                foreach (Control ctrl in selectedControls)
                {
                    if (ctrl is DateTimePicker)
                    {
                        DateTimePicker picker = (DateTimePicker)ctrl;
                        picker.CalendarForeColor = colorDlg.Color;
                    }
                    else
                        ctrl.ForeColor = colorDlg.Color;
                }
        }

        private void btnCtrlBgCol_Click(object sender, EventArgs e)
        {
            ColorDialog colorDlg = new ColorDialog();
            if (selectedControls.Count == 1)
                colorDlg.Color = selectedControls[0].BackColor;

            DialogResult result = colorDlg.ShowDialog();

            if (result != DialogResult.Cancel)
                foreach (Control ctrl in selectedControls)
                {
                    if (ctrl is DateTimePicker)
                    {
                        DateTimePicker picker = (DateTimePicker)ctrl;
                        picker.CalendarTitleBackColor = colorDlg.Color;
                    }
                    else
                        ctrl.BackColor = colorDlg.Color;
                }
        }
        #endregion //Control Font and Colors.

        #region Label Font and Colors

        private void btnLblFont_Click(object sender, EventArgs e)
        {
            FontDialog fontDlg = new FontDialog();
            ControlData prop = null;
            Control selected = null;

            //Set dialog font to that of selected control [0]
            if (selectedControls.Count > 0) //Get that label's font.
            {
                selected = selectedControls[0];
                prop = GetLabelPropFromControl(selected);

                if (prop == null)
                {
                    G.ErrorShow("Control " + selected.Name +
                    " is missing a LabelProp.", "Nerp");
                    return;
                }

                fontDlg.Font = prop.Font.GetFont();
            }
            else
                return; //If no controls selected, quit since
            //     The results would make no difference.

            DialogResult result = fontDlg.ShowDialog();

            if (result != DialogResult.Cancel)
                foreach (Control ctrl in selectedControls)
                {
                    //Apply font to selected label prop,
                    prop = GetLabelPropFromControl(ctrl);
                    prop.Font.SetFontData(fontDlg.Font);
                    labelChanged = true; //Update label change.

                    Control parent = ctrl.Parent;
                    if (parent != null)
                    {
                        Label autoLabel = FindCtrlsAutoLabel(ref parent, ctrl);
                        if (autoLabel != null)
                            autoLabel.Font = prop.Font.GetFont();
                    }
                }


        }

        private void btnLblFgCol_Click(object sender, EventArgs e)
        {
            ColorDialog colorDlg = new ColorDialog();
            ControlData prop = null;
            Control selected = null;

            //Set dialog font to that of selected control [0]
            if (selectedControls.Count > 0) //Get that label's font.
            {
                selected = selectedControls[0];
                prop = GetLabelPropFromControl(selected);

                if (prop == null)
                {
                    G.ErrorShow("Control " + selected.Name +
                    " is missing a LabelProp.", "Nerp");
                    return;
                }

                colorDlg.Color = Color.FromArgb(prop.Font.ForeColor);
            }
            else
                return; //If no controls selected, quit since
            //     The results would make no difference.

            DialogResult result = colorDlg.ShowDialog();

            if (result != DialogResult.Cancel)
                foreach (Control ctrl in selectedControls)
                {
                    //Apply font to selected label prop,
                    prop = GetLabelPropFromControl(ctrl);
                    prop.Font.ForeColor = colorDlg.Color.ToArgb();
                    labelChanged = true; //Update label change.

                    Control parent = ctrl.Parent;
                    if (parent != null)
                    {
                        Label autoLabel = FindCtrlsAutoLabel(ref parent, ctrl);
                        if (autoLabel != null)
                            autoLabel.ForeColor = colorDlg.Color;
                    }
                }
        }

        private void btnLblBgCol_Click(object sender, EventArgs e)
        {
            ColorDialog colorDlg = new ColorDialog();
            ControlData prop = null;
            Control selected = null;

            //Set dialog font to that of selected control [0]
            if (selectedControls.Count > 0) //Get that label's font.
            {
                selected = selectedControls[0];
                prop = GetLabelPropFromControl(selected);

                if (prop == null)
                {
                    G.ErrorShow("Control " + selected.Name +
                    " is missing a LabelProp.", "Nerp");
                    return;
                }

                colorDlg.Color = Color.FromArgb(prop.Font.BackColor);
            }
            else
                return; //If no controls selected, quit since
            //     The results would make no difference.

            DialogResult result = colorDlg.ShowDialog();

            if (result != DialogResult.Cancel)
                foreach (Control ctrl in selectedControls)
                {
                    //Apply font to selected label prop,
                    prop = GetLabelPropFromControl(ctrl);
                    prop.Font.BackColor = colorDlg.Color.ToArgb();
                    labelChanged = true; //Update label change.

                    Control parent = ctrl.Parent;
                    if (parent != null)
                    {
                        Label autoLabel = FindCtrlsAutoLabel(ref parent, ctrl);
                        if (autoLabel != null)
                            autoLabel.BackColor = colorDlg.Color;
                    }
                }
        }

        #endregion //Label Font and Colors

        #region Add Control Buttons

        private void picLabel_MouseDown(object sender, MouseEventArgs e)
        {
            CreateNewCtrl(sender);
        }

        private void picChkBox_MouseDown(object sender, MouseEventArgs e)
        {
            CreateNewCtrl(sender);
        }

        private void picRadBut_MouseDown(object sender, MouseEventArgs e)
        {
            CreateNewCtrl(sender);
        }

        private void picTxtSL_MouseDown(object sender, MouseEventArgs e)
        {
            CreateNewCtrl(sender);
        }

        private void picTxtML_MouseDown(object sender, MouseEventArgs e)
        {
            CreateNewCtrl(sender);
        }

        private void picCboBox_MouseDown(object sender, MouseEventArgs e)
        {
            CreateNewCtrl(sender);
        }

        private void picDtePick_MouseDown(object sender, MouseEventArgs e)
        {
            CreateNewCtrl(sender);
        }

        private void picPanel_MouseDown(object sender, MouseEventArgs e)
        {
            CreateNewCtrl(sender);
        }


        #endregion

        #region Copy To Clipboard

        private void btnCopyTagsVisual_Click(object sender, EventArgs e)
        {
            CopySelectedTagsToClipboard();
        }

        #endregion

        #endregion

        #region Action Buttons

        private void btnDelete_Click(object sender, EventArgs e)
        {
            DeleteCurrentAction();
        }

        #region Copy To Clipboard

        private void btnCopyTagAction_Click(object sender, EventArgs e)
        {
            CopySelectedActionToClipboard();
        }

        #endregion

        #endregion

        #region DocRead Buttons

        private void btnOpenDoc_Click(object sender, EventArgs e)
        {
            OpenSampleDocDialog();
        }

        private void btnNextRegex_Click(object sender, EventArgs e)
        {
            int count = cboFields.Items.Count;
            int currentIndex = cboFields.SelectedIndex;

            if (currentIndex == count - 1)
                cboFields.SelectedIndex = 0;
            else
                cboFields.SelectedIndex++;
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            int count = cboFields.Items.Count;
            int currentIndex = cboFields.SelectedIndex;

            if (currentIndex == 0)
                cboFields.SelectedIndex = count - 1;
            else
                cboFields.SelectedIndex--;
        }

        #region Copy To Clipboard

        private void btnCopyTagDocReader_Click(object sender, EventArgs e)
        {
            CopyDocReaderFieldTagToClipboard();
        }

        #endregion

        #endregion

        #region Test Button

        //Evaluate
        private void btnTest_Click(object sender, EventArgs e)
        {
            EvaluateAction();
        }

        //Test Something.
        private void btnTest_Click_1(object sender, EventArgs e)
        {
            int Count = 0;
            Console.WriteLine("List of Action Tags");
            Console.WriteLine("");
            foreach (string key in actionList.Keys)
            {
                Console.WriteLine("TagName: " + actionList[key].tagName);
                Count++;
            }
            Console.WriteLine("End Action List: Total: " + Count);

            Count = 0;
            foreach (Control ctrl in logicPane.Controls)
            {
                if (ctrl is LogicButton)
                {
                    Count++;
                    LogicButton button = (LogicButton)ctrl;
                    Console.WriteLine("Name: " + ctrl.Text + "type: " + button.type);

                    if (button.Text == "")
                        Console.WriteLine("Hrm.");
                }
                else
                    Console.WriteLine("Name: " + ctrl.Text);
            }

            Console.WriteLine("End Button List: Total: " + Count);
        }

        //List what controls are selected.
        private void btnListSelections_Click(object sender, EventArgs e)
        {
            FormAndFuncs ff = GetCurrentFormFunc();
            int foreignItemCount = 0;
            foreach (Control selected in this.Controls)
            {
                if (selected == layoutPane ||
                    selected == complexPane ||
                    selected == menuStrip ||
                    selected == statusStrip)
                    continue;

                foreignItemCount++;
                Console.WriteLine(selected.Name);
            }

            Console.WriteLine("End Main Contains " + foreignItemCount + " items.");

            foreignItemCount = 0;
            foreach (Control child in ff.Controls)
            {
                foreignItemCount++;
                Console.WriteLine(child.Name);
            }

            Console.WriteLine("End FF Contains " + foreignItemCount + " items.");
        }

        #endregion

 
        #endregion

        #region Mouse Button Clicks

        private void OnMouseButtonDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                mouseLeftIsDown = true;

            if (e.Button == MouseButtons.Right)
            {
                Control legalLanding = GetLegalLanding();

                if (legalLanding != null)
                    pasteLocation = legalLanding.PointToClient(Cursor.Position);

                mouseRightIsDown = true;
            }

            UpdateDebugInfo((Control)sender);
        }

        private void OnMouseButtonUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                mouseLeftIsDown = false;

            if (e.Button == MouseButtons.Right)
                mouseRightIsDown = false;

            UpdateDebugInfo((Control)sender);
        }

        #endregion

        #region Key Presses

        /// <summary>
        /// Checks to see if a property control (such as labelText, Width, etc.) has the focus.
        /// </summary>
        /// <returns></returns>
        //private bool IsIsolatedFocus()
        //{
        //    if (txtLabel.Focused ||
        //        txtComboEdit.Focused ||
        //        txtRegex.Focused ||
        //        txtTagName.Focused ||
        //        numTop.Focused ||
        //        numLeft.Focused ||
        //        numHeight.Focused ||
        //        numWidth.Focused ||
        //        cboLabelPosition.Focused)
        //        return true;
        //    else
        //        return false;

        //}

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
                if (!EditingFieldHasFocus())
                    DeleteSelected();

          //  if (!IsIsolatedFocus())
            bool arrowPressed = RegisterArrowKeyPress(e, true); //True for down.          
            UpdateDebugInfo(null);

            if (ModifierKeys == Keys.Control)
                SlowMoveControls(e);
            else if (arrowPressed)
                StatusMessage("Use Ctrl + Arrow Keys to precisely position page objects.", true, 3000);
            
        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            RegisterArrowKeyPress(e, false); //False for up.     
            RegisterAlphaNumKeys(e);
            UpdateDebugInfo(null);
        }

        private void RegisterAlphaNumKeys(KeyEventArgs e)
        {
            if (ModifierKeys == Keys.Control)
            {

                if (EditingFieldHasFocus()) return;

                int duration = (int)(DateTime.Now - lastKeyActionAt).TotalMilliseconds;

                if (duration > c_repasteDelay)
                {
                    if (e.KeyCode == Keys.Z)
                        Undo();
                    else if (e.KeyCode == Keys.Y)
                        Redo();
                    else if (e.KeyCode == Keys.C)
                        CopySelectedControls();
                    else if (e.KeyCode == Keys.X)
                        CutSelectedControls();
                    else if (e.KeyCode == Keys.V)
                        PasteControls();

                    lastKeyActionAt = DateTime.Now;
                }
                else
                    Console.WriteLine("Would have double pasted.");
            }
        }

      //  private bool IgnoreKeyPress()
        //{
        //    FormAndFuncs ff = GetCurrentFormFunc();
        //    Control hasFocus = ControlHasFocus(ff);

        //    if (hasFocus != null)
        //    {
        //        Console.WriteLine("No. " + hasFocus.Name + " focused.");
        //        return true;
        //    }
        //    else
        //        return false;

        //}

        private bool EditingFieldHasFocus()
        {
            if (txtComboEdit.Focused)
                return true;
            else if (txtLabel.Focused)
                return true;
            else if (txtTagName.Focused)
                return true;
            else if (txtRegex.Focused)
                return true;
            else if (txtFilePath.Focused)
                return true;

            return false;
        }

        private Control LayoutControlHasFocus(Control root)
        {
            Control hasFocus = null;

            if (root.Focused)
                return root;

            else if (root.HasChildren)
            {
                foreach (Control child in root.Controls)
                {
                    hasFocus = LayoutControlHasFocus(child);

                    if (hasFocus != null)
                        return hasFocus;
                }
            }

            return hasFocus;
        }

        private bool TextControlHasFocus(Control root = null)
        {
            if (root == null)
                root = GetCurrentFormFunc();

            Control layoutControl = LayoutControlHasFocus(root);

            if (layoutControl == null)
                return false;

            if (layoutControl is TextBox || 
                layoutControl is ComboBox)
                return true;
            else
                return false; 
        }

        private bool RegisterArrowKeyPress(KeyEventArgs e, bool onKeyDown)
        {
            if (e.KeyCode == Keys.Up)
            {
                keyUpPressed = onKeyDown;
                return true;
            }

            if (e.KeyCode == Keys.Down)
            {
                keyDownPressed = onKeyDown;
                return true;
            }

            if (e.KeyCode == Keys.Left)
            {
                keyLeftPressed = onKeyDown;
                return true;
            }

            if (e.KeyCode == Keys.Right)
            {
                keyRightPressed = onKeyDown;
                return true;
            }

            return false;
        }

        private void txtRegex_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                e.Handled = true;
                btnNextRegex.PerformClick();
            }
        }

        #endregion
    
        #region Context Menu Clicks

        #region Sample File View
        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (rtbRawDoc.SelectedText == string.Empty)
                rtbRawDoc.SelectAll();

            Clipboard.SetText(rtbRawDoc.SelectedText);
        }
        private void openSampleFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenSampleDocDialog();
        }
        #endregion

        #region FormFunc

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewFormFunc();
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFormFunc();
        }

        private void fillFromFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FillFromFile();
        }

        private void clearFieldsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClearFields();
        }

        #region Edit

        private void cutToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            CutSelectedControls();
        }

        private void copyToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            //Copy selected to clipboard
            CopySelectedControls();
        }     

        private void pasteToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            //Instantiate new controls from clipboard onto FF at landing zone loc.
            Control landingZone = GetLegalLanding();
            PasteControls(landingZone, pasteLocation);
        }

        #endregion

        #endregion

        #endregion

        #region FormFunc Control Property Changed

        //update selected 
        private void txtTagName_TextChanged(object sender, EventArgs e)
        {
            if (!newControl)
            {
                if (selectedControls.Count == 1)
                {
                    if (txtTagName.Focused && !tagnameChanged)
                    {
                        tagnameChanged = true;
                        AddUndoPoint();
                    }

                    Control selected = selectedControls[0];
                    SynchronizeTagName(selected, false);
                    UpdateAutoLabelPosition(selected);                   
                }    
            }
        }

        private void txtLabel_TextChanged(object sender, EventArgs e)
        {
                labelChanged = true;
        }

        private void numTop_ValueChanged(object sender, EventArgs e)
        {
            if (numTop.Focused && !numLeft.Focused)
                MoveSelectedToLocation();      
        }

        private void numLeft_ValueChanged(object sender, EventArgs e)
        {
            if (numLeft.Focused && !numTop.Focused)
                MoveSelectedToLocation();  
        }

        private void numHeight_ValueChanged(object sender, EventArgs e)
        {
            if (numHeight.Focused)
                SyncNumHeightWidthOnSelected();
        }

        private void numWidth_ValueChanged(object sender, EventArgs e)
        {
            if (numWidth.Focused)
                SyncNumHeightWidthOnSelected();
        }

        private void cboLabelDisplay_SelectedIndexChanged(object sender, EventArgs e)
        {
            labelChanged = true;
        }

        private void txtComboEdit_TextChanged(object sender, EventArgs e)
        {
            comboItemsChanged = true;
        }

        #endregion  

        #region Visual Editing Events.

        #region Drag Enter

        private void Tab_Designer_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        private void layoutPane_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        #endregion

        #region Form Func Specific Events

        private void FormFunc_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        private void FormFunc_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] fileNames = (string[])e.Data.GetData(DataFormats.FileDrop);
                string fileName = fileNames[0];
                Console.WriteLine(fileName);

                Control page = layoutPane.TabPages[0];
                string textToRead = FormAndFuncs.ReadDocumentFile(fileName);
                FormAndFuncs.FillTabPage(
                    ref textToRead, page);

                actionChanged = true;
            }

        }

        private void FormFunc_Scroll(object sender, ScrollEventArgs e)
        {
            if (sender == null)
                return;

            FormAndFuncs ff = (FormAndFuncs)sender;

            if (selectedControls.Contains(ff))
            {
                DeselectControl(ff);
                Console.WriteLine("Removing FF from selection.");
            }

            ff.Refresh();
        }

        private void Page_MouseWheel(object sender, MouseEventArgs e)
        {
            FormAndFuncs ff = GetCurrentFormFunc();
            if (selectedControls.Contains(ff))
            {
                DeselectControl(ff);
                ff.Invalidate();
            }

            if (ff == null) 
                return;

            //Console.WriteLine(e.Delta);
            //Console.WriteLine(ff.VerticalScroll.Value);

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

        #endregion
       
        #region Control/DragSpace Events

        private void picker_DropDown(object sender, EventArgs e)
        {
            calandarDropped = true;
        }

        private void ComboBox_DropDown(object sender, EventArgs e)
        {
            //G.AutoAdjustComboBoxDropDownSize(ref sender);
           // Console.WriteLine("Adjusting size.");
        }

        private void picker_CloseUp(object sender, EventArgs e)
        {
            calandarDropped = false;
        }

        private void OverControl_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Control ctrl = (Control)sender;
                UpdateMouseCursor(ctrl); //For resizing.
                ToggleSelect(ctrl);            
            }

            if (e.Button == MouseButtons.Right)
            {
                
            }
        }

        private void OverControl_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (!draggingControls && !resizing)
                    StopRubberbanding(sender, e);

                TryControlDrop();
                TryStopResize();
                             
                SelectRegexFieldName();
            }

            if (e.Button == MouseButtons.Right)
            {
                
            }
        }

        private void DragSpace_MouseMove(object sender, MouseEventArgs e)
        {
            Control ctrlSender = (Control)sender;         
            UpdateDebugInfo(ctrlSender);
            

            if (resizing)
            {
                MouseDragResizeSelectedControls();
                UpdateSizeGUI(ctrlSender);
                InvalidateControls(); // Mouse Move Resizing.
            }
            else if (draggingControls)
            {
                DragSelectedControls();
                UpdatePosGUI(ctrlSender);
                InvalidateControls(); // Mouse Move Dragging
            }
            else if (e.Button == MouseButtons.Left)
            {
                if (!skipOnce)
                {
                    if (mouseDown)
                    {
                        if (mouseMoving)
                            RubberbandRectangle(ptMouseDownAt, ptOldMouse);

                        if (rbLanding == null) rbLanding = GetCurrentFormFunc();
                        if (rbLanding == null) return;

                        Point mouseLoc = rbLanding.PointToClient(Cursor.Position);

                        RubberbandRectangle(ptMouseDownAt, mouseLoc);
                        mouseMoving = true;
                        ptOldMouse = mouseLoc;
                    }
                }
                else
                    skipOnce = false;
            }
            else
                UpdateMouseCursor(ctrlSender);
        }

        private void Control_GotFocus(object sender, EventArgs e)
        {
            Control ctrl = (Control)sender;

            if (ctrl != null)
            {
                if (ctrl is FormAndFuncs) return;


                if (!selectedControls.Contains(ctrl))
                {
                    if (selectedControls.Count == 1)
                    {
                       // ToggleSelect(ctrl, true);
                    }
                }
            }
        }

        #endregion
      
        #region Misc

        private void PreventFocus(object sender, EventArgs e)
        {
            Control ctrl = (Control)sender;
            ctrl.Enabled = false;
            ctrl.Enabled = true;
        }


        #endregion

        #endregion

        #region Doc Reader Fields Changed

        private void txtRegex_TextChanged(object sender, EventArgs e)
        {        
            regexChanged = true;
        }

        private void cboFields_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((string)cboFields.SelectedItem != string.Empty)
            {
                //Load selected regex
                string fieldName = (string)cboFields.SelectedItem;
                DisplayFlatRegex(fieldName);

                //Force evaluation of regex.
                regexChanged = false;
                PerformRegexCapture();
                UpdateFlatRegexOnControl();
            }

        }

        private void radNone_CheckedChanged(object sender, EventArgs e)
        {
            UpdateFlatRegexOnControl();
        }

        private void radSingleLine_CheckedChanged(object sender, EventArgs e)
        {
            UpdateFlatRegexOnControl();
        }

        #region Visual Editing Hover Picture Swapping

        //Label
        private void pctLabel_MouseEnter(object sender, EventArgs e)
        {
            G.SwapImg(ref picLabel);
        }
        private void pctLabel_MouseLeave(object sender, EventArgs e)
        {
            G.SwapImg(ref picLabel);
        }

        //Check Box
        private void picChkBox_MouseEnter(object sender, EventArgs e)
        {
            G.SwapImg(ref picChkBox);
        }
        private void picChkBox_MouseLeave(object sender, EventArgs e)
        {
            G.SwapImg(ref picChkBox);
        }

        //Radio Button
        private void picRadBut_MouseEnter(object sender, EventArgs e)
        {
            G.SwapImg(ref picRadBut);
        }
        private void picRadBut_MouseLeave(object sender, EventArgs e)
        {
            G.SwapImg(ref picRadBut);
        }

        //Single Line Text Box
        private void picTxtSL_MouseEnter(object sender, EventArgs e)
        {
            G.SwapImg(ref picTxtSL);
        }
        private void picTxtSL_MouseLeave(object sender, EventArgs e)
        {
            G.SwapImg(ref picTxtSL);
        }

        //Mulit line Textbox
        private void picTxtML_MouseEnter(object sender, EventArgs e)
        {
            G.SwapImg(ref picTxtML);
        }
        private void picTxtML_MouseLeave(object sender, EventArgs e)
        {
            G.SwapImg(ref picTxtML);
        }

        //Combo Box
        private void picCboBox_MouseEnter(object sender, EventArgs e)
        {
            G.SwapImg(ref picCboBox);
        }
        private void picCboBox_MouseLeave(object sender, EventArgs e)
        {
            G.SwapImg(ref picCboBox);
        }

        //Date Picker
        private void picDtePick_MouseEnter(object sender, EventArgs e)
        {
            G.SwapImg(ref picDtePick);
        }
        private void picDtePick_MouseLeave(object sender, EventArgs e)
        {
            G.SwapImg(ref picDtePick);
        }

        //Panel
        private void picPanel_MouseEnter(object sender, EventArgs e)
        {
            G.SwapImg(ref picPanel);
        }
        private void picPanel_MouseLeave(object sender, EventArgs e)
        {
            G.SwapImg(ref picPanel);
        }

        #endregion //control hiding.

        #endregion //Misc

        #region Ticker

        private void ticker_Tick(object sender, EventArgs e)
        {
            //Update Regex eval.
            if (regexChanged)
            {
                if (txtRegex.Text != string.Empty)
                {
                    UpdateFlatRegexOnControl();
                    PerformRegexCapture();            
                }
                else
                    RemoveAllHighlighting();

                regexChanged = false;
            }

            //Update Action Eval.
            if (actionChanged)
            {
                actionChanged = false;
            }

            //Update to propogate changes in Label Font, Text
            if (labelChanged)
            {
                if (ignoreLabelChange)
                    ignoreLabelChange = false;
                else
                {
                    //Handle font and colors immediately after dialog windows.
                    UpdateAutoLabelTextAndPos();
                 }

                labelChanged = false;
            }

            //If tagname changed and txtTagName lost focus,
            // reset tagnamechanged flag.
            if (tagnameChanged && !txtTagName.Focused)
            {
                tagnameChanged = false;
            }

            if (comboItemsChanged)
            {
                UpdateComboControl();
                comboItemsChanged = false;
            }

            if (statusMessageOn)
            {
                if (statusMessageCountdown > 0)
                    statusMessageCountdown -= ticker.Interval;
                else
                { //time expired.
                    StatusMessage("", false, 0);                  
                }      
            }

            if (complexPane.SelectedTab == pageActions)
            {
                EvaluateAction();
            }            
        }

        #endregion

        #region Paint Events

        //Paint on the Outside
        private void PaintSelected_Paint(object sender, PaintEventArgs e)
        {
            if (sender != null)
            {
                Control ff = GetCurrentFormFunc();

                if (ff != null)
                    PaintSelectedChildren(ff);                
            }
        }

        private void PaintSelectedChildren(Control parent)
        {
            if (parent != null)
            {
               //Console.WriteLine("Painting FF: " + parent.Name);

                int offset = 0;
                float[] blackDash = { 1, 1 };
                //float[] whiteDash = { 4, 4 };
                Pen blackPen = new Pen(Color.Black, 1);
                Pen whitePen = new Pen(Color.White, 1);
                blackPen.DashPattern = blackDash;
                //whitePen.DashPattern = whiteDash;
                Graphics g = parent.CreateGraphics();

                if (!draggingControls && !resizing)
                {
                    //Console.WriteLine(parent.Name + " " + parent.Controls.Count + " controls.");
                    foreach (Control child in parent.Controls)
                    {
                        if (IsCtrlSelected(child))
                        {
                            //Console.WriteLine("Painted around: " + child.Name);

                            if (child is TextBox)
                                offset = 7;
                            else if (child is Panel)
                                offset = 5;
                            else if (child is DateTimePicker)
                                offset = 7;
                            else if (child is ComboBox)
                                offset = 2;
                            else
                                offset = 3;

                            Rectangle r = child.DisplayRectangle;
                            Point p = new Point(child.Location.X - 1, child.Location.Y - 1);
                            g.DrawRectangle(
                                whitePen,
                                p.X - 1,
                                p.Y - 1,
                                r.Width + offset,
                                r.Height + offset);
                            g.DrawRectangle(
                                blackPen,
                                p.X - 1,
                                p.Y - 1,
                                r.Width + offset,
                                r.Height + offset);
                        }

                        if (child.HasChildren)
                            PaintSelectedChildren(child);

                    }

                    g.Dispose();
                }
            }
        }

        //Paint on the inside.
        private void SelectInside_Paint(object sender, PaintEventArgs e)
        {
            if (sender != null)
            {
                FormAndFuncs page = (FormAndFuncs)sender;
                float[] blackDash = { 1, 1 };
                Pen blackPen = new Pen(Color.Black, 1);
                Pen whitePen = new Pen(Color.White, 1);
                blackPen.DashPattern = blackDash;

               // Console.WriteLine("Painting inside: " + page.Name);

                if (page != null)
                {
                    if (selectedControls.Contains(page))
                    {
                        Graphics g = page.CreateGraphics();
                        Rectangle r = new Rectangle(1, 1,
                            page.ClientRectangle.Width - 2,
                            page.ClientRectangle.Height - 2);
                        g.DrawRectangle(whitePen, r);
                        g.DrawRectangle(blackPen, r);

                        //Console.WriteLine("Paint Inside: " + page.Name);
                    }
                }
            }
        }

        #endregion Paint Events

        #endregion //Events

        #region Add/Remove Events

        #region FormFuncs

        private void AddFFEvents(FormAndFuncs formFunc)
        {
            formFunc.ContextMenuStrip = ctxFormFunc;
            formFunc.DragDrop += new DragEventHandler(FormFunc_DragDrop);
            formFunc.DragEnter += new DragEventHandler(FormFunc_DragEnter);
            formFunc.Scroll +=new ScrollEventHandler(FormFunc_Scroll);
            formFunc.Paint += new PaintEventHandler(SelectInside_Paint);
            formFunc.Paint += new PaintEventHandler(PaintSelected_Paint);
            //RecurseAddKeyPressEvent(formFunc);
            RecurseAddMouseButtonEvents(formFunc);
            RecurseAddControlEvents(formFunc);
        }

        private void RemoveFFEvents(FormAndFuncs formFunc)
        {
            formFunc.DragDrop -= FormFunc_DragDrop;
            formFunc.DragEnter -= FormFunc_DragEnter;
            formFunc.MouseUp -= OverControl_MouseUp;
            formFunc.MouseDown -= OverControl_MouseDown;
            formFunc.MouseMove -= DragSpace_MouseMove;
            formFunc.Paint -= SelectInside_Paint;
            formFunc.Paint -= PaintSelected_Paint;
            RecurseRemoveKeyPressEvent(formFunc);
            RecurseRemoveMouseButtonEvents(formFunc);
            RecurseRemoveControlEvents(formFunc);
        }

        private void RecurseAddKeyPressEvent(Control parent)
        {
            parent.KeyUp += new KeyEventHandler(OnKeyUp);
            parent.KeyDown += new KeyEventHandler(OnKeyDown);

            foreach (Control child in parent.Controls)
                RecurseAddKeyPressEvent(child);
        }

        private void RecurseAddMouseButtonEvents(Control parent)
        {
            if (!FormAndFuncs.IsAutoLabelName(parent.Text))
            {
                parent.MouseUp += new MouseEventHandler(OnMouseButtonUp);
                parent.MouseDown += new MouseEventHandler(OnMouseButtonDown);

                foreach (Control child in parent.Controls)
                    RecurseAddMouseButtonEvents(child);
            }
        }

        private void RecurseRemoveKeyPressEvent(Control parent)
        {
            parent.KeyUp -= OnKeyUp;
            parent.KeyDown -= OnKeyDown;

            foreach (Control child in parent.Controls)
                RecurseRemoveKeyPressEvent(child);
        }

        private void RecurseRemoveMouseButtonEvents(Control parent)
        {
            parent.MouseUp -= OnMouseButtonUp;
            parent.MouseDown -= OnMouseButtonDown;

            foreach (Control child in parent.Controls)
                RecurseRemoveMouseButtonEvents(child);
        }

        private void RecurseAddControlEvents(Control parent)
        {

            if (!FormAndFuncs.IsAutoLabelName(parent.Name))
            {
                foreach (Control child in parent.Controls)
                    RecurseAddControlEvents(child);

                // Console.WriteLine("Added Control Events for " + parent.Name);
                AddControlEvents(parent);
            }
        }

        private void RecurseRemoveControlEvents(Control parent)
        {
            foreach (Control child in parent.Controls)
                    RecurseRemoveControlEvents(child);

            RemoveControlEvents(parent);
        }


        //On Form Load events for designer form.
        private void AddDesignerEvents()
        {
            AddDragSpaceRecurse(this);
            this.MouseWheel +=new MouseEventHandler(Page_MouseWheel);
        }

        private void AddDragSpaceRecurse(Control parent)
        {
            parent.MouseMove += new MouseEventHandler(DragSpace_MouseMove);

            if (parent.HasChildren)
                foreach (Control child in parent.Controls)
                    AddDragSpaceRecurse(child);
        }

        private void AddControlEvents(Control ctrl)
        {       
            ctrl.MouseMove += new MouseEventHandler(DragSpace_MouseMove);
            ctrl.MouseDown += new MouseEventHandler(OverControl_MouseDown);
            ctrl.MouseDown += new MouseEventHandler(OnMouseButtonDown);
            ctrl.MouseUp += new MouseEventHandler(OnMouseButtonUp);
            ctrl.MouseUp += new MouseEventHandler(OverControl_MouseUp);
            ctrl.GotFocus +=new EventHandler(Control_GotFocus);

            if (ctrl is Panel)
            {
                ctrl.Paint += new PaintEventHandler(PaintSelected_Paint);
            }

            if (ctrl is ComboBox)
            {
                //ComboBox box = (ComboBox)ctrl;
                //box.DropDown += new EventHandler(ComboBox_DropDown);
            }

            if (ctrl is DateTimePicker)
            {
                DateTimePicker picker = (DateTimePicker)ctrl;
                picker.DropDown += new EventHandler(picker_DropDown);
                picker.CloseUp += new EventHandler(picker_CloseUp);
            }
        }

        private void RemoveControlEvents(Control ctrl)
        {
            
            ctrl.MouseMove -= DragSpace_MouseMove;
            ctrl.MouseDown -= OverControl_MouseDown;
            ctrl.MouseDown -= OnMouseButtonDown;
            ctrl.MouseUp -= OverControl_MouseUp;
            ctrl.MouseUp -= OnMouseButtonUp;

            if (ctrl is Panel)
            {
                ctrl.Paint -= PaintSelected_Paint;
            }

            if (ctrl is ComboBox)
            {
                //ComboBox box = (ComboBox)ctrl;
                //box.DropDown -= ComboBox_DropDown;
            }

            if (ctrl is DateTimePicker)
            {
                DateTimePicker picker = (DateTimePicker)ctrl;
                picker.DropDown -= picker_DropDown;
                picker.CloseUp -= picker_CloseUp;
            }

            //ctrl.KeyDown -= OnKeyDown;
        }

        #endregion

        #region Form Level events.

        //private void AddFormEvents()
        //{
        //    this.MouseWheel += new MouseEventHandler(Form_MouseWheel);
        //}

        #endregion

        #endregion

        #region Visual/FormFunc Section

        #region Save/Load/New FormFunc


        //Return true if saved.
        private bool SaveFormFunc()
        {
            string errCode = G.c_NoErr;
            bool didSave = false;
            string savePath = string.Empty;

            if (lastOpenFilePath != string.Empty &&
                lastOpenFilePath != "null")
                savePath = lastOpenFilePath;
            else
                savePath = G.SelectFNFSavePath(GetCurrentFormFunc());

            if (savePath != string.Empty)
            {
                FormAndFuncs formFunc = GetCurrentFormFunc();

                //formFunc.AutoScrollPosition = Point.Empty; //To save properly if the page was scrolled.
                errCode = formFunc.Serialize(
                    savePath,
                    (string)cboActionList.SelectedItem,
                    (string)cboFields.SelectedItem);

                changesMade = false; //False, All changes saved.
            }

            if (errCode != G.c_NoErr)
                G.ErrorShow(errCode, "Couldn't save FormFunc");
            else
            {
                StatusMessage("Tab Saved!", true, 3000);
                didSave = true;
            }

            return didSave;
        }

        private void LoadFormFunc(string filePath = "", bool bypassSaveQuery = false)
        {
            FormAndFuncs currentFF = GetCurrentFormFunc();

            if (!bypassSaveQuery)
                if (currentFF != null)
                {
                    DialogResult result = DialogResult.None;

                    if (changesMade)
                        result = G.YesNoQuestion("All unsaved changes will be lost. Save " +
                        currentFF.Text + " before loading?", "Save first?");
                    else
                        result = DialogResult.No;
                       

                    if (result == DialogResult.Yes)
                        SaveFormFunc();
                }

            if (filePath == "")
                filePath = G.SelectFNFLoadPath();

            if (filePath != string.Empty)
            {
                FormAndFuncs openFormFuncs = new FormAndFuncs(filePath);

                if (openFormFuncs != null)
                {
                    LoadFFForEditing(ref openFormFuncs);
                    lastOpenFilePath = filePath;
                }
            }

            return;
        }

        private void NewFormFunc()
        {
            FormAndFuncs formFunc = new FormAndFuncs();
            formFunc.Name = GetNewControlName(formFunc);
            InsertFormFunc(ref formFunc);
            lastOpenFilePath = string.Empty;
            Console.WriteLine("Adding " + formFunc.Name);
        }

        #endregion //Save/Load/New

        #region Insert/Remove/Bind FormFunc

        private void InsertFormFunc(ref FormAndFuncs newFormFunc, bool ignoreConfirmationDialog = false)
        {
            DialogResult result;
            FormAndFuncs currentFF = GetCurrentFormFunc();
           
            if (currentFF != null)
            {               
                if (ignoreConfirmationDialog)
                    result = DialogResult.No;
                else
                    result = G.YesNoQuestion("Would you like to save '" + currentFF.Text +
                        "' before continuing?", "Save or poof?");

                if (result == DialogResult.Yes)
                {
                    bool didSave = SaveFormFunc();
                    didSave = SaveFormFunc();

                    if (!didSave)
                        return;
                }
                    
                else if (result == DialogResult.Cancel)
                    return;
            }

            //Add Events, bind Context Menu.
            LoadFFForEditing(ref newFormFunc);
        }

        private void LoadFFForEditing(ref FormAndFuncs formFunc)
        {
            FormAndFuncs currentFF = GetCurrentFormFunc();

            if (currentFF != null)
                if (editMode)
                    DisassociateFormFunc();
                else
                    RemoveFormFunc(currentFF);

            AddFFEvents(formFunc);
            formFunc.AutoScrollMargin = new Size(0, 650);

            layoutPane.TabPages.Add(formFunc);
            Console.WriteLine("Added TabPage containting " + formFunc.Controls.Count);
            this.actionList = formFunc.actions;

            InvalidateRegexList();
            InvalidateActionList();
            InvalidateControls();

            if (formFunc.lastActionTagSelected != null)
                if (cboActionList.Items.Contains(formFunc.lastActionTagSelected))
                {
                    cboActionList.SelectedItem = formFunc.lastActionTagSelected;
                    cboActionList_SelectionChangeCommitted(null, null);
                }
                else
                    if (cboActionList.Items.Count > 0)
                    {
                        cboActionList.SelectedIndex = 0;
                        cboActionList_SelectionChangeCommitted(null, null);
                    }

            if (formFunc.lastRegexTagSelected != null)
                if (cboFields.Items.Contains(formFunc.lastRegexTagSelected))
                {
                    cboFields.SelectedItem = formFunc.lastRegexTagSelected;
                    cboFields.Invalidate();
                }
                else
                    if (cboFields.Items.Count > 0)
                        cboFields.SelectedIndex = 0;
        }

        private void RemoveFormFunc(FormAndFuncs ffToRemove)
        {
            if (ffToRemove != null)
            {
                RemoveFFEvents(ffToRemove);
                layoutPane.Controls.Remove(ffToRemove);
                ffToRemove.Dispose();
            }

            ClearEverything();
        }

        #endregion //AddRemoveFormFunc

        #region Clear

        private void ClearEverything()
        {
            #region Clear Actions
            layoutPane.Controls.Clear();

            //Clear Logic Window.
            Action thisAction = GetCurrentAction();
            if (thisAction != null)
                thisAction.ClearLogicPane(ref logicPane, false);
            logicPane.Controls.Clear();

            if (actionList != null)
                actionList.Clear();

            cboActionList.Items.Clear();
            actionChanged = true;

            #endregion

            #region Clear Regex

            if (regexList != null)
                regexList.Clear();

            txtRegex.Text = string.Empty;
            cboFields.Items.Clear();
            regexChanged = true;

            #endregion

            #region Clear Visual Editing Properties

            ClearSelections();
            RefreshSelectedProperties();

            #endregion
        }

        private void ClearFields()
        {
            if (layoutPane.TabPages.Count > 0)
            {
                TabPage clearPage = layoutPane.TabPages[0];
                FormAndFuncs.ClearFields(clearPage);
                actionChanged = true;
            }
        }

        private void DisassociateFormFunc()
        {
            FormAndFuncs currentFF = GetCurrentFormFunc();

            if (currentFF != null)
            {
                RemoveFFEvents(currentFF);
                layoutPane.Controls.Clear();
            }

            ClearEverything();
            editMode = false;
        }

        #endregion

        #region Control Editing Functions

        #region Mouse Finding
        
        public bool IsMouseOverLandingZone()
        {
            Control dragSurface = GetDragSurface();
            if (IsContainedInFF(dragSurface))
                return true;
            else
                return false;

        }

        //public Control GetLandingZoneControl()
        //{
        //    Control dragSurface = GetDragSurface();

        //    if
        //}

        public bool IsContainedInFF(Control ctrl)
        {
            if (ctrl is FormAndFuncs)
                return true;
            else if (ctrl.Parent != null)
                return IsContainedInFF(ctrl.Parent);
            else
                return false;
        }

        public bool ControlContainsMouse(Control ctrl)
        {
            if (ctrl.ClientRectangle.Contains(ctrl.PointToClient(Cursor.Position)))
                return true;
            else
                return false;
        }

        public Control RecurseControlUnderMouse(Control parent)
        {
            if (ControlContainsMouse(parent))
            {
                if (parent.HasChildren)
                {
                    foreach (Control child in parent.Controls)
                    {
                        if (ControlContainsMouse(child))
                            return RecurseControlUnderMouse(child);
                    }
                }
                return parent;
            }

            return null;
        }

        private Control GetControlUnderMouse()
        {
            Control ff = GetCurrentFormFunc();
            Control underMouse = null;

            if (ff != null) 
            {
                underMouse = RecurseControlUnderMouse(ff);
                if (underMouse != null) 
                    return underMouse;
            }

            underMouse = RecurseControlUnderMouse(this);
                return underMouse;
        }

        private Cursor GetResizeIcon(Control ctrl)
        {
            if (ctrl == null)
                return Cursors.Default;

            Point overCtrlLoc = ctrl.PointToClient(Cursor.Position);

            int margin = 9;
            bool bottomContains = false;
            bool rightContains = false;

            Rectangle bottom = new Rectangle(
                0,
                ctrl.Height - margin,
                ctrl.Width,
                margin);

            Rectangle right = new Rectangle(
                ctrl.Width - margin,
                0,
                margin,
                ctrl.Height);

            bottomContains = bottom.Contains(overCtrlLoc);
            rightContains = right.Contains(overCtrlLoc);

            //Deactivate the vertical cursor for a single lined textbox.
            if (bottomContains)
                if (ctrl is TextBox)
                {
                    TextBox box = (TextBox)ctrl;
                    if (!box.Multiline)
                        if (rightContains)
                            return Cursors.SizeWE;
                        else
                            return Cursors.IBeam;
                }          

            if (bottomContains && rightContains)
                return Cursors.SizeNWSE;
            else if (bottomContains)
                return Cursors.SizeNS;
            else if (rightContains)
                return Cursors.SizeWE;
            else
                return Cursors.Default;      
        }

        /// <summary>
        /// Recursively search this Control for the highest TabPage, Panel, or Form containing the mouse.
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        private Control GetDragSurface(Control parent)
        {
            if (parent == null)
                return null;

            if (parent.HasChildren)
            {
                foreach (Control child in parent.Controls)
                {
                    if (child is TabPage ||
                        child is Panel ||
                        child is Form)
                        if (ControlContainsMouse(child))
                            return GetDragSurface(child);
                }
            }

            if (ControlContainsMouse(parent))
                return parent;
            else
                return null;
        }

        private Control GetDragSurface()
        {

            Control ff = GetCurrentFormFunc();
            Control dragSurface = null;

            //Is the mouse over a control of the pageDesign page?
            //dragSurface = GetDragSurface(pageDesign);
            //if (dragSurface != null)
            //    return dragSurface;

            //Is the mouse over a the formFunc?
            dragSurface = GetDragSurface(ff);
            if (dragSurface != null)
                return dragSurface;
            else
                return this;

            //Return the control 
            //dragSurface = GetDragSurface(this);
            //    return dragSurface;       
        }

        /// <summary>
        /// Returns a DragSurface which is a child of the FormFunc
        /// </summary>
        /// <returns></returns>
        private Control GetLegalLanding()
        {
            Control ff = GetCurrentFormFunc();
            Control legalLanding = GetDragSurface(ff);
            if (legalLanding != null)
                return legalLanding;
            else
                return null;

        }

        #endregion

        #region New Control

        /// <summary>
        /// Call on every begin drag/create new control.
        /// </summary>
        private void CreateNewCtrl(object sender)
        {

            if (!IsFormFuncsOpen())
                return;

            if (sender == null)
                return;

            if (newControl)
                CancelNewCtrl();

            Control ctrl = null;
            ControlData label = new ControlData();

            #region Label
            if (sender == picLabel)
            {
                ctrl = new Label();
                ctrl.Text = "Label";
                ctrl.Size = new Size(38, 15);
                ((Label)ctrl).AutoSize = true;
                label.LabelPos = LabelPosition.None;


            }

            #endregion

            #region Check Box

            if (sender == picChkBox)
            {
                ctrl = new CheckBox();
                CheckBox chkBox = (CheckBox)ctrl;
                ctrl.Text = "Check Box";
                chkBox.AutoSize = true;
                chkBox.Checked = false;
                label.LabelPos = LabelPosition.Right;
            }
            #endregion

            #region Radio Button
            if (sender == picRadBut)
            {
                ctrl = new RadioButton();
                RadioButton radButt = (RadioButton)ctrl;
                ctrl.Text = "Radio Button";
                radButt.AutoSize = true;
                radButt.Checked = false;
                label.LabelPos = LabelPosition.Right;
            }
            #endregion

            #region Single Line Text Box
            if (sender == picTxtSL)
            {
                TextBox box = new TextBox();
                box.Multiline = false;
                ctrl = box;
            }
            #endregion

            #region Multi Line Text Box
            if (sender == picTxtML)
            {
                TextBox box = new TextBox();
                box.Multiline = true;
                ctrl = box;
                ctrl.Height = 50;
                ctrl.Width = 110;
            }
            #endregion

            #region Combo Box
            if (sender == picCboBox)
            {
                ctrl = new ComboBox();
               // ComboBox box = (ComboBox)ctrl;
                
            }
            #endregion

            #region Date Picker
            if (sender == picDtePick)
            {
                DateTimePicker picker = new DateTimePicker();
                DateTime defaultDate = new DateTime(DateTime.Today.Year, 1, 1);
                picker.Value = defaultDate;
                picker.Format = DateTimePickerFormat.Short;
                picker.Width = 115;
                ctrl = picker;
            }
            #endregion

            #region Panel
            if (sender == picPanel)
            {
                ctrl = new Panel();
                ((Panel)ctrl).BorderStyle = BorderStyle.FixedSingle;
                ctrl.BackColor = Color.LightGray;
            }
            #endregion
           
            if (ctrl != null)
            {
                ctrl.Location = this.PointToClient(Cursor.Position);
                newControl = true;
                label.LabelText = ctrl.Text;
                ctrl.Name = GetNewControlName(ctrl);
                ctrl.Tag = label;
                AddControlEvents(ctrl);              
                ToggleSelect(ctrl);               
                TryDragStart();
            }
            else
                Console.WriteLine("Failed to add Control");
        }

        //private void RecurseNormalizeNames(Control root, FormAndFuncs currentFF)
        //{
        //    if (root == null ||
        //        currentFF == null)
        //    {
        //        G.ErrorShow("Unable to normalize tag names. Duplicates names may have occured.", "That's strange.");
        //        return;
        //    }

        //    List<string> takenNames = new List<string>();
        //    RecurseFindTakenNames(currentFF, ref takenNames);

        //    foreach (Control ctrl in root.Controls)
        //    {
        //        if (takenNames.Contains(ctrl.Name))
        //        {
        //            ctrl.Name = GetNewControlName(ctrl);
        //            takenNames.Add(ctrl.Name);
        //        }

        //        if (ctrl is Panel)
        //            RecurseNormalizeNames(ctrl, currentFF);
        //    }

        //}

        private string GetNewControlName(Control ctrl)
        {
            FormAndFuncs thisFormFunc = GetCurrentFormFunc();
            if (thisFormFunc == null)
                thisFormFunc = new FormAndFuncs();

            List<string> takenNames = new List<string>();
            string ctrlType = string.Empty;
            char[] numArr = {'0','1','2','3','4','5','6','7','8','9'};

            if (ctrl is Label)
                ctrlType = ControlType.label.ToString();
            else if (ctrl is CheckBox)
                ctrlType = ControlType.checkBox.ToString();
            else if (ctrl is RadioButton)
                ctrlType = ControlType.radioButton.ToString();
            else if (ctrl is TextBox)
            {
                if (((TextBox)ctrl).Multiline)
                    ctrlType = ControlType.textBoxML.ToString();
                else
                    ctrlType = ControlType.textBoxSL.ToString();
            }
            else if (ctrl is ComboBox)
                ctrlType = ControlType.comboBox.ToString();
            else if (ctrl is DateTimePicker)
                ctrlType = ControlType.datePicker.ToString();
            else if (ctrl is TabPage)
                ctrlType = ControlType.tabPage.ToString();
            else if (ctrl is Panel)
                ctrlType = ControlType.panel.ToString();

            string trimmedCtrlName = string.Empty;
            string newControlName = string.Empty;

            if (ctrl.Name == string.Empty)
                trimmedCtrlName = ctrlType;
            else
                trimmedCtrlName = ctrl.Name.TrimEnd(numArr);

            //RecurseFindTakenNames(leftPane, ref takenNames);
            //RecurseFindTakenNames(rightPane, ref takenNames);
            RecurseFindTakenNames(thisFormFunc, ref takenNames);

            if (!takenNames.Contains(ctrl.Name) && ctrl.Name != string.Empty)
                newControlName = ctrl.Name;
            // else if (!takenNames.Contains(ctrlType))
            // newControlName = ctrlType; //Can we just use this name?
            else
                for (int i = 1; i < 9000; i++)
                    if (!takenNames.Contains(trimmedCtrlName + i)) //Cycle up through the numbers.
                    {
                        newControlName = trimmedCtrlName + i;
                        break;
                    }
            //Console.WriteLine(newControlName);
            return newControlName;
        }

        private bool IsTagNameTaken(string newName, ref List<string> takenNames)
        {
            if (takenNames.Contains(newName))
                return true;
            else 
                return false;
        }

        private void RecurseFindTakenNames(Control rootControl, ref List<string> takenNames)
        {
            if (!(rootControl is TabControl))
                takenNames.Add(rootControl.Name);

            if (rootControl.HasChildren)
                foreach (Control child in rootControl.Controls)
                        RecurseFindTakenNames(child, ref takenNames);

        }

        private void CancelNewCtrl()
        {
            //Remove Events
            foreach (Control ctrl in selectedControls)
            {
                RemoveControlEvents(ctrl);
                ctrl.Parent.Controls.Remove(ctrl);
                ctrl.Dispose();
            }

            ClearSelections();
            newControl = false;
            draggingControls = false;           
        }

        #endregion

        #region Delete Control

        private void DeleteControl(Control ctrl)
        {
            if (ctrl == null)
                return;

            Control parent = ctrl.Parent;
            RemoveAutoLabel(ref parent, ctrl);

            if (IsCtrlSelected(ctrl))
                RemoveFromSelection(ctrl);

            RemoveControlEvents(ctrl);

            ctrl.Parent.Controls.Remove(ctrl);
            ctrl.Dispose();
        }

        private void DeleteSelected()
        {
            if (EditingFieldHasFocus())
                return;

            bool delete = true;

            AddUndoPoint();           

            while (delete && selectedControls.Count > 0)
            {
                if (selectedControls[0] is FormAndFuncs)
                {
                    if (selectedControls.Count > 1)
                        DeleteControl(selectedControls[1]);
                    else
                        delete = false;
                }
                else
                {
                    if (selectedControls.Count > 0)
                        DeleteControl(selectedControls[0]);
                    else
                        delete = false;
                }
            }

            newControl = false;
            draggingControls = false;
            RefreshSelectedProperties();
            InvalidateControls(); //Delete Selected.
        }

        #endregion

        #region Copy / Paste Controls

        private void CopySelectedControls()
        {
            if (selectedControls.Count < 1)
                return;

            if (EditingFieldHasFocus())
                return;

            List<FlatControl> flatControls = new List<FlatControl>();

            foreach (Control ctrl in selectedControls)
            {
                FlatControl flatControl = new FlatControl(ctrl);
                flatControls.Add(flatControl);
            }

            Clipboard.SetData(DataFormats.Serializable, flatControls);

            CheckToEnablePaste();
        }

        private void CutSelectedControls()
        {
            CopySelectedControls();
            DeleteSelected();
        }

        private void PasteControls()
        {
            if (EditingFieldHasFocus())
                return;

            Control landingZone = GetLegalLanding();
            Point pasteAt = Point.Empty;

            if (landingZone == null)
                PasteSuccession();
            else
            {
                pasteAt = landingZone.PointToClient(Cursor.Position);
                PasteControls(landingZone, pasteAt);
            }
        }

        private void PasteControls(Control landingZone, Point pasteAt)
        {
            FormAndFuncs ff = GetCurrentFormFunc();

            if (landingZone == null) landingZone = ff;
            if (landingZone == null) return;
            if (pasteAt == null ||
                pasteAt == Point.Empty) 
                pasteAt = new Point(10, 10);
            
            List<FlatControl> flatControls =
                (List<FlatControl>)Clipboard.GetData(DataFormats.Serializable);
            if (flatControls == null) return;

            List<Control> pasteControls = new List<Control>();


            flatControls.Sort(new Comparison<FlatControl>(FlatControl.ComparePositions));
            FlatControl topLeftControl = flatControls[0];
            Point topLeft = new Point(topLeftControl.Location.X, topLeftControl.Location.Y);

            AddUndoPoint();
            ClearSelections();            

            foreach (FlatControl flatCon in flatControls)
            {
                //Normalize
                flatCon.Location.X -= topLeft.X;
                flatCon.Location.Y -= topLeft.Y;
                //Add Mouseloc
                flatCon.Location.X += pasteAt.X;
                flatCon.Location.Y += pasteAt.Y;

                Control newControl = flatCon.RecurseCreateControls(flatCon);
                newControl.Name = GetNewControlName(newControl);              
                landingZone.Controls.Add(newControl);

                SynchronizeTagName(newControl);
                if (newControl is Panel)
                    RecurseSynchronizeTagName(newControl);
                
                RecurseAddControlEvents(newControl);              
                AddToSelection(newControl);
            }
        }

        private void PasteSuccession()
        {
            FormAndFuncs ff = GetCurrentFormFunc();
            int highestY = 0;
            int lowestX = 10;
            int heightOf = 0;

            foreach (Control ctrl in ff.Controls)
            {
                if (ctrl.Location.Y > highestY)
                {
                    highestY = ctrl.Location.Y;
                    heightOf = ctrl.Height;
                }

                if (ctrl.Location.X < lowestX)
                    lowestX = ctrl.Location.X;
            }

            Point pasteAt = new Point(lowestX, highestY + heightOf + 5);

            Control landingZone = GetLegalLanding();

            PasteControls(landingZone, pasteAt);
        }

        private void CheckToEnableCopy()
        {
            if (selectedControls.Count < 1)
            {
                copyEnabled = false;
                return;
            }

            bool ffSelected = selectedControls.Contains(
                GetCurrentFormFunc());

            if (selectedControls.Count == 1 && ffSelected)
                copyEnabled = false;

            else if (selectedControls.Count >= 1)
                copyEnabled = true;

            //Enable/disable menu items.
            copyToolStripMenuItem.Enabled = copyEnabled;
            copyToolStripMenuItem1.Enabled = copyEnabled;
            copyToolStripMenuItem2.Enabled = copyEnabled;
            
            //If copy is available, so is cut.
            cutToolStripMenuItem.Enabled = copyEnabled;
            cutToolStripMenuItem1.Enabled = copyEnabled;
        }

        private void CheckToEnablePaste()
        {
            pasteEnabled = Clipboard.ContainsData(DataFormats.Serializable);

            pasteToolStripMenuItem.Enabled = pasteEnabled;
            pasteToolStripMenuItem1.Enabled = pasteEnabled;
        }


        #endregion

        #region Control Moving/Resizing

        #region Dragging / Dropping

        private void TryDragStart()
        {
            //PrintPos(viewPos);
            //PrintPos(viewPos, "2829 StartDrag");
            if (resizing) return;

            if (newControl)
            {
                if (chkPreventDrag.Checked)
                {
                    chkPreventDrag.Checked = false;
                    StatusMessage("Control dragging re-enabled.", true, 3000);
                }
            }

            if (chkPreventDrag.Checked)
            {
                StatusMessage("Control drag prevented.",
                    true, 2000);
                return;
            }

            if (complexPane.SelectedTab != pageDesign)
            {
                StatusMessage("Select Tab Design to edit layout.", true, 3000);
                return;
            }

            //PrintPos(viewPos, "2834 StartDrag");

            FormAndFuncs ff = GetCurrentFormFunc();
           // Point pos = ff.AutoScrollPosition;
           // ff.AutoScroll = false;

            //Don't drag the FF.
            if (selectedControls.Contains(ff))
                 selectedControls.Remove(ff);
                
            //PrintPos(viewPos, "2840 StartDrag");

            //If there's something to drag...
            if (selectedControls.Count > 0)
            {
                //Drag item 0.
                Control mainDragItem = selectedControls[0];
                lastMouseLoc = this.PointToClient(Cursor.Position);

                if (newControl)
                    controlGrabLoc = G.GetControlCenter(mainDragItem);
                else
                {
                    controlGrabLoc = mainDragItem.PointToClient(Cursor.Position);
                    //Console.WriteLine("Grabbed at: " + controlGrabLoc);
                }

                if (!selectedControls.Contains(ff) && selectedControls.Count == 1)
                    AddUndoPoint(); //Create an undo point.      

                for (int i = 0; i < selectedControls.Count; i++)
                {
                    //One of the selected controls
                    Control selected = selectedControls[i];

                    //Dragging of control occurs on the Form level, 
                    //move into Designer Form if not there already.
                    if (!IsCtrlInForm(selected))
                    {
                        //Try remove auto label.
                        Control parent = selected.Parent;
                        if (parent != null)
                        {
                            selected.Parent.Controls.Remove(selected);

                            //if (RemoveAutoLabel(ref parent, selected))
                            //    Console.WriteLine("Label Removed");
                            //else
                            //    Console.WriteLine("No Auto Label Removed.");
                        }

                        int offset = 0;
                        //Hack to prevent controls from moving strangely
                        //When clicking.
                        if (selected is TextBox)
                            offset = 2;
                        else if (selected is DateTimePicker)
                            offset = 2;
                        else
                            offset = 0;

                        if (newControl)
                        {
                            selected.Location = new Point(
                               selected.Location.X - controlGrabLoc.X - offset,
                               selected.Location.Y - controlGrabLoc.Y - offset);
                        }
                        else
                        {
                            selected.Location = this.PointToClient(parent.PointToScreen(selected.Location));
                        }

                        //Add this selected Control to the Designer Form.
                        this.Controls.Add(selected);
                        selected.BringToFront();
                    }
                }


                draggingControls = true;
                List<string> takenNames = new List<string>();
                RecurseFindTakenNames(ff, ref takenNames);
                ClearBadAutoLabels(ff, takenNames);
                return;
            }

        }

        // Hop drag controls in and to parent controls.
        private void DragSelectedControls()
        {
            if (!draggingControls)
                return;

            string items = "Moving: ";

            Control dragSurface = GetDragSurface();

            if (this == null) return;

            Point currentMouseLoc = this.PointToClient(Cursor.Position);
            Point dragDifference = GetDragDifference(currentMouseLoc);

            if (dragSurface == null)
                return;           
             
            //Drag each selected control in selectedControls.
            foreach (Control selected in selectedControls)
            {
                selected.Location = FormLevelDragLoc(selected, dragDifference);
                items += selected.Name + " ";
                selected.BringToFront();
            }           

            //Redraw the drag surface to clean it up.
            lastMouseLoc = currentMouseLoc; //store this mouseloc as old for next drag event call.

            lblMoving.Text = items;
            lblInside.Text = "Inside: " + dragSurface.Name;

            this.Refresh();
        }

        private Point GetDragDifference(Point currentMouseLoc)
        {
            Point diff = new Point(
                lastMouseLoc.X - currentMouseLoc.X,
                lastMouseLoc.Y - currentMouseLoc.Y);

                return diff;
        }

        private Point FormLevelDragLoc(Control dragItem, Point dragDifference)
        {
            //Don't drag FFs
            if (dragItem is FormAndFuncs)
                return Point.Empty;

            return new Point(
                dragItem.Location.X - dragDifference.X,
                dragItem.Location.Y - dragDifference.Y);
        }

        private void TryControlDrop()
        {
            if (!draggingControls)
                return;

            FormAndFuncs ff = GetCurrentFormFunc();
            Control landingZone = GetLegalLanding();
            Point landingLoc = Point.Empty;


            //If the landing zone is illegal, and this is a new control, destroy.
            if (landingZone == null)
            {
                if (newControl)
                    CancelNewCtrl();

                return;
            }
            else
            {
                if (ff == null)
                    return;

                landingLoc = landingZone.PointToClient(Cursor.Position);
                Point pushback = GetPushback(landingZone);

                foreach (Control selected in selectedControls)
                {
                    if (selected is FormAndFuncs)
                        continue;

                    if (IsControlsRelated(landingZone, selected))
                        continue;

                   // bufferPane.Controls.Add(ff);
                    landingZone.Controls.Add(selected);
                   // layoutPane.Controls.Add(ff);

                    selected.Location = landingZone.PointToClient(this.PointToScreen(selected.Location));
                    selected.Location = new Point(
                        selected.Location.X + pushback.X,
                        selected.Location.Y + pushback.Y);                                     

                    if (selected is Panel)
                        selected.BringToFront();
                    else
                        selected.SendToBack();


                    AddAutoLabel(ref landingZone, selected);
                }

                UpdatePosGUI(selectedControls[0]);

                draggingControls = false;
                newControl = false;

                //Select Control Zero
                Control ctrlZero = selectedControls[0];

                InvalidateRegexList();

                if (ctrlZero is TextBox)
                    ctrlZero.Focus();
                else
                    ff.Focus();
            }
        }

        /// <summary>
        /// Returns how far dropped controls would need to be pushed back
        /// to avoid dropping any in negative coordinates.
        /// </summary>
        /// <returns></returns>
        private Point GetPushback(Control landingZone)
        {
            int xPushback = 0;
            int yPushback = 0;

            foreach (Control selected in selectedControls)
            {
                if (IsControlsRelated(landingZone, selected))
                    continue;

                Point loc = landingZone.PointToClient(this.PointToScreen(selected.Location));

                if (loc.X < xPushback) xPushback = loc.X;
                if (loc.Y < yPushback) yPushback = loc.Y;
            }

            return new Point(-xPushback, -yPushback);
        }

        private void MoveSelectedToLocation()
        {
            foreach (Control selected in selectedControls)
            {
                selected.Top = (int)numTop.Value;
                selected.Left = (int)numLeft.Value;
            }

            InvalidateControls(); // NumTopLeft move
        }

        #endregion //Dragging

        #region Resizing

        private bool TryStartResize(Control sender)
        {
            if (sender == null) return false;

            if (complexPane.SelectedTab != pageDesign)
                return false;

            if (Cursor.Current != Cursors.Default)
            {
                //Horizontal resize.
                if (Cursor.Current == Cursors.SizeWE ||
                    Cursor.Current == Cursors.SizeNS ||
                    Cursor.Current == Cursors.SizeNWSE)
                {
                    resizing = true;
                    AddUndoPoint();
                    //Get the upper left hand corner of the control being resized.
                    dragStart = sender.Location;
                    return resizing;
                }

            }

            resizing = false;
            return resizing;
        }

        private void MouseDragResizeSelectedControls()
        {
            Point p = Cursor.Position;
            int minSize = c_minCtrlSize;

            foreach (Control selected in selectedControls)
            {
                Point change = new Point(
                selected.Parent.PointToClient(p).X - dragStart.X,
                selected.Parent.PointToClient(p).Y - dragStart.Y);

                //Resize control.
                //Prevent control from getting too small to click on.
                if (Cursor.Current == Cursors.SizeNWSE)
                {
                    if (change.X > minSize) 
                        selected.Width = change.X;
                    if (change.Y > minSize) 
                        selected.Height = change.Y;
                }
                else if (Cursor.Current == Cursors.SizeWE)
                {
                    if (change.X > minSize)
                        selected.Width = change.X;
                }
                else if (Cursor.Current == Cursors.SizeNS)
                {
                    if (change.Y > minSize)
                        selected.Height = change.Y;
                }
                else
                {
                    return;
                }
            }    
        }

        private void TryStopResize()
        {
            if (resizing)
                resizing = false;

            //AdjustPageBottomMargin();
        }

        private void SyncNumHeightWidthOnSelected()
        {
            foreach (Control selected in selectedControls)
                {
                    selected.Width = (int)numWidth.Value;
                    selected.Height = (int)numHeight.Value;
                }

                InvalidateControls(); // NumWidthHeight Move
        }

        #endregion Resizing

        //private bool IsInvalidMovement()
        //{
        //    if (numTop.Focused ||
        //        numLeft.Focused ||
        //        numHeight.Focused ||
        //        numWidth.Focused)
        //    {
        //        return true;
        //    }
        //    else
        //        return false;
        //}

        private void SlowMoveControls(KeyEventArgs e)
        {
            const int change = 1;
            bool didUpdate = false;

            foreach (Control ctrl in selectedControls)
            {
                if (keyUpPressed)
                {
                    ctrl.Location = new Point(ctrl.Location.X, ctrl.Location.Y - change);
                    didUpdate = true;
                }
                else if (keyDownPressed)
                {
                    ctrl.Location = new Point(ctrl.Location.X, ctrl.Location.Y + change);
                    didUpdate = true;
                }

                if (keyLeftPressed)
                {
                    ctrl.Location = new Point(ctrl.Location.X - change, ctrl.Location.Y);
                    didUpdate = true;
                }
                else if (keyRightPressed)
                {
                    ctrl.Location = new Point(ctrl.Location.X + change, ctrl.Location.Y);
                    didUpdate = true;
                }

                UpdateAutoLabelPosition(ctrl);
            }


            if (didUpdate)
            {
                if (selectedControls.Count > 0)
                    UpdatePosGUI(selectedControls[0]);

                e.Handled = true; //Do not register a change to a focused control.

                InvalidateControls(); // Arrow move
            }
        }
      
        private void UpdateMouseCursor(Control sender)
        {
            //Make the resize cursor if it's over a 
            //valid resize area.
            if (sender is TextBox ||
                sender is Panel)
            {
                if (IsContainedInFF(sender))
                {
                    Cursor.Current = GetResizeIcon(sender);
                    return;
                }
            }
            
           // Cursor.Current = Cursors.Default;
        }

        #endregion //Control Moving / Resizing

        #region Control Selecting

        /// <summary>
        /// Recurse through panels to see if it contains a selected child.
        /// </summary>
        /// <param name="root"></param>
        /// <returns></returns>
        private bool HasSelectedChild(Panel root)
        {
            bool childSelected = false;

            //If any of this panels controls are selected...
            foreach (Control ctrl in root.Controls) 
                if (selectedControls.Contains(ctrl))
                    return true;
                else if (ctrl is Panel) //If this panel contains a panel.
                {
                    //Check it's children for a selected child.
                    childSelected = HasSelectedChild((Panel)ctrl);
                    if (childSelected)
                        return childSelected; //Return if true.
                }

            return childSelected; //Should be false.
        }

        //Recursive
        private bool PanelContainsControl(Panel parent, Control ctrl)
        {
            bool panelContains = false;

            if (parent.Controls.Contains(ctrl))
                return true;
            else
            {
                foreach (Control child in parent.Controls)
                    if (child is Panel)
                    {
                        panelContains = PanelContainsControl((Panel)child, ctrl);

                        if (panelContains)
                            return panelContains;
                    }                      
            }

            return panelContains;
        }

        private bool IsChildOfSelectedPanel(Control ctrl)
        {
            bool isChildOfSelectedPanel = false;

            foreach (Control selectedCtrl in selectedControls)
                if (selectedCtrl is Panel)
                {
                    isChildOfSelectedPanel = PanelContainsControl((Panel)selectedCtrl, ctrl);

                    if (isChildOfSelectedPanel)
                        return isChildOfSelectedPanel;
                }

            return isChildOfSelectedPanel;
        }
         
        //On mouse down
        private void ToggleSelect(Control ctrl, bool preventMouseDrag = false)
        {
            bool alreadySelected = IsCtrlSelected(ctrl);
            bool hasSelectedChild = false;
            bool isChildOfSelected = false;
            bool didDeselect = false;
            bool ctrlKeyPressed = ModifierKeys == Keys.Control;

            // Trying to select a Panel (not a FF)
            if (ctrl is Panel && !(ctrl is FormAndFuncs))
            {
                //Trying to select a panel.
                //Does this panel or a child panel contain a selected control?
                hasSelectedChild = HasSelectedChild((Panel)ctrl);
            }

            //Do any selected panels contain this control?
            isChildOfSelected = IsChildOfSelectedPanel(ctrl);
            if (isChildOfSelected)
                DeselectParent(ctrl);
                 
            //Ctrl Left Click
            if (ctrlKeyPressed)
            {
                //If this ctrl is already selected.
                if (alreadySelected)
                {
                    RemoveFromSelection(ctrl);
                    didDeselect = true;
                }
                else
                {
                    if (hasSelectedChild)
                        DeselectChildren((Panel)ctrl);

                    AddToSelection(ctrl);
                }

                //ignoreLabelChange = true;
                InvalidateControls();
            }
            else //Left Click.
            {
                if (alreadySelected)
                {
                    if (ctrl is RadioButton)
                    {
                        RadioButton radButt = (RadioButton)ctrl;
                        radButt.Checked = !radButt.Checked;
                    }

                    if (ctrl is CheckBox)
                    {
                        CheckBox chkBox = (CheckBox)ctrl;
                        chkBox.Checked = !chkBox.Checked;
                    }
                }

                if (!alreadySelected)
                {
                    ClearSelections();
                    AddToSelection(ctrl);
                }

                if (ctrl is ComboBox)
                    if (((ComboBox)ctrl).DroppedDown)
                        return;

                if (ctrl is DateTimePicker)
                    if (calandarDropped)
                        return;

                TryStartResize(ctrl);
             
            }

            if (!resizing && !preventMouseDrag)
                if (!(ctrl is FormAndFuncs) && !ctrlKeyPressed)
                {
                    TryDragStart();
                }
                else //Start drawing selection rubberband.
                {
                    if (!didDeselect)
                        StartRubberbandSelect();
                }

            if (ctrl is Panel)
                ctrl.BringToFront();
            else
                ctrl.SendToBack();

            if (!rubberbanding)
                RefreshSelectedProperties(); 
        }

        private void AddToSelection(Control ctrl)
        {
            selectedControls.Add(ctrl);
        }

        private void RemoveFromSelection(Control ctrl)
        {
            selectedControls.Remove(ctrl);            
        }

        private void DeselectChildren(Panel root)
        {
            foreach (Control ctrl in root.Controls)
                if (selectedControls.Contains(ctrl))
                {
                    if (ctrl is Panel)
                        DeselectChildren((Panel)ctrl);

                    RemoveFromSelection(ctrl);
                }
        }

        private void DeselectParent(Control ctrl)
        {
            if (ctrl.Parent == null)
                return;
            else
            {
                if (selectedControls.Contains(ctrl.Parent))
                    RemoveFromSelection(ctrl.Parent);
                DeselectParent(ctrl.Parent);
            }
        }

        private void DeselectControl(Control ctrl)
        {
            if (ctrl == null) return;
            else
            {
                if (selectedControls.Contains(ctrl))
                {
                    RemoveFromSelection(ctrl);
                    ctrl.Invalidate();
                }
                    
            }
        }

        private void ClearSelections()
        {
            selectedControls.Clear();
            InvalidateControls(); // Clear Selections
        }

        private bool IsCtrlSelected(Control thisControl)
        {
            if (selectedControls.Contains(thisControl))
                return true;
            else
                return false;
        }

        private bool IsMouseCtrlArrow(Control ctrl)
        {


            return false;
        }

        #region Rubberband Select

        private void RubberbandRectangle(Point p1, Point p2)
        {
            if (rbLanding == null) return;

            Graphics g = rbLanding.CreateGraphics();

            RubberbandRects.RubberbandRectangle r =
                new RubberbandRects.RubberbandRectangle();
            r.DrawXORRectangle(g, p1.X, p1.Y, p2.X, p2.Y);

        }

        private void StartRubberbandSelect()//Point mouseLoc)
        {
            //Store starting location.
            rbLanding = GetDragSurface();
            Point mouseLoc = rbLanding.PointToClient(Cursor.Position);
            ptOldMouse = ptMouseDownAt = mouseLoc;        
            //ignoreLabelChange = true;
            mouseDown = true;
            mouseMoving = false;
            rubberbanding = true;
            //Console.WriteLine("Start RB");      
        }

        private void StopRubberbanding(object sender, MouseEventArgs e)
        {
            mouseDown = false;
            mouseMoving = false;
            //ignoreLabelChange = true;
            

            if (rubberbanding)
            {
                rubberbanding = false;

                Rectangle selectionArea = new Rectangle();
                Point p1 = ptMouseDownAt;
                //Point p2 = e.Location;
                Point p2 = rbLanding.PointToClient(Cursor.Position);

                //Normalize rectangle formation parameters.
                if (p1.X < p2.X)
                {
                    selectionArea.X = p1.X;
                    selectionArea.Width = p2.X - p1.X;
                }
                else
                {
                    selectionArea.X = p2.X;
                    selectionArea.Width = p1.X - p2.X;
                }
                if (p1.Y < p2.Y)
                {
                    selectionArea.Y = p1.Y;
                    selectionArea.Height = p2.Y - p1.Y;
                }
                else
                {
                    selectionArea.Y = p2.Y;
                    selectionArea.Height = p1.Y - p2.Y;
                }

                FormAndFuncs ff = GetCurrentFormFunc();

                if (Control.ModifierKeys != Keys.Control)
                    ClearSelections();

                //RecurseAddRubberbandedArea(rbLanding, selectionArea);
                IterateRubberbandedArea(rbLanding, selectionArea);
                selectedControls.Remove(rbLanding);


                //The landing was clicked, no other controls selected.
                if (selectedControls.Count < 1)
                    AddToSelection(rbLanding);

                //if (selectedControls.Contains(ff))
                //    complexPane.Select();

                rbLanding.Invalidate();
                RefreshSelectedProperties();
                //Console.WriteLine("Stop RB");
            }
        }

        private void RecurseAddRubberbandedArea(Control root, Rectangle selectionArea)
        {
            //Add all contained controls to the selection list.
            foreach (Control ctrl in root.Controls)
            {
                if (!IsAutoLabelName(ctrl.Name))
                {
                    Rectangle controlArea = new Rectangle(ctrl.Location, ctrl.Size);

                    if (selectionArea.IntersectsWith(controlArea))
                    {
                        if (!selectedControls.Contains(ctrl))
                        {
                            if (ctrl is Panel)
                                RecurseAddRubberbandedArea(ctrl, selectionArea);

                            AddToSelection(ctrl);
                        }
                    }
                }
            }
        }

        private void IterateRubberbandedArea(Control root, Rectangle selectionArea)
        {
            foreach (Control ctrl in root.Controls)
            {
                if (!IsAutoLabelName(ctrl.Name))
                {
                    Rectangle controlArea = new Rectangle(ctrl.Location, ctrl.Size);

                    if (selectionArea.IntersectsWith(controlArea))
                    {
                        if (!selectedControls.Contains(ctrl))
                        {
                            AddToSelection(ctrl);
                        }
                    }
                }
            }
        }

       
        #endregion


        #endregion

        #region Visual Editing GUI

        private void UpdateComboControl()
        {
            foreach (Control selected in selectedControls)
            {
                if (selected == null)
                    continue;

                if (!(selected is ComboBox))
                    continue;

                string comboItemsString = txtComboEdit.Text.TrimEnd();

                string[] items = comboItemsString.Split('\n');
                ComboBox cboBox = (ComboBox)selected;
                cboBox.Items.Clear();
                foreach (string item in items)
                {
                    if (item != Environment.NewLine)
                        cboBox.Items.Add(item);
                }

                G.AutoAdjustComboBoxDropDownSize(selected);

                break;
            }
        }

        private void FillInProperties()
        {
            DeactivateUnusedProperties();
            
            foreach (Control selected in selectedControls)
            {
                ControlData prop = GetLabelPropFromControl(selected);

                if (prop == null)
                {
                    MessageBox.Show("Prop for " + selected.Name + " is null");
                    continue;
                }

                //Item Properties
                txtTagName.Text = (txtTagName.Enabled) ? selected.Name : "";
                numTop.Value = (numTop.Enabled) ? selected.Location.Y : 0;
                numLeft.Value = (numLeft.Enabled) ? selected.Location.X : 0;
                numHeight.Value = (numHeight.Enabled) ? selected.Height : c_minCtrlSize;
                numWidth.Value = (numWidth.Enabled) ? selected.Width : c_minCtrlSize;

                if (prop != null)
                {
                    //Label Properties
                    if (selected is FormAndFuncs)
                        txtLabel.Text = selected.Text;
                    else 
                        txtLabel.Text = (txtLabel.Enabled) ? prop.LabelText : "";

                    cboLabelPosition.SelectedItem =
                        ControlData.EnumToFormattedLabelname(prop.LabelPos);
                }
                else
                    Console.WriteLine("Prop for " + selected.Name + " is null.");

                if (selected is ComboBox)
                {
                    if (txtComboEdit.Enabled)
                    {
                        ComboBox cboBox = (ComboBox)selected;
                        string itemsString = string.Empty;

                        foreach (string item in cboBox.Items)
                        {
                            if (item != string.Empty &&
                                item.EndsWith(Environment.NewLine))
                                itemsString += item;
                            else
                                itemsString += item + Environment.NewLine;
                        }

                        txtComboEdit.Text = itemsString;
                    }
                }
            }
        }
     
        private void DeactivateUnusedProperties()
        {
            //ToggleForCheckBox(false);
            int comboBoxCount = 0;

            if (selectedControls.Count > 1)
            {
                txtLabel.Enabled = false;
                txtTagName.Enabled = false;
                numTop.Enabled = false;
                numLeft.Enabled = false;
            }


            foreach (Control ctrl in selectedControls)
            {
                if (!(ctrl is ComboBox))
                {
                    lblComboEdit.Visible = false;
                    txtComboEdit.Visible = false;
                    lblComboEdit.Enabled = false;
                    txtComboEdit.Enabled = false;
                }

                if (ctrl is FormAndFuncs)
                {
                    TogglePropsSize(false);
                    TogglePropsLoc(false);

                    txtTagName.Enabled = false;

                    btnCtrlFont.Enabled = false;
                    btnCtrlFgCol.Enabled = false;

                    cboLabelPosition.Enabled = false;
                    btnLblFont.Enabled = false;
                    btnLblBgCol.Enabled = false;
                    btnLblFgCol.Enabled = false;
                }

                if (ctrl is Label)
                {
                    TogglePropsSize(false);
                    btnCtrlFgCol.Enabled = false;
                    btnCtrlBgCol.Enabled = false;
                    btnCtrlFont.Enabled = false;
                    cboLabelPosition.Enabled = false;
                    cboLabelPosition.Text = LabelPosition.None.ToString();
                }



                if (ctrl is TextBox)
                {
                    if (!((TextBox)ctrl).Multiline)
                    {
                        numHeight.Enabled = false;
                    }
                }

                if (ctrl is Panel)
                {
                    btnCtrlFont.Enabled = false;
                    btnCtrlFgCol.Enabled = false;
                }

                if (ctrl is CheckBox ||
                    ctrl is RadioButton)
                {
                    TogglePropsSize(false);
                    //ToggleForCheckBox(true);
                    btnCtrlFont.Enabled = false;
                    btnCtrlFgCol.Enabled = false;
                    btnCtrlBgCol.Enabled = false;              
                }

                if (ctrl is ComboBox)
                {
                    numHeight.Enabled = false;
                    comboBoxCount++;

                    if (comboBoxCount > 1)
                    {
                        txtComboEdit.Enabled = false;
                        txtComboEdit.Visible = false;
                    }
                }

                if (ctrl is DateTimePicker)
                {
                    numHeight.Enabled = false;
                }
            }
        }

        private void TogglePropEditing(bool turnOn)
        {
            //Enable Item's Label.
            //paneLabel.Enabled = turnOn;
            txtLabel.Enabled = turnOn;
            cboLabelPosition.Enabled = turnOn;
            btnLblFont.Enabled = turnOn;
            btnLblFgCol.Enabled = turnOn;
            btnLblBgCol.Enabled = turnOn;

            txtLabel.Text = "";

            if (!turnOn)
                cboLabelPosition.SelectedItem = 0;

            //Enable Item Properties
            txtTagName.Enabled = turnOn;
            numTop.Enabled = turnOn;
            numLeft.Enabled = turnOn;
            numHeight.Enabled = turnOn;
            numWidth.Enabled = turnOn;

            btnCtrlFont.Enabled = turnOn;
            btnCtrlFgCol.Enabled = turnOn;
            btnCtrlBgCol.Enabled = turnOn;

            txtTagName.Text = "";
            numTop.Value = 0;
            numLeft.Value = 0;
            numHeight.Value = c_minCtrlSize;
            numWidth.Value = c_minCtrlSize;

            lblComboEdit.Visible = turnOn;
            lblComboEdit.Enabled = turnOn;
            txtComboEdit.Visible = turnOn;
            txtComboEdit.Enabled = turnOn;
            //txtComboEdit.Text = "";

        }

        /// <summary>
        /// Switch on/off showing height and width.
        /// </summary>
        /// <param name="turnOn"></param>
        private void TogglePropsSize(bool turnOn)
        {
            numHeight.Enabled = turnOn;
            numWidth.Enabled = turnOn;
        }

        private void TogglePropsLoc(bool turnOn)
        {
            numTop.Enabled = turnOn;
            numLeft.Enabled = turnOn;
        }

        /// <summary>
        /// Examines the selectedItems List to allow edinting of one or more controls.
        /// </summary>
        private void RefreshSelectedProperties()
        {
            CheckToEnableCopy();
            CheckToEnablePaste();

            //No items selected, disable all control editing, since any
            //editing would be meaningless.
            if (selectedControls.Count == 0)
            {
                TogglePropEditing(false);
                return;
            }
            else
                TogglePropEditing(true);
           
            FillInProperties();         
        }

        private void UpdatePosGUI(Control selected)
        {
            if (selected == null)
                return;

            if (IsCtrlInForm(selected))
            {
                numLeft.Value = selected.Location.X - c_ffOffsetX;
                numTop.Value = selected.Location.Y - c_ffOffsetY;
            }
            else
            {
                numLeft.Value = selected.Location.X;
                numTop.Value = selected.Location.Y;
            }         
        }

        private void UpdateSizeGUI(Control selected)
        {
            if (
                numHeight.Value != 0 &&
                numWidth.Value != 0)
            {
                numHeight.Value = selected.Height;
                numWidth.Value = selected.Width;
            }
        }

        #endregion //De-activate properties
      
        #region Misc / Helper

        //private void UpdateSelectedSizeLoc()
        //{
        //    LabelPosition pos = GetSelectedLabelPos();

        //    if (numTop.Value != 0 &&
        //        numLeft.Value != 0 &&
        //        numHeight.Value != 0 &&
        //        numWidth.Value != 0)
        //        foreach (Control ctrl in selectedControls)
        //        {
        //            ctrl.Top = (int)numTop.Value;
        //            ctrl.Left = (int)numLeft.Value;
        //            ctrl.Height = (int)numHeight.Value;
        //            ctrl.Width = (int)numWidth.Value;

        //            //Try to retrieve the corresponding autoLabel for this control.
        //            Control parent = ctrl.Parent;
        //            Label label = FindCtrlsAutoLabel(ref parent, ctrl);

        //            if (label == null)
        //                continue;

        //            label.Location = GetLabelPoint(ctrl, label, pos);
        //            parent.Invalidate();
        //        }
        //}

        private bool IsFormFuncsOpen()
        {
            if (layoutPane.TabPages.Count > 0)
                return true;
            else
                return false;
        }

        private FormAndFuncs GetCurrentFormFunc()
        {
            FormAndFuncs formFunc = null;

            foreach (Control ctrl in layoutPane.Controls)
                if (ctrl is FormAndFuncs)
                {
                    formFunc = (FormAndFuncs)ctrl;
                    formFunc.actions = this.actionList;
                    break;
                }


            return formFunc;
        }
        
        private void InvalidateControls() //Declaration
        {
            Control ff = GetCurrentFormFunc();
            if (ff != null)
            {
                ff.Invalidate();
                InvalidateChildren(ff);
            }

        }

        private void InvalidateChildren(Control parent)
        {
            foreach (Control child in parent.Controls)
            {
                if (!(child is DateTimePicker ||
                    child is TextBox ||
                    child is ComboBox)) 
                    child.Invalidate();

                if (child.HasChildren)
                    InvalidateChildren(child);

                
            }
        }

        private bool IsControlsRelated(Control parent, Control child)
        {
            if (parent == child)
                return true;
            else
                return false;          
        }

        private bool IsCtrlInForm(Control ctrl)
        {
            foreach (Control child in this.Controls)
                if (ctrl == child)
                    return true;

            return false;
        }

        private bool IsCtrlHasField(Control ctrl)
        {
            if (ctrl is TextBox ||
                ctrl is ComboBox ||
                ctrl is DateTimePicker ||
                ctrl is RadioButton ||
                ctrl is CheckBox)
                return true;
            else
                return false;
        }

        #endregion

        #endregion //Control editing functions

        #region Label Prop Functions

        #region Auto Label Functions

        private bool OnlyChkBoxRadioSelected()
        {
            //If any selected is not a radiobutton or checkbox, return false.
            foreach (Control selected in selectedControls)
                if (!(selected is CheckBox) && !(selected is RadioButton))
                    return false;

            //None selected are Radio or Checkboxes, retuen true;
            return true;
        }

        private void UpdateAutoLabelTextAndPos()
        {
            //If more than one item, don't apply.
            bool apply = (selectedControls.Count > 1) ? false : true;
            bool onlyBoxesSelected = OnlyChkBoxRadioSelected();
            FormAndFuncs ff = GetCurrentFormFunc();

            if (ff == null) return;

            LabelPosition selectedPos = GetSelectedLabelPos();

            //Update Position, text
            foreach (Control selected in selectedControls)
            {
                Control parent = selected.Parent;
                if (parent == null)
                    continue;

                if (selected is FormAndFuncs)
                {                   
                    ff.Text = txtLabel.Text;
                    continue;
                }

                   
                ControlData prop = GetLabelPropFromControl(selected);

                //Label, RadioButton and Checkbox contain their auto label.
                if (selected is Label ||
                    selected is RadioButton ||
                    selected is CheckBox)
                {
                    if (apply)
                    {
                        prop.LabelText = txtLabel.Text;
                        selected.Text = txtLabel.Text;
                        prop.LabelPos = selectedPos;
                    }
                    
                    selected.Font = prop.Font.GetFont();
                    selected.ForeColor = Color.FromArgb(prop.Font.ForeColor);
                    selected.BackColor = Color.FromArgb(prop.Font.BackColor);

                    ContentAlignment specPos =
                          ControlData.GetContentAlignment(selectedPos);

                    if (onlyBoxesSelected)
                    {
                        if (selected is RadioButton ||
                            selected is CheckBox)
                        {
                            if (selected is CheckBox)
                                ((CheckBox)selected).CheckAlign = specPos;
                            if (selected is RadioButton)
                                ((RadioButton)selected).CheckAlign = specPos;
                        }
                    }
                }
                else //Not a label, checkbox, or radiobutton.
                {
                    //Only change pos/text if 1 control is selected.
                    if (apply)
                    {
                        prop.LabelText = txtLabel.Text;
                        prop.LabelPos = selectedPos;

                        Control label = FindCtrlsAutoLabel(ref parent, selected);

                        if (label != null)
                        {
                            label.Text = txtLabel.Text;
                            UpdateAutoLabelPosition(selected);
                        }
                    }

                }
            }

            ff.Invalidate();

        }

        private void RecurseSynchronizeTagName(Control root)
        {
            foreach (Control ctrl in root.Controls)
            {
                SynchronizeTagName(ctrl);
                UpdateAutoLabelPosition(ctrl);
                if (ctrl is Panel)
                    RecurseSynchronizeTagName(ctrl);
            }

            
        }

        private void SynchronizeTagName(Control ctrl, bool suppressConflictNotice = true)
        {
            //update the name of this label to mirror it's parent, and refresh regexTagList

            int cursorPos = txtTagName.SelectionStart;
            int len = txtTagName.Text.Length;
            string newTagName = G.Tagify(txtTagName.Text);
            bool oldAutoLabelFound = false;

            if (newTagName == string.Empty)
                return;

            FormAndFuncs currentFF = GetCurrentFormFunc();

            if (currentFF == null) 
                return;

            List<string> takenNames = new List<string>();
            RecurseFindTakenNames(currentFF, ref takenNames); //Seek names from all pages
            //RecurseFindTakenNames(leftPane, ref takenNames);
            //RecurseFindTakenNames(rightPane, ref takenNames);

            //Remove selected from the list.
            takenNames.Remove(ctrl.Name);

            if (IsTagNameTaken(newTagName, ref takenNames))
            {
                if (!suppressConflictNotice)
                    StatusMessage("Tag Name: ' " + newTagName + "' is already in use.",
                        true, 5000);
                newTagName = GetNewControlName(ctrl);
            }

            ControlData prop = (ControlData)ctrl.Tag;

            //Mirror good tagName on selected Control,
            //And the txtTagName
            string oldName = ctrl.Name;
            ctrl.Name = newTagName;
            txtTagName.Text = newTagName;

            //Reposition the cursor in case illegal chars were entered.
            if (cursorPos > newTagName.Length)
                txtTagName.SelectionStart = newTagName.Length;
            else
                if (len == newTagName.Length)
                    txtTagName.SelectionStart = cursorPos;
                else if (cursorPos != 0)
                    txtTagName.SelectionStart = cursorPos - 1;
                else
                    txtTagName.SelectionStart = 0;

            //Try to find the autoLabel and rename it.
            if (ctrl.Parent != null)
                foreach (Control control in ctrl.Parent.Controls)
                {
                    if (control is Label)
                        if (IsAutoLabelName(control.Name))
                            if (control.Name == oldName)
                            {
                                control.Name = newTagName;
                                oldAutoLabelFound = true;
                            }
                }

            if (oldAutoLabelFound)
                return; //It's all good.
            else
            {   //Clear any auto labels that don't have a corresponding parent.
                ClearBadAutoLabels(currentFF, takenNames);
                Control landingZone = ctrl.Parent;
                AddAutoLabel(ref landingZone, ctrl);
            }

            InvalidateRegexList();
        }

        private bool IsAutoLabelName(string controlName)
        {
            if (controlName.StartsWith(c_labelPrefix))
                return true;
            else
                return false;
        }

        private void ClearBadAutoLabels(Control landingZone, List<string> takenNames)
        {
            foreach (Control ctrl in landingZone.Controls)
            {
                if (ctrl is Panel)
                    ClearBadAutoLabels(ctrl, takenNames);

                foreach (string controlName in takenNames)
                {
                    //Where is your parent, kid?
                    if (IsAutoLabelName(controlName))
                    {
                        string parentName = controlName.Replace(c_labelPrefix, "");

                        //No parent eh? Out of the pool.
                        if (!takenNames.Contains(parentName))
                            RemoveAutoLabel(landingZone, controlName);

                    }
                }
            }
        }

        /// <summary>
        /// Returns the LabelPosition enum for the selected item in cboLabelPosition
        /// </summary>
        /// <returns></returns>
        private LabelPosition GetSelectedLabelPos()
        {
            string item = (string)cboLabelPosition.SelectedItem;

            if (item == null)
                //What's happening if this is null?
                return LabelPosition.None;


            LabelPosition pos = (LabelPosition)Enum.Parse(typeof(LabelPosition), item.Replace('-', '_'));
            return pos;
        }

        /// <summary>
        /// Returns true if a label is auto added to the landing zone.
        /// </summary>
        /// <param name="landingZone"></param>
        /// <param name="selected"></param>
        /// <returns></returns>
        private Label AddAutoLabel(ref Control landingZone, Control selected)
        {
            if (selected == null)
                return null;

            ControlData prop = GetLabelPropFromControl(selected);
            if (prop == null)
                return null;

            if (prop.LabelPos == LabelPosition.None)
                return null;

            if (!(landingZone is Panel ||
                landingZone is FormAndFuncs))
                return null;

            if (selected is Label ||
                selected is CheckBox ||
                selected is RadioButton)
                return null;

            Label label = prop.GetAutoLabel();
            if (label == null)
                return null;

            
            label.Name = GetLabelName(selected.Name);
            landingZone.Controls.Add(label);
            Console.WriteLine("Added label for " + selected.Name);
            label.Location = GetLabelPoint(selected, label, prop.LabelPos);
            return label;

        }

        /// <summary>
        /// Returns true if a label is removed from the landing zone.
        /// </summary>
        /// <param name="landingZone"></param>
        /// <param name="selected"></param>
        /// <returns></returns>
        private bool RemoveAutoLabel(ref Control landingZone, Control selected)
        {
            if (selected == null)
                return false;

            Control lblToRemove = FindCtrlsAutoLabel(ref landingZone, selected);

            if (lblToRemove == null)
            {
                return false;
            }
            else
            {
                for (int i = 0; i < landingZone.Controls.Count; i++)
                {
                    if (landingZone.Controls[i].Name == lblToRemove.Name)
                    {
                        landingZone.Controls.RemoveAt(i);
                        i--;
                    }
                    
                }

                return true;
            }

        }

        private void RemoveAutoLabel(Control rootControl, string removeThisName)
        {
            List<Control> toRemove = null;

            if (rootControl.HasChildren)
                foreach (Control child in rootControl.Controls)
                {
                    if (child is Label)
                    {
                        if (child.Name == removeThisName)
                        {
                            if (toRemove == null)
                                toRemove = new List<Control>();

                            toRemove.Add(child);
                        }
                    }
                    else
                    {
                        if (child is Panel)
                            RemoveAutoLabel(child, removeThisName);
                    }
                }

            if (toRemove != null)
                while (toRemove.Count > 0)
                {
                    rootControl.Controls.Remove(toRemove[0]);
                    toRemove.RemoveAt(0);
                }
        }

        private void UpdateAutoLabelPosition(Control ctrl)
        {
            if (ctrl == null)
                return;

            Control parent = ctrl.Parent;
            if (ctrl.Parent == null)
                return;

            ControlData prop = GetLabelPropFromControl(ctrl);
            if (prop == null)
                return;

            Label label = FindCtrlsAutoLabel(ref parent, ctrl);
            if (label == null)
                return;
            
            label.Location = GetLabelPoint(ctrl, label, prop.LabelPos);
                
        }

        private string GetLabelName(string parentName)
        {
            return c_labelPrefix + parentName;
        }

        private Label FindCtrlsAutoLabel(ref Control landingZone, Control ctrl)
        {
            if (ctrl is FormAndFuncs ||
                ctrl is CheckBox ||
                ctrl is RadioButton ||
                landingZone == null)
                return null;

            string labelName = GetLabelName(ctrl.Name);
            Label foundLabel = null;

            foreach (Control child in landingZone.Controls)
                if (child is Label)
                    if (child.Name == labelName)
                    {
                        foundLabel = (Label)child;
                    }

            if (foundLabel == null)
                foundLabel = AddAutoLabel(ref landingZone, ctrl);

            return foundLabel;
        }

        private bool IsLabelName(string controlName)
        {
            if (controlName.StartsWith(c_labelPrefix))
                return true;
            else
                return false;
        }

        private Point GetLabelPoint(Control ctrl, Label label, LabelPosition pos)
        {
            //Get all positioning reletive to the ctrl, since it's location data
            //Is reletive to it's parent container.

            if (ctrl is FormAndFuncs ||
                ctrl is CheckBox ||
                ctrl is RadioButton)
                return Point.Empty;

            Point labelPos = Point.Empty;
            int margin = 3;
            Point ctrlLoc = ctrl.Location;

            switch (pos)
            {
                case LabelPosition.None:
                    return labelPos;

                case LabelPosition.Left:
                    labelPos = new Point(
                        ctrlLoc.X - label.Width - margin,
                        ctrlLoc.Y);
                    break;

                case LabelPosition.Right:
                    labelPos = new Point(
                        ctrlLoc.X + ctrl.Width + margin,
                        ctrlLoc.Y);
                    break;

                case LabelPosition.Top:
                    labelPos = new Point(
                        ctrlLoc.X + ((ctrl.Width / 2) - (label.Width / 2)),
                        ctrlLoc.Y - label.Height - margin);
                    break;

                case LabelPosition.Bottom:
                    labelPos = new Point(
                        ctrlLoc.X + ((ctrl.Width / 2) - (label.Width / 2)),
                        ctrlLoc.Y + ctrl.Height + margin);
                    break;

                case LabelPosition.Top_Left:
                    labelPos = new Point(
                        ctrlLoc.X,
                        ctrlLoc.Y - label.Height - margin);
                    break;

                case LabelPosition.Top_Right:
                    labelPos = new Point(
                        ctrlLoc.X + ctrl.Width - label.Width,
                        ctrlLoc.Y - label.Height - margin);
                    break;

                case LabelPosition.Bottom_Left:
                    labelPos = new Point(
                        ctrlLoc.X,
                        ctrlLoc.Y + ctrl.Height + margin);
                    break;

                case LabelPosition.Bottom_Right:
                    labelPos = new Point(
                        ctrlLoc.X + ctrl.Width - label.Width,
                        ctrlLoc.Y + ctrl.Height + margin);
                    break;
                        
                default:
                    break;
            }

            return labelPos;
        }

       
        #endregion

        private ControlData GetLabelPropFromControl(Control ctrl)
        {
            if (ctrl.Tag == null)
                return null;

            ControlData prop = (ControlData)ctrl.Tag;
            return prop;
        }

        #endregion //Label Prop Functions

        #region Paint Functions

        private void DrawReversibleRectangle(Point p1, Point p2)
        {
            
            FormAndFuncs ff = GetCurrentFormFunc();

            Rectangle r = new Rectangle();
            //Convert points to screen coordinates.
            p1 = PointToScreen(p1);
            p2 = PointToScreen(p2);

           // Normalize the rectangle?
            if (p1.X < p2.X)
            {
                r.X = p1.X;
                r.Width = p2.X - p1.X;
            }
            else
            {
                r.X = p2.X;
                r.Width = p1.X - p2.X;
            }

            if (p1.Y < p2.Y)
            {
                r.Y = p1.Y;
                r.Height = p2.Y - p1.Y;
            }
            else
            {
                r.Y = p2.Y;
                r.Height = p1.Y - p2.Y;
            }

            r.X += layoutPane.Location.X;
            r.Y += layoutPane.Location.Y;

            //Draw reversible frame.
            ControlPaint.DrawReversibleFrame(r, Color.Red, FrameStyle.Dashed);
        }

        #endregion

        #region Misc

        //private void AdjustPageBottomMargin()
        //{
        //    FormAndFuncs ff = GetCurrentFormFunc();
        //    int mostHeight = 0;
        //    int mostDown = 0;
        //    Control lowestBiggestCtrl = null;

        //    foreach (Control ctrl in ff.Controls)
        //    {
        //        if ((ctrl.Location.Y + ctrl.Height) > mostHeight + mostDown)
        //        {
        //            mostHeight = ctrl.Height;
        //            mostDown = ctrl.Location.Y;
        //            lowestBiggestCtrl = ctrl;
        //        }
        //    }

        //   // ff.AutoScrollMargin = new Size(0, 50);
        //}

        #endregion

        #endregion

        #region Action Section

        #region Mouse

        private void logicPane_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Scroll;
        }


        #endregion

        #region Create New Action.

        //New Action
        private void btnNewAction_Click(object sender, EventArgs e)
        {
            AddNewAction();
        }

        #endregion //New Action

        #region Delete Action

        private void DeleteCurrentAction()
        {
            Action currentAction = GetCurrentAction();

            if (currentAction != null)
            {
                AddUndoPoint();

                string actionTagName = currentAction.tagName;
                FormAndFuncs currentFF = GetCurrentFormFunc();

                currentFF.actions.Remove(actionTagName);
                currentAction.ClearLogicPane(ref logicPane, true);
            
                InvalidateActionList();

                //Select action 0 if any actions still exist.
                if (currentFF.actions.Count > 0)
                {
                    cboActionList.SelectedIndex = 0;
                    cboActionList_SelectionChangeCommitted(null, null);
                }
                                     
            }       
        }

        #endregion

        #region Evaluate Action

        private void EvaluateAction()
        {
            Action rootAction = GetCurrentAction();

            if (rootAction != null)
            {
                try
                {
                    Fields fields = rootAction.GetFields();
                    txtOutput.Text = rootAction.EvalStructure(ref fields);
                }
                catch (Exception ex)
                {
                    txtOutput.Text = ex.Message;
                    txtOutput.Text += G.NL + " Unknown Eval End. ";
                }
            }
            else
                txtOutput.Text = "Click <NewAction> to add an Action.";
        }

        #endregion

        #region Get Action Handle

        public Action GetCurrentAction()
        {
            Action rootAction = null;

            foreach (Control ctrl in logicPane.Controls)
            {
                if (ctrl is Action)
                {
                    rootAction = (Action)ctrl;
                    break;
                }
            }

            return rootAction;
        }

        #endregion

        #region Add Buttons

        private void AddNewAction(string tagName = "", string label = "")
        {
            FormAndFuncs currentFF = GetCurrentFormFunc();
            bool cancelClicked = false;
            Action newAction = new Action(ref this.logicPane, ref currentFF);
            newAction.tagName = tagName;
            newAction.label = label;
            Fields fields = new Fields();
            fields.PopulateFromPane(layoutPane);
            ActionEdit newActionWindow = new ActionEdit(ref newAction, ref fields);
            newActionWindow.ffLabel = currentFF.Text;
            newActionWindow.currentFF = currentFF;
            newActionWindow.ShowDialog();

            while (!cancelClicked)
            {
                if (newActionWindow.thisAction == null)
                    return;
                else //Create a new Action.
                {
                    if (actionList.ContainsKey(newAction.tagName))
                    {
                        G.MessageShow("This tab already contains an Action named: "
                            + G.TagWrap(newAction.tagName) + Environment.NewLine +
                            "Please give this new Action a unique name.",
                            "Make it uniquer.");
                        newActionWindow.ShowDialog();
                    }
                    else
                    {
                        //UpdateAction in ActionList of any old action before inserting a new one.
                        //Clear any old Action in the LogicPane
                        Action oldAction = GetCurrentAction();
                        if (oldAction != null)
                        {
                            UpdateListAction(oldAction);
                            oldAction.ClearLogicPane(ref logicPane, false);
                        }

                        newAction.FillLogicPane(ref logicPane);
                        AddListAction(newAction);
                        InvalidateActionList();
                        cboActionList.SelectedItem =
                        G.TagWrap(newAction.tagName);
                        actionChanged = true;
                        break;
                    }
                }

                if (newActionWindow.DialogResult == DialogResult.Cancel)
                    cancelClicked = true;
            }

        }

        #endregion

        #region Manage Action List/Dic

        #region Action Selected Changed

        private void cboActionList_SelectionChangeCommitted(object sender, EventArgs e)
        {
            //Save this Action.
            Action currentAction = GetCurrentAction();

            if (currentAction != null)
            {
                UpdateListAction(currentAction);
                currentAction.ClearLogicPane(ref logicPane, false);
            }         

            //Load other action, buttons and events.
            Action loadAction = actionList[G.TagUnwrap(cboActionList.Text)];
            loadAction.FillLogicPane(ref logicPane);
        }

        #endregion

        private void AddListAction(Action action)
        {
            actionList.Add(action.tagName, action);
        }

        private void RenameListAction(string oldTagName, string newTagName)
        {
            Action thisAction = actionList[oldTagName];
            thisAction.tagName = newTagName;
            actionList.Remove(oldTagName);
            actionList.Add(newTagName, thisAction);
            InvalidateActionList();
            
        }

        /// <summary>
        /// Syncronizes the Action button in the ActionList with what
        /// is in the LogicPane
        /// </summary>
        /// <param name="action"></param>
        private void UpdateListAction(Action action)
        {
            actionList[action.tagName] = action;
        }

        private void InvalidateActionList()
        {
            cboActionList.Items.Clear();

            foreach (string tagName in actionList.Keys)
            {
                cboActionList.Items.Add(G.TagWrap(tagName));
            }
        }

        #endregion Manage Action List/Dic

        //Format the Action tree.
        public void UpdateActionView(ref LogicButton button)
        {
            Action getAction = button.GetRootAction();

            if (getAction != null)
            {
                if (button is Action)
                {
                    string oldName = G.TagUnwrap(cboActionList.SelectedItem.ToString());
                    string newName = getAction.tagName;

                    //Rename action in CboActionList
                    if (newName != oldName)
                    {
                        RenameListAction(oldName, newName);
                        InvalidateActionList();
                    }
                }              

                getAction.FormatFamily();
                cboActionList.SelectedItem = G.TagWrap(getAction.tagName);
            }
        }

        public void UpdateActionView()
        {
            LogicButton actionHandle = GetCurrentAction();
            this.UpdateActionView(ref actionHandle);
        }

        #endregion //Insert Logic Button
        
        #region Document Reader

        private void FillFromFile()
        {
            string filePath = G.SelectFillFromFile();
            FormAndFuncs formFunc = GetCurrentFormFunc();
            formFunc.FillFromFile(filePath);       
        }

        private void OpenSampleDocDialog()
        {

            string filePath = G.SelectFillFromFile();

            if (filePath == string.Empty)
                return;

            txtFilePath.Text = filePath;
            rtbRawDoc.Text = FormAndFuncs.ReadDocumentFile(filePath);
            regexChanged = true;
        }

        private void UpdateFlatRegexOnControl()
        {
            if (complexPane.SelectedTab == pageDesign)
                return;

            string currentField = (string)cboFields.SelectedItem;
            string currentRegex = txtRegex.Text;

            if (currentField == string.Empty)
                G.ErrorShow("No field appears selected, so " +
                    "I don't know what field to save to.",
                    "Honestly...");

            FormAndFuncs ff = GetCurrentFormFunc();
            FlatRegexField flatRegex = GetFlatRegexForTagName(ff, currentField);

            if (flatRegex == null)
            {
                G.ErrorShow("Hmm. For some reason the key for " +
               currentField + " does not exist. So we can't " +
               "save it.", "That ain't no more good.");
                return;
            }

            flatRegex.regex = currentRegex;
            flatRegex.option = GetCurrentRegexOption();
        }

        private void SelectRegexFieldName()
        {
            if (!draggingControls)
                if (selectedControls.Count == 1)
                {
                    string selectedName = selectedControls[0].Name;


                    if (cboFields.Items.Contains(selectedName))
                        cboFields.SelectedItem = selectedName;

                }
        }

        private void DisplayFlatRegex(string fieldName)
        {
            if (draggingControls) return;

            FormAndFuncs ff = GetCurrentFormFunc();
            FlatRegexField flatRegex = GetFlatRegexForTagName(ff, fieldName);

            if (flatRegex != null)
            {
                txtRegex.Text = flatRegex.regex;
                SelectRegexOption(flatRegex.option);
            }

        }

        private RegexOptions GetCurrentRegexOption()
        {
            RegexOptions option = RegexOptions.None;

            if (radSingleLine.Checked)
                option = RegexOptions.Singleline;

            return option;
        }

        private void SelectRegexOption(RegexOptions option)
        {
            if (option == RegexOptions.None)
                radNone.Checked = true;
            if (option == RegexOptions.Singleline)
                radSingleLine.Checked = true;
        }

        private void InvalidateRegexList()
        {
            FormAndFuncs currentFF = GetCurrentFormFunc();
            List<string> tagNames = new List<string>();
            GetControlFieldNames(currentFF, ref tagNames);

            tagNames.Sort();

            cboFields.Items.Clear();

            foreach (string tagname in tagNames)
                cboFields.Items.Add(tagname);
        }

        private void GetControlFieldNames(Control rootControl, ref List<string> tagNames)
        {
            if (rootControl == null) return;

            foreach (Control child in rootControl.Controls)
            {
                if (child is DateTimePicker ||
                    child is TextBox ||
                    child is RadioButton ||
                    child is CheckBox ||
                    child is ComboBox)
                {
                    tagNames.Add(child.Name);
                }

                if (child is Panel)
                    GetControlFieldNames(child, ref tagNames);

            }
        }

        private string GetSelectedRegexTagName()
        {
            return cboFields.SelectedItem.ToString();
        }

        private FlatRegexField GetCurrentFlatRegex()
        {
            FormAndFuncs ff = GetCurrentFormFunc();
            string selectedTagName = GetSelectedRegexTagName();

            return GetFlatRegexForTagName(ff, selectedTagName);
        }

        private FlatRegexField GetFlatRegexForTagName(Control rootControl, string tagName)
        {
            if (rootControl == null)
                return null;

            FlatRegexField foundField = null;

            foreach (Control child in rootControl.Controls)
            {
                if (child is DateTimePicker ||
                   child is TextBox ||
                   child is RadioButton ||
                   child is CheckBox ||
                   child is ComboBox)
                {
                    if (child.Name == tagName)
                    {
                        if (child.Tag != null)
                        {
                            ControlData data = (ControlData)child.Tag;
                            foundField = data.FlatRegex;
                            return foundField;
                        }
                        
                        return foundField;
                    }
                }

                if (child is Panel)
                {
                    foundField = GetFlatRegexForTagName(child, tagName);

                    if (foundField != null)
                        return foundField;
                }
                    
            }

            return foundField;
        }

        private void PerformRegexCapture()
        {
            if (complexPane.SelectedTab == pageDesign)
                return;

            FlatRegexField flatRegex = GetCurrentFlatRegex();

            if (flatRegex == null) return;

            #region Check for DocData and Regex
            if (flatRegex.regex == string.Empty)
            {
                txtCapture.Text = "Empty Expression. Nothing to match.";
                RemoveAllHighlighting();
                return;
            }

            if (rtbRawDoc.Text == "")
            {
                txtCapture.Text = "Empty Document. Load a sample file to test against.";
                RemoveAllHighlighting();
                return;
            }
            #endregion

            #region Function Variables
            string captureMsg = string.Empty;
            string capture = string.Empty;
            Regex thisRegex = null;
            try
            {
                thisRegex = new Regex(flatRegex.regex, flatRegex.option);
            }
            catch (Exception ex)
            {
                txtCapture.Text = "Error: " + ex.Message;
                RemoveAllHighlighting();
                return;
            }

            MatchCollection matches = thisRegex.Matches(rtbRawDoc.Text);
            int index = -1;
            int length = -1;
            string nl = Environment.NewLine;
            #endregion Variables.

            RemoveAllHighlighting();

            string regexStr = flatRegex.regex;

            if (matches.Count == 0)
            {
                captureMsg += "No Matches.";
            }
            else if (matches.Count > 0) //Found a match.
            {
                if (matches.Count > 1)
                    captureMsg += matches.Count +
                        " matches were found. " + nl + nl;

                if (!(regexStr.Contains('(') && regexStr.Contains(')'))) //Is there a ()?
                {
                    captureMsg += "A capture area was not specified. " + nl + nl;
                    captureMsg += "To specify a capture area, remember to place a ( ) around the ";
                    captureMsg += "portion of the expression you'd like to capture. " + nl + nl;
                    captureMsg += @"Take for example the expression: Chris(\w+)" + nl;
                    captureMsg += "This expression would capture 1 or more word characters after 'Chris'. ";
                    captureMsg += @"Therefore, the regex 'Chris(\w+)' ran on the text 'Christopher' ";
                    captureMsg += "would capture the letters 'topher'. " + nl + nl;
                    captureMsg += "For more information on Regular Expressions, please check online.";
                    captureMsg += nl + nl;
                }

                if (matches.Count > 0) //If has matches.
                    for (int m = 0; m < matches.Count; m++)
                    {
                        Match thisMatch = matches[m];
                        Capture thisCapture = null;

                        if (m == 0) //Highlight first match.
                        {
                            //Highlight whole match.
                            index = thisMatch.Index;
                            length = thisMatch.Length;
                            HighlightArea(Color.LightGreen, index, length);

                            if (thisMatch.Groups.Count > 1)
                            {
                                thisCapture = thisMatch.Groups[1];
                                string captureValue = thisCapture.Value;

                                index = thisCapture.Index;
                                length = thisCapture.Length;
                                HighlightArea(Color.Green, index, length);
                                
                                captureMsg += "Text captured: ( " + captureValue + " )" + nl + nl;
                            }
                            else
                            {
                                captureMsg += "No text captured.";
                            }

                            //Scroll to primary match.
                            rtbRawDoc.ScrollToCaret();
                        }
                        else //Highlight extra matches.
                        {
                            //Highlight whole match.
                            index = thisMatch.Index;
                            length = thisMatch.Length;
                            HighlightArea(Color.Yellow, index, length);

                            //Highlight capture.
                            if (thisMatch.Groups.Count > 1)
                            {
                                thisCapture = thisMatch.Groups[1];

                                index = thisCapture.Index;
                                length = thisCapture.Length;
                                HighlightArea(Color.Orange, index, length);
                            }
                                
                        }
                    }

                txtCapture.Text = captureMsg;
            }

            txtCapture.Text = captureMsg;
        }

        private void HighlightArea(Color color, int index, int length)
        {
            rtbRawDoc.Select(index, length);
            rtbRawDoc.SelectionBackColor = color;
        }

        private void RemoveAllHighlighting()
        {
            rtbRawDoc.Select(0, rtbRawDoc.Text.Length - 1);
            rtbRawDoc.SelectionBackColor = rtbRawDoc.BackColor;
            rtbRawDoc.Select(0, 0);
        }

        #endregion

        #region Debug

        private void UpdateDebugInfo(Control sender)
        {
            if (!debug)
                return;

            Point p = Cursor.Position;
            Point clientP = this.PointToClient(p);
            lblCursorLoc.Text = "Client Cursor: " + clientP;

            Control overCtrl = GetControlUnderMouse();
            Control dragSurface = GetDragSurface();
            Control legalLanding = GetLegalLanding();


            if (overCtrl != null)
            {
                lblOverCtrl.Text = "Over Ctrl: " + overCtrl.Name;
                lblCtrlLoc.Text = "Control Loc: " + overCtrl.PointToClient(p);
            }
            else
            {
                lblOverCtrl.Text = "Over Ctrl: Null";
                lblCtrlLoc.Text = "Loc: Null";
            }
            
            if (dragSurface != null)
            {
                lblDragSurface.Text = "Drag Surface: " + dragSurface.Name;
                lblDragLoc.Text = "Surface Loc: " + dragSurface.PointToClient(p);
            }
            else
            {
                lblDragSurface.Text = "Drag Surface: Null";
                lblDragLoc.Text = "Loc: Null";
            }

            if (legalLanding != null)
            {
                lblLegalLanding.Text = "Legal Landing: " + legalLanding.Name;
                lblParentLoc.Text = "Landing Loc: " + legalLanding.PointToClient(p);
            }
            else
            {
                lblLegalLanding.Text = "Landing: Null";
                lblParentLoc.Text = "Loc: Null";
            }

            if (sender != null)
                lblSender.Text = "Sender: " + sender.Name;
            else
                lblSender.Text = "Sender: Null";


            lblDragging.Text = "Dragging?: " + draggingControls.ToString();
            lblNewControl.Text = "New Ctrl?: " + newControl.ToString();

            lblUp.Text = "Up: " + keyUpPressed.ToString();
            lblDown.Text = "Down: " + keyDownPressed.ToString();
            lblLeftArrow.Text = "Left: " + keyLeftPressed.ToString();
            lblRightArrow.Text = "Right: " + keyRightPressed.ToString();

           // if (mouseRightIsDown)
                lblMouseRight.Text = "Mouse Right: " + mouseRightIsDown.ToString();
           // else
                //lblMouseRight.Text = "Mouse Right: " + mouseRightIsDown.ToString();

           // if (mouseLeftIsDown)
                lblMouseLeft.Text = "Mouse Left: " + mouseLeftIsDown.ToString();
           // else
              //  lblMouseLeft.Text = "Mouse Left: " + mouseLeftIsDown.ToString();

                    
        }

        private string PosStr(Point Loc)
        {
            return "X: " + Cursor.Position.X + " " +
                "Y: " + Cursor.Position.Y;
                    
        }

        private void PrintLoc(Control ctrl)
        {
            Console.WriteLine(this.PointToClient(ctrl.Location));
        }

        #endregion //Debug

        #region Display Minor Messages

        private void StatusMessage(string displayMessage, bool turnOn, int durationMS)
        {
            statusLabel.Text = displayMessage;
            statusMessageOn = turnOn;
            statusMessageCountdown = durationMS;
            statusLabel.Invalidate();
        }

        #endregion

        #region Undo / Redo

        private void AddUndoPoint(bool newPath = true)
        {
            changesMade = true;
            //Add deep copy of the current FormAndFunc           
            FormAndFuncs ff = GetCurrentFormFunc();

            //If stack is full, clear space for another point.
            if (undoStack.Count >= c_layersOfUndoRedo)
            {
                Stack<FormAndFuncs> tempStack = new Stack<FormAndFuncs>(c_layersOfUndoRedo);

                //Transfer items to temp stack -1;
                for (int i = 0; i < undoStack.Count - 1; i++)
                    tempStack.Push(undoStack.Pop());

                undoStack.Clear();

                //Put items back in, minus the one.
                for (int i = 0; i < tempStack.Count; i++)
                    undoStack.Push(tempStack.Pop());
            }

            if (newPath)
            {
                redoStack.Clear();
                redoToolStripMenuItem.Enabled = false;
            }

            FormAndFuncs addUndoFF = new FormAndFuncs(ff);
            NormalizeScroll(addUndoFF, ff.AutoScrollPosition.X, ff.AutoScrollPosition.Y);
            undoStack.Push(addUndoFF);
                      
            undoToolStripMenuItem.Enabled = true;
        }

        private void AddRedoPoint()
        {
            //If redo stack is full, clear space for another point.
            if (redoStack.Count >= c_layersOfUndoRedo)
            {
                Stack<FormAndFuncs> tempStack = new Stack<FormAndFuncs>(c_layersOfUndoRedo);

                //Transfer items to temp stack -1;
                for (int i = 0; i < redoStack.Count - 1; i++)
                    tempStack.Push(redoStack.Pop());

                redoStack.Clear();

                //Put items back in, minus the one.
                for (int i = 0; i < tempStack.Count; i++)
                    redoStack.Push(tempStack.Pop());
            }

            FormAndFuncs currentFF = GetCurrentFormFunc();
            //Add the current FF onto the Redo stack.
            FormAndFuncs newFF = new FormAndFuncs(currentFF);
            NormalizeScroll(newFF,
                currentFF.AutoScrollPosition.X,
                currentFF.AutoScrollPosition.Y);

            redoStack.Push(newFF);
            redoToolStripMenuItem.Enabled = true;
        }

        private void Undo()
        {
            if (undoStack.Count > 0)
            {
                //Add Redo point.
                AddRedoPoint();             

                FormAndFuncs currentFF = GetCurrentFormFunc();
                FormAndFuncs revertToThisFF = undoStack.Pop();
                bufferPane.Controls.Add(revertToThisFF);
                revertToThisFF.AutoScrollPosition = new Point(
                    -currentFF.AutoScrollPosition.X,
                    -currentFF.AutoScrollPosition.Y);

                InsertFormFunc(ref revertToThisFF, true);
                
                Console.WriteLine("Undo Stack = " + undoStack.Count);
            }


            if (undoStack.Count < 1)
                undoToolStripMenuItem.Enabled = false;
        }

        private void Redo()
        {
            if (redoStack.Count > 0)
            {
                AddUndoPoint(false);
                
                FormAndFuncs currentFF = GetCurrentFormFunc();
                FormAndFuncs revertToThisFF = redoStack.Pop();
                bufferPane.Controls.Add(revertToThisFF);
                revertToThisFF.AutoScrollPosition = new Point(
                    -currentFF.AutoScrollPosition.X,
                    -currentFF.AutoScrollPosition.Y);
                    
                InsertFormFunc(ref revertToThisFF, true);

                Console.WriteLine("Redo Stack = " + undoStack.Count);
            }

            if (redoStack.Count < 1)
                redoToolStripMenuItem.Enabled = false;
        }

        /// <summary>
        /// Adjust the X, Y locations of each control.
        /// </summary>
        /// <param name="ff"></param>
        /// <param name="adjX"></param>
        /// <param name="adjY"></param>
        private void NormalizeScroll(FormAndFuncs ff, int adjX, int adjY)
        {
            foreach (Control ctrl in ff.Controls)
            {
                ctrl.Location = new Point(
                    ctrl.Location.X - adjX,
                    ctrl.Location.Y - adjY);
            }
        }

        #endregion        

        #region Copy Items to Clipboard

        #region Tag Text

        private void CopySelectedTagsToClipboard()
        {
            string selectedTags = string.Empty;
            bool multiple = selectedControls.Count > 1;

            selectedControls.Sort(ControlSort);

            if (selectedControls.Count > 0)
            {
                foreach (Control ctrl in selectedControls)
                    //If Control has a field.
                    if (IsCtrlHasField(ctrl))
                        selectedTags += G.TagWrap(ctrl.Name) +
                            ((multiple) ? " " : ""); // Add a space after each tag if multiple.
            }
            else
                return;

            if (selectedTags != string.Empty)
                Clipboard.SetText(selectedTags);
        }

        private int ControlSort(Control a, Control b)
        {
            return a.TabIndex.CompareTo(b.TabIndex);
        }

        private void CopyDocReaderFieldTagToClipboard()
        {
            string tag = cboFields.SelectedItem.ToString();

            if (tag != string.Empty)
                Clipboard.SetText(G.TagWrap(tag));
        }

        private void CopySelectedActionToClipboard()
        {

            Action currentAction = GetCurrentAction();

            if (currentAction == null)
                return;

            switch (currentAction.actionType)
            {
                case ActionType.None:
                    return;
                case ActionType.Fill:
                    Clipboard.SetText(G.TagWrap(currentAction.tagName));
                    return;
                case ActionType.SfxMod:
                    Clipboard.SetText(currentAction.tagName);
                    return;
                case ActionType.Show:
                    string tag = currentAction.tagName;
                    Clipboard.SetText(
                        G.TagWrap(tag)
                        + " " +
                        G.TagWrap("/" + tag));
                    return;
            }
        }

        #endregion

        #endregion

        #region Page Zooming

        private void Zoom(bool zoomIn)
        {
            FormAndFuncs ff = GetCurrentFormFunc();
            if (ff == null) return;

            SizeF zoom = SizeF.Empty;
            float fontChange = 0.001f;
            float zoomChange = 0.005f;
            
            if (zoomIn)
            {
                zoom = new SizeF(1.0f + zoomChange, 1.0f + zoomChange);
            }
            else
            {
                zoom = new SizeF(1.0f - zoomChange, 1.0f - zoomChange);
                fontChange = -fontChange;
            }

            foreach (Control ctrl in ff.Controls)
            {            
              //  float fontSize = ctrl.Font.Size + fontChange;
               // Font newSize = new Font(ctrl.Font.FontFamily, fontSize);
             //   ctrl.Font = newSize;
                ctrl.Scale(zoom);
            }
        }     

        //private void testToolStripMenuItem_Click(object sender, EventArgs e)
        //{

        //    int xRepeats = 8;
        //    int xZooms = 120;

        //    for (int i = 0; i < xRepeats; i++)
        //    {

        //        for (int j = 0; j < xZooms; j++)
        //        {
        //            btnZoomIn.PerformClick();
        //            System.Threading.Thread.Sleep(1);
        //        }

        //        this.Refresh();
        //        System.Threading.Thread.Sleep(1000);

        //        for (int k = 0; k < xZooms; k++)
        //        {
        //            btnZoomOut.PerformClick();
        //            System.Threading.Thread.Sleep(1);
        //        }

        //    }
        //}

        #endregion

        #region Send FF to Workspace

        private void SendFFToWorkspace(FormAndFuncs ff)
        {
            List<string> conflicts = G.GetTagNameConflicts(
                            ref leftPane,
                            ref rightPane,
                            ff);

            if (conflicts.Count > 0)
            {
                string conflictString = G.GetConflictString(
                    ref conflicts);

                G.ErrorShow(conflictString, "TagName Conflict.");
                return;
            }

            backupFF = new FormAndFuncs(ff);

            if (fromLeft) //Back where it came from.
                leftPane.Controls.Add(backupFF);
            else
                rightPane.Controls.Add(backupFF);

            DisassociateFormFunc();
        }


        #endregion


        #region Debug

        private bool CheckForIllegalPosition(FormAndFuncs ff)
        {
            foreach (Control ctrl in ff.Controls)
            {
                if (ctrl.Tag == null)
                {
                    if (IsAutoLabelName(ctrl.Name))
                        continue;
                   
                    Console.WriteLine("this should not happen.");
                    return true;
                }
                else if (ctrl is RadioButton && ((RadioButton)ctrl).CheckAlign == ContentAlignment.MiddleCenter)
                {
                    Console.WriteLine("this should not happen.");

                    ControlData labelData = (ControlData)ctrl.Tag;

                    return true;
                }
                else
                {
                    ControlData labelData = (ControlData)ctrl.Tag;

                    if (labelData.LabelPos == LabelPosition.None &&
                        ctrl is RadioButton)
                    {
                        Console.WriteLine("this should not happen.");
                        return true;
                    }
                }

            }

            return false;
        }

        private void testToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //PrintPos(viewPos, "5364 Testing Button");
            FormAndFuncs ff = GetCurrentFormFunc();

            //CheckForIllegalPosition(ff);
            PrintProps(ff, "test");
        }

        private void PrintPos(string controlName, string lineNum)
        {
            FormAndFuncs ff = GetCurrentFormFunc();
            foreach (Control ctrl in ff.Controls)
            {
                if (ctrl.Name.Contains(viewPos))
                {
                    ControlData prop = (ControlData)ctrl.Tag;
                    Console.WriteLine(ctrl.Name + " = " + prop.LabelPos.ToString() + " @ " + lineNum);
                }
            }
        }

        private void PrintProps(Control root, string controlName = "test")
        {
            Control ctrl = GetControlByName(null, controlName);

            ControlData labelData = (ControlData)ctrl.Tag;

            Console.WriteLine("Name: " + ctrl.Name);
            Console.WriteLine("Type: " + ctrl.GetType().ToString());

            if (ctrl is Label)
            {
                Label labelType = (Label)ctrl;
                Console.WriteLine("Text Alignment: " + labelType.TextAlign.ToString());
            }

            
        }

        private Control GetControlByName(Control root, string controlName)
        {
            Control toReturn = null;

            if (root == null)
                root = GetCurrentFormFunc();

            foreach (Control ctrl in root.Controls)
            {
                if (ctrl.Name == controlName)
                    return ctrl;

                if (ctrl is Panel)
                    toReturn = GetControlByName(ctrl, controlName);

                if (toReturn != null)
                    return toReturn;
            }

            return toReturn;
        }

        #endregion

    }
}


        
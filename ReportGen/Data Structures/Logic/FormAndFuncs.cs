using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Text.RegularExpressions;

namespace ReportGen
{
    public class FormAndFuncs : TabPage
    {
        #region Variables

        public string myFileName;
        public string lastActionTagSelected;
        public string lastRegexTagSelected;
        public Dictionary<string, Action> actions;      

        #endregion

        #region Consts

        //Visual Editing Consts
        public static string c_labelPrefix = "__lbl";

        #endregion

        #region Constructors

        private void Init()
        {
            this.myFileName = string.Empty;
            this.lastActionTagSelected = string.Empty;
            this.lastRegexTagSelected = string.Empty;
            this.actions = new Dictionary<string, Action>();
            //this.regexes = new Dictionary<string, FlatRegexField>();
            ControlData labelProp = new ControlData();
            this.Text = "New Page";
            this.Name = "newPage";
            labelProp.LabelText = this.Text;
            labelProp.LabelPos = LabelPosition.Top_Left;
            this.Tag = labelProp;
            this.AllowDrop = true;
            this.AutoScroll = true;
            //this.DoubleBuffered = true;
            //this.AutoScrollMinSize = new Size(0, 647);
            //this.AutoScrollMargin = new Size(0, 50);
        }

        public FormAndFuncs()
        {
            Init();
        }

        //Load FormFunc from XML
        public FormAndFuncs(string filePath)
        {
            this.Init();
            this.Deserialize(filePath);
        }

        /// <summary>
        /// Create Form func from TabPage
        /// </summary>
        /// <param name="page"></param>
        /// <param name="actions"></param>
        /// <param name="regexes"></param>
        public FormAndFuncs(
            TabPage page, 
            List<Action> actions)
        {
            Init();

            if (page != null)
            {
                //FlatControl flatFF = this.GetFlatLayout(page);
                this.PopulateVisualLayout(page);
            }

            if (actions != null)
                foreach (Action action in actions)
                    this.actions.Add(action.tagName, action);         
        }

        /// <summary>
        /// Copy Constructor.
        /// </summary>
        /// <param name="copyFF"></param>
        public FormAndFuncs(FormAndFuncs copyFF)
        {
            Init();
            FlatControl flatFF = copyFF.GetFlatLayout(copyFF);
            List<FlatAction> flatActions = copyFF.FlattenActions();

            this.PopulateVisualLayout(copyFF);
            this.PopulateActions(flatActions);
        }

        //Debug
        //private string viewPos = "checkBox1";
        //private void PrintPos(FormAndFuncs ff, string controlName, string lineNum)
        //{
        //    foreach (Control ctrl in ff.Controls)
        //    {
        //        if (ctrl.Name.Contains(viewPos))
        //        {
        //            ControlData prop = (ControlData)ctrl.Tag;
        //            Console.WriteLine(ctrl.Name + " = " + prop.LabelPos.ToString() + " @ " + lineNum);
        //        }
        //    }
        //}

        //private delegate void PrintPosDel(string controlName, string lineNum);



        #endregion

        #region Serialize

        public string Serialize(
            string saveAs, 
            string lastAction = "", 
            string lastRegex = "")
        {
            string errCode = G.c_NoErr;
            this.lastActionTagSelected = lastAction;
            this.lastRegexTagSelected = lastRegex;

            try
            {
                //Must set scroll to empty before saving.
                Point oldLoc = this.AutoScrollPosition;
                oldLoc = new Point(oldLoc.X, oldLoc.Y);
                this.AutoScrollPosition = new Point(0, 0);

                //Get a flatFormFunc object.
                FlatFormAndFuncs flatFormFuncs = this.Flatten();
                flatFormFuncs.lastActionTagSelected = lastAction;
                flatFormFuncs.lastRegexTagSelected = lastRegex;

                Type typeLayout = typeof(FlatFormAndFuncs);
                XmlSerializer formFuncSerializer = new XmlSerializer(typeLayout);
                using (FileStream stream = new FileStream(saveAs, FileMode.Create))
                {
                    formFuncSerializer.Serialize(stream, flatFormFuncs);
                }

                //Restore scrolled position back for user.
                this.AutoScrollPosition = new Point(-oldLoc.X, -oldLoc.Y);

      
            }
            catch (Exception ex)
            {
                errCode = "Couldn't save: " + saveAs + ". ";
                errCode += ex.Message;
            }

            return errCode;
        }

        #endregion //Serialize

        #region Deserialize

        private void Deserialize(string filePath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(FlatFormAndFuncs));
            using (FileStream stream = new FileStream(filePath, FileMode.Open))
            {
                object obj = serializer.Deserialize(stream);
                FlatFormAndFuncs fafFromFile = (FlatFormAndFuncs)obj;

                //Set class Props to recently read FormAndFunction Props.
                this.lastActionTagSelected = fafFromFile.lastActionTagSelected;
                this.lastRegexTagSelected = fafFromFile.lastRegexTagSelected;

                FlatControl flatFF = fafFromFile.flatFF;
                Control ffCtrl = flatFF.RecurseCreateControls(flatFF);
                FormAndFuncs ff = (FormAndFuncs)ffCtrl;

                this.PopulateVisualLayout(ff);
                this.PopulateActions(fafFromFile.actionsList);
                //this.PopulateRegexes(fafFromFile.regexes);
            }

        }

        public void PopulateVisualLayout(FormAndFuncs fromFF)
        {
            PopulateVisualLayout((TabPage)fromFF);
        }

        public void PopulateVisualLayout(TabPage tabPage)
        {
            if (tabPage == null)
                return;

            //Clear any controls in this Form.
            this.Controls.Clear();

            this.Name = tabPage.Name;
            this.Text = tabPage.Text;
            this.Font = tabPage.Font;
            this.Size = tabPage.Size;
            this.BackColor = tabPage.BackColor;
            this.ForeColor = tabPage.ForeColor;

            if (tabPage.Tag == null)
            {
                ControlData pageProp = new ControlData();
                pageProp.LabelText = tabPage.Text;

                this.Tag = pageProp;                
            }
            //Console.WriteLine("The tabpage to add contains: " + tabPage.Controls.Count);

            this.PopulateFromControl(tabPage);

            //SetTabOrder(this, 20);

            this.AddAutoLabels(this);
        }

        private void PopulateFromControl(TabPage copyFromPage)
        {

            if (copyFromPage == null)
                return;

            int added = 0;

            this.Name = copyFromPage.Name;
            this.Text = copyFromPage.Text;
            this.Font = copyFromPage.Font;
            this.Size = copyFromPage.Size;
            //this.AutoScroll = true;

            if (copyFromPage.Tag != null &&
                copyFromPage.Tag is ControlData)
            {
                copyFromPage.Tag = (ControlData)copyFromPage.Tag;
            }
            else
            {
                ControlData newProp = new ControlData();
                newProp.LabelText = copyFromPage.Text;
                copyFromPage.Tag = newProp;
            }            


            //Console.WriteLine("CopyPage has " + copyFromPage.Controls.Count);

            foreach(Control ctrl in copyFromPage.Controls)
            {
                FlatControl flatCtrl = new FlatControl(ctrl);
                Control newCtrl = flatCtrl.RecurseCreateControls(flatCtrl);

                if (newCtrl == null)
                    continue; //Auto Labels are null, and are to be added after population for FF.

           
                if (newCtrl.Tag == null)
                {
                    ControlData newProp = new ControlData();
                    newProp.Font.SetFontData(newCtrl.Font);
                    newProp.Font.BackColor = copyFromPage.BackColor.ToArgb();

                    if (newCtrl is RadioButton ||
                        newCtrl is CheckBox ||
                        newCtrl is Label ||
                        newCtrl is TabPage)
                        newProp.LabelText = newCtrl.Text;
                    
                    newCtrl.Tag = newProp;
                }

                this.Controls.Add(newCtrl);

                if (newCtrl is Panel)
                    newCtrl.BringToFront();
                else
                    newCtrl.SendToBack();

                added++;
            }

            //Console.WriteLine("Added: " + added + " to " + this.Name);
        }

        public void PopulateActions(List<FlatAction> flatActions)
        {
            this.actions.Clear();

            foreach (FlatAction flatAction in flatActions)
            {
                Action newAction = new Action(flatAction, this);
                actions.Add(newAction.tagName, newAction);
            }
        }
        
        //public void PopulateRegexes(List<FlatRegexField> regexes)
        //{
        //    this.regexes.Clear();
        //    foreach (FlatRegexField regex in regexes)
        //    {
        //        this.regexes.Add(regex.fieldName, regex);
        //    }
        //}


        #endregion //Deserialize

        #region Fields

        private bool PanelContainsField()
        {
            bool containsField = false;

            foreach (Control ctrl in this.Controls)
                if (ctrl is Panel)
                {
                    containsField = ContainsField(ctrl);

                    if (containsField)
                        return true;
                }

            return containsField;
        }

        private bool ContainsField(Control panel)
        {
            bool containsField = false;

            foreach (Control ctrl in panel.Controls)
            {
                if (IsFieldType(ctrl))
                    return true;
                else if (ctrl is Panel)
                {
                    containsField = ContainsField(ctrl);

                    if (containsField)
                        return true;
                }                
            }

            return containsField;
        }


        /// <summary>
        /// Sets Tab rrders High is X, Y is low.
        /// </summary>
        public int SetTabOrder(Control rootControl, int orderFrom)
        {
            //Create List to count this.controls
            List<PointNamePair> tabOrderList = new List<PointNamePair>();

            //Fill the list with points and names.
            PopulateTabOrderList(this, rootControl, ref tabOrderList);

            tabOrderList.Sort(new Comparison<PointNamePair>(PointNamePair.ComparePositions));

            PointNamePair.AssignIndexes(ref tabOrderList, orderFrom);

            Dictionary<string, int> nameOrderDic = new Dictionary<string, int>();

            foreach (PointNamePair item in tabOrderList)
                nameOrderDic.Add(item.name, item.tabIndex);

            //Assign the calculated tab order to the controls of rootControl
            orderFrom = AssignTabOrder(rootControl, nameOrderDic, orderFrom);

            return orderFrom;
                    
        }

        /// <summary>
        /// Returns the next order to begin using.
        /// </summary>
        /// <param name="parentControl"></param>
        /// <param name="tabOrderList"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        private int AssignTabOrder(Control rootControl, Dictionary<string, int> nameOrderDic, int startFrom)
        {
            foreach (Control child in rootControl.Controls)
            {
                if (child is TextBox ||
                child is ComboBox ||
                child is RadioButton ||
                child is CheckBox ||
                child is DateTimePicker ||
                child is Panel)
                {
                    if (nameOrderDic.ContainsKey(child.Name))
                        child.TabIndex = nameOrderDic[child.Name];
                }

                if (child is Panel)
                    startFrom = AssignTabOrder(child, nameOrderDic, startFrom);
            }


          //  Console.WriteLine(rootControl.Name + " had " + controlsAssigned + " TabOrders set.");
            return startFrom;
        }

        private void PopulateTabOrderList(FormAndFuncs thisFF, Control root, ref List<PointNamePair> tabOrderList)
        {
            foreach (Control ctrl in root.Controls)
            {
                if (ctrl is TextBox ||
                    ctrl is ComboBox ||
                    ctrl is RadioButton ||
                    ctrl is CheckBox ||
                    ctrl is DateTimePicker ||
                    ctrl is Panel)
                {
                    PointNamePair pair = new PointNamePair(thisFF.PointToScreen(ctrl.Location), ctrl.Name);
                    tabOrderList.Add(pair);
                }

                if (ctrl is Panel)
                {
                    PopulateTabOrderList(thisFF, ctrl, ref tabOrderList);
                }
            

            }
        }

        public void ClearFields()
        {
            ClearFields(this);
        }

        public static void ClearFields(Control parentControl)
        {
            foreach (Control ctrl in parentControl.Controls)
            {
                if (ctrl is TextBox)
                    ctrl.Text = "";

                if (ctrl is ComboBox)
                    ctrl.Text = "";

                if (ctrl is CheckBox)
                {
                    CheckBox chkBox = (CheckBox)ctrl;
                    chkBox.Checked = false;
                }

                if (ctrl is RadioButton)
                {
                    RadioButton radbutton = (RadioButton)ctrl;
                    radbutton.Checked = false;
                }

                if (ctrl is DateTimePicker)
                {
                    DateTimePicker picker = (DateTimePicker)ctrl;
                    DateTime defaultDate = new DateTime(DateTime.Today.Year, 1, 1);
                    picker.Value = defaultDate;
                }

                if (ctrl.HasChildren)
                    ClearFields(ctrl);
            }
        }

        public void FillFromFile(string filePath)
        {
            if (filePath != string.Empty)
                try
                {
                    string fileContents = ReadDocumentFile(filePath);
                    ReadFieldsFromFile(ref fileContents, this);
                }
                catch (Exception ex)
                {
                    G.ErrorShow(ex.Message, "Trouble reading file.");
                }                
        }

        private void ReadFieldsFromFile(
            ref string fileContents, Control parentControl)
        {
            FillTabPage(ref fileContents,
                parentControl);
        }

        private bool IsFieldType(Control ctrl)
        {
            return (ctrl is TextBox ||
                ctrl is ComboBox ||
                ctrl is CheckBox ||
                ctrl is RadioButton ||
                ctrl is DateTimePicker);
        }

        #endregion

        #region Auto Label

        #region Add

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
            label.Location = GetLabelPoint(selected, label, prop.LabelPos);

            return label;

        }

        public void AddAutoLabels(Control parent)
        {
            foreach (Control child in parent.Controls)
            {
                AddAutoLabel(ref parent, child);

                if (child is Panel)
                    AddAutoLabels(child);
            }
        }

        #endregion

        #region Removal

        public void ClearAutoLabels()
        {
            Control thisFF = this;
            RemoveAutoLabel(ref thisFF, thisFF);
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
            bool labelRemoved = false;

            if (lblToRemove == null)
                return false;
            else
            {
                //Cycle through every control to make sure all instances
                //Of the auto label's name get removed.
                for (int i = 0; i < landingZone.Controls.Count; i++)
                {
                    //Shares name of label to remove?
                    if (landingZone.Controls[i].Name == lblToRemove.Name)
                    {
                        //Remove it.
                        landingZone.Controls.RemoveAt(i);
                        i--; //Backtrack iteration to prevent going past end.
                        labelRemoved = true;
                    }
                }

                return labelRemoved;
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

        #endregion

        #region Helper

        /// <summary>
        /// Locates a Label Control with the right name. If not found, adds one. Returns it.
        /// </summary>
        /// <param name="landingZone"></param>
        /// <param name="ctrl"></param>
        /// <returns></returns>
        private Label FindCtrlsAutoLabel(ref Control landingZone, Control ctrl)
        {
            if (ctrl is FormAndFuncs ||
                ctrl is CheckBox ||
                ctrl is RadioButton)
                return null;

            string labelName = GetLabelName(ctrl.Name);
            Label foundLabel = null;

            foreach (Control child in landingZone.Controls)
                if (child is Label)
                    if (child.Name == labelName)
                    {
                        foundLabel = (Label)child;
                    }

            if (foundLabel != null)
                return foundLabel;
            else
                return null;
        }

        private ControlData GetLabelPropFromControl(Control ctrl)
        {
            if (ctrl.Tag == null)
                return null;

            ControlData prop = (ControlData)ctrl.Tag;
            return prop;
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

        private string GetLabelName(string parentName)
        {
            return c_labelPrefix + parentName;
        }

        public static bool IsAutoLabelName(string controlName)
        {
            if (controlName.StartsWith(c_labelPrefix))
                return true;
            else
                return false;
        }


        #endregion

        #endregion

        #region Document Reading

        public static string ReadDocumentFile(string filePath)
        {
            string fileContents = string.Empty;
            fileContents = DocReader.ReadFileContents(filePath);

            return fileContents;
        }

        public static void FillTabPage(
            ref string textToRead,
            Control parentControl)
        {
                foreach (Control child in parentControl.Controls)
                {
                    if (child is TextBox ||
                        child is ComboBox ||
                        child is DateTimePicker)
                    {
                        ControlData ctrlData = (ControlData)child.Tag;

                        if (ctrlData.FlatRegex.regex != string.Empty)
                        {
                            Regex regex = new Regex(
                                ctrlData.FlatRegex.regex,
                                ctrlData.FlatRegex.option);

                            Match match = regex.Match(textToRead);
                            string capturedText = G.c_EmptyRead;

                            if (match.Groups.Count > 1)
                                capturedText = match.Groups[1].Value;

                            if (!(child is DateTimePicker))
                            {
                                child.Text = capturedText;
                            }
                            else
                            {
                                //Control is a datetimePicker
                                DateTimePicker picker = (DateTimePicker)child;
                                DateTime value = DateTime.MinValue;
                                capturedText = capturedText.Replace('-', '/');
                                capturedText = capturedText.Replace('\\', '/');
                                if (DateTime.TryParse(capturedText, out value))
                                    picker.Value = value;
                            }
                        }

                    }

                    if (child.HasChildren)
                        FillTabPage(ref textToRead, child);

                }
            
        }

        #endregion

        #region Flatten

        public FlatFormAndFuncs Flatten()
        {
            FlatFormAndFuncs flatFormFuncs = new FlatFormAndFuncs();
            //Flatten Layout.
            flatFormFuncs.flatFF = this.GetFlatLayout(this);

            //Flatten Actions.
            flatFormFuncs.actionsList = this.FlattenActions();

            return flatFormFuncs;
        }

        #region TabControls

        public FlatControl GetFlatLayout(TabPage page)
        {
            FlatControl flatFF = new FlatControl();

            //PrintPos((FormAndFuncs)page, viewPos, "812 FF");
            flatFF = flatFF.RecurseMakeFlatControl(page);
            //PrintPos((FormAndFuncs)page, viewPos, "814 FF");

            return flatFF;
        }

        #endregion

        #region Actions

        private List<FlatAction> FlattenActions()
        {
            List<FlatAction> listOfActions = new List<FlatAction>();

            foreach (string key in this.actions.Keys)
            {
                Action action = this.actions[key];
                FlatAction flatAction = action.Flatten();
                listOfActions.Add(flatAction);
            }

            return listOfActions;
        }

        #endregion

        #region Regexes

        //private List<FlatRegexField> FlattenRegexes()
        //{
        //    List<FlatRegexField> regexList = new List<FlatRegexField>();

        //    foreach (string fieldName in this.regexes.Keys)
        //    {
        //        if (this.regexes[fieldName].regex != string.Empty)
        //        {
        //            FlatRegexField addToList = this.regexes[fieldName];
        //            regexList.Add(addToList);
        //        }
        //    }

        //    return regexList;
        //}

        #endregion

        #endregion

        #region Misc

        public void GetConflicts(
            FormAndFuncs ffToCompare, 
            ref List<string> fieldConflicts, 
            ref List<string> actionConflicts)
        {
            //If this contains tags which appear in ffToCompare, add to conflicts list.

            //Compare my tags.
            List<string> myFields = this.GetFieldNames();
            List<string> compareFields = ffToCompare.GetFieldNames();

            foreach (string field in compareFields)
                if (myFields.Contains(field))
                    fieldConflicts.Add(field);

            //Compare my tagNames
            foreach (string actionName in ffToCompare.actions.Keys)
                if (this.actions.ContainsKey(actionName))
                    actionConflicts.Add(actionName);
        }

        public List<string> GetFieldNames()
        {
            List<string> fieldNames = new List<string>();

            foreach (Control ctrl in this.Controls)
            {
                if (ctrl is TextBox ||
                    ctrl is ComboBox ||
                    ctrl is DateTimePicker ||
                    ctrl is CheckBox ||
                    ctrl is RadioButton)
                {
                    fieldNames.Add(ctrl.Name);
                }
            }

            return fieldNames;
        }

        #endregion

        #region Overrides (Custom Auto Scroll Prevention)

        protected override Point ScrollToControl(Control activeControl)
        {
            Point keepAt = this.DisplayRectangle.Location;

            return new Point(keepAt.X, keepAt.Y);
        }

        #endregion
    }
}



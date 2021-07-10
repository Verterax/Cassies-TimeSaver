using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.Text.RegularExpressions;

namespace ReportGen
{
    [Serializable]
    public class FlatControl : IComparable<FlatControl>
    {

        #region Variables
        //Type of Control
        public string TypeName;

        //Universal Props
        public string Name; //TagName
        public Size Size;
        public Point Location;   
        public FontData Font; 
        public ControlData Label;
        public List<FlatControl> Children;

        //Specific Props
        public string Text;
        public bool MultiLine;
        public bool Checked;
        public List<string> ComboItems;

        private Dictionary<string, string> typeDic;

        #endregion

        #region Consts

        public const string c_FormFunc = "FormFunc";
        public const string c_Label = "Label";
        public const string c_TextBox = "TextBox";
        public const string c_DateTimePicker = "DateTimePicker";
        public const string c_CheckBox = "CheckBox";
        public const string c_ComboBox = "ComboBox";
        public const string c_RadioButton = "RadioButton";
        public const string c_Panel = "Panel";

        #endregion

        #region Constructors

        /// <summary>
        /// Parameterless ctr is required to make
        /// class serializable
        /// </summary>
        public FlatControl()
        {
            Init();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctrl"></param>
        public FlatControl(Control ctrl)
        {
            Init();
            FlatControl flatCtrl = RecurseMakeFlatControl(ctrl);

            if (flatCtrl == null)
                return;

            this.Checked = flatCtrl.Checked;
            this.Children = flatCtrl.Children;
            this.ComboItems = flatCtrl.ComboItems;
            this.Font = flatCtrl.Font;
            this.Label = flatCtrl.Label;
            this.Location = flatCtrl.Location;
            this.MultiLine = flatCtrl.MultiLine;
            this.Name = flatCtrl.Name;
            this.Size = flatCtrl.Size;
            this.Text = flatCtrl.Text;
            this.TypeName = flatCtrl.TypeName;            
        }

        private void Init()
        {
            this.ComboItems = null;
            this.Children = null;
            this.Font = new FontData();
            this.Label = new ControlData();
        }

        #endregion

        #region Return a new Control

        
        public Control RecurseCreateControls(FlatControl rootControl)
        {
            if (typeDic == null)
                typeDic = GetCachedControlTypeNames();

            Control newCtrl = null; //The control to be created and returned.

            if (this.TypeName == typeDic[c_FormFunc])
            {
                newCtrl = this.MakeFormAndFunc();
                FormAndFuncs ff = (FormAndFuncs)newCtrl;
                ff.AutoScroll = true;

                foreach (FlatControl child in rootControl.Children)
                {
                    Control addCtrl = child.RecurseCreateControls(child);

                    if (addCtrl is Label ||
                        addCtrl is CheckBox ||
                        addCtrl is RadioButton)
                    {
                        ControlData checkProp = (ControlData)addCtrl.Tag;
                        if (checkProp != null)
                        {
                            if (checkProp.Font.BackColor == 0)
                                checkProp.Font.BackColor = ff.BackColor.ToArgb();
                        }
                    }

                    ff.Controls.Add(addCtrl);
                }

            }
            else if (this.TypeName == typeDic[c_Label])
            {
                newCtrl = this.MakeLabel();
            }
            else if (this.TypeName == typeDic[c_TextBox])
            {
                newCtrl = this.MakeTextBox();
            }
            else if (this.TypeName == typeDic[c_DateTimePicker])
            {
                newCtrl = this.MakeDateTimePicker();
            }
            else if (this.TypeName == typeDic[c_CheckBox])
            {
                newCtrl = this.MakeCheckBox();
            }
            else if (this.TypeName == typeDic[c_ComboBox])
            {
                newCtrl = this.MakeComboBox();
            }
            else if (this.TypeName == typeDic[c_RadioButton])
            {
                newCtrl = this.MakeRadioButton();
            }
            else if (this.TypeName == typeDic[c_Panel])
            {
                newCtrl = this.MakePanel();
                foreach (FlatControl child in rootControl.Children)
                    newCtrl.Controls.Add(
                        child.RecurseCreateControls(child));
            }

            return newCtrl;
        }

        /// <summary>
        ///  Returns a FormAndFunc object generated from the this prop.
        /// </summary>
        /// <returns></returns>
        public FormAndFuncs MakeFormAndFunc()
        {
            FormAndFuncs newFF = new FormAndFuncs();

            newFF.Tag = this.Label;
            newFF.Name = this.Name;
            newFF.Text = this.Text;
            newFF.ForeColor = Color.FromArgb(this.Font.ForeColor);
            newFF.BackColor = Color.FromArgb(this.Font.BackColor);
            newFF.Font = this.Font.GetFont();
            newFF.AutoScroll = true;
     
            return newFF;
        }

        /// <summary>
        /// Returns a Label object generated from the prop index.
        /// </summary>
        /// <param name="ctrl"></param>
        public Label MakeLabel()
        {
            Label newLabel = new Label();
            newLabel.Tag = this.Label;
            newLabel.Name = this.Name;
            newLabel.Text = this.Text;
            newLabel.ForeColor = Color.FromArgb(Font.ForeColor);
            newLabel.BackColor = Color.FromArgb(Font.BackColor);
            newLabel.TextAlign = ContentAlignment.TopLeft;
            newLabel.AutoSize = true;
            newLabel.Location = this.Location;
            newLabel.Font = this.Font.GetFont();

            return newLabel;
        }

        /// <summary>
        /// Returns a TextBox object generated from the prop index.
        /// </summary>
        /// <param name="ctrl"></param>
        public TextBox MakeTextBox()
        {
            TextBox newTextBox = new TextBox();
            newTextBox.Tag = this.Label;
            newTextBox.Name = this.Name;
            newTextBox.Text = this.Text;
            newTextBox.ForeColor = Color.FromArgb(Font.ForeColor);
            newTextBox.BackColor = Color.FromArgb(Font.BackColor);
            newTextBox.Size = this.Size;
            newTextBox.Location = this.Location;
            newTextBox.Font = this.Font.GetFont();
            newTextBox.Multiline = this.MultiLine;

            if (!this.MultiLine) //center the text.
            {
                newTextBox.TextAlign = HorizontalAlignment.Center;
            }

            return newTextBox;
        }

        /// <summary>
        /// Returns a ComboBox object generated from the prop index.
        /// </summary>
        /// <param name="ctrl"></param>
        public ComboBox MakeComboBox()
        {
            ComboBox newComboBox = new ComboBox();
            newComboBox.Tag = this.Label;
            newComboBox.Name = this.Name;
            newComboBox.Text = this.Text;
            newComboBox.ForeColor = Color.FromArgb(Font.ForeColor);
            newComboBox.BackColor = Color.FromArgb(Font.BackColor);
            newComboBox.Size = this.Size;
            newComboBox.Location = this.Location;
            newComboBox.Font = this.Font.GetFont();

            if (this.ComboItems != null)
                foreach (string item in this.ComboItems)
                    newComboBox.Items.Add(item);

            G.AutoAdjustComboBoxDropDownSize(newComboBox);

            return newComboBox;
        }

        /// <summary>
        /// Returns a DateTimePicker object generated from the prop index.
        /// </summary>
        /// <param name="ctrl"></param>
        public DateTimePicker MakeDateTimePicker()
        {
            DateTimePicker newDateTimePicker = new DateTimePicker();
            newDateTimePicker.Tag = this.Label;
            newDateTimePicker.Name = this.Name;
            newDateTimePicker.Value = DateTime.Parse(this.Text);
            newDateTimePicker.Format = DateTimePickerFormat.Short;
            newDateTimePicker.ForeColor = Color.FromArgb(Font.ForeColor);
            newDateTimePicker.BackColor = Color.FromArgb(Font.BackColor);
            newDateTimePicker.Size = this.Size;
            newDateTimePicker.Location = this.Location;
            newDateTimePicker.Font = this.Font.GetFont();

            return newDateTimePicker;
        }

        /// <summary>
        /// Returns a CheckBox object generated from the prop index.
        /// </summary>
        /// <param name="ctrl"></param>
        public CheckBox MakeCheckBox()
        {
            CheckBox newCheckBox = new CheckBox();
            newCheckBox.Tag = this.Label;
            newCheckBox.Name = this.Name;
            newCheckBox.Text = this.Text;
            newCheckBox.ForeColor = Color.FromArgb(Font.ForeColor);
            newCheckBox.BackColor = Color.FromArgb(Font.BackColor);
            newCheckBox.Size = this.Size;
            newCheckBox.Location = this.Location;
            newCheckBox.Font = this.Font.GetFont();
            newCheckBox.Checked = this.Checked;
            newCheckBox.AutoSize = true;
            newCheckBox.CheckAlign =
                ControlData.GetContentAlignment(this.Label.LabelPos);

            return newCheckBox;
        }

        /// <summary>
        /// Returns a RadioButton object generated from the prop index.
        /// </summary>
        /// <param name="ctrl"></param>
        public RadioButton MakeRadioButton()
        {
            RadioButton newRadioButton = new RadioButton();
            newRadioButton.Tag = this.Label;
            newRadioButton.Name = this.Name;
            newRadioButton.Text = this.Text;
            newRadioButton.ForeColor = Color.FromArgb(Font.ForeColor);
            newRadioButton.BackColor = Color.FromArgb(Font.BackColor);
            newRadioButton.Size = this.Size;
            newRadioButton.Location = this.Location;
            newRadioButton.Font = this.Font.GetFont();
            newRadioButton.Checked = this.Checked;
            newRadioButton.AutoSize = true;
            newRadioButton.CheckAlign =
                 ControlData.GetContentAlignment(this.Label.LabelPos);

            return newRadioButton;
        }

        public Panel MakePanel()
        {
            Panel newPanel = new Panel();
            newPanel.Tag = this.Label;
            newPanel.Name = this.Name;
            newPanel.ForeColor = Color.FromArgb(Font.ForeColor);
            newPanel.BackColor = Color.FromArgb(Font.BackColor);
            newPanel.Size = this.Size;
            newPanel.Location = this.Location;
            newPanel.BorderStyle = BorderStyle.FixedSingle;

            return newPanel;
        }

        #endregion

        #region Get/Set Control Values

        /// <summary>
        /// Method to read control values and assign it to this FlatControl
        /// </summary>
        /// <param name="rootCtrl"></param>
        public FlatControl RecurseMakeFlatControl(Control rootCtrl)
        {
            if (rootCtrl == null)
                return null;

            if (typeDic == null)
                typeDic = GetCachedControlTypeNames();

            if (FormAndFuncs.IsAutoLabelName(rootCtrl.Name))
                return null;


            FlatControl newFlat = new FlatControl();
           // bool newProps = false;

            newFlat.Name = rootCtrl.Name;
            newFlat.Text = rootCtrl.Text;
            newFlat.Font = new FontData(rootCtrl);
            newFlat.Location = rootCtrl.Location;
            newFlat.Size = rootCtrl.Size;

            if (rootCtrl.Tag == null || rootCtrl.Tag is int)
            {
                //newProps = true;
                newFlat.Label = new ControlData();
            }
            else
                newFlat.Label = (ControlData)rootCtrl.Tag;

            if (rootCtrl is FormAndFuncs)
            {
                newFlat.TypeName = typeDic[c_FormFunc];

                if (rootCtrl.HasChildren)
                {
                    newFlat.Children = new List<FlatControl>();

                    foreach (Control child in rootCtrl.Controls)
                    {
                        FlatControl flatChild = RecurseMakeFlatControl(child);

                        if (flatChild != null)
                            newFlat.Children.Add(flatChild);
                    }
                }
            }


            if (rootCtrl is Label)
                newFlat.TypeName = typeDic[c_Label];

            if (rootCtrl is TextBox)
            {
                newFlat.TypeName = typeDic[c_TextBox];
                newFlat.MultiLine = ((TextBox)rootCtrl).Multiline;
            }

            if (rootCtrl is DateTimePicker)
            {
                newFlat.TypeName = typeDic[c_DateTimePicker];
                newFlat.Text = ((DateTimePicker)rootCtrl).Value.ToShortDateString();
            }

            if (rootCtrl is CheckBox)
            {
                newFlat.TypeName = typeDic[c_CheckBox];
                newFlat.Checked = ((CheckBox)rootCtrl).Checked;
            }

            if (rootCtrl is RadioButton)
            {
                newFlat.TypeName = typeDic[c_RadioButton];
                newFlat.Checked = ((RadioButton)rootCtrl).Checked;
            }

            if (rootCtrl is ComboBox)
            {
                char[] trimChar = FlatControl.GetTrimChars();
                newFlat.TypeName = typeDic[c_ComboBox];
                newFlat.ComboItems = new List<string>();
                newFlat.Text = rootCtrl.Text.TrimEnd(trimChar);
                ComboBox box = (ComboBox)rootCtrl;
                foreach (string item in box.Items)
                {
                    string addItem = item.TrimEnd(trimChar);
                    newFlat.ComboItems.Add(addItem);
                }
                             
            }           

            if (rootCtrl is Panel && !(rootCtrl is FormAndFuncs))
            {
                newFlat.TypeName = typeDic[c_Panel];
                Panel panel = (Panel)rootCtrl;
                newFlat.Children = new List<FlatControl>();
                foreach (Control child in rootCtrl.Controls)
                {
                    FlatControl flatChild = RecurseMakeFlatControl(child);

                    if (flatChild != null)
                        newFlat.Children.Add(flatChild);
                }
            }


            if (rootCtrl is RadioButton ||
                rootCtrl is CheckBox ||
                rootCtrl is Label ||
                rootCtrl is FormAndFuncs)
            {
                newFlat.Label.LabelText = rootCtrl.Text;
                newFlat.Label.Font.SetFontData(rootCtrl.Font);
                newFlat.Font.ForeColor = rootCtrl.ForeColor.ToArgb();
            }
          

            if (newFlat.TypeName == "null")
                Console.Write("Error");

            return newFlat;
            
        }

        #endregion

        #region Helper Functions

        public static Dictionary<string, string> GetCachedControlTypeNames()
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();

            dic.Add(c_FormFunc, typeof(FormAndFuncs).FullName);
            dic.Add(c_Label, typeof(Label).FullName);
            dic.Add(c_TextBox, typeof(TextBox).FullName);
            dic.Add(c_DateTimePicker, typeof(DateTimePicker).FullName);
            dic.Add(c_CheckBox, typeof(CheckBox).FullName);
            dic.Add(c_ComboBox, typeof(ComboBox).FullName);
            dic.Add(c_RadioButton, typeof(RadioButton).FullName);
            dic.Add(c_Panel, typeof(Panel).FullName);

            return dic;

        }

        private static char[] GetTrimChars()
        {
            char[] trimChar = new char[2];
            trimChar[0] = '\r';
            trimChar[1] = '\n';

            return trimChar;
        }

        #endregion

        #region Comparaer

        public static int ComparePositions(FlatControl a, FlatControl b)
        {
            return a.CompareTo(b);
        }

        public int CompareTo(FlatControl other)
        {
            
            if (this.Location.Y < other.Location.Y)
                return this.Location.Y.CompareTo(other.Location.Y);
            else
                return this.Location.X.CompareTo(other.Location.X) +
                    this.Location.Y.CompareTo(other.Location.Y);
        }

        #endregion
    }

    [Serializable]
    public class ControlData
    {
        const char c_underscoreReplacement = '-';

        public string LabelText;
        public FontData Font;
        public LabelPosition LabelPos;
        public FlatRegexField FlatRegex;

        public ControlData()
        {
            Init();
        }

        private void Init()
        {
            LabelText = "";
            Font = new FontData();
            LabelPos = LabelPosition.None;
            FlatRegex = new FlatRegexField();
        }

        /// <summary>
        /// Set this control
        /// </summary>
        /// <param name="ctrl"></param>
        public void UpdateToControl(ref Control ctrl)
        {
            if (ctrl is FormAndFuncs)
                ctrl.Text = LabelText;

            if (ctrl is Label)
            {
                Label label = (Label)ctrl;
                label.AutoSize = true;
                label.Text = LabelText;
                label.ForeColor = Color.FromArgb(this.Font.ForeColor);
                label.BackColor = Color.FromArgb(this.Font.BackColor);
                label.Font = Font.GetFont();
            }
           
        }

        public Label GetAutoLabel()
        {
            Control autoLabel = new Label();
            this.UpdateToControl(ref autoLabel);
            return (Label)autoLabel;
        }

        public static string[] GetLocationNames()
        {
            string[] labelNames = Enum.GetNames(typeof(LabelPosition));
            for (int i = 0; i < labelNames.Length; i++)
                labelNames[i] = labelNames[i].Replace('_', c_underscoreReplacement);
            return labelNames;
        }

        public static string EnumToFormattedLabelname(LabelPosition pos)
        {
            string formattedPosName = pos.ToString().Replace('_', c_underscoreReplacement);
            return formattedPosName;
        }

        public static ContentAlignment GetContentAlignment(LabelPosition pos)
        {
            switch (pos)
            {
                case LabelPosition.None:
                    return ContentAlignment.MiddleCenter; // None
                case LabelPosition.Left:
                    return ContentAlignment.MiddleRight;
                case LabelPosition.Right:
                    return ContentAlignment.MiddleLeft;

                case LabelPosition.Top_Left:
                    return ContentAlignment.BottomRight;
                case LabelPosition.Top:
                    return ContentAlignment.BottomCenter;
                case LabelPosition.Top_Right:
                    return ContentAlignment.BottomLeft;

                case LabelPosition.Bottom_Left:
                    return ContentAlignment.TopRight;
                case LabelPosition.Bottom:
                    return ContentAlignment.TopCenter;
                case LabelPosition.Bottom_Right:
                    return ContentAlignment.TopLeft;
                default:
                    return ContentAlignment.MiddleCenter; //None
            }
        }

        public static LabelPosition GetLabelPosition(ContentAlignment alignment)
        {
            switch (alignment)
            {
                case ContentAlignment.MiddleCenter:
                    return LabelPosition.None; // None
                case ContentAlignment.MiddleRight:
                    return LabelPosition.Left;
                case ContentAlignment.MiddleLeft:
                    return LabelPosition.Right;

                case ContentAlignment.TopRight:
                    return LabelPosition.Top_Left;
                case ContentAlignment.TopCenter:
                    return LabelPosition.Top;
                case ContentAlignment.TopLeft:
                    return LabelPosition.Top_Right;

                case ContentAlignment.BottomRight:
                    return LabelPosition.Bottom_Left;
                case ContentAlignment.BottomCenter:
                    return LabelPosition.Bottom;
                case ContentAlignment.BottomLeft:
                    return LabelPosition.Bottom_Right;
                default:
                    return LabelPosition.None; //None
            }
        }

    }

    [Serializable]
    public class FontData
    {
        public string FontName;
        public FontStyle FontStyle;
        public float FontSize;
        public int ForeColor;
        public int BackColor;
        

        public FontData()
        {
            Init();
        }

        public FontData(Control ctrl)
        {
            SetFontData(ctrl.Font);
            this.ForeColor = ctrl.ForeColor.ToArgb();
            this.BackColor = ctrl.BackColor.ToArgb();
        }

        private void Init()
        {
            FontName = G.c_DefaultFont;
            FontStyle = FontStyle.Regular;
            ForeColor = Color.Black.ToArgb();
            BackColor = 0;
            FontSize = 8.5f;
        }

        public Font GetFont()
        {
            //If font not default, and not installed, use default.
            if (FontName != G.c_DefaultFont)
                if (!IsFontInstalled(FontName))
                    FontName = G.c_DefaultFont;

            return new Font(FontName, FontSize, FontStyle);
        }

        //public FontStyleEnum GetFontStyle(System.Drawing.FontStyle style)
        //{
        //    switch (style)
        //    {
        //        case System.Drawing.FontStyle.Regular:
        //            return FontStyleEnum.Regular;
        //        case System.Drawing.FontStyle.Italic:
        //            return FontStyleEnum.Italic;
        //        case System.Drawing.FontStyle.Bold:
        //            return FontStyleEnum.Bold;
        //        case System.Drawing.FontStyle.Underline:
        //            return FontStyleEnum.Underline;
        //        default:
        //            Console.WriteLine("Style for control not found.");
        //            return FontStyleEnum.Regular;
        //    }
        //}

        private bool IsFontInstalled(string fontName)
        {
            using (var testFont = new Font(fontName, 8))
            {
                return 0 == string.Compare(
                  fontName,
                  testFont.Name,
                  StringComparison.InvariantCultureIgnoreCase);
            }
        }

        public void SetFontData(Font font)
        {
            this.FontName = font.FontFamily.Name;
            this.FontStyle = font.Style;
            this.FontSize = font.Size;
        }


    }

    [Serializable]
    public class FlatRegexField
    {
        #region Variables

        public string regex;
        //public string fieldName;
        public RegexOptions option;

        #endregion

        #region Constructors

        public FlatRegexField()
        {
            regex = string.Empty;
            //fieldName = string.Empty;
            option = RegexOptions.None;
        }

        public FlatRegexField(
            string fieldName,
            string regex,
            RegexOptions option = RegexOptions.None)
        {
            //this.fieldName = fieldName;
            this.regex = regex;
            this.option = option;
        }

        #endregion
    }

    [Serializable]
    public enum LabelPosition {None, Left, Right, Top, Bottom, Top_Left, Top_Right,
                                 Bottom_Left, Bottom_Right};

    [Serializable]
    public enum FontStyleEnum { Regular, Italic, Bold, Underline};

    //For Sorting TabOrder
    public enum ControlType { label, checkBox, radioButton, textBoxSL, textBoxML, 
                                comboBox, datePicker, tabPage, panel };
    
    public class PointNamePair : IComparable<PointNamePair>
    {
        public string name;
        public Point location;
        public int tabIndex;

        public PointNamePair()
        {
            Init();
        }

        private void Init()
        {
            name = "";
            location = Point.Empty;
            tabIndex = 0;
        }

        #region CompareTo sort overload

        public static int ComparePositions(PointNamePair a, PointNamePair b)
        {
            return a.CompareTo(b);
        }

        public int CompareTo(PointNamePair other)
        {

            if (this.location.Y < other.location.Y)
                return this.location.Y.CompareTo(other.location.Y);
            else
                return this.location.Y.CompareTo(other.location.Y) +
                    this.location.X.CompareTo(other.location.X);
                
                
        }

        #endregion

        public PointNamePair(Point location, string name, int tabIndex = 0)
        {
            this.location = location;
            this.name = name;
            this.tabIndex = tabIndex;
        }

        public bool ComesBefore(PointNamePair other)
        {
            //First is higher?
            if (this.location.Y < other.location.Y)
                return true;

            if (this.location.Y > other.location.Y)
                return false;
            else
            {
                //Compare X.
                if (this.location.X <= other.location.X)
                    return true;
                else
                    return false;
            }
        }

        public static void AssignIndexes(ref List<PointNamePair> sortedList, int orderFrom)
        {
            int count = sortedList.Count;
            int tabIndex = orderFrom;

            for (int i = 0; i < count; i++)
            {
                sortedList[i].tabIndex = tabIndex;
                tabIndex++;
            }
        }

        public static void SortPairList(ref List<PointNamePair> listToSort)
        {
            bool didSwap = true;

            while (didSwap)
            {
                didSwap = false;

                for (int i = 0; i < listToSort.Count - 1; i++)
                {
                    if (!listToSort[i].ComesBefore(listToSort[i + 1]))
                    {
                        //Swap
                        PointNamePair temp = null;
                        temp = listToSort[i];
                        listToSort[i] = listToSort[i + 1];
                        listToSort[i + 1] = temp;

                        didSwap = true;
                    }
                }
            }


        }
    }



}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Drawing;


namespace ReportGen
{
    public class FormFuncXMLizer
    {
        #region Variables

        private TabControl _layoutPane;
        private TabPage _layoutPage;
        private Dictionary<string, Action> _actionList;
        private FlatControl _flatLayout;
        private List<FlatAction> _flatActions;

        public TabControl LayoutPane
        {
            get { return _layoutPane; }
            set { _layoutPane = value; }
        }
        public TabPage LayoutPage
        {
            get { return _layoutPage; }
            set { _layoutPage = value; }
        }
        public Dictionary<string, Action> ActionList
        {
            get { return _actionList; }
            set { _actionList = value; }
        }
        public FlatControl FlatLayout
        {
            get { return _flatLayout; }
            set { _flatLayout = value; }
        }
        public List<FlatAction> FlatActions
        {
            get { return _flatActions;}
            set { _flatActions = value;}
        }

        public string errString;
        public bool errFlag;

        #endregion

        #region Constructors

        private void InitObject()
        {
            this.LayoutPane = new TabControl();
            this.LayoutPage = new TabPage();
            this.ActionList = new Dictionary<string, Action>();
            this.FlatLayout = new FlatControl();
            this.FlatActions = new List<FlatAction>();
            this.errString = string.Empty;
            this.errFlag = false;
        }

        /// <summary>
        /// Default Constructor
        /// </summary>
        public FormFuncXMLizer()
        {
            this.InitObject();
        }

        /// <summary>
        /// For Quick Saving XML Tabs
        /// </summary>
        /// <param name="ctrl"></param>
        public FormFuncXMLizer(
            ref TabControl layoutPane, 
            ref Dictionary<string, Action> actionList)
        {
            this.InitObject();

            this.LayoutPane.Dispose();
            this.LayoutPane = layoutPane;

            this.ActionList = actionList;
        }

        /// <summary>
        /// For Loading XML Tabs
        /// </summary>
        /// <param name="fileName"></param>
        public FormFuncXMLizer(string fileName)
        {
            this.InitObject();

            try
            {
                this.Deserialize(fileName);
            }
            catch (Exception ex)
            {
                this.errString = ex.Message;
                this.errFlag = true;
            }
        }

        #endregion

        #region Serialize

        public void Serialize(string filePath)
        {
            FlatFormAndFuncs formAndFunc = new FlatFormAndFuncs();

            //Get Props for each control item
            if (LayoutPane != null)
            {
                this.FlatLayout = new FlatControl();
                ReadProps(this.LayoutPane.Controls[0]);
                formAndFunc.flatFF = this.FlatLayout;
            }
            //Get Flat Action List from LogicButton Actions Dictionary.
            if (ActionList != null)
                formAndFunc.actionsList = this.GetFlatActions();         

            try 
            {
                Type typeLayout = typeof(FlatFormAndFuncs);
                XmlSerializer formFuncSerializer = new XmlSerializer(typeLayout);
                using (FileStream stream = new FileStream(filePath, FileMode.Create))
                {
                    formFuncSerializer.Serialize(stream, formAndFunc);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion //Serialize

        #region Deserialize

        public void Deserialize(string filePath)
        {
            if (this.LayoutPane != null)
            {
                try
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(FlatFormAndFuncs));
                    using (FileStream stream = new FileStream(filePath, FileMode.Open))
                    {
                        object obj = serializer.Deserialize(stream);
                        FlatFormAndFuncs fafFromFile = (FlatFormAndFuncs)obj;

                        //Set class Props to recently read FormAndFunction Props.
                        this.FlatLayout = fafFromFile.flatFF;
                        this.FlatActions = fafFromFile.actionsList;                 
                    }
                }
                catch (Exception ex)
                {
                    errFlag = true;
                    errString = ex.Message;
                }
            }
            else
            {
                throw new Exception("Please hande this");
            }
        }

        #endregion //Deserialize

        #region Gather Data from Containers

        private void ReadProps(Control formFunc)
        {
            FlatControl flatFF = new FlatControl(formFunc);
            this.FlatLayout = flatFF;
        }

        private List<FlatAction> GetFlatActions()
        {
            List<FlatAction> listOfActions = new List<FlatAction>();

            foreach (string key in this.ActionList.Keys)
            {
                Action action = this.ActionList[key];
                FlatAction flatAction = action.Flatten();
                listOfActions.Add(flatAction);
            }

            return listOfActions;
        }

        #endregion

        #region Populate Containers

        //public void PopulateActionDictionary(
        //    ref Dictionary<string, Action> actionList)
        //{
        //    actionList.Clear();

        //    foreach(FlatAction flatAction in this.FlatActions)
        //    {
        //        Action newAction = new Action(flatAction);
        //        actionList.Add(newAction.tagName, newAction);
        //    }
        //}

        public void PopulateTabPage(ref TabPage layoutPage)
        {
            //uh oh
            //FlatControl prop0 = null;
            //int propCount = this.FlatLayout.Count;
            //layoutPage.Controls.Clear();

            //if (this.FlatLayout.Count > 0)
            //    prop0 = this.FlatLayout[0];
            //else
            //    throw new Exception("This form is empty.");

            ////Fill in TabPage Properties
            //layoutPage.Name = prop0.Name;
            //layoutPage.Text = prop0.Text;
            //layoutPage.ForeColor = Color.FromArgb(prop0.Font.ForeColor);
            //layoutPage.BackColor = Color.FromArgb(prop0.Font.BackColor);
            //layoutPage.Font = prop0.Font.GetFont();
            //layoutPage.AutoScroll = true;

            ////Get TypeNames in string format.
            //string Label = typeof(Label).FullName;
            //string TextBox = typeof(TextBox).FullName;
            //string DateTimePicker = typeof(DateTimePicker).FullName;
            //string CheckBox = typeof(CheckBox).FullName;
            //string ComboBox = typeof(ComboBox).FullName;
            //string RadioButton = typeof(RadioButton).FullName;
            //string Panel = typeof(Panel).FullName;


            //for (int i = 1; i < propCount; i++)
            //{
            //    string typeName = Type.GetType(this.FlatLayout[i].TypeName).FullName;

            //    if (typeName == Label)
            //    {
            //        Label label = this.FlatLayout[i].MakeLabel();
            //        layoutPage.Controls.Add(label);
            //    }
            //    else if (typeName == TextBox)
            //    {
            //        TextBox textBox = this.FlatLayout[i].MakeTextBox();
            //        layoutPage.Controls.Add(textBox);
            //    }
            //    else if (typeName == DateTimePicker)
            //    {
            //        DateTimePicker dateTimePicker = this.FlatLayout[i].MakeDateTimePicker();
            //        layoutPage.Controls.Add(dateTimePicker);
            //    }
            //    else if (typeName == CheckBox)
            //    {
            //        CheckBox checkBox = this.FlatLayout[i].MakeCheckBox();
            //        layoutPage.Controls.Add(checkBox);
            //    }
            //    else if (typeName == ComboBox)
            //    {
            //        ComboBox comboBox = this.FlatLayout[i].MakeComboBox();
            //        layoutPage.Controls.Add(comboBox);
            //    }
            //    else if (typeName == RadioButton)
            //    {
            //        RadioButton radioButton = this.FlatLayout[i].MakeRadioButton();
            //        layoutPage.Controls.Add(radioButton);
            //    }
            //    else if (typeName == Panel)
            //    {
            //        Panel panel = this.FlatLayout[i].MakePanel();
            //        layoutPage.Controls.Add(panel);
            //    }

            //} //End Prop Iteration
        }


        public void PopulateTabPane(ref TabControl layoutPane)
        {      
            layoutPane.Controls.Clear();
            this.LayoutPage = new TabPage();
            TabPage thisTabPage = this.LayoutPage;

            PopulateTabPage(ref thisTabPage);

            //Add TabPage to TabPane
            layoutPane.Controls.Add(this.LayoutPage);
        }

        #endregion //Populate containers
    }
}

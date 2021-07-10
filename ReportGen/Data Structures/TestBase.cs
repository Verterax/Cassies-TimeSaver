using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;

namespace ReportGen
{
    public class TestBase : Dictionary<string, string>
    {

        #region Universal Test Consts

        public const int c_PreLen = 3;
        public const string c_PageCode = "Page";

        #endregion

        #region Variables

        public bool hasData;
        public string testPrefix;

        #endregion

        #region Test Base Constructors

        public TestBase()
        { //Empty Dictionary
            hasData = false;
        }

        public TestBase(Control control, string prefix)
        {
            if (control != null)
            {
               this.ReadControl(prefix, control);

                //Set testIdentifying name.
                this.testPrefix = prefix;
            }
            else
            {
                //It's just an empty dictionary.
            }

        }    

        #endregion

        #region Read Into Dictionary

        public void ReadControl(string prefix, Control control)
        {
            //Read Form, add each of type...

            foreach (Control item in control.Controls)
            {   //Each fieldName prefixed with 'info'

                //TextBoxes
                #region TextBoxes
                if (item.GetType() == typeof(TextBox))
                {
                    if (item.Name.Contains(prefix))
                    {
                        //Add key,value to hash.
                        string fieldName = item.Name;
                        string fieldText = item.Text;
                        this.Add(fieldName, fieldText);
                    }
                }
                #endregion
                //DateTimePicker
                #region DateTimePicker
                if (item.GetType() == typeof(DateTimePicker))
                {
                    if (item.Name.Contains(prefix))
                    {
                        //Add key,value to hash.
                        string fieldName = item.Name;
                        string fieldText = ((DateTimePicker)item).Value.ToShortDateString();
                        this.Add(fieldName, fieldText);
                    }
                }
                #endregion
                //ComboBox
                #region ComboBox
                if (item.GetType() == typeof(ComboBox))
                {
                    if (item.Name.Contains(prefix))
                    {
                        //Add key,value to hash.
                        string fieldName = item.Name;
                        string fieldText = item.Text;
                        this.Add(fieldName, fieldText);
                    }
                }
                #endregion
                //RadioButton
                #region RadioButton
                if (item.GetType() == typeof(RadioButton))
                {
                    if (item.Name.Contains(prefix))
                    {
                        //Add key,value to hash.
                        string fieldName = item.Name;
                        string fieldText = ((RadioButton)item).Checked ? "1" : "0";
                        this.Add(fieldName, fieldText);
                    }
                }
                #endregion
                //Checkbox
                #region CheckBox
                if (item.GetType() == typeof(CheckBox))
                {
                    if (item.Name.Contains(prefix))
                    {
                        //Add key,value to hash.
                        string fieldName = item.Name;
                        string fieldText = ((CheckBox)item).Checked ? "1" : "0";
                        this.Add(fieldName, fieldText);
                    }
                }
                #endregion
            }
        }

        #endregion

        #region Test Functions

        #endregion

        #region Test Initializations

        #endregion

    }
}

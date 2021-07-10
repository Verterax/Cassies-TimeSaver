using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace ReportGen
{
    public class Fields : Dictionary<string, string>
    {
        #region Variables
        public string anyField;
        #endregion

        #region Consts

        //recognized global values
        public static string c_EmptyField = "Empty Field";
        public static string c_TodaysDate = "today";       
        public static string c_Random = "random";
        public static string c_AnyField = "any";

        //recognized custom action tags.
        public static string c_FileName = "fileName";

        //Document commands
        public static string c_DelTable = "delTable";
        public static string c_DelCol = "delCol";
        public static string c_DelRow = "delRow";
        public static string c_DelPar = "delPar";
        public static string c_Shrink = "shrink";


        #endregion

        #region Constructors / Initialization / Load
        public Fields()
        {
            Init();
        }

        public Fields(List<FormAndFuncs> formFuncs)
        {
            //Populate fields from each field in formFuncs.
            this.PopulateFromFormFuncList(formFuncs);

            //Add Global Fields.
            this.AddGlobalTags();
        }

        /// <summary>
        /// Global TagNames are replaced live by the GetFieldValue Function
        /// </summary>
        private void Init()
        {
        }

        #endregion //Constructor / Load

        #region Populate Fields From...

        /// <summary>
        /// This must be called after the form input fields have been read.
        /// </summary>
        /// <param name="actionList"></param>
        public void PopulateFromActionList(Dictionary<string, Action> actionList)
        {
            Fields theseFields = this;

            foreach (string key in actionList.Keys)
            {
                Action thisAction = actionList[key];
                string result = thisAction.EvalStructure(ref theseFields);

                this.Add(thisAction.tagName, result);
            }
        }

        public void PopulateFromFormFunc(Panel formFunc, bool onlyTextFields = false)
        {

            string errCode = G.c_NoErr;

            //Add Field for each input control.
            foreach (Control control in formFunc.Controls)
            {
                bool addItem = false;
                string text = string.Empty;
                string label = string.Empty;

                //TextBoxes
                if (control is TextBox)
                {
                    text = control.Text;
                    label = control.Name;

                    addItem = true;
                }

                //DateTimePickers
                if (control is DateTimePicker)
                {
                    DateTimePicker date = (DateTimePicker)control;
                    text = date.Value.ToShortDateString();
                    label = date.Name;

                    addItem = true;
                }

                //Combo Box
                if (control is ComboBox)
                {
                    text = control.Text;
                    label = control.Name;

                    addItem = true;
                }

                //CheckBox
                if (control is CheckBox)
                {
                    if (onlyTextFields) continue; //Skip non-text fields.

                    CheckBox checkBox = (CheckBox)control;
                    label = checkBox.Name;
                    if (checkBox.Checked) text = RPNCalc.c_True;
                    else text = RPNCalc.c_False;

                    addItem = true;
                }

                //Radio Button
                if (control is RadioButton)
                {
                    if (onlyTextFields) continue; //Skip non-text fields.

                    RadioButton radButt = (RadioButton)control;
                    label = radButt.Name;
                    if (radButt.Checked) text = RPNCalc.c_True;
                    else text = RPNCalc.c_False;

                    addItem = true;
                }

                if (control is Panel)
                {
                    this.PopulateFromFormFunc((Panel)control, onlyTextFields);
                }


                //Add whatever was found, if anything.
                if (addItem)
                {
                    if (!this.ContainsKey(label))
                    {
                        this.Add(label, text);
                    }
                    else
                        errCode += ", " + label ;
               }   
                    
            }

            if (errCode != G.c_NoErr)
                G.ErrorShow("Duplicate fields names found for: " + errCode, "My mistake.");

        }

        public void PopulateFromPane(TabControl containerPane, bool onlyTextFields = false)
        {
            foreach (Control ctrl in containerPane.Controls)
            {
                if (ctrl is FormAndFuncs)
                {
                    FormAndFuncs formFuncs = (FormAndFuncs)ctrl;
                    PopulateFromFormFunc(formFuncs, true);
                }
            }
        }

        public void PopulateFromFormFuncList(List<FormAndFuncs> formFuncs)
        {
            foreach (FormAndFuncs formFunc in formFuncs)
            {
                PopulateFromFormFunc(formFunc, false);
            }
        }

        #endregion

        #region Get Field Data

        /// <summary>
        /// Also add all global functions here.
        /// </summary>
        /// <param name="label"></param>
        /// <returns></returns>
        public string GetFieldValue(string label)
        {
            string result = string.Empty;

            //Check if we need a global value.
            if (label == c_TodaysDate)
            {
                result = DateTime.Now.ToShortDateString();
                return result;
            }

            else if (label == c_Random)
            {
                Random rand = new Random();
                result = rand.NextDouble().ToString();
                return result;
            }

            else if (label == c_AnyField)
            {
                result = anyField;
                return result;
            }

            //Document Command Tags (return themselves)
            else if (label == c_DelCol)
            {
                return G.TagWrap(c_DelCol);
            }
            else if (label == c_DelRow)
            {
                return G.TagWrap(c_DelRow);
            }
            else if (label == c_DelPar)
            {
                return G.TagWrap(c_DelPar);
            }
            else if (label == c_Shrink)
            {
                return G.TagWrap(c_Shrink);
            }

            //Check if we need a raw field.
            if (this.ContainsKey(label))
            {
                result = this[label];
                return result;
            }

            return c_EmptyField;
        }

        public string ReplaceTags(string expression)
        {
            string tagPattern = @"<(.*?)>";
            char[] trimChar = new char[2]{' ', '('};
            string replaceWith = string.Empty;
            Regex findTags = new Regex(tagPattern);
            Dictionary<string, string> cmdTags = GetAllCommandTags();
            MatchCollection matches = findTags.Matches(expression);


            foreach (Match match in matches)
            {
                string textFound = match.Value;
                textFound = textFound.Substring(1, textFound.Length - 2);

                //Break if a Command tag is found to prevent and endless loop.
                if (cmdTags.Keys.Contains(textFound)) continue;

                //Break if Tag begins with number or (, as it could be the area between a LT and a GT.
                string LTGTCheck = textFound.TrimStart(trimChar);
                if (G.IsNum(LTGTCheck[0])) 
                    continue;

                replaceWith = GetFieldValue(textFound);
                if (replaceWith == G.c_ErrCode)
                    replaceWith = Environment.NewLine + "Field not found: ( " + textFound + " )";
                
                //Replace numbers without double quotes.
                double number = 0;
                if (double.TryParse(replaceWith, out number))
                    expression = expression.Replace(RPNCalc.c_TagO + textFound + RPNCalc.c_TagClo,
                    replaceWith);
                else if (replaceWith == RPNCalc.c_True ||
                    replaceWith == RPNCalc.c_False)
                {
                    //Straight replace literal truth values.
                    expression = expression.Replace(RPNCalc.c_TagO + textFound + RPNCalc.c_TagClo, 
                        replaceWith);
                }
                else//Replace literals with double quotes.
                    expression = expression.Replace(RPNCalc.c_TagO + textFound + RPNCalc.c_TagClo,
                        RPNCalc.c_QuoteDbl + replaceWith + RPNCalc.c_QuoteDbl);
            }

            return expression;
        }

        /// <summary>
        /// Add all Global Tags here.
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GetAllGlobalTags()
        {
            Dictionary<string, string> globalTagList = new Dictionary<string, string>();
            globalTagList.Add(c_TodaysDate, "TODAY'S DATE");
            globalTagList.Add(c_Random, "From 0.0 = 1.0");
            globalTagList.Add(c_AnyField, "Field Wildcard");

            return globalTagList;
        }

        /// <summary>
        /// Add all Command Tags here.
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GetAllCommandTags()
        {
            Dictionary<string, string> commandTagList = new Dictionary<string, string>();
            commandTagList.Add(c_DelTable, "Delete Table");          
            commandTagList.Add(c_DelCol, "Delete Column");
            commandTagList.Add(c_DelRow, "Delete Row");
            commandTagList.Add(c_DelPar, "Delete Paragraph");
            commandTagList.Add(c_Shrink, "Shrink Row");

            return commandTagList;
        }

        public void AddGlobalTags()
        {
            Dictionary<string, string> globalTagList = GetAllGlobalTags();

            foreach (string key in globalTagList.Keys)
                this.Add(key, globalTagList[key]);
        }

        #endregion


    }
}

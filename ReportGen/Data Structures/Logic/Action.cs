using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace ReportGen
{
    [Serializable]
    public enum ActionType { None, Fill, Show, SfxMod };

    [Serializable]
    public enum ActionPreset { FileName, AutoShow}

    [Serializable]
    public class Action : LogicBranch
    {
        #region Consts

        //Action Types
        public const string c_type = "Action";
        public const string c_Fill = "Fill";
        public const string c_Show = "Show";


        //Action Preset Built-in Tags
        public const string c_FileName = "FileName";

        #endregion
        //Action properties
        #region Variables/Properties
        public ActionType actionType;
        public string tagName;
        public Panel logicPaneRef;

        protected FormAndFuncs currentFF;
        #endregion

        #region Constructors

        public Action(ref Panel logicPaneRef, ref FormAndFuncs currentFF)
            : base("", c_type)
        {
            this.actionType = ActionType.None;
            this.BackColor = c_ActionColor;
            this.logicPaneRef = logicPaneRef;
            this.currentFF = currentFF;
        }

        public Action(ActionType actionType, string tagName, string label)
            : base(label, c_type)
        {
            this.BackColor = c_ActionColor;
            this.actionType = actionType;
            this.tagName = tagName;
            this.label = label;
            this.logicPaneRef = null;
        }

        /// <summary>
        /// Envoke the creation of a pre-defined type of action.
        /// </summary>
        /// <param name="actionPreset"></param>
        /// <param name="tagName"></param>
        /// <param name="label"></param>
        public Action(ActionPreset actionPreset, FormAndFuncs currentFF, string tagName, string label)
            : base(label, c_type)
        {
            this.BackColor = c_ActionColor;
            this.label = label;
            this.logicPaneRef = null;
            LogicButton me = this;
            this.currentFF = currentFF;

            switch (actionPreset)
            {
                case ActionPreset.FileName:
                    this.actionType = ActionType.Fill;
                    this.tagName = c_FileName;
                    this.label = label;
                    this.result = new Expression(ref me, "Filename Out", 
                        G.TagWrap(Fields.c_Random));
                    break;

                case ActionPreset.AutoShow:
                    this.actionType = ActionType.Show;
                    this.tagName = tagName;
                    this.label = label;
                    this.result = new Expression(ref me, "Show", InfixCalc.c_True );
                    break;

                default:
                    this.actionType = ActionType.None;
                    this.tagName = tagName;
                    this.label = label;
                    this.result = new Expression("", "");
                    break;
            }

        }


        //Inflation Constructor
        public Action(FlatAction flatAction, FormAndFuncs parentFF) : base("", c_type)
        {
            this.BackColor = c_ActionColor;
            this.Inflate(flatAction);
            this.currentFF = parentFF;
        }

        #endregion

        #region Utils

        public string GetActionName()
        {
            switch (this.actionType)
            {
                case ActionType.Fill:
                    return c_Fill;
                case ActionType.Show:
                    return c_Show;
                default:
                    return "Unknown";
            }
        }

        public override void InvalidateText()
        {
            if (this.HasLabel())
                this.Text = this.label;
            else
                this.Text =
                      DocGen.c_OpTag
                    + this.tagName
                    + DocGen.c_ClTag;
        }

        public override bool IsRealized()
        {
            if (this != null)
            {
                if (HasLabel())
                    return true;

                if (this.tagName != string.Empty &&
                    this.tagName != "")
                    return true;
            }

            return false;
        }

        public string GetWildcardSuffix()
        {
            string suffix = this.tagName;
            suffix = suffix.Replace("<" + Fields.c_AnyField + ">", "");
            return suffix;
        }

        public static List<string> GetActionNames()
        {
            List<string> actionNamesList = new List<string>();

            string[] names = Enum.GetNames(typeof(ActionType));

            foreach (string name in names)
                actionNamesList.Add(name);

            return actionNamesList;
        }

        #endregion

        #region GetLogicPaneRef

        public override Panel GetLogicPaneRef()
        {
            return this.logicPaneRef;
        }

        #endregion

        #region GetFormAndFuncsRef

        public override FormAndFuncs GetFormAndFunc()
        {
            return this.currentFF;
        }

        #endregion

        #region Formatting

        public override void FormatFamily()
        {
            this.FullSize();
            this.ManageStubs(ref logicPaneRef);
            this.FormatChildren(0);

            if (logicPaneRef != null)
                logicPaneRef.Invalidate();
        }

        /// <summary>
        /// andDispose set to true will dispose all children of the Action object.
        /// </summary>
        /// <param name="andDispose"></param>
        public void ClearLogicPane(ref Panel logicPane, bool andDispose = false)
        {
            string lastActionName = "";

            //Remove button event handlers.
            foreach (Control control in logicPane.Controls)
            {
                if (control is Action)
                {
                    Action action = (Action)control;
                    lastActionName = action.Text;
                    action.RemoveFamily(ref logicPane, andDispose);
                }
            }

            //Verify LogicButtons have been wiped.
            foreach (Control ctrl in logicPane.Controls)
            {
                if (ctrl is LogicButton)
                {
                    Console.WriteLine("Could not clear:");

                    LogicButton button = (LogicButton)ctrl;
                    Console.WriteLine("Action: " + lastActionName);
                    Console.WriteLine("Type: " + button.type);

                    if (button.parent != null)
                        Console.WriteLine("Parent: " + button.parent.Text);

                    Console.WriteLine();
                }
            }

            logicPane.Invalidate();
        }

        public override void RemoveFamily(ref Panel logicPane, bool andDispose = false)
        {
            if (this.ifBranch != null)
                this.ifBranch.RemoveFamily(ref logicPane, andDispose);
            if (this.result != null)
                this.result.RemoveFamily(ref logicPane, andDispose);

            this.MouseUp -= (LogicButton_MouseUp);
            logicPaneRef.Controls.Remove(this);

            if (andDispose)
                this.Dispose();
        }

        //Add self, then fill any children.
        public override void FillLogicPane(ref Panel logicPane)
        {
            //Set this action's logicpane ref.
            if (this.logicPaneRef == null)
                this.logicPaneRef = logicPane;

            //Add self and events.
            logicPane.Controls.Add(this);
            this.AddEvents(ref logicPane);

            if (ifBranch != null)
                this.ifBranch.FillLogicPane(ref logicPane);
            if (result != null)
                this.result.FillLogicPane(ref logicPane);

            this.FormatFamily();
        }

        #endregion //Formatting

        #region Flatten/Inflate for Serialization/Deserialization

        public new FlatAction Flatten()
        {
            FlatAction flatAction = new FlatAction();

            flatAction.Type = this.type;
            flatAction.Label = this.label;
            flatAction.ActionType = (int)this.actionType;
            flatAction.TagName = this.tagName;

            if (this.result != null)
                flatAction.Result = this.result.Flatten();

            if (this.ifBranch != null)
                flatAction.IfBranch = this.ifBranch.Flatten();

            return flatAction;

        }

        public void Inflate(FlatAction flatAction)
        {
            this.type = c_type;
            this.label = flatAction.Label;
            this.tagName = flatAction.TagName;
            this.actionType = (ActionType)flatAction.ActionType;
            this.parent = null; //How sad, actions have no parents.
            LogicButton me = (LogicButton)this;

            if (flatAction.Result != null)
            {
                LogicButton addResult = new Expression(flatAction.Result);
                this.InsertButton(ref addResult);
            }

            if (flatAction.IfBranch != null)
            {
                LogicButton addIf = new If(flatAction.IfBranch);             
                this.InsertButton(ref addIf);
            }

        }

        #endregion //Flatten

        #region Set From Action

        public void SetFromAction(Action fromAction)
        {
            this.actionType = fromAction.actionType;
            this.label = fromAction.label;
            this.tagName = fromAction.tagName;
            this.result = fromAction.result;
            this.ifBranch = fromAction.ifBranch;
        }

        #endregion

    }
}

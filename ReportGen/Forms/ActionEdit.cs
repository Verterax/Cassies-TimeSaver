using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ReportGen
{
    public partial class ActionEdit : Form
    {
        //Const
        public const string c_tagNameDefaultText = "NewTagName";
        public const string c_labelDefaultText = "";

        //Variable.
        public Action thisAction;
        public Fields fields;
        public string ffLabel;
        public FormAndFuncs currentFF;

        private void AutoGoto()
        {
            btnOkay.PerformClick();
        }


        #region Constructor/Load
        public ActionEdit(
            ref Action thisAction,
            ref Fields fields)
        {
            //Set a reference to any passed Action.
            this.thisAction = thisAction;
            this.fields = fields;

            Init();                   
        }

        private void Init()
        {
            InitializeComponent();
            AddEvents(); //For Keypress Esc, and Enter.

            List<string> actionNames = Action.GetActionNames();

            //Remove None, it's not an option, it's a null.
            actionNames.Remove(ActionType.None.ToString());
            //Add Action names to combo list.
            foreach (string name in actionNames)
                cboActionType.Items.Add(name);

            //We have a reference.
            //Set Action Edit form to reflect the Logic Button.
            if (this.thisAction.actionType != ActionType.None)
            {
                UpdateActionInfo();
            }
            else
            {
                cboActionType.SelectedItem = ActionType.Fill.ToString();
                txtTagName.Text = c_tagNameDefaultText;
                txtLabel.Text = c_labelDefaultText;
            }

            if (!IsNewAction()) //Only show Menu strip for new Actions
                menuStrip1.Visible = false;
                
        }


        private void ActionEdit_Load(object sender, EventArgs e)
        {
            //AutoGoto();
        }
        #endregion //Constructor/Load

        #region ButtonClicks

        private void btnOkay_Click(object sender, EventArgs e)
        {

            string tagName = txtTagName.Text;
            string label = txtLabel.Text;
            ActionType actionType = ActionType.None;

            //Set action type.
            bool castAction = Enum.TryParse(
                cboActionType.SelectedItem.ToString(), 
                out actionType);

            if (!castAction)
                G.MessageShow("Unable to cast " + cboActionType.SelectedItem.ToString() +
                    " to it's enumeration.", "That ain't no more good.");

            if (fields != null)
            {
                Dictionary<string, string> globalTags = fields.GetAllGlobalTags();

                if (fields.ContainsKey(tagName))
                {
                    G.MessageShow("A field named " + tagName +
                        " already exists on this page.\n\n" +
                        "Please choose a name besides: " + tagName,
                        "Tag Name Taken.");
                    return;
                }
                else if (globalTags.ContainsKey(tagName))
                {
                    G.MessageShow("The Tag: " + tagName + " is reserved as a " +
                        "'global' Tag value, and cannot be used.\n\n" +
                        "Please choose a name besides: " + tagName,
                    "Tag Name Reserved.");
                    return;
                }
            }
            
            thisAction.label = label;
            thisAction.tagName = tagName;

            if (castAction) //Assign action type if no error.
                thisAction.actionType = actionType;

            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            thisAction = null;
            this.Close();
        }

        #endregion

        #region Add KeyPress Events

        private void AddEvents()
        {
            AddKeyPressEvents(this);

            foreach (Control ctrl in this.Controls)
            {
                AddKeyPressEvents(ctrl);
            }
        }

        private void AddKeyPressEvents(object sender)
        {
            if (sender is Control)
            {
                Control control = (Control)sender;
                control.KeyUp += new KeyEventHandler(Enter_Esc_Press);
            }

            if (sender is Form)
            {
                Form form = (Form)sender;
                form.KeyUp += new KeyEventHandler(Enter_Esc_Press);
            }
        }

        private void Enter_Esc_Press(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
                btnOkay.PerformClick();
            else if (e.KeyData == Keys.Escape)
                btnCancel.PerformClick();
        }

        #endregion //Keypress events.

        #region Events

        private void cboActionType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboActionType.SelectedItem.ToString() ==
                ActionType.SfxMod.ToString())
            {
                SetPrefixModTag();
                lblLeft.Location = new Point(txtTagName.Location.X - lblLeft.Width, 
                    txtTagName.Location.Y);

                lblTagLabel.Text = "This Suffix Modifier's Name:";
            }
            else
            {
                SetStandardTag();
                lblLeft.Location = new Point(txtTagName.Location.X - 27, txtTagName.Location.Y);

                lblTagLabel.Text = "This Action's Tag Name:";
            }

        }

        #endregion

        #region Set ActionType Layout

        private void SetStandardTag()
        {
            lblLeft.Text = "<";
            lblRight.Text = ">";
        }

        private void SetPrefixModTag()
        {
            lblLeft.Text = "<anyTag";
            lblRight.Text = ">";
        }

        #endregion

        #region Update Action Info

        private void UpdateActionInfo()
        {
            ActionType actionType = thisAction.actionType;

            cboActionType.SelectedItem = actionType.ToString();
            if (thisAction.tagName != "") txtTagName.Text = thisAction.tagName;
            if (thisAction.label != "") txtLabel.Text = thisAction.label;
        }

        #endregion

        #region Add Action Presets

        private bool IsNewAction()
        {
            return (txtLabel.Text == c_labelDefaultText &&
                txtTagName.Text == c_tagNameDefaultText);
        }

        private void customFileNameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (IsNewAction())
            {
                thisAction.SetFromAction(new Action(ActionPreset.FileName, currentFF, "", ""));
                UpdateActionInfo();
            }
        }

        private void autoShowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (IsNewAction())
            {
                string newTag = this.ffLabel.Replace(" ", ""); //AutoShow Label is ffLabel sans spaces.
                thisAction.SetFromAction(new Action(ActionPreset.AutoShow, currentFF, newTag, ""));
                UpdateActionInfo();
            }
        }

        #endregion

        




    }
}

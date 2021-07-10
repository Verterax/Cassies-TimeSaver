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
    public partial class ExprsEdit : Form
    {
        #region Variables
        public bool exprsChanged;
        public Expression refResult;
        public If refIf;
        public Fields fields;

        private Expression tempExprs;
        #endregion

        private void AutoGoto()
        {
            if (refResult != null)
                if (refResult.parent != null)
                {
                    txtLabel.Text = refResult.parent.Text + "'s child";
                }
                else //Assume it's an Else If Exprs with no parent.
                {
                    txtLabel.Text = "ElseIf";
                }
                else if (refIf != null)
                    txtLabel.Text = refIf.parent.Text + "'s child";

            txtExpression.Text = "1 == 1";

            btnOkay.PerformClick();
        }

        #region Constructors/Load

        public ExprsEdit(ref Expression oldExprs)
        {
            //Load the fields of the parent form as hash/dic
            this.fields = oldExprs.GetFields();
            this.refResult = oldExprs;
            Initialize();
        }

        public ExprsEdit(ref Expression oldExprs, ref Fields fields)
        {
            //Load the fields of the parent form as hash/dic
            this.fields = fields;
            this.refResult = oldExprs;
            Initialize();
        }

        public ExprsEdit(ref If oldIf)
        {
            //Load the fields of the parent form as hash/dic
            this.fields = oldIf.GetFields();
            this.refIf = oldIf;
            Initialize();
        }

        private void Initialize()
        {
            InitializeComponent();
            tempExprs = new Expression(txtLabel.Text, txtExpression.Text);
            exprsChanged = true;
            ticker.Start();
            PopulateTags();
        }

        private void ExprsEdit_Load(object sender, EventArgs e)
        {
            if (refResult != null)
            {
                txtExpression.Text = this.refResult.exprs;
                txtLabel.Text = this.refResult.label;
                lblEditType.Text = "Output: ";
            }
            else if (refIf != null)
            {
                txtExpression.Text = refIf.IfExprs.exprs;

                if (this.refIf.label == string.Empty)
                    this.refIf.label = "If";

                txtLabel.Text = this.refIf.label;
                lblEditType.Text = this.refIf.type;
            }

            
            ExprsEvaluate();
            txtExpression.Focus();
            //AutoGoto();
        }

        private void PopulateTags()
        {
            List<string> tagList = new List<string>();

            foreach (string tagName in fields.Keys)
            {
                tagList.Add(tagName);
            }

            tagList.Sort();

            ToolStripMenuItem[] items = new ToolStripMenuItem[tagList.Count];

            for (int i = 0; i < items.Length; i++)
            {
                items[i] = new ToolStripMenuItem(G.TagWrap(tagList[i]));
                items[i].Click += new EventHandler(tagItem_Click);
            }

            tagNameToolStripMenuItem.DropDownItems.AddRange(items);
        }


        #endregion

        #region Button Clicks

        private void btnOkay_Click(object sender, EventArgs e)
        {
            string label = txtLabel.Text;
            string exprs = txtExpression.Text;

            if (refIf != null)
            {
                refIf.IfExprs.exprs = exprs;
                refIf.IfExprs.label = label;
                refIf.label = label;
                

                if (refIf.parent != null)
                    refIf.FormatFamily();
             
            }
            else if (refResult != null)
            {
                refResult.label = label;
                refResult.exprs = exprs;

                if (refResult.parent != null)
                    refResult.FormatFamily();
            }

            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            //Set thisExprs to null, signifying no changes.
            this.refResult = null;
            this.refIf = null;

            this.Close();
        }

        #endregion

        #region Menu Items

        #region Insert

        #region <TagName>

        #region Globals

        private void todaysDateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InsertAtCursor(G.TagWrap(Fields.c_TodaysDate));
        }

        private void randomNumberToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InsertAtCursor(G.TagWrap(Fields.c_Random));
        }

        private void anyFieldNameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InsertAtCursor(G.TagWrap(Fields.c_AnyField));
        }

        #endregion //Globals

        #region Document Commands

        private void deleteRowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InsertAtCursor('"' + G.TagWrap(Fields.c_DelRow) + '"');
        }

        private void deleteColumnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InsertAtCursor('"' + G.TagWrap(Fields.c_DelCol) + '"');
        }

        private void deleteTableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InsertAtCursor('"' + G.TagWrap(Fields.c_DelTable) + '"');
        }

        private void shrinkRowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InsertAtCursor('"' + G.TagWrap(Fields.c_Shrink) + '"');
        }

        private void deleteParagraphToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InsertAtCursor('"' + G.TagWrap(Fields.c_DelPar) + '"');
        }

        #endregion

        #endregion //Tagname

        #region Operator

        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InsertAtCursor(RPNCalc.c_Plus);
        }

        private void subtractToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InsertAtCursor(RPNCalc.c_Minus);
        }

        private void multiplyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InsertAtCursor(RPNCalc.c_Mul);
        }

        private void divideToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InsertAtCursor(RPNCalc.c_Div);
        }

        private void modulusToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InsertAtCursor(RPNCalc.c_Mod);
        }

        private void logicalORToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InsertAtCursor(RPNCalc.c_Or);
        }

        private void logicalANDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InsertAtCursor(RPNCalc.c_And);
        }

        #endregion //Operator

        #region Function Wrap

        private void parenthesesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WrapSelectedText(RPNCalc.c_ParO, RPNCalc.c_ParClo);
        }

        private void absoluteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WrapSelectedText(RPNCalc.c_Abs, RPNCalc.c_ParClo);
        }

        #region Rounding

        private void roundUpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WrapSelectedText(RPNCalc.c_Ceil, RPNCalc.c_ParClo);
        }

        private void roundDownToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WrapSelectedText(RPNCalc.c_Floor, RPNCalc.c_ParClo);
        }


        #endregion

        #region Trig

        private void sineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WrapSelectedText(RPNCalc.c_Sin, RPNCalc.c_ParClo);
        }

        private void cosineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WrapSelectedText(RPNCalc.c_Cos, RPNCalc.c_ParClo);
        }

        private void tangentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WrapSelectedText(RPNCalc.c_Tan, RPNCalc.c_ParClo);
        }

        #region Hyperbolic

        private void sineHyperbolicToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WrapSelectedText(RPNCalc.c_Sinh, RPNCalc.c_ParClo);
        }

        private void cosineHyperbolicToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WrapSelectedText(RPNCalc.c_Cosh, RPNCalc.c_ParClo);
        }

        private void tangentHyperBolicToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WrapSelectedText(RPNCalc.c_Tanh, RPNCalc.c_ParClo);
        }

        #endregion //Hyperbolic

        #region Arc

        private void arcSineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WrapSelectedText(RPNCalc.c_Asin, RPNCalc.c_ParClo);
        }

        private void arcCosineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WrapSelectedText(RPNCalc.c_Acos, RPNCalc.c_ParClo);
        }

        private void arcTangentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WrapSelectedText(RPNCalc.c_Atan, RPNCalc.c_ParClo);
        }

        #endregion //Arc

        #endregion //Trig

        #region Roots

        private void naturalLogarithmToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WrapSelectedText(RPNCalc.c_Log, RPNCalc.c_ParClo);
        }

        private void base10LogarithmToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WrapSelectedText(RPNCalc.c_Log10, RPNCalc.c_ParClo);
        }

        #endregion //Roots

        private void exponentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WrapSelectedText(RPNCalc.c_Exp, RPNCalc.c_ParClo);
        }

        #endregion

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            txtExpression.Text = "";
        }

        #endregion //Insert

        #endregion

        #region Insert/Wrap at Cursor

        private void tagItem_Click(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem)
            {
                ToolStripMenuItem tagItem = (ToolStripMenuItem)sender;
                InsertAtCursor(tagItem.Text);
            }
        }

        private void InsertAtCursor(string text)
        {
            if (!txtExpression.Focused)
                txtExpression.Focus();

            int insertAt = txtExpression.SelectionStart;
            txtExpression.Text = txtExpression.Text.Insert(insertAt, text);

            SetCursorLoc(insertAt + text.Length);
        }

        private void WrapSelectedText(string start, string end)
        {
            if (!txtExpression.Focused)
                txtExpression.Focus();

            int insertStart = txtExpression.SelectionStart;
            int insertEnd = txtExpression.SelectionStart + txtExpression.SelectionLength;

            txtExpression.Text = txtExpression.Text.Insert(insertEnd, end);
            txtExpression.Text = txtExpression.Text.Insert(insertStart, start);

            SetCursorLoc(insertEnd + start.Length);    
        }

        private void SetCursorLoc(int loc)
        {
            txtExpression.SelectionStart = loc;
        }

        #endregion

        #region Events

        private void txtExpression_TextChanged(object sender, EventArgs e)
        {
            exprsChanged = true;
        }

        private void ticker_Tick(object sender, EventArgs e)
        {
            if (exprsChanged == true)
            {
                ExprsEvaluate();
                exprsChanged = false;
            }

        }

        #endregion

        #region Expression Parsing

        private void ExprsEvaluate()
        {
           tempExprs.exprs = txtExpression.Text;

            try
            {
               txtResult.Text = tempExprs.Evaluate(ref this.fields);
            }
            catch (Exception ex)
            {
                txtResult.Text = ex.Message;
            }
        }

        #endregion

        





 
 
    }
}

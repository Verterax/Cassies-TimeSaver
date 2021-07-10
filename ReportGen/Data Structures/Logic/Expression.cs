using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace ReportGen
{
    [Serializable]
    public class Expression : LogicButton
    {
        #region Consts
        public const string c_type = "Expression";
        #endregion

        #region Variables/Properties

        public string exprs;

        private InfixCalc evaluator;

        #endregion

        #region Constructor

        public Expression(ref LogicButton parent, string label = "", string exprs = "")
            : base(label, c_type)
        {
            this.BackColor = c_ExpressionColor;
            this.exprs = exprs;
            this.parent = parent;
        }

        /// <summary>
        /// Blank Constructor, must manually set parent.
        /// </summary>
        /// <param name="label"></param>
        /// <param name="exprs"></param>
        public Expression(string label = "", string exprs = "")
            : base(label, c_type)
        {
            this.BackColor = c_ExpressionColor;
            this.exprs = exprs;
        }

        //Inflation Constructor
        public Expression(FlatExprs flatExprs)
        {
            this.BackColor = c_ExpressionColor;
            this.Inflate(flatExprs);
        }

        #endregion

        #region Utils

        public override void InvalidateText()
        {
            if (this != null)
            {
                if (this.HasLabel())
                    this.Text = this.label;
                else if (this.HasExpression())
                    this.Text = this.exprs;
                else
                    this.Text = "Result";
            }
        }

        public override bool IsRealized()
        {
            return HasExpression();
        }

        public bool HasExpression()
        {
            if (this.exprs == ""
                || this.exprs == string.Empty)
                return false;
            else
                return true;
        }

        public void ForceRealize()
        {
            this.exprs = RPNCalc.c_True;
            this.FormatFamily();
        }

        public override bool HasChildButtons()
        {
            return false;
        }

        #endregion

        #region Formatting

        public override void RemoveFamily(ref Panel logicPane, bool andDispose = false)
        {
            if (andDispose)
            {
                LogicBranch myParent = (LogicBranch)this.parent;
                myParent.result = null;
            }

            this.RemoveEvents(ref logicPane);

            if (logicPane == null)
                return;
            else
            {
                logicPane.Controls.Remove(this);

                if (andDispose)
                    this.Dispose();
            }          
        }

        public override void FillLogicPane(ref Panel logicPane)
        {
            logicPane.Controls.Add(this);
            this.AddEvents(ref logicPane);
        }

         #endregion

        #region Flatten/Inflate

        public FlatExprs Flatten()
        {
            FlatExprs flatExprs = new FlatExprs();

            flatExprs.Type = this.type;
            flatExprs.Label = this.label;
            flatExprs.Exprs = this.exprs;

            return flatExprs;
        }

        public void Inflate(FlatExprs flatExprs)
        {
            this.type = flatExprs.Type;
            this.label = flatExprs.Label;
            this.exprs = flatExprs.Exprs;
        }

        #endregion

        #region Expression Eval/Parse

        public string Evaluate(ref Fields fields)
        {
            string result = string.Empty;

            if (evaluator == null)
                evaluator = new InfixCalc();

            evaluator.Clear();

            //Replace tags with values from Fields.
            string tempExprs = fields.ReplaceTags(this.exprs);

            result = evaluator.Eval(tempExprs);

            result = result.Trim('\"');

            return result;
        }

        #endregion //Eval/Parse
    }

}

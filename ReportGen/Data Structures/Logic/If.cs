using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace ReportGen
{
    [Serializable]
    public class If : LogicBranch
    {
        //Const
        #region Consts
        public const string c_typeIf = "If";
        public const string c_typeElseIf = "Else If";
        public const string c_typeElse = "Else";

        public const int c_shortX = 40;
        #endregion

        //If Properties
        #region Variables/Properties

        //Test each for True. If true, execute down that path.
        public Expression IfExprs;
        public List<If> ElseIfs;
        public LogicBranch Else; // <--- Cannot be given a ThisIfsExprs; just execute.

        #endregion

        #region Constructors

        public If(string ifType, ref LogicButton parent, string label = "")
            : base(label, ifType)
        {
            this.BackColor = c_IfElseColor;
            this.type = ifType;
            this.parent = parent;
            LogicButton thisAsParent = (LogicButton)this;
            this.IfExprs = new Expression(ref thisAsParent);
            this.ElseIfs = new List<If>();
        }

        public If(FlatIf flatIf)
            : base("", "")
        {
            this.BackColor = c_IfElseColor;
            this.Inflate(flatIf);
        }

        //public If(ref LogicButton myParent, FlatBranch flatBranch) :
        //    base("", "")
        //{
        //    this.parent = myParent;
        //    this.BackColor = c_IfElseColor;
        //    this.Inflate(flatBranch);
        //}

        #endregion //Constructors

        #region Utilities

        public override void InvalidateText()
        {
            if (this != null)
            {
                if (this.HasLabel())
                    this.Text = this.label;
                else if (this.IfExprs.HasExpression())
                    this.Text = this.IfExprs.exprs;
                else
                    this.Text = this.type;
            }
        }

        public override bool IsRealized()
        {
            return this.IfExprs.HasExpression();
        }

        #endregion

        #region Add/Remove

        public void AddIfStub(ref Panel logicPane)
        {
            //Add Exp/If stub.
            this.AddExpIfStub(ref logicPane);
            LogicButton myParent = this;
            //Add Else Stub.
            //If this is not null, overwriting would orphan Else children.
            if (this.Else == null) 
            {
                LogicButton elseFinally = new If(c_typeElse, ref myParent);
                this.InsertButton(ref logicPane, ref elseFinally);
                ((If)elseFinally).AddExpIfStub(ref logicPane);
            }
        }

        public void AddElseIfStub(ref Panel logicPane, ref Expression elseIfsExprs)
        {
            LogicButton myParent = this;
            LogicButton elseIf = new If(c_typeElseIf, ref myParent);
            If elseIfAsIf = (If)elseIf;

            //swap parents.
            elseIf.parent = elseIfsExprs.parent;
            elseIfsExprs.parent = elseIf;

            //Set expression for Else if.
            elseIfAsIf.IfExprs = elseIfsExprs;
            elseIfAsIf.label = elseIfsExprs.label;

            //Add new elseIf
            this.InsertButton(ref logicPane, ref elseIf);
            ((If)elseIf).AddExpIfStub(ref logicPane);
        }

        public void AddElseStub(ref Panel logicPane)
        {
            LogicButton myParent = this;
            LogicButton finalElse = new LogicBranch("", If.c_typeElse);
            this.InsertButton(ref logicPane, ref finalElse);
        }

        public override void RemoveFamily(
            ref Panel logicPane, 
            bool andDispose = false)
        {
            LogicBranch myParent = (LogicBranch)this.parent;

            //Remove children
            if (this.ifBranch != null)
                this.ifBranch.RemoveFamily(ref logicPane, andDispose);

            if (this.result != null)
                this.result.RemoveFamily(ref logicPane, andDispose);

            if (this.ElseIfs != null)
                foreach (If elseIf in this.ElseIfs)
                    elseIf.RemoveFamily(ref logicPane, andDispose);

            if (this.Else != null)
                this.Else.RemoveFamily(ref logicPane, andDispose);

            if (andDispose)
                this.IfExprs.Dispose();

            //Deref from parent, unless ElseIf.
            //Handle removing ElseIfs outside of RemoveFamily
            //Due to list iteration conflict.
            if (andDispose)
                if (this.type != If.c_typeElseIf)
                    myParent.ifBranch = null;

            logicPane.Controls.Remove(this);
            this.RemoveEvents(ref logicPane);

            if (andDispose)
                this.Dispose();

        }

        #endregion

        #region Formatting.

        public override void FillLogicPane(ref Panel logicPane)
        {

            //Add self and events.
            logicPane.Controls.Add(this);
            this.AddEvents(ref logicPane);

            if (this.result != null)
                this.result.FillLogicPane(ref logicPane);

            if (this.ifBranch != null)
                this.ifBranch.FillLogicPane(ref logicPane);

            //Fill Else IFs
            if (this.ElseIfs != null)
                foreach (If elseIf in this.ElseIfs)
                    elseIf.FillLogicPane(ref logicPane);

            //Insert Else's
            if (this.Else != null)
                this.Else.FillLogicPane(ref logicPane);
        }

        #endregion

        #region Flatten/Inflate for (De)Serialization

        new public FlatIf Flatten()
        {
            FlatIf flatIf = new FlatIf();

            flatIf.Type = this.type;
            flatIf.Label = this.label;
            
            if (this.IfExprs != null)
                flatIf.IfExprs = this.IfExprs.Flatten();

            if (this.result != null)
                flatIf.Result = this.result.Flatten();

            if (this.ifBranch != null)
                flatIf.IfBranch = this.ifBranch.Flatten();

            if (this.ElseIfs != null)
                foreach (If elseIf in this.ElseIfs)
                {
                    if (flatIf.ElseIfs == null)
                        flatIf.ElseIfs = new List<FlatIf>();

                    FlatIf flatElseIf = elseIf.Flatten();
                    flatIf.ElseIfs.Add(flatElseIf);
                }

            if (this.Else != null)
                flatIf.Else = this.Else.Flatten();

            return flatIf;

        }

        public void Inflate(FlatIf flatIf)
        {
            this.type = flatIf.Type;
            this.label = flatIf.Label;

            if (flatIf.IfExprs != null)
            {
                this.IfExprs = new Expression(flatIf.IfExprs);
                this.IfExprs.parent = this;
            }

            if (flatIf.Result != null)
            {
                LogicButton addResult = new Expression(flatIf.Result);
                this.InsertButton(ref addResult);
            }

            if (flatIf.IfBranch != null)
            {
                LogicButton addIfBranch = new If(flatIf.IfBranch);
                this.InsertButton(ref addIfBranch);
                this.ifBranch.Inflate(flatIf.IfBranch);
            }

            if (flatIf.ElseIfs != null)
                foreach (FlatIf flatElseIf in flatIf.ElseIfs)
                {
                    LogicButton addElseIf = new If(flatElseIf);
                    this.InsertButton(ref addElseIf);
                    If inflateElseIf = (If)addElseIf;
                    inflateElseIf.Inflate(flatElseIf);
                }
            else
                this.ElseIfs = new List<If>();

            if (flatIf.Else != null)
            {
                LogicButton me = this;
                LogicButton addElse = new If(If.c_typeElse, ref me);
                this.InsertButton(ref addElse);
                this.Else.Inflate(flatIf.Else);
            }
        }

        #endregion //Flatten

        #region DrawIfEnclosure

        /// <summary>
        /// When adding PaintIfEnclosure for LogicPane, only add on Elsefinally button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="paintArgs"></param>
        public void PaintIfEnclosure(object sender, PaintEventArgs paintArgs)
        {
            bool doPaint = false;
            Point pt1 = new Point(0, 0);
            Point pt2 = new Point(0, this.Height);

            //Line color and width
            int lineWidth = 16;

            //Logic Pane
            if (sender is Panel)
            {
                if (this.IsRealized())
                {
                    pt1 = this.Location;
                    pt1.X -= lineWidth / 2;
                    pt1.Y += 1;
                    pt2 = this.Else.Location;
                    pt2.Y += this.Height - 1;
                    pt2.X -= lineWidth / 2;
                    doPaint = true;
                }
                else doPaint = false;
            }
                
            if (sender is LogicBranch)
            {
                lineWidth = 7;

                //Only paint if If is realized.
                LogicButton button = (LogicButton)sender;
                if (button.type == If.c_typeIf)
                {
                    pt1.Y += 2;
                    pt2.Y -= 2;

                    if (button.IsRealized())
                        doPaint = true;
                }
                else
                {
                    pt1.Y += 2;
                    pt2.Y -= 2;
                    
                    doPaint = true; //Else is always realized.
                }
            }

            Pen pen = new Pen(LogicButton.c_IfElseColor, lineWidth);

            if (doPaint)
                paintArgs.Graphics.DrawLine(pen, pt1, pt2);        
        }

        #endregion
    }

}

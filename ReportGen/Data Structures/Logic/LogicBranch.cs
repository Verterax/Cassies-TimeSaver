using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace ReportGen
{
    [Serializable]
    public class LogicBranch : LogicButton
    {
        #region Variables

        public If ifBranch;
        public Expression result;

        #endregion

        #region Constructor

        public LogicBranch(string label, string type)
            : base(label, type)
        {
            this.type = type;
            this.label = label;
            this.ifBranch = null;
            this.result = null;
            this.parent = null;
        }

        //public LogicBranch(ref LogicButton myParent, string type, FlatBranch flatBranch)
        //    : base("", type)
        //{
        //    this.parent = myParent;
        //    this.BackColor = c_IfElseColor;
        //    this.Inflate(flatBranch);
        //}
        #endregion

        #region InsertButton

        /// <summary>
        /// Adds the passed button to the logicPane, binds it to it's parent, 
        /// and adds the mouseUp event.
        /// </summary>
        /// <param name="button"></param>
        public void InsertButton(ref Panel logicPane, ref LogicButton button)
        {

            this.InsertButton(ref button);

            if (logicPane == null) return;

            //Add button events.
            logicPane.Controls.Add(button);
            button.AddEvents(ref logicPane);
        }

        /// <summary>
        /// Only inserts into a logicBranch button.
        /// </summary>
        /// <param name="button"></param>
        public void InsertButton(ref LogicButton button)
        {            
            if (button is If && this is If )
            {
                If thisIf = (If)this;
                If addIf = (If)button;

                if (addIf.type == If.c_typeIf)
                    thisIf.ifBranch = addIf;
                else if (addIf.type == If.c_typeElseIf)
                {
                    if (thisIf.ElseIfs == null)
                        thisIf.ElseIfs = new List<If>();

                    thisIf.ElseIfs.Add(addIf);
                }               
                else if (addIf.type == If.c_typeElse)
                    thisIf.Else = addIf;

            }
            else if (button is LogicBranch && this is Action) //Adding If to Action
                this.ifBranch = (If)button;
            else if (button is Expression)
                this.result = (Expression)button;

            //Set Parent
            button.parent = this;
        }

        public void AddExpIfStub(ref Panel logicPane)
        {
            LogicButton thisButton = this;
            LogicButton ifStub = new If(If.c_typeIf, ref thisButton);
            LogicButton exprsStub = new Expression(ref thisButton);

            this.InsertButton(ref logicPane, ref ifStub);
            this.InsertButton(ref logicPane, ref exprsStub);
        }

        public override void FillLogicPane(ref Panel logicPane)
        {
            //Add self and events.
            logicPane.Controls.Add(this);
            this.AddEvents(ref logicPane);

            if (this.result != null)
                this.result.FillLogicPane(ref logicPane);

            if (this.ifBranch != null)
                this.ifBranch.FillLogicPane(ref logicPane);
        }

        #endregion

        #region Utility

        public bool Has2Children()
        {
            if (this.ifBranch != null &&
                this.result != null)
                return true;
            else
                return false;
        }

        public override bool HasChildButtons()
        {
            if (this.ifBranch == null &&
                this.result == null)
                return false;
            else
                return true;
        }

        #endregion

        #region Stub Management

        public void ManageStubs(ref Panel logicPane)
        {
            #region 2 Children
            if (this.Has2Children())
            {
                //Destory unrealized child.
                if (this.result.IsRealized())
                {
                    this.result.DestroyOtherChild(ref logicPane);
                }
                else if (this.ifBranch.IsRealized())
                {
                    this.ifBranch.DestroyOtherChild(ref logicPane);
                }
            }
            #endregion 2 Children

            #region No Children
            if (!this.HasChildButtons())
            {
                //This Button is a...

                #region Action

                if (this is Action)
                {
                    this.AddExpIfStub(ref logicPane);
                }
                #endregion //Action
                #region If
                else if (this is If)
                {
                    If thisIf = (If)this;

                    if (thisIf.type == If.c_typeIf)
                    {
                        if (thisIf.IsRealized())
                            thisIf.AddIfStub(ref logicPane);
                        else //No buttons, not realized.
                            thisIf.AddExpIfStub(ref logicPane);
                    }

                    if (thisIf.type == If.c_typeElseIf)
                    {
                        if (thisIf.IsRealized())
                            thisIf.AddExpIfStub(ref logicPane);
                    }

                    if (thisIf.type == If.c_typeElse)
                    {
                        LogicBranch elseBranch = (LogicBranch)this;
                        elseBranch.AddExpIfStub(ref logicPane);
                    }
                }
                #endregion //If
                #region LogicBranch AKA Else
                else if (this is LogicBranch)
                {
                    LogicBranch elseBranch = (LogicBranch)this;
                    elseBranch.AddExpIfStub(ref logicPane);
                }
                #endregion LogicBranch AKA Else

            }
            #endregion No Children.

            #region Travel down If, ElseIf and Else branches.
            if (this.ifBranch != null && this.ifBranch.IsRealized())
            {
                this.ifBranch.ManageStubs(ref logicPane);

                if (this.ifBranch.ElseIfs != null)
                {
                    foreach (If elseIf in ifBranch.ElseIfs)
                    {
                        elseIf.ManageStubs(ref logicPane);
                    }
                }

                if (this.ifBranch.Else != null)
                {
                    this.ifBranch.Else.ManageStubs(ref logicPane);
                }
            }
            #endregion Travel Down If,ElseIf,Else branches.

        } //End manage stubs.

        #endregion

        #region Format

        public virtual int FormatChildren(int currentY)
        {
            //If 2 short buttons side by side.
            if (this.Has2Children())
            {
                this.FormatSideBySide();
                currentY = this.Location.Y +c_height;
            }
            else if (this.result != null)//Otherwise, place if, or result, as full size.
            {
                this.result.Subdent();
                currentY = this.Location.Y + c_height;
            }
            else if (this.ifBranch != null)
            {
                currentY += this.ifBranch.Subdent();
                //Branch downIf
                currentY = this.ifBranch.FormatChildren(currentY);

                //Added ifNull
                if (this.ifBranch.ElseIfs != null)
                    foreach (If elseIf in this.ifBranch.ElseIfs)
                    {
                        currentY += elseIf.SubNoIndent(currentY);
                        currentY = elseIf.FormatChildren(currentY);
                    }

                if (this.ifBranch.Else != null)
                {
                    currentY += this.ifBranch.Else.SubNoIndent(currentY);
                    currentY = this.ifBranch.Else.FormatChildren(currentY);
                }

                currentY -= c_height;
            }
           

            currentY += c_height;

            //Set Text.
            this.InvalidateText();
            //Return height increase.
            return currentY;
        }

        public int FormatSideBySide()
        {
            Point leftButton;
            Point rightButton;
            int shortWidth = LogicButton.GetShortWidth(c_xyPadding);

            leftButton = this.GetChildLoc(); //Calc sidebySide buttons.
            rightButton = new Point(leftButton.X, leftButton.Y);
            rightButton.X += shortWidth + c_xyPadding;

            this.result.Location = leftButton;
            this.result.HalfSize();
            this.result.InvalidateText();
            this.ifBranch.Location = rightButton;
            this.ifBranch.HalfSize();
            this.ifBranch.InvalidateText();

            return c_height;
        }
     
        //Removes
        public override void RemoveFamily(ref Panel logicPane, bool andDispose = false)
        {

           // if (ifBranch != null)
           //     ifBranch.RemoveFamily(ref logicPane, andDispose);

           // if (result != null)
           //     result.RemoveFamily(ref logicPane, andDispose);

           // //If Disposing, remove reference to this object from parent.
           // //(The final goodbye.)
           // if (andDispose)
           // {
           //     LogicBranch myParent = (LogicBranch)this.parent;
           //     myParent.ifBranch = null;
           // }

           // //Remove events.
           // this.RemoveEvents(ref logicPane);
           // logicPane.Controls.Remove(this);

           //if (andDispose)
           //     this.Dispose();

            throw new Exception("Function not written: LogicBranch.RemoveFamily()");
        }

        public override void InvalidateText()
        {
            if (this.type == If.c_typeElse)
                this.Text = "Else";
            else
                base.InvalidateText();
        }


        #endregion //Format

        #region Flatten/Inflate

        public virtual FlatBranch Flatten()
        {
            FlatBranch flatBranch = new FlatBranch();

            flatBranch.Type = this.type;
            flatBranch.Label = this.label;

            if (this.result != null)
                flatBranch.Result = this.result.Flatten();

            if (this.ifBranch != null)
                flatBranch.IfBranch = this.ifBranch.Flatten();

            return flatBranch;
        }

        public void Inflate(FlatBranch flatBranch)
        {
            this.type = flatBranch.Type;
            this.label = flatBranch.Label;

            if (flatBranch.Result != null)
            {
                LogicButton addResult = new Expression(flatBranch.Result);
                this.InsertButton(ref addResult);
            }

            if (flatBranch.IfBranch != null)
            {
                if (this.ifBranch != null)
                {
                    LogicButton addIfBranch = new If(flatBranch.IfBranch);
                    this.ifBranch.Inflate(flatBranch.IfBranch);
                    this.InsertButton(ref addIfBranch);
                }
            }
        }

        #endregion //Make Flat

        #region Return Result

        public string EvalStructure(ref Fields fields)
        {
            string result = G.c_ErrCode;

            //There's an end exprs.
            if (this.result != null)
            {
                if (this.result.IsRealized())
                {
                    result = this.result.Evaluate(ref fields);
                }
            }
            else if (this.ifBranch != null)
            { //there's an ifBranch.

                //Will we branch? Or return the final result?
                //Get the if Result

                result = this.ifBranch.IfExprs.Evaluate(ref fields);

                if (result == InfixCalc.c_True)
                    result = this.ifBranch.EvalStructure(ref fields);
                else if (result == InfixCalc.c_False)
                {
                    if (this.ifBranch.ElseIfs != null)
                        foreach (If elseIf in this.ifBranch.ElseIfs)
                        {
                            result = elseIf.IfExprs.Evaluate(ref fields);

                            if (result == InfixCalc.c_True)
                            {
                                result = elseIf.EvalStructure(ref fields);
                                return result;
                            }
                        }
                    //Else
                    result = this.ifBranch.Else.EvalStructure(ref fields);
                }
            }
              
            if (result == G.c_ErrCode)
                result = "Evaluation ends at: " + this.Text + Environment.NewLine +
                    "Fill out a Result/If for: " + this.Text;
            
            return result;
        }

        #endregion
    }

}

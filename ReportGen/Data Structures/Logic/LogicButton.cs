using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace ReportGen
{
    [Serializable]
    public class LogicButton : Button
    {
        #region Consts
        public static readonly Color c_ActionColor = Color.FromArgb(255, 102, 0);
        public static readonly Color c_ExpressionColor = Color.FromArgb(0, 192, 0);
        public static readonly Color c_IfElseColor = Color.FromArgb(30, 172, 224);
        public static readonly Color c_ExpOrIfColor = Color.FromArgb(0, 192, 0);
        public const int c_height = 30;
        public const int c_width = 110;
        public const int c_xyPadding = 5;
        public const int c_indentWidth = 55;
        #endregion //Consts

        #region Variables
        public string type;
        public string label;
        public LogicButton parent;
        #endregion //Variables

        #region Constructor

        public LogicButton(string label, string type)
        {
            this.label = label;
            this.type = type;
        }

        public LogicButton()
        {
        }

        #endregion Constructor

        #region Utils

        public bool HasLabel()
        {
            if (this.label == string.Empty ||
                this.label == "")
                return false;
            else
                return true;
        }

        public virtual void InvalidateText()
        {
            G.ErrorShow("Invalidated the Base Logic Button.", "Yerp");
        }

        public virtual bool IsRealized()
        {
            return false;
        }

        public virtual bool HasChildButtons()
        {
            return false;
        }

        public Action GetRootAction()
        {
            if (this is Action)
                return (Action)this;
            else if (this.parent != null)
                return this.parent.GetRootAction();
            else
                return null;
        }

        #endregion //Utils

        #region GetLogicPaneRef

        public virtual Panel GetLogicPaneRef()
        {
            //Console.WriteLine(this.Text + "(" + this.GetHashCode() + ") wants ref from " +
            //    this.parent.Text + "(" + this.parent.GetHashCode() + ")");

            //if (this == this.parent)
            //    throw new Exception(this.Text + " is it's own parent.");

            return this.parent.GetLogicPaneRef();
        }

        public virtual FormAndFuncs GetFormAndFunc()
        {
            return this.parent.GetFormAndFunc();
        }

        #endregion

        #region Formatting

        public void HalfSize()
        {
            this.InvalidateText();
            this.Size = new Size(c_width / 2, c_height);
        }

        public void FullSize()
        {
            this.InvalidateText();
            this.Size = new Size(c_width, c_height);
        }

        public static Point RootLocation()
        {
            Point p = new Point(c_xyPadding, c_xyPadding);

            return p;
        }

        public virtual void FormatFamily()
        {        
            this.parent.FormatFamily();
        }

        public void UpdateFormView(ref Control ctrl)
        {
            if (ctrl is Page_Designer)
            {
                Page_Designer tabDesignerForm = (Page_Designer)ctrl;
                LogicButton thisButton = (LogicButton)this;
                tabDesignerForm.UpdateActionView(ref thisButton);
                tabDesignerForm.actionChanged = true; 

            }
            else
            {
                if (ctrl.Parent != null)
                {
                    Control controlParent = ctrl.Parent;
                    UpdateFormView(ref controlParent);
                }
                else
                {
                    Console.WriteLine("End of the line.");
                }
            }
        }

        public int Subdent()
        {
            if (this.parent != null)
            {
                Point p = this.parent.GetChildLoc();
                this.Location = p;          
            }

            this.FullSize();
            return c_height; //Return current height;
        }

        public int SubNoIndent(int currentY)
        {
            Point p = this.GetPsuedoChildLoc(currentY);
            this.Location = p;
            this.FullSize();

            return c_height;
        }

        public static int GetShortWidth(int padding)
        {
            return c_width  / 2;
        }

        public Point GetChildLoc()
        {
            int pX = 0;
            int pY = 0;
            pX = this.Location.X + c_indentWidth;
            pY = this.Location.Y + c_height;
            Point p = new Point(pX, pY);

            return p;
        }

        public Point GetPsuedoChildLoc(int currentHeight)
        {
            int pX = this.parent.Location.X;
            int pY = 0;
            pY = currentHeight + c_height;
            Point p = new Point(pX, pY);

            return p;
        }

        public virtual void FillLogicPane(ref Panel logicPane)
        {
            throw new Exception("Do not call the base LogicButton class on" +
                " FillLogicPane()");
        }

        #endregion

        #region Remove Buttons from logicPane

        #region Remove

        public virtual void RemoveFamily(
            ref Panel logicPane, 
            bool andDispose = false)
        {
            throw new Exception("Do not call the base LogicButton method of " +
                " RemoveFamily()");
        }

        /// <summary>
        /// Removes the unrealized sibling button.
        /// </summary>
        public void DestroyOtherChild(ref Panel logicPane)
        {
            if (parent != null)
                if (parent is LogicBranch)
                {
                    LogicBranch parentBranch = (LogicBranch)parent;

                    if (this is Expression)
                        parentBranch.ifBranch.RemoveFamily(ref logicPane, true);
                    else if (this is If)
                        parentBranch.result.RemoveFamily(ref logicPane, true);
                }
        }

        #endregion

        #endregion

        #region Mouse Actions

        protected static void LogicButton_MouseUp(object sender, MouseEventArgs e)
        {
            LogicButton button = (LogicButton)sender;

            if (e.Button == MouseButtons.Left)
            {
                LeftClick(ref button);
            }
            else if (e.Button == MouseButtons.Right)
            {
                RightClick(ref button);
            }
        }

        protected static void LeftClick(ref LogicButton button)
        {
            //Hack to lose focus and prevent logicPane AutoScroll
            button.Enabled = false;
            button.Enabled = true;

            if (button is Action)
            {
                Action action = (Action)button;
                Fields fields = button.GetFields();
                ActionEdit actionEditor = new ActionEdit(ref action, ref fields);
                actionEditor.ShowDialog();
            }
            else if (button is Expression)
            {
                Expression oldExprs = (Expression)button;
                Fields fields = button.GetFields();
                ExprsEdit exprsEditor = new ExprsEdit(ref oldExprs, ref fields);
                exprsEditor.ShowDialog();
            }
            else if (button is If)
            {
                If ifExprs = (If)button;

                if (ifExprs.type == If.c_typeElse)
                {
                    If addElseIf = (If)ifExprs.parent;

                    //Get new expression for ElseIf Stub.
                    Expression elseIfExprs = new Expression();
                    Fields fields = button.GetFields();
                    ExprsEdit exprsEditor = new ExprsEdit(ref elseIfExprs, ref fields);
                    exprsEditor.ShowDialog();

                    if (elseIfExprs.IsRealized())
                    {
                        //Set parent(s) for exprs.
                        //LC p, is button's parent, UC P is form parent.
                        elseIfExprs.parent = button.parent;
                        Panel logicPane = button.GetLogicPaneRef();
                        addElseIf.AddElseIfStub(ref logicPane, ref elseIfExprs);
                    }                    
                }
                else
                {
                    ExprsEdit exprsEditor = new ExprsEdit(ref ifExprs);
                    exprsEditor.ShowDialog();
                }  
            }

            Control buttonAsControl = (Control)button;
            button.UpdateFormView(ref buttonAsControl);
        }

        //A hack to retrieve fields of the Designer Layout Tab
        //So they can be passed to the expression editor.
        public Fields GetFields()
        {
            Fields fields = new Fields();
            FormAndFuncs ff = GetFormAndFunc();
            if (ff == null) return fields;

            fields.PopulateFromFormFunc(ff, false);

            return fields;
        }

        protected static void RightClick(ref LogicButton button)
        {
            if (button is Action)
            {
                Action actionPeek = (Action)button;
            }
            if (button is Expression)
            {
                Expression exprsPeek = (Expression)button;
            }
            if (button is If)
            {
                If ifPeek = (If)button;
            }
            if (button is LogicBranch)
            {
                LogicBranch branchPeek = (LogicBranch)button;
            }

            if (button is Action)
            {
                G.MessageShow("Instead of deleting " + button.Text
                + ", create a new Action or Edit this one.",
                    "Try Edit or New");
                return;
            }


            Point p = button.Location;

            if (button.type == If.c_typeElse)
                return;

            LogicButton myParent = (LogicButton)button.parent;

            if (button.IsRealized())
            {
                Panel logicPane = button.GetLogicPaneRef();

                if (button is If)
                {
                    //Special case, remove this ElseIF from list last.
                    //Must be done outside the recursive RemoveFamily and Foreach loops.
                    if (button.type == If.c_typeElseIf)
                    {
                        //Dispose of button.
                        If parentIf = (If)myParent;
                        If thisIf = (If)button;
                        parentIf.ElseIfs.Remove(thisIf);
                        button.RemoveFamily(ref logicPane, true);
                    }
                    else
                        button.RemoveFamily(ref logicPane, true);
                }
                else if (button is Expression)
                {
                    //Dispose of family.
                    button.RemoveFamily(ref logicPane, true);
                }
            }

            myParent.FormatFamily();
            Control myParentAsControl = (Control)myParent;
            myParent.UpdateFormView(ref myParentAsControl);
        }

        #endregion //Mouse Actions

        #region Add/Remove Events

        public void AddEvents(ref Panel logicPane)
        {
            //Mouseclick
            this.MouseUp += new MouseEventHandler(LogicButton.LogicButton_MouseUp);

            //Draw IfEnclose
            if (this.type == If.c_typeIf ||
                this.type == If.c_typeElseIf ||
                this.type == If.c_typeElse)
            {
                If ifButton = (If)this;
                this.Paint += new PaintEventHandler(ifButton.PaintIfEnclosure);

                if (this.type == If.c_typeIf) //Only add Paint event for Parent If Statement.
                    logicPane.Paint += new PaintEventHandler(ifButton.PaintIfEnclosure);
            }

        }

        public void RemoveEvents(ref Panel logicPane)
        {
            //Mouseclick
            this.MouseUp -= (LogicButton_MouseUp);

            //Draw IfEnclose
            if (this.type == If.c_typeIf ||
                this.type == If.c_typeElseIf ||
                this.type == If.c_typeElse)
            {
                If ifButton = (If)this;
                this.Paint -= ifButton.PaintIfEnclosure;

                if (this.type == If.c_typeIf)
                    logicPane.Paint -= ifButton.PaintIfEnclosure;
            }
        }

        #endregion

    }
  
}

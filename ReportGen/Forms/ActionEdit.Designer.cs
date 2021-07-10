namespace ReportGen
{
    partial class ActionEdit
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ActionEdit));
            this.btnOkay = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lblSelectAction = new System.Windows.Forms.Label();
            this.txtTagName = new System.Windows.Forms.TextBox();
            this.txtLabel = new System.Windows.Forms.TextBox();
            this.lblLeft = new System.Windows.Forms.Label();
            this.lblRight = new System.Windows.Forms.Label();
            this.cboActionType = new System.Windows.Forms.ComboBox();
            this.lblTagLabel = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.addActionPresetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.customFileNameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.autoShowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOkay
            // 
            this.btnOkay.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOkay.Location = new System.Drawing.Point(107, 242);
            this.btnOkay.Name = "btnOkay";
            this.btnOkay.Size = new System.Drawing.Size(98, 34);
            this.btnOkay.TabIndex = 0;
            this.btnOkay.Text = "Accept";
            this.btnOkay.UseVisualStyleBackColor = true;
            this.btnOkay.Click += new System.EventHandler(this.btnOkay_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.Location = new System.Drawing.Point(231, 242);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(98, 34);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // lblSelectAction
            // 
            this.lblSelectAction.AutoSize = true;
            this.lblSelectAction.BackColor = System.Drawing.Color.DarkOrange;
            this.lblSelectAction.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSelectAction.Location = new System.Drawing.Point(145, 38);
            this.lblSelectAction.Name = "lblSelectAction";
            this.lblSelectAction.Size = new System.Drawing.Size(146, 29);
            this.lblSelectAction.TabIndex = 2;
            this.lblSelectAction.Text = "Action Type:";
            // 
            // txtTagName
            // 
            this.txtTagName.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtTagName.Location = new System.Drawing.Point(107, 145);
            this.txtTagName.Name = "txtTagName";
            this.txtTagName.Size = new System.Drawing.Size(222, 29);
            this.txtTagName.TabIndex = 5;
            this.txtTagName.Text = "UniqueTagName";
            this.txtTagName.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtLabel
            // 
            this.txtLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtLabel.Location = new System.Drawing.Point(107, 207);
            this.txtLabel.Name = "txtLabel";
            this.txtLabel.Size = new System.Drawing.Size(222, 29);
            this.txtLabel.TabIndex = 6;
            this.txtLabel.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lblLeft
            // 
            this.lblLeft.AutoSize = true;
            this.lblLeft.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLeft.Location = new System.Drawing.Point(80, 145);
            this.lblLeft.Name = "lblLeft";
            this.lblLeft.Size = new System.Drawing.Size(21, 24);
            this.lblLeft.TabIndex = 7;
            this.lblLeft.Text = "<";
            this.lblLeft.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblRight
            // 
            this.lblRight.AutoSize = true;
            this.lblRight.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRight.Location = new System.Drawing.Point(335, 145);
            this.lblRight.Name = "lblRight";
            this.lblRight.Size = new System.Drawing.Size(21, 24);
            this.lblRight.TabIndex = 8;
            this.lblRight.Text = ">";
            // 
            // cboActionType
            // 
            this.cboActionType.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboActionType.FormattingEnabled = true;
            this.cboActionType.Location = new System.Drawing.Point(122, 70);
            this.cboActionType.Name = "cboActionType";
            this.cboActionType.Size = new System.Drawing.Size(188, 32);
            this.cboActionType.TabIndex = 9;
            this.cboActionType.Text = "Select Action Type";
            this.cboActionType.SelectedIndexChanged += new System.EventHandler(this.cboActionType_SelectedIndexChanged);
            // 
            // lblTagLabel
            // 
            this.lblTagLabel.AutoSize = true;
            this.lblTagLabel.Location = new System.Drawing.Point(104, 129);
            this.lblTagLabel.Name = "lblTagLabel";
            this.lblTagLabel.Size = new System.Drawing.Size(100, 13);
            this.lblTagLabel.TabIndex = 10;
            this.lblTagLabel.Text = "Action\'s Tag Name:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(104, 191);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(78, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "Optional Label:";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addActionPresetToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(363, 24);
            this.menuStrip1.TabIndex = 12;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // addActionPresetToolStripMenuItem
            // 
            this.addActionPresetToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.customFileNameToolStripMenuItem,
            this.autoShowToolStripMenuItem});
            this.addActionPresetToolStripMenuItem.Name = "addActionPresetToolStripMenuItem";
            this.addActionPresetToolStripMenuItem.Size = new System.Drawing.Size(76, 20);
            this.addActionPresetToolStripMenuItem.Text = "Add Preset";
            // 
            // customFileNameToolStripMenuItem
            // 
            this.customFileNameToolStripMenuItem.Name = "customFileNameToolStripMenuItem";
            this.customFileNameToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.customFileNameToolStripMenuItem.Text = "Filename Formatting";
            this.customFileNameToolStripMenuItem.Click += new System.EventHandler(this.customFileNameToolStripMenuItem_Click);
            // 
            // autoShowToolStripMenuItem
            // 
            this.autoShowToolStripMenuItem.Name = "autoShowToolStripMenuItem";
            this.autoShowToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.autoShowToolStripMenuItem.Text = "Auto Show";
            this.autoShowToolStripMenuItem.Click += new System.EventHandler(this.autoShowToolStripMenuItem_Click);
            // 
            // ActionEdit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ClientSize = new System.Drawing.Size(363, 286);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lblTagLabel);
            this.Controls.Add(this.cboActionType);
            this.Controls.Add(this.lblRight);
            this.Controls.Add(this.lblLeft);
            this.Controls.Add(this.txtLabel);
            this.Controls.Add(this.txtTagName);
            this.Controls.Add(this.lblSelectAction);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOkay);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "ActionEdit";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Action Edit";
            this.Load += new System.EventHandler(this.ActionEdit_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOkay;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label lblSelectAction;
        private System.Windows.Forms.TextBox txtTagName;
        private System.Windows.Forms.TextBox txtLabel;
        private System.Windows.Forms.Label lblLeft;
        private System.Windows.Forms.Label lblRight;
        private System.Windows.Forms.ComboBox cboActionType;
        private System.Windows.Forms.Label lblTagLabel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem addActionPresetToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem customFileNameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem autoShowToolStripMenuItem;
    }
}
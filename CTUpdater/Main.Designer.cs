namespace CTUpdater
{
    partial class Main
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.lblMain = new System.Windows.Forms.Label();
            this.txtFacts = new System.Windows.Forms.TextBox();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.zipWorker = new System.ComponentModel.BackgroundWorker();
            this.SuspendLayout();
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(12, 141);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(310, 23);
            this.progressBar.TabIndex = 0;
            // 
            // lblMain
            // 
            this.lblMain.AutoSize = true;
            this.lblMain.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMain.Location = new System.Drawing.Point(8, 118);
            this.lblMain.Name = "lblMain";
            this.lblMain.Size = new System.Drawing.Size(181, 20);
            this.lblMain.TabIndex = 1;
            this.lblMain.Text = "This is some sample text";
            // 
            // txtFacts
            // 
            this.txtFacts.BackColor = System.Drawing.Color.LightSteelBlue;
            this.txtFacts.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtFacts.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.txtFacts.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtFacts.Location = new System.Drawing.Point(12, 12);
            this.txtFacts.Multiline = true;
            this.txtFacts.Name = "txtFacts";
            this.txtFacts.ReadOnly = true;
            this.txtFacts.Size = new System.Drawing.Size(310, 94);
            this.txtFacts.TabIndex = 2;
            this.txtFacts.Text = "This is some text";
            // 
            // timer
            // 
            this.timer.Interval = 7000;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // zipWorker
            // 
            this.zipWorker.WorkerReportsProgress = true;
            this.zipWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.zipWorker_DoWork);
            this.zipWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.zipWorker_ProgressChanged);
            this.zipWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.zipWorker_RunWorkerCompleted);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightSteelBlue;
            this.ClientSize = new System.Drawing.Size(334, 176);
            this.Controls.Add(this.txtFacts);
            this.Controls.Add(this.lblMain);
            this.Controls.Add(this.progressBar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "CT Updater";
            this.Load += new System.EventHandler(this.Main_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label lblMain;
        private System.Windows.Forms.TextBox txtFacts;
        private System.Windows.Forms.Timer timer;
        private System.ComponentModel.BackgroundWorker zipWorker;
    }
}


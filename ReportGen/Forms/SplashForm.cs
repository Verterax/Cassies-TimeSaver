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
    public partial class SplashForm : Form
    {
        private Timer t;

        public SplashForm()
        {
            InitializeComponent();
        }

        private void SplashForm_Load(object sender, EventArgs e)
        {

            //txtBetaMessage.Text = "Licensed to " + UserLicense.UserName +
            //    UserLicense.ExpDesc;

            t = new Timer();
            t.Interval = 3000;
            t.Tick += new EventHandler(t_Tick);
            t.Start();
        }

        void t_Tick(object sender, EventArgs e)
        {
            t.Stop();
            this.Close();
        }


    }
}

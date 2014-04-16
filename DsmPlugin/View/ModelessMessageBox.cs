using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Tcdev.Dsm.View
{
    public partial class ModelessMessageBox : Form
    {
        public ModelessMessageBox( string task)
        {
            InitializeComponent();

            Task = task;
        }
        public string Task
        {
            private set { lblTask.Text = value; }
            get { return lblTask.Text;  } 
        }

        public string Message
        {
            get { return lblMessage.Text; }
            set { lblMessage.Text = value;}
        }

        private void ModelessMessageBox_Load(object sender, EventArgs e)
        {
            BringToFront();
        }

        public void UpdateProgress( int value, string message )
        {
            if (Visible == false)
                Show();

            this.lblMessage.Text = message;
            this.progressBar1.Value = value;

            this.lblMessage.Refresh();
            this.progressBar1.Refresh();
            this.lblTask.Refresh();
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Tcdev.Dsm.View
{
    /// <summary>
    /// Dialog for reporting unexepcted errors and sending report to author
    /// </summary>
    public partial class ErrorDialog : Form
    {
        //-----------------------------------------------------------------------------------------------
        internal ErrorDialog()
        {
            InitializeComponent();
        }
        //-----------------------------------------------------------------------------------------------

        public ErrorDialog(string errorText)
        {
            InitializeComponent();

            this.txtBoxError.Text = errorText;
        }
    }
}
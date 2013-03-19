using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using System.IO;

namespace Tcdev.Dsm.View
{
    /// <summary>
    /// A form used for displaying Html text (can be used to display reports etc)
    /// </summary>
    public partial class HtmlViewer : Form
    {
        /// <summary>
        /// Create a form with content provided at the uri
        /// </summary>
        /// <param name="uri"></param>
        public HtmlViewer(string uri)
        {
            InitializeComponent();
            webBrowser1.Url = new Uri(uri);
        }
        //-------------------------------------------------------------------------------------------------------------
        HtmlViewer()
        {
            InitializeComponent();
        }
        
        //-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Save the displayed content to a local file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.CheckPathExists = true;
            dlg.DefaultExt = "html";
            dlg.AddExtension = true;
            dlg.OverwritePrompt = true;
            dlg.Filter = "Html (*.html)|*.html|All files (*.*)|*.*";

            FileInfo fi = new FileInfo(webBrowser1.Url.LocalPath);

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    fi.CopyTo( dlg.FileName, true );

                    this.Text = "DSM Report - " +  dlg.FileName;
                    webBrowser1.Url = new Uri(dlg.FileName);
                }
                catch (System.IO.IOException ioe)
                {
                    MessageBox.Show("Error Saving File." + Environment.NewLine + ioe.Message,
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    ErrorDialog dlgE = new ErrorDialog(ex.ToString());
                    dlgE.ShowDialog();
                }
            }
        }
        //-------------------------------------------------------------------------------------------------------------
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
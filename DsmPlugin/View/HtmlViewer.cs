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
        /// Create a form with content provided in the given stream
        /// </summary>
        /// <param name="uri"></param>
        public HtmlViewer(Stream stream)
        {
            InitializeComponent();
            stream.Position = 0;
            webBrowser1.DocumentStream = stream;
            //webBrowser1.Navigating += new WebBrowserNavigatingEventHandler(webBrowser1_Navigating);
            webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_DocumentCompleted);
        }

        

        void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            webBrowser1.DocumentStream.Close();
        }
        //-------------------------------------------------------------------------------------------------------------
        HtmlViewer()
        {
            InitializeComponent();
            Font sysFont = SystemFonts.MessageBoxFont;
            this.Font = new Font(sysFont.Name, sysFont.SizeInPoints, sysFont.Style);
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

            //FileInfo fi = new FileInfo(webBrowser1.Url.LocalPath);

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                FileStream fs = new FileStream(dlg.FileName, FileMode.Create);
                try
                {
                    
                    Byte[] bytes = new Byte[webBrowser1.DocumentStream.Length];
                    webBrowser1.DocumentStream.Read(bytes, 0, bytes.Length);
                    fs.Write(bytes, 0, bytes.Length);
                    fs.Flush();
                    
                    //fi.CopyTo( dlg.FileName, true );

                    //this.Text = "DSM Report - " +  dlg.FileName;
                    //webBrowser1.Url = new Uri(dlg.FileName);
                }
                catch (System.IO.IOException ioe)
                {
                    MessageBox.Show("Error Saving File." + Environment.NewLine + ioe.Message,
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    ErrorDialog.Show(ex.ToString() );
                }
                finally
                {
                    fs.Close();
                    webBrowser1.DocumentStream.Close();
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
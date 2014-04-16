using System;
using System.IO;
using System.Windows.Forms;
using Tcdev.Dsm;
//using Tcdev.Dsm.Adapters;
using Tcdev.Dsm.Engine;
using Tcdev.Dsm.View;

namespace Tcdev.DsmVsAddin
{
    public class VisualStudioAdapter : Form//, IAdapter
    {
        MainControl _mainControl = null;

        //IAnalyser _analyser = null;

        public VisualStudioAdapter() 
        { 
            InitializeComponent();
            _mainControl = new MainControl();
            
            this.Controls.Add( _mainControl );
            _mainControl.Dock = DockStyle.Fill;
        }

        public void LoadAssembly( string assemblyPath, bool refOnly )
        {
            try
            {
                FileInfo fi = new FileInfo(assemblyPath);
                _mainControl.AddAssembly(new Target(fi.Name, fi.FullName), !refOnly);
            }
            catch (Exception e)
            {
                MessageBox.Show(assemblyPath + Environment.NewLine + e.Message);
            }
        }

        public void Open(string directory, string name)
        {
            DirectoryInfo projectPath = new DirectoryInfo(directory);
            FileInfo[] files = projectPath.GetFiles("*.dsm" );

            if (files.Length == 0)
            {
                string msg =
                    "No project file found.  Do you wish to run the analyser now?" +
                    System.Environment.NewLine + System.Environment.NewLine +
                    "Choose Yes to include all project assemblies" + System.Environment.NewLine +
                    "Otherwise, choose No to include only selected assemblies";

                DialogResult result = MessageBox.Show(msg, "New project", MessageBoxButtons.YesNo,MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    _mainControl.btnAnalyse_Click(this, EventArgs.Empty);
                }
            }
            else if (files.Length == 1)
            {
                _mainControl.DoProjectOpen(files[0]);
            }
            else
            {
                if ( _mainControl.OpenFile(projectPath) == true )
                    _mainControl.btnAnalyse_Click(this, EventArgs.Empty);
            }
            this.Show();
            this.BringToFront();
            
        }

        public void Reanalyser()
        {
            _mainControl.ReAnalyse();
        }
        
        #region IAdapter Membres

        //public Tcdev.Dsm.Engine.IAnalyser GetAnalyser()
        //{
        //    _analyser = new CecilAnalyserecil


        //    return _analyser;
        //}

        #endregion

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VisualStudioAdapter));
            this.SuspendLayout();
            // 
            // VisualStudioAdapter
            // 
            this.ClientSize = new System.Drawing.Size(1001, 602);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "VisualStudioAdapter";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Dependency Structure Matrix for Visual Studio";
            this.ResumeLayout(false);
        }
    }
}

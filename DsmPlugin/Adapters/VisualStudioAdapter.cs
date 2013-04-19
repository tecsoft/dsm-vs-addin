using System;
using System.Collections.Generic;
using System.Text;
using Tcdev.Dsm.Engine;
using System.Windows.Forms;
using Tcdev.Dsm.View;
using System.IO;
using System.Reflection;
using System.Collections;
using Tcdev.Outil;
using Tcdev.Dsm.Commands;

namespace Tcdev.Dsm.Adapters
{
    public delegate string OnResolveAssembly( string name );
    
    public class VisualStudioAdapter : Form, IAdapter,IDisposable
    {
        MainControl _mainControl = null;

        IAnalyser _analyser = null;

        public VisualStudioAdapter() 
        { 
            InitializeComponent();
            _mainControl = new MainControl();
            _mainControl.Adapter = this;
            this.Controls.Add( _mainControl );

            _mainControl.Dock = DockStyle.Fill;
        }

        public DirectoryInfo ProjectPath { get; set; }
        public string ProjectName { get; set; }
        
        public void LoadAssembly( string assemblyPath, bool refOnly )
        {   
            FileInfo fi = new FileInfo(assemblyPath);
            _mainControl.AddAssembly(new Target(fi.Name, fi.FullName), !refOnly );
        }

        public void Open(string directory, string name)
        {
            ProjectPath = new DirectoryInfo(directory);
            ProjectName = name;

            FileInfo fi = new FileInfo( Path.Combine(directory, name) );
            if (fi.Exists)
            {
                MessageBox.Show("existing file found");

                // ask  if open and reanalyse,
                // or open but not updated
                // cancel for empty project

                _mainControl.DoProjectOpen( fi );
                
            }
            else
            {
                MessageBox.Show("new project ?");
                // ask if analyse automatically
                 _mainControl.btnAnalyse_Click(this, EventArgs.Empty);
            }
            this.Show();
        }

        public void Reanalyser()
        {
            _mainControl.ReAnalyse();
        }
        
        #region IAdapter Membres

        public Tcdev.Dsm.Engine.IAnalyser GetAnalyser()
        {
            _analyser = new CecilAnalyser();

            FileInfo[] files = ProjectPath.GetFiles(ProjectName + ".dsm", SearchOption.TopDirectoryOnly);

            if (files.Length == 1)
                _analyser.ProjectFile = files[0];
            else if (files.Length == 0)
                _analyser.ProjectFile = new FileInfo(Path.Combine(ProjectPath.FullName, ProjectName + ".dsm"));
            else
                _analyser.ProjectFile = null; // toodo

            return _analyser;
        }

        #endregion

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VisualStudioAdapter));
            this.SuspendLayout();
            // 
            // VisualStudioAdapter
            // 
            this.ClientSize = new System.Drawing.Size(682, 456);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "VisualStudioAdapter";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Dependency Structure Matrix Visual Studio";
            this.ResumeLayout(false);

        }

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            if (_analyser != null)
                _analyser.Dispose();
        }

        #endregion
    }
}

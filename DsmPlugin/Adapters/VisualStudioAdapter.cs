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

namespace Tcdev.Dsm.Adapters
{
    public delegate string OnResolveAssembly( string name );
    
    public class VisualStudioAdapter : Form, IAdapter
    {
        MainControl _mainControl = null;

        public VisualStudioAdapter() 
        { 
            InitializeComponent();
            _mainControl = new MainControl();
            _mainControl.Adapter = this;
            this.Controls.Add( _mainControl );

            _mainControl.Dock = DockStyle.Fill;
        }
        
        public void LoadAssembly( string assemblyPath, bool refOnly )
        {   
            FileInfo fi = new FileInfo(assemblyPath);
            _mainControl.AddAssembly(new Target(fi.Name, fi.FullName), !refOnly );
        }
        
        #region IAdapter Membres

        public Tcdev.Dsm.Engine.IAnalyser GetAnalyser()
        {
            return new CecilAnalyser();
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
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Dependency Structure Matrix Visual Studio";
            this.ResumeLayout(false);

        }
    }
}

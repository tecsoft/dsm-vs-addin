using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using Tcdev.Dsm.Engine;
using Tcdev.Dsm.View;
using System.IO;
using System.Reflection;

namespace Tcdev.Dsm.Adapters
{
    public class StandaloneAdapter : Form, IAdapter
    {
        MainControl _mainControl;
        public StandaloneAdapter()
        {
            InitializeComponent();

            _mainControl = new MainControl();
            _mainControl.Adapter = this;

            (_mainControl as Control).Dock = DockStyle.Fill;
            this.Controls.Add(_mainControl as Control);
        }
        
        public void AddAssemblies( Assembly[] assemblies )
        {
            foreach( Assembly a in assemblies )
            {
                Target t = new Target( a.FullName, a.Location, a );
                _mainControl.AddAssembly( t );
            }
        }
        
        public void AddAssemblies( string[] args )
        {
            for( int i = 0; i < args.Length; i++ )
            {
                if ( "-p".Equals( args[i] ) )
                {
                    string assembly = args[++i];
                    Target target = LoadAssembly( assembly );
                    
                    if ( target != null )
                    {
                        MessageBox.Show( (target.AssemblyObject as Assembly).FullName );
                        _mainControl.AddAssembly( target );   
                    }
                } 
                else if ( "-r".Equals( args[i] ) )
                {
                    string assembly = args[ i++ ];
                    LoadAssembly( assembly );
                }
                
            }
        }
        
        private Target LoadAssembly( string path )
        {        
            Target target = null;
            try
            {
                using ( FileStream sw = new FileStream( path, FileMode.Open ) )
                {
                    int len = (int)sw.Length;
                    byte[] buffer = new byte[ len ];
                    sw.Read( buffer, 0, len );

                    Assembly assembly = Assembly.ReflectionOnlyLoad( buffer );
                    
                    if ( assembly != null )
                    {
                        FileInfo fi = new FileInfo( path );
                        target = new Target( fi.Name, path, assembly );
                    }
                }
            }
            catch ( Exception err )
            {
                //_log.Trace( "Error pre loading assembly target.FullPath : " + err.ToString() );
                
                MessageBox.Show( err.Message);
            }
            
            return target;
        }

        public IAnalyser GetAnalyser()
        {
            return new FrameworkAnalyser();
        }

        [STAThread]
        static void Main( string [] args)
        {  
            Application.SetCompatibleTextRenderingDefault(false);
            Application.EnableVisualStyles();
            StandaloneAdapter dsmApp = new StandaloneAdapter();
            
            dsmApp.AddAssemblies( args );

            Application.Run(dsmApp);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // StandaloneAdapter
            // 
            this.ClientSize = new System.Drawing.Size(738, 562);
            this.Name = "StandaloneAdapter";
            this.Text = "Dependency Structure Matrix";
            this.ResumeLayout(false);

        }
    }
}

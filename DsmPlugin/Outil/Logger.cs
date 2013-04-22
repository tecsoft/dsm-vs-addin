using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Tcdev.Outil
{
    public class Logger
    {
        private StreamWriter logfile = null;

        public Logger(string filename)
        {  
            #if DEBUG
            try
            {
                FileInfo file = new FileInfo( filename );
                
                logfile = new StreamWriter( filename, false, System.Text.Encoding.UTF8);
                //logfile = new StreamWriter(Console.OpenStandardOutput());
                logfile.AutoFlush = true;
                //MessageBox.Show( " logging to : " + file.FullName );
            }
            catch( IOException ioe )
            {
                System.Windows.Forms.MessageBox.Show("Cannot open trace file : " + ioe.Message);
            }
            #endif
        }

        public void Trace(string text)
        {
            #if DEBUG
            try
            {
                logfile.WriteLine( text);
            }
            finally
            {
                logfile.Flush();
            }
            #endif
        }

        public void Dispose()
        {
            #if DEBUG
            if ( logfile != null )
            {
                logfile.Flush();
                logfile.Close();
                logfile.Dispose();
            }
            #endif
        }

    }
}

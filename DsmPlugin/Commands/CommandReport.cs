using System;
using System.Collections.Generic;
using System.Text;
using Tcdev.Dsm.Model;
using System.IO;
using Tcdev.Dsm.View;

namespace Tcdev.Dsm.Commands
{
    class CommandReport : ICommand
    {
        //-----------------------------------------------------------------------------------------
        DsmModel _model;
        bool     _done = false;
        
        //-----------------------------------------------------------------------------------------

        public CommandReport(DsmModel model)
        {
            _model = model;
        }

        //-----------------------------------------------------------------------------------------

        public bool Completed
        {
            get { return _done; }
        }

        //-----------------------------------------------------------------------------------------

        public void Execute()
        {
            StreamWriter sw = null;
            string filename = GetTemporaryFile();

            try
            {
                sw = File.CreateText(filename);
                WriteHtmlPreamble( sw );
                WriteReport( sw );
                CloseHtml( sw );

                ViewReport( filename );

                _done = true;
            }
            catch( IOException ioe )
            {
                if ( sw != null ) sw.Close();

                throw new DsmException( "Error creating report file", ioe );
            }
        }

        //-----------------------------------------------------------------------------------------

        string GetTemporaryFile()
        {
            string tempDir = Environment.GetEnvironmentVariable("TEMP");

            if (tempDir == null)
            {
                throw new DsmException("%TEMP% is not set");
            }

            return String.Format("{0}\\{1:X}.html", tempDir, DateTime.Now.Ticks);
        }

        //-----------------------------------------------------------------------------------------

        void WriteHtmlPreamble(StreamWriter sw)
        {
            sw.WriteLine("<html><head><style>");
            sw.WriteLine("body    { font-family:  Arial, Helvetica; font-size: 90%; color: #444444; }");
            sw.WriteLine("b       { color: #000000; font-weight: bold; }");
            sw.WriteLine("a       { color: #000080; }");
            sw.WriteLine("a:hover { color: #0000c0; }");
            sw.WriteLine("h1      { font-size: 100%; }");
            sw.WriteLine("h2      { font-size: 95%; }");
            sw.WriteLine("h3      { font-size: 90%; }");
            sw.WriteLine("ul	{ font-size: 90%; }");
            sw.WriteLine("</style></head><body>");

            sw.Flush();
        }

        //-----------------------------------------------------------------------------------------

        void CloseHtml(StreamWriter sw)
        {
            sw.WriteLine("<p>---End of Report---</p></body></html>");
            sw.Flush();
            sw.Close();
        }

        //-----------------------------------------------------------------------------------------

        void WriteReport(StreamWriter sw)
        {
            _model.DoReport(sw);
            sw.Flush();
        }

        //-----------------------------------------------------------------------------------------

        void ViewReport(string filename )
        {
            HtmlViewer view = new HtmlViewer(filename);
            view.Text = "DSM Report - " + filename;
            view.Show();
        }

        //-----------------------------------------------------------------------------------------
    }
}

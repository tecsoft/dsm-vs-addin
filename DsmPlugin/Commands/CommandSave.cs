using System;
using System.Collections.Generic;
using System.Text;
using Tcdev.Dsm.Model;
using System.Windows.Forms;

namespace Tcdev.Dsm.Commands
{
    class CommandSave : ICommand
    {
        //-----------------------------------------------------------------------------------------

        DsmModel _model;
        bool     _done;

        //-----------------------------------------------------------------------------------------

        public CommandSave( DsmModel model ) 
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
            try
            {
                string filename = GetFile();

                if (filename != null)
                {
                    _model.SaveModel(filename);
                    _model.Modified = false;
                    _done = true;
                }
            }
            catch (Exception e)
            {
                throw new DsmException("Unable to save project", e);
            }
        }

        //-----------------------------------------------------------------------------------------

        string GetFile()
        {
            System.Windows.Forms.SaveFileDialog dlg = new SaveFileDialog();
            dlg.AddExtension = true;
            dlg.OverwritePrompt = true;
            dlg.DefaultExt = "dsm";
            dlg.Filter = "DSM project files (*.dsm)|*.dsm";
            dlg.Title = "Save DSM project";

            DialogResult result = dlg.ShowDialog();

            if (result == DialogResult.OK)
            {
                return dlg.FileName;
            }

            return null;
        }

        //-----------------------------------------------------------------------------------------
    }
}

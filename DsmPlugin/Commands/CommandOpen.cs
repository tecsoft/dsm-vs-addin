using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Tcdev.Dsm.View;

namespace Tcdev.Dsm.Commands
{
    class CommandOpen : ICommand
    {
        //-----------------------------------------------------------------------------------------

        Dsm.Model.DsmModel _model = null;
        bool               _done  = false;
        FileInfo _file = null;

        //-----------------------------------------------------------------------------------------

        public CommandOpen( Dsm.Model.DsmModel model,FileInfo file)
        {
            _model = model;
            _file = file;
        }

        //-----------------------------------------------------------------------------------------

        public bool Completed
        {
            get { return _done; }
        }

        //-----------------------------------------------------------------------------------------

        public void Execute(MainControl.ProgressUpdateDelegate updateFunction)
        {
            try
            {
                string filename = GetFile();

                if (filename != null)
                {
                    if (updateFunction != null) 
                        updateFunction(0, filename);

                    _model = Model.DsmModel.LoadModel(filename, _model);

                    if (updateFunction != null)
                        updateFunction(33, filename );

                    _model.AllocateIds();

                    if (updateFunction != null)
                        updateFunction(33, filename);

                    _model.AnalyseCyclicDepedencies();

                    if (updateFunction != null)
                        updateFunction(100, "done");

                    _done = true;
                }
            }
            catch (Exception e)
            {
                throw new DsmException("Error opening Dsm file", e);
            }
        }

        //-----------------------------------------------------------------------------------------

        string GetFile()
        {
            if (_file == null)
            {
                OpenFileDialog dlg = new OpenFileDialog();
                dlg.AddExtension = true;
                dlg.CheckFileExists = true;
                dlg.CheckPathExists = true;
                dlg.DefaultExt = "dsm";
                dlg.Filter = "DSM project files (*.dsm)|*.dsm|All files (*.*)|*.*";
                dlg.Title = "Open DSM project";

                DialogResult result = dlg.ShowDialog();

                if (result == DialogResult.OK)
                {
                    return dlg.FileName;
                }

                return null;
            }
            else
            {
                return _file.FullName;
            }
        }

        //-----------------------------------------------------------------------------------------
    }
}

using System;
using System.Collections.Generic;
using System.Text;

using Tcdev.Dsm.Model;
using Tcdev.Dsm.View;

namespace Tcdev.Dsm.Commands
{
    class CommandPartition : ICommand
    {
        DsmModel _model;

        public CommandPartition(DsmModel model)
        {
            _model = model;

            
        }
        public void Execute(MainControl.ProgressUpdateDelegate updateFunction)
        {
            // show confirmation box
            if (_model.SelectedNode != null)
            {
                _model.Partition();
            }
        }

        public bool Completed
        {
            get { return true; }
        }
    }
}

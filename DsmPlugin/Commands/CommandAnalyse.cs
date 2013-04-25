using System;
using System.Collections.Generic;
using System.Text;
using Tcdev.Dsm.Model;
using Tcdev.Dsm.View;
using Tcdev.Dsm.Engine;

namespace Tcdev.Dsm.Commands
{
    public class CommandAnalyse :ICommand
    {
        IAnalyser _analyser;
        DsmModel  _model;
        bool      _done = false;

        //-----------------------------------------------------------------------------------------

        public bool Completed
        {
            get { return _done; }
        }

        //-----------------------------------------------------------------------------------------
        public CommandAnalyse(
            IAnalyser analyser, 
            DsmModel model)
        {
            _analyser = analyser;
            _model = model;
            
        }

        //------------------------------------------------------------------------------------------------------

        public void Execute(MainControl.ProgressUpdateDelegate updateFunction)
        {
            if (ValidateParameters())
            {
                _analyser.Model = _model;

                updateFunction(0, "Loading assemblies");
                var types = _analyser.LoadTypes();

                updateFunction(20, "Building module hierarchy");
                _model.BuildHierarchy( types);

                updateFunction(30, "Assigning IDs");
                _model.AllocateIds();

                updateFunction(40, "Analysing inter-module relationships");
                _analyser.AnalyseRelations();

                updateFunction(80, "Calculating subtotal weights");
                _model.CalculateParentWeights();

                updateFunction(90, "Looking for cyclic dependencies");
                _model.AnalyseCyclicDepedencies();

                updateFunction(100, "Analyse completed");

                _model.IsModified = true;

                _done = true;
            }
        }

        //------------------------------------------------------------------------------------------------------
        
        bool ValidateParameters()
        {
            return true; 
        }
    }
}

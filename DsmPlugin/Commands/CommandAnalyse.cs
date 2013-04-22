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
        MainControl.ProgressUpdateDelegate _updateFunction;

        //-----------------------------------------------------------------------------------------

        public bool Completed
        {
            get { return _done; }
        }

        //-----------------------------------------------------------------------------------------
        public CommandAnalyse(
            IAnalyser analyser, 
            DsmModel model, 
            MainControl.ProgressUpdateDelegate updateFunction )
        {
            _analyser = analyser;
            _model = model;
            if (updateFunction == null )
                _updateFunction = (int i, string x ) => Console.WriteLine( i + " : " + x );
            else
                _updateFunction = updateFunction;

            //_model.Options.DsmModelType = modelType;
        }

        //------------------------------------------------------------------------------------------------------

        public void Execute()
        {
            if (ValidateParameters())
            {
//                _model.StartBuild();

                _analyser.Model = _model;

                _updateFunction(0, "Loading assemblies");
                var types = _analyser.LoadTypes();

                _updateFunction(10, "Analysing inter-module relationships");
                //_analyser.AnalyseRelations();

                _updateFunction(40, "Building module hierarchy");
//_model.EndBuild();
                _model.BuildHierarchy( types);

                _updateFunction(70, "Assigning IDs");
                _model.AllocateIds();

                _analyser.AnalyseRelations();

                _updateFunction(80, "Calculating subtotal weights");
                _model.CalculateParentWeights();

                _updateFunction(90, "Looking for cyclic dependencies");
                _model.AnalyseCyclicDepedencies();

                _updateFunction(100, "Analyse completed");

                _model.IsModified = true;

                _done = true;

                
            }
        }

        //------------------------------------------------------------------------------------------------------
        
        bool ValidateParameters()
        {
            /*if (checkedListBox1.CheckedItems.Count == 0)
            {
                MessageBox.Show("At least one assembly must be selected", "DSM Plugin warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                return false;
            }*/

            return true;
                
        }
    }
}

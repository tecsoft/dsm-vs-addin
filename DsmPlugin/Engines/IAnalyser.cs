
using System;
using Tcdev.Dsm.Model;

namespace Tcdev.Dsm.Engine
{
	/// <summary>
	/// The interface to Analysis engine implementations
	/// </summary>
	public interface IAnalyser : IDisposable
	{
        /// <summary>
        /// The _model to populate
        /// </summary>
        DsmModel Model
        {
            set;
        }

        /// <summary>
        /// User options for the analysis
        /// </summary>
        DsmOptions Options
        {
            get;
            set;
        }

        /// <summary>
        /// Allows the user to include an assembly in the analysis
        /// </summary>
        /// <param name="assembly"></param>
        void IncludeAssembly( Target assembly);

        /// <summary>
        /// Stage where the types are determined from the assemblies
        /// </summary>
        void LoadTypes();

        /// <summary>
        /// Stage where the relations between the _loaded types are determined
        /// </summary>
        void AnalyseRelations();   
	}
}

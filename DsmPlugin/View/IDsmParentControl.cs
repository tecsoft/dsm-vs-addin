using System;
using System.Collections;
using System.Text;

//using Tcdev.Dsm.Adapters;

namespace Tcdev.Dsm.View
{
    /// <summary>
    /// An interface to a UserControl 
    /// </summary>
    public interface IDsmParentControl
    {
        /// <summary>
        /// Allows a list of potential assemblies to be included in the control for selection by user for example
        /// Adapters must create a Target object to encaspulate the assembly
        /// </summary>
        /// <param name="assembly"></param>
        void AddAssembly(Target assembly);

        /// <summary>
        /// Clear all assemblies from list
        /// </summary>
        void ClearAssemblies();

        /// <summary>
        /// Function to be called on form closure - zuch as to give chance to save current project
        /// </summary>
        /// <returns>True if form is indeed closed else false</returns>
        bool OnClosing();
    }
}

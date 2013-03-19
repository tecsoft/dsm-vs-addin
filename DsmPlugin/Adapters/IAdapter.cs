using System;
using System.Collections.Generic;
using System.Text;

using Tcdev.Dsm.Engine;

namespace Tcdev.Dsm.Adapters
{
    /// <summary>
    /// Interface to adapters which wrap the DSM Plugin  up in an interface compatible
    /// with whatever its hosted in 
    /// </summary>
    public interface IAdapter
    {
        /// <summary>
        /// Retrieve a new instance of an Analyser used on the Adpater's environment
        /// </summary>
        /// <returns></returns>
        IAnalyser GetAnalyser();
    }
}

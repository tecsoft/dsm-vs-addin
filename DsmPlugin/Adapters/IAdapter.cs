using System;
using System.Collections.Generic;
using System.Text;

using Tcdev.Dsm.Engine;
using System.IO;

namespace Tcdev.Dsm.Adapters
{
    /// <summary>
    /// Interface to adapters which wrap the DSM Plugin  up in an interface compatible
    /// with whatever its hosted in 
    /// </summary>
    public interface IAdapter
    {
        void Open( string directory, string name );
    }
}

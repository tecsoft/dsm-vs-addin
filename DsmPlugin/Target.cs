using System;
using System.Collections.Generic;
using System.Text;

namespace Tcdev.Dsm
{
    /// <summary>
    /// Wrapper around an assembly loaded into the DSM
    /// </summary>
    public class Target
    {
        public Target( string shortName, string fullPath )
        {
            ShortName = shortName;
            FullPath  = fullPath;
        }

        public string ShortName
        {
            get;
            private set;
        }

        public string FullPath
        {
            get;
            private set;
        }

        public override string  ToString()
        {
 	        return string.Format( "{0} [{1}]", ShortName, FullPath );
        }
    }
}

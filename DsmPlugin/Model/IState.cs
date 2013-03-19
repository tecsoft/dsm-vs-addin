using System;
using System.Collections.Generic;
using System.Text;

namespace Tcdev.Dsm.Model
{
    /// <summary>
    /// Represents display state of a module
    /// </summary>
    internal interface IState
    {
        //int Id
        //{
        //    get;
        //    set;
        //}
        
        bool CanCollapse
        {
            get;
        }

        bool IsCollapsed
        {
            get;
            set;
        }

        bool IsHidden
        {
            get;
            set;
        }

        int Depth
        {
            get;
            set;
        }
    }
}

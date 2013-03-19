using System;
using System.Collections.Generic;
using System.Text;

namespace Tcdev.Dsm.Model
{
    /// <summary>
    /// Simple class to hold details of a relation between the holder of the object and a consumer
    /// </summary>
    public class Relation
    {
        public Module Consumer  = null;
        public bool   IsCyclic  = false;
        public int    Weight    = 0;

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// Constructor 
        /// </summary>
        /// <param name="consumer">The module on the depending side of the relation</param>
        public Relation(Module consumer)
        {
            Consumer = consumer;
            IsCyclic = false;
            Weight = 0;
        }
        //-----------------------------------------------------------------------------------------
    }
}

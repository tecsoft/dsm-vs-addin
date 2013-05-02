using System;
using System.Collections.Generic;
using System.Text;

namespace Tcdev.Dsm.Model
{
    /// <summary>
    /// Simple class to hold details of a relation between a provider and a consumer
    /// </summary>
    public class Relation
    {
        public Module Consumer  = null;
        public Module Provider = null;
        public bool   IsCyclic  = false;
        public int    Weight    = 0;

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// Constructor 
        /// </summary>
        /// <param name="provider">Provider side of the relation</param>
        /// <param name="consumer">The module on the depending side of the relation</param>
        public Relation(Module provider, Module consumer )
        {
            Consumer = consumer;
            Provider = provider;
            IsCyclic = false;
            Weight = 0;
        }
        //-----------------------------------------------------------------------------------------
    }
}

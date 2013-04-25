using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tcdev.Dsm.Model.DependencyRules
{
    public abstract class DependencyRule
    {
        public Module Provider { get; protected set; }
        public Module Consumer { get; protected set; }

        public DependencyRule(Module provider, Module consumer)
        {
            Provider = provider;
            Consumer = consumer;
        }

        public abstract bool IsViolated(Module provider, Module consumer);
    }
}

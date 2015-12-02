using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tcdev.Dsm.Model.Rules
{
    public class AllowDependencyRule : DependencyRule
    {
        public AllowDependencyRule(Module provider, Module consumer)
            : base( provider, consumer )
        {
        }

        public override bool IsViolated(Module provider, Module consumer, DsmModel model)
        {
            return true; // throw new NotImplementedException();
        }
    }
}
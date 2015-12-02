using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tcdev.Dsm.Model.Rules
{
    public class UpperTriangleDependencyRule : CannotUseRule
    {
        public UpperTriangleDependencyRule(Module provider, Module consumer)
            : base( provider, consumer )
        {
        }

        public override bool IsViolated(Module provider, Module consumer, DsmModel model)
        {
            return true;
        }
    }
}
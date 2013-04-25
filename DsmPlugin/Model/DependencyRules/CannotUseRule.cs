using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tcdev.Dsm.Model.DependencyRules
{
    /// <summary>
    /// A rule which says the provider module must not be consumed by the consumer Module.
    /// Applies to all child modules
    /// </summary>
    public class CannotUseRule : DependencyRule
    {
        public CannotUseRule(Module provider, Module consumer) : base( provider, consumer )
        {
        }

        public override bool IsViolated(Module provider, Module consumer)
        {
            return !(provider.FullName.Equals(Provider) && consumer.FullName.Equals(Consumer));
        }
    }
}

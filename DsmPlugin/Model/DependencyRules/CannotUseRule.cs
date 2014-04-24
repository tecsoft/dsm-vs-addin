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
    /// <remarks>
    /// If provider is null then consumer may not use any other
    /// If consumer is null then provider may not be consumed
    /// </remarks>
    public class CannotUseRule : DependencyRule
    {
        public CannotUseRule(Module provider, Module consumer)
            : base(provider, consumer)
        {
        }

        public override bool IsViolated()
        {
            return Provider.Relations[Consumer].Weight > 0;
        }
    }
}
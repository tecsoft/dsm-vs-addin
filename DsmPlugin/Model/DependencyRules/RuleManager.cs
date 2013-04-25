using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tcdev.Dsm.Model.DependencyRules
{
    public class RuleManager
    {
        IDictionary<string, IList<DependencyRule>> ruleIndex = new Dictionary<string, IList<DependencyRule>>();

        public void Add(DependencyRule rule)
        {
            var list = Rules(rule.Provider);
            if ( list.Count == 0 )
                ruleIndex[rule.Provider.FullName] = list;

            list.Add(rule);
        }

        public IList<DependencyRule> Rules(Module provider)
        {
            if (ruleIndex.ContainsKey(provider.FullName))
                return ruleIndex[provider.FullName];
            return new List<DependencyRule>();
        }
    }
}

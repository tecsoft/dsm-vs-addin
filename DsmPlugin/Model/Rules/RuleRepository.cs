using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Collections.Generic;

namespace Tcdev.Dsm.Model.Rules
{
    public class RuleRepository
    {
        IList<DependencyRule> _rules = new List<DependencyRule>();

        IDictionary<Module, IList<DependencyRule>> rulesByModule = new Dictionary<Module, IList<DependencyRule>>();

        public ReadOnlyCollection<DependencyRule> Rules
        {
            get { return _rules as ReadOnlyCollection<DependencyRule>; }
        }

        public void Add(DependencyRule rule)
        {
            _rules.Add( rule );

            if (rule.Provider != null)
            {
                AddRuleToLookup( rule.Provider, rule );
            }

            if (rule.Consumer != null)
            {
                AddRuleToLookup( rule.Consumer, rule );
            }
        }

        private void AddRuleToLookup(Module module, DependencyRule rule)
        {
            if (rulesByModule.ContainsKey( module ))
            {
                rulesByModule[module].Add( rule );
            }
            else
            {
                var newList = new List<DependencyRule>() { rule };
                rulesByModule.Add( module, newList );
            }
        }

        public DependencyRule GetRule(Module provider, Module consumer)
        {
            if (rulesByModule.ContainsKey( provider ))
            {
                var rules = rulesByModule[provider];

                return rules.FirstOrDefault( x => x.Provider == provider && x.Consumer == consumer );
            }
            return null;
        }

        private IList<DependencyRule> GetRulesForModule(Module module)
        {
            return rulesByModule[module];
        }

        /// <summary>
        /// Returns a dependency rule violated by the provider/consumer relation
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="consumer"></param>
        /// <returns></returns>
        public DependencyRule ViolatedRule(Module provider, Module consumer, DsmModel model)
        {
            if (provider == consumer) return null;

            DependencyRule violation = null;

            Relation relation = provider.GetRelation( consumer );
            if (relation != null && relation.Weight > 0)
            {
                // need to check

                if (provider.Id < consumer.Id)
                {
                    // upper triangle
                    violation = new UpperTriangleDependencyRule( provider, consumer );

                    violation = CheckViolationOverride( violation );
                }

                else
                {
                    violation = _rules.FirstOrDefault( x => x.IsViolated( provider, consumer, model ) );
                }
            }

            //

            return violation;
        }

        private DependencyRule CheckViolationOverride(DependencyRule violation)
        {
            DependencyRule result = null;

            CannotUseRule cannotUseRule = violation as CannotUseRule;

            if (cannotUseRule != null)
            {
                result = _rules.FirstOrDefault( x => Overrides( cannotUseRule, x ) );
            }

            return result == null ? violation : null;
        }

        private bool Overrides(CannotUseRule cannotUseRule, DependencyRule candidate)
        {
            return cannotUseRule != candidate;
        }
    }
}
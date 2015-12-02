using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tcdev.Dsm.Model.Rules
{
    /// <summary>
    /// A rule which says the provider module must not be consumed by the consumer Module.
    /// </summary>
    /// <remarks>
    /// If provider is null then consumer may not use any other
    /// If consumer is null then provider may not be consumed
    /// </remarks>
    public class CannotUseRule : DependencyRule
    {
        public CannotUseRule(Module provider, Module consumer)
            : base( provider, consumer )
        {
        }

        private bool IsViolated(Module testProvider, Module testConsumer)
        {
            var relations = testProvider.Relations;
            return relations.ContainsKey( testConsumer ) && relations[testConsumer].Weight > 0;
        }

        private bool IsSameOrDescendant(Module parent, Module test, DsmModel model)
        {
            var testNode = model.FindNode( test );
            var parentNode = model.FindNode( parent );
            return testNode == parentNode || DsmModel.IsDescendent( parentNode, testNode );
        }

        private IEnumerable<Module> GetSiblingModules(Module module, DsmModel model)
        {
            var node = model.FindNode( module );
            var nextSibling = node.Parent.FirstChild;
            while (nextSibling != null)
            {
                yield return nextSibling.NodeValue;
                nextSibling = nextSibling.NextSibling;
            }
        }

        public override bool IsViolated(Module testProvider, Module testConsumer, DsmModel model)
        {
            bool isViolated = false;
            if (Provider != null && Consumer != null)
            {
                if (IsSameOrDescendant( Provider, testProvider, model ) &&
                    IsSameOrDescendant( Consumer, testConsumer, model ))
                {
                    isViolated = IsViolated( testProvider, testConsumer );
                }
            }
            else if (Provider != null)
            {
                // Provider cannot be consumed by anyone

                //foreach( var relation in Provider.Relations )
                //{
                //    isViolated = IsViolated( Provider, sibling );
                //    if (isViolated)
                //        break;
                //}
            }

            return isViolated;
        }
    }
}
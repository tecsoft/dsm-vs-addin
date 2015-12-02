using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tcdev.Collections.Generic;
using Tcdev.Dsm.Model;

namespace Tcdev.Dsm.Tests.Model
{
    internal class TestModel : DsmModel
    {
        public TestModel()
            : base()
        {
        }

        public Module Add(string name)
        {
            return CreateModule( name, "", "", false );
        }

        public Tree<Module>.Node Add(Module module, Tree<Module>.Node parentNode)
        {
            var result = Add( module, module.Name, parentNode, 0 );

            AllocateIds();

            return result;
        }

        public void SetRelation(Module provider, Module consumer, int weight)
        {
            provider.AddRelation( consumer, weight );
        }

        public void Print()
        {
            TreeIterator<Module> iterator = new TreeIterator<Module>( Hierarchy );
            Tree<Module>.Node nodeY = iterator.Next();

            while (nodeY != null)
            {
                Console.Write( nodeY.NodeValue.FullName );
                nodeY = iterator.Next();
            }
        }
    }
}
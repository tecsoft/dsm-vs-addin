using System;
using System.Collections.Generic;
using System.Text;

using Tcdev.Collections;
using Tcdev.Dsm.Model;

namespace TreeTests
{
    class Program
    {
        static void Main(string[] args)
        {

            Tree tree = new Tree();

            Tree.Node br1 = CreateAndAdd(tree, null, "br1");
            Tree.Node br2 = CreateAndAdd(tree, null, "br2");
            Tree.Node br3 = CreateAndAdd(tree, null, "br3");
            Tree.Node br4 = CreateAndAdd(tree, null, "br4");

            Tree.Node br11 = CreateAndAdd(tree, br1, "br11");
            Tree.Node br12 = CreateAndAdd(tree, br1, "br12");
            Tree.Node br13 = CreateAndAdd(tree, br1, "br13");

            Console.WriteLine(br1.Children.Count.ToString());

            Tree.Node br111 = CreateAndAdd(tree, br11, "br111");
            Tree.Node br21 = CreateAndAdd(tree, br2, "br21");
            Tree.Node br22 = CreateAndAdd(tree, br2, "br22");

            Print(tree);

            //bool de = Model.IsDescendent(br2, br21);
            //if (de == true) Console.WriteLine("true");


            Console.WriteLine("----------------------------");

            TreeIterator iterator = new TreeIterator(tree);
            Tree.Node node = iterator.Next();
            Console.WriteLine(node.NodeValue);
            node = iterator.Next();

            Console.WriteLine(node.NodeValue);
            node = iterator.Next();

            while (node != null)
            {
                Console.WriteLine(node.NodeValue);
                node = iterator.Skip();      
            }

            Console.WriteLine("--------------------");

            //tree.Remove(br3);
            
            //tree.InsertBefore(tree.CreateNode( "test" ), br2 );
            //Print(tree);

            //Console.WriteLine(node.NodeValue.ToString());

            //tree.Remove( br111 );

            //tree.InsertBefore(br111, br11);

            //Console.WriteLine("--------------------");
            //Print(tree);
            

            /*tree.Remove(br4);
            tree.InsertBefore(br4, br111);

            Print(tree);
            */
        }

        static private Tree.Node CreateAndAdd(Tree hierarchy, Tree.Node parent, object val)
        {
            Tree.Node node = hierarchy.CreateNode(val);
            hierarchy.Add(parent, node);

            return node;
        }

        private static void Print(Tree tree)
        {
            TreeIterator iterator = new TreeIterator(tree);

            Tree.Node node = iterator.Next();

            while (node != null)
            {
                Console.WriteLine(node.NodeValue);
                node = iterator.Next();
            }
        }
    }
}

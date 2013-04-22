using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tcdev.Dsm.Matrix;
using Tcdev.Collections.Generic;

namespace Tcdev.Dsm.Model
{
    internal class Partitionner
    {
        Tree<Module> Tree;
        public Partitionner(Tree<Module> tree)
        {
            Tree = tree;
        }
        //-------------------------------------------------------------------------------------------------
        public void Partition(Tree<Module>.Node parent)
        {
            PartitionGroup(parent.Children);
        }
        //-------------------------------------------------------------------------------------------------
        void PartitionGroup(IList<Tree<Module>.Node> nodes)
        {
            if (nodes.Count > 1)
            {
                SquareMatrix matrix = BuildPartitionMatrix(nodes);

                PartitionerMarkII p = new PartitionerMarkII(matrix);

                Vector v = p.Partition();

                ReorderNodes(nodes, v);

               // _log.Trace("reorder done");
            }
        }
        //-------------------------------------------------------------------------------------------------
        SquareMatrix BuildPartitionMatrix(IList<Tree<Module>.Node> nodes)
        {
            SquareMatrix matrix = new SquareMatrix(nodes.Count);

            for (int i = 0; i < nodes.Count; i++)
            {
                Module provider = nodes[i].NodeValue;

                for (int j = 0; j < nodes.Count; j++)
                {
                    if (j != i)
                    {
                        Module consumer = nodes[j].NodeValue;

                        Relation relation = provider.GetRelation(consumer);

                        if (relation != null && relation.Weight > 0)
                        {
                            matrix.Set(i, j, 1);
                        }
                        else
                        {
                            matrix.Set(i, j, 0);
                        }
                    }
                }
            }

            return matrix;
        }
        //-------------------------------------------------------------------------------------------------
        void ReorderNodes(IList<Tree<Module>.Node> nodes, Vector permutationVector)
        {
            Tree<Module>.Node parentNode = nodes[0].Parent;

            foreach (Tree<Module>.Node node in parentNode.Children)
            {
                Tree.Remove(node);
            }

            for (int i = 0; i < permutationVector.Size; i++)
            {
                Tree.AddLast(parentNode, nodes[permutationVector.Get(i)]);
            }
        }
    }
}

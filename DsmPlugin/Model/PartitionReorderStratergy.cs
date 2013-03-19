using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;

using Tcdev.Collections.Generic;

namespace Tcdev.Dsm.Model
{
    /// <summary>
    /// Encapsulates the Reorder only partitioning stratergy which allows modules to be reordered at
    /// each level - modules are not moved from one branch to another.
    /// 
    /// This algorithm sorts nodes so that those modules that provide to the fewest number of
    /// other modules move to the top and those to the most to the bottom
    /// 
    /// At a given level, if there are > 1 nodes, compare node 1 with node 2, if it provides to more
    /// modules then move node down. If this operation increases the number of upper triangle cells then
    /// the operation is reversed : NextNode
    /// Repeat with current node (compare to next node).  Tests using current
    /// node finish if the node cannot be moved down or it is the last node
    /// Next node 
    /// </summary>
    class PartitionReorderStratergy
    {
        DsmModel _model;

        public PartitionReorderStratergy(DsmModel model )
        {
            _model = model;
        }

        public void Execute()
        {
            Tree<Module>.Node startNode = _model.Hierarchy.Root;

            PartitionChildren( startNode );
        }

        void PartitionChildren( Tree<Module>.Node startNode )
        {
            if (startNode != null )
            {
                IList nodes = startNode.Children;

                if ( nodes.Count > 1 )
                {
                    // Partition nodes
                    PartitionSet( nodes );
                }
                
                if (startNode.firstChild != null )
                {
                    PartitionChildren( startNode.firstChild );
                }
                else if ( startNode.NextSibling != null )
                {
                    PartitionChildren( startNode.NextSibling );
                }
                else if (startNode.Parent != null)
                {
                    startNode = startNode.Parent;

                    while (startNode.NextSibling == null)
                    {
                        startNode = startNode.Parent;
                    }

                    if ( startNode != null )
                    {
                        PartitionChildren( startNode );
                    }
                }
            }
        }

        //-----------------------------------------------------------------------------------------

        void PartitionSet(IList nodes)
        {
            int nbModules = nodes.Count;

            // compare node i && node 2
            // if n1 provides to more than n2 then move n1 down
            // if nb of upper triangle relations increases - undo move

            int currentNb = CountUpperTriangleRelations(nodes);
            int newNb = 0;

            int nbExchanges = 0;
            do
            {
                int i = 0;
                //
                //LOOP TODO
                //
                Tree<Module>.Node n1 = nodes[0];
                Tree<Module>.Node n2 = nodes[1];

                if (CountNbOfRelations(n1.NodeValue) > CountNbOfRelations(n2.NodeValue))
                {
                    bool moved = _model.MoveDown(n1);
                    if (moved)
                    {
                        newNb = CountUpperTriangleRelations(nodes);
                        if (newNb <= currentNb)
                        {
                            currentNb = newNb;

                            // comapre n1 with next position && continue test TODO
                        }
                        else
                        {
                            // return model to previous state
                            _model.MoveUp(n1);

                            // move index on one and start test again
                        }
                    }
                    else
                    {
                        // restart comparison at position 0
                    }
                }
            }
            while (nbExchanges > 0);   // if no exchanges have occurrred we sop the partition
            
            
        }

        //-----------------------------------------------------------------------------------------

        int CountNbOfRelations(Module module)
        {
            int nb = 0;
            foreach( Relation r in module.relations.Values )
            {
                if (r.Weight > 0) nb++;
            }

            return nb;
        }

        //-----------------------------------------------------------------------------------------

        int CountUpperTriangleRelations(IList nodes)
        {
            int nb = 0;

            foreach (Tree<Module>.Node n1 in nodes)
            {
                foreach (Tree<Module>.Node n2 in nodes)
                {
                    Module m1 = n1.NodeValue;
                    Module m2 = n2.NodeValue;

                    if (m1.Id < m2.Id)
                    {
                        // Upper triangle relation

                        Relation r = m1.GetRelation(m2);
                        if (r != null & r.Weight > 0)
                        {
                            nb++;
                        }
                    }
                }
            }

            return nb;
        }
    }
}

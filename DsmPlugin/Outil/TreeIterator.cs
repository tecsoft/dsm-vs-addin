using System;
using System.Collections.Generic;
using System.Text;

namespace Tcdev.Collections.Generic
{
    /// <summary>
    /// A class for easily traversing the nodes in a given tree (depth first)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TreeIterator<T>
    {
        //-----------------------------------------------------------------------------------------------

        private Tree<T> tree         = null;
        private Tree<T>.Node current = null;

        //-----------------------------------------------------------------------------------------------
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="theTree"></param>
        public TreeIterator( Tree<T> theTree )
        {
            tree    = theTree;
            current = tree.Root;
        }

        //-----------------------------------------------------------------------------------------------
        /// <summary>
        /// Pass to next node - descends to first child if it exists
        /// </summary>
        /// <returns></returns>
        public Tree<T>.Node Next()
        {
            if (current.firstChild != null)
            {
                current = current.firstChild;
            }
            else if (current.nextSibling != null)
            {
                current = current.nextSibling;
            }
            else if (current.parent != null)
            {
                current = current.parent;

                while (current.nextSibling == null)
                {
                    current = current.parent;

                    if (current == null) return null;
                }

                current = current.nextSibling;
            }

            return current;
        }

        //-----------------------------------------------------------------------------------------------
        /// <summary>
        /// Pass to next node ignoring any children of the current node
        /// </summary>
        /// <returns></returns>
        public Tree<T>.Node Skip()
        {
            if (current.nextSibling != null)
            {
                current = current.nextSibling;
            }
            else if (current.parent != null)
            {
                current = current.parent;

                while (current.nextSibling == null)
                {
                    current = current.parent;

                    if (current == null) return null;
                }

                current = current.nextSibling;
            }

            return current;
        }
        //-----------------------------------------------------------------------------------------------
    }
}

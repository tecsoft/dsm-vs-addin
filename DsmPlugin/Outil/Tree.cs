using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using Tcdev.Dsm.Model;

namespace Tcdev.Collections.Generic
{
    /// <summary>
    /// A simple generic tree structure used for representing the type hierarchy
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Tree<T>
    {
        #region Definition of nested class Node
        /// <summary>
        /// A node in the tree
        /// </summary>
        public class Node : IState
        {
            private T nodeValue = default(T);

            internal int  childCount      = 0;
            internal Node firstChild      = null;
            internal Node lastChild       = null;
            internal Node nextSibling     = null;
            internal Node previousSibling = null;
            internal Node parent          = null;

            internal Node()
            {
                Depth = 0;
                IsCollapsed = true;
                IsHidden = false;
            }

            public Tree<T>.Node FirstChild
            {
                get { return firstChild; }
            }

            public Tree<T>.Node NextSibling
            {
                get { return nextSibling; }
            }

            public Tree<T>.Node Parent
            {
                get { return parent; }
            }

            public T NodeValue
            {
                get { return nodeValue; }
                set { nodeValue = value; }
            }

            public bool HasChildren
            {
                get { return this.childCount > 0; }
            }

            public IList<Tree<T>.Node> Children
            {
                get
                {
                    IList<Node> list = new List<Node>(this.childCount);

                    Node next = this.firstChild;

                    while (next != null)
                    {
                        list.Add(next);
                        next = next.nextSibling;
                    }

                    return list;
                }
            }

            #region IState Membres
            
            public bool CanCollapse
            {
                get { return HasChildren; }
            }

            //-------------------------------------------------------------------------------------------------
            private bool hidden;
            /// <summary>
            /// Gets or sets whether the module in the matrix is currently hidden or visible
            /// </summary>
            public bool IsHidden
            {
                get { return hidden; }
                set { hidden = value; }
            }

            //-------------------------------------------------------------------------------------------------
            private int depth;
            /// <summary>
            /// Gets or sets the depth in the hierarchy tree of the module
            /// </summary>
            public int Depth
            {
                get { return depth; }
                set { depth = value; }
            }

            ////-------------------------------------------------------------------------------------------------
            //private int id;
            ///// <summary>
            ///// Gets or sets the model ID of this module
            ///// </summary>
            //public int Id
            //{
            //    get { return id; }
            //    set { id = value; }
            //}
            
            private bool isCollapsed;
            public bool IsCollapsed
            {
                get { return isCollapsed; }
                set { isCollapsed = value; }
            }

            #endregion
        }

        #endregion

        private Node rootNode = null;
        private int count = 0;                // root node is not counted

        //-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Constructor
        /// </summary>
        public Tree()
        {
            rootNode = new Node();
        }

        //-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the root node
        /// </summary>
        internal Node Root
        {
            get { return rootNode; }
        }

        //-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Get the total number of nodes in the tree (excludes root node )
        /// </summary>
        public int Count
        {
            get { return count; }
        }
        //-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Node factory method
        /// </summary>
        /// <param name="theNodeValue">Valaue to be associated with this node</param>
        /// <returns></returns>
        public Tree<T>.Node CreateNode(T theNodeValue)
        {
            Tree<T>.Node newNode = new Tree<T>.Node();
            newNode.NodeValue = theNodeValue;

            return newNode;
        }
        //-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Adds a node to end of parents child list
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="node"></param>
        public void Add( Node parent, Tree<T>.Node node)
        {
            if (node == null)
                throw new ArgumentNullException("node");

            Node realParent = (parent == null) ? rootNode : parent;
            Node last = realParent.lastChild;

            node.parent = realParent;
            realParent.lastChild = node;
            realParent.childCount++;
            
            if (last == null)
            {
                realParent.firstChild = node;
            }
            else
            {
                last.nextSibling = node;
                node.previousSibling = last;
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Remove the node specified
        /// </summary>
        /// <param name="node"></param>
        public void Remove(Node node)
        {
            if (node == null)
                throw new ArgumentNullException("node");

            node.parent.childCount--;

            if (node.previousSibling == null)
            {
                node.parent.firstChild = node.nextSibling;
            }
            else
            {
                node.previousSibling.nextSibling = node.nextSibling;
            }

            if (node.nextSibling == null)
            {
                node.parent.lastChild = node.previousSibling;
            }
            else
            {
                node.nextSibling.previousSibling = node.previousSibling;
            }

            node.parent = null;
            node.previousSibling = null;
            node.nextSibling = null;
            
        }
        //-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Insert a node before the position
        /// </summary>
        /// <param name="node"></param>
        /// <param name="position"></param>
        public void InsertBefore(Tree<T>.Node node, Tree<T>.Node position )
        {
            if ( position == null )
                throw new ArgumentNullException("position" );

            if ( node == null )
                throw new ArgumentNullException("node");

            Tree<T>.Node parent = position.parent;
            node.parent = parent;
            parent.childCount++;

            if (parent.firstChild == position)
            {
                parent.firstChild = node;
                node.nextSibling = position;
                position.previousSibling = node;
            }
            else
            {
                node.nextSibling = position;
                node.previousSibling = position.previousSibling;
                position.previousSibling.nextSibling = node;
                position.previousSibling = node;
            }
        }
    }
}

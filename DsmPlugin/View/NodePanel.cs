using System;
using System.Drawing;
using Tcdev.Collections.Generic;
using Tcdev.Dsm.Model;

namespace Tcdev.Dsm.View
{
    /// <summary>
    /// Used to represent a type/namespace rectangle displayed in the TypePanel
    /// </summary>
    internal class NodePanel
    {
        public Tree<Module>.Node Node;
        public Rectangle Bounds;

        public NodePanel(Tree<Module>.Node node, Rectangle bounds)
        {
            Node = node;
            Bounds = bounds;
        }

        private NodePanel()
        {
            Node = null;
            Bounds = new Rectangle();
        }

        public bool HitTest(Point p)
        {
            return Bounds.Contains(p);
        }
    }
}

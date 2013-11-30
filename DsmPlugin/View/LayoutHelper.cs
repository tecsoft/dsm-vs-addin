using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Tcdev.Dsm.View
{
    internal class LayoutHelper
    {
        IList<NodePanel> _layout;
        //-----------------------------------------------------------------------------------------
        public LayoutHelper()
        {
            _layout = new List<NodePanel>();
        }
        //-----------------------------------------------------------------------------------------
        public void Add( NodePanel panel )
        {
            _layout.Add( panel );
        }
        //-----------------------------------------------------------------------------------------
        public void Clear()
        {
            _layout.Clear();
        }
        //-----------------------------------------------------------------------------------------
        public NodePanel LocatePanel(Point p)
        {
            NodePanel panel = null;

            for (int i = 0; i < _layout.Count && panel == null; i++)
            {
                if (_layout[i].HitTest(p))
                {
                    panel = _layout[i];
                }
            }

            return panel;
        }

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// Returns true if any the mouse at the given point has moved from the previous node panel to the new
        /// </summary>
        /// <param name="old"></param>
        /// <param name="current"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        static public bool MovedTest(NodePanel old, NodePanel current, Point p )
        {
            bool changed = false;

            if (current == null && old == null)
            {
                // no change
            }
            else if (old == null)
            {
                //  old was not set and current is not null so changed
                changed = true;
            }
            else if (current == null)
            {
                // old was set but current is no longer
                changed = true;
            }
            else
            {
                // old and new set
                changed = !old.HitTest(p);  // current is different from previous
            }

            return changed;
        }

        //-----------------------------------------------------------------------------------------
        


    }
}

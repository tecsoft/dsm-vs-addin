using System;
using System.Windows.Forms;

namespace Tcdev.Outil
{
    /// <summary>
    /// Used to hold current cursor state prior to an operation and then reset it afterwards
    /// </summary>
    public class CursorStateHelper
    {
        private Cursor initialState;
        private Control ctrl;
        /// <summary>
        /// Create state for the cursor of a given control
        /// </summary>
        /// <param name="control"></param>
        /// <param name="cursor"></param>
        public CursorStateHelper(Control control, Cursor cursor)
        {
            ctrl = control;
            initialState = ctrl.Cursor;
            ctrl.Cursor = cursor;
            ctrl.Refresh();
        }
        /// <summary>
        /// Reset the control cursor state
        /// </summary>
        public void Reset()
        {
            ctrl.Cursor = initialState;
            ctrl.Refresh();
        }
    }
}

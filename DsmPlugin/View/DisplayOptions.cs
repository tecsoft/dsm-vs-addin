using System;
using System.Collections.Generic;
using System.Text;

using System.Drawing;

namespace Tcdev.Dsm.View
{
    /// <summary>
    /// Groups view parameters for the matrix
    /// </summary>
    internal class DsmDisplayOptions
    {   
        MatrixControl _matrix;
        bool          _showCyclic;         
        int                _zoomLevel    = 3;                   // Default index to ZoomLevel array
        static ZoomLevel[] _zoomSettings = new ZoomLevel[ 6 ];  // Some standard displays - small to large
        
        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// Internal structure regrouping various hard coded settings for a particular zoom level 
        /// </summary>
        private struct ZoomLevel
        {
            public int RootHeight;
            public int CellHeight;
            public Font TextFont;

            public ZoomLevel( int rootHeight, int cellHeight, Font font )
            {
                RootHeight = rootHeight;
                CellHeight = cellHeight;
                TextFont = font;
            }
        }

        //-------------------------------------------------------------------------------------------
        /// <summary>
        /// Static constructor - Zoom levels are hard codes :-(
        /// </summary>
        static DsmDisplayOptions()
        {
            Font sysFont = SystemFonts.MessageBoxFont;
            //this.Font = new Font(sysFont.Name, sysFont.SizeInPoints, sysFont.Style);

            _zoomSettings[0] = new ZoomLevel(35, 14, new Font(sysFont.Name, 6));
            _zoomSettings[1] = new ZoomLevel(36, 16, new Font(sysFont.Name, 6));
            _zoomSettings[2] = new ZoomLevel(37, 17, new Font(sysFont.Name, 7));
            _zoomSettings[3] = new ZoomLevel(38, 18, new Font(sysFont.Name, 8));
            _zoomSettings[4] = new ZoomLevel(39, 19, new Font(sysFont.Name, 8));
            _zoomSettings[5] = new ZoomLevel(40, 21, new Font(sysFont.Name, 9));
        }

        //-------------------------------------------------------------------------------------------
        /// <summary>
        /// Constructor of display options for the matrix control
        /// </summary>
        /// <param name="ctrl"></param>
        public DsmDisplayOptions(MatrixControl ctrl)
        {
            _matrix = ctrl;
        }

        //-------------------------------------------------------------------------------------------
        /// <summary>
        /// Fixed size font for dependency weight values
        /// </summary>
        public Font WeightFont
        {
            get { return new Font( "Tahoma", 5.75f ); }
        }
        
        //-------------------------------------------------------------------------------------------
        /// <summary>
        /// Turn on or off cyclic relation highlighter
        /// </summary>
        public bool ShowCyclicRelations
        {
            get 
            { 
                return _showCyclic; 
            }
            set
            {
                _showCyclic = value;
                _matrix.NodeListModified(false);
            }
        }

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the number of pixels for the height of matrix cells at the current zoom level
        /// </summary>
        public int CellHeight
        {
            get
            {
                return ZoomSetting.CellHeight;
            }
        }

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the height of the header row in pixels at the current zoom level
        /// </summary>
        public int RootHeight
        {
            get
            {
                return ZoomSetting.RootHeight;
            }
        }

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// Get the font for type labels at the current zoom level
        /// </summary>
        public Font TextFont
        {
            get
            {
                return ZoomSetting.TextFont;
            }
        }
        //-------------------------------------------------------------------------------------------
        /// <summary>
        /// Set the zoom level
        /// </summary>
        /// <param name="level"></param>
        public void SetZoomLevel( int level )
        {
            if (level != _zoomLevel)
            {
                if (level < 1)
                    _zoomLevel = 1;
                else if (level > 5)
                    _zoomLevel = 5;
                else
                    _zoomLevel = level;

                _matrix.NodeListModified(true);
            }
        }
        //-------------------------------------------------------------------------------------------
        
        ZoomLevel ZoomSetting
        {
            get { return _zoomSettings[_zoomLevel]; }
        }

        
    }
}

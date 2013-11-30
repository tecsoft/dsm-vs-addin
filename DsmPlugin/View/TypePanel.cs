using System;
//using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Collections.Generic;
using Tcdev.Dsm.Model;
using Tcdev.Collections.Generic;

namespace Tcdev.Dsm.View
{
	/// <summary>
	/// This is the panel which displays the types/namespaces and racts to expand/collapse requests
	/// </summary>
	public class TypePanel : System.Windows.Forms.UserControl
    {
        private IContainer components = null;

        Pen          _borderPen;         // panel _borderPen pen
        //Pen          _fcPen;             // main font color Pen
        //Brush        _fcBrush;           // main font color Brush
        StringFormat _vStringFormat;     // vertical string format

        Image _imgExpanded;  
        Image _imgCollapsed;

        // TODO Correct public fields
        public MatrixControl Controller;
        public Rectangle ViewRectangle;

        LayoutHelper _layout;
        ToolTip      _tooltip;
        Timer        _ttTimer;
        NodePanel    _nodePanel;

        //-------------------------------------------------------------------------------------------------
        /// <summary>
        /// Constructor
        /// </summary>
		public TypePanel()
		{
			InitializeComponent();

            Font sysFont = SystemFonts.MessageBoxFont;
            this.Font = new Font(sysFont.Name, sysFont.SizeInPoints, sysFont.Style);

            _borderPen = new Pen(Brushes.DarkGray, 1);
            //_fcPen     = new Pen(Brushes.Black, 1);
            //_fcBrush   = new SolidBrush(this.ForeColor);
           
            _vStringFormat =  new StringFormat(StringFormatFlags.DirectionVertical);

            _imgExpanded  = Tcdev.Dsm.Properties.Resources.Expanded;
            _imgCollapsed = Tcdev.Dsm.Properties.Resources.Collpased;

            _tooltip = new ToolTip();
            _ttTimer = new Timer();
            _ttTimer.Interval = 4000;
            _ttTimer.Tick += new EventHandler(_ttTimer_Tick);

            _layout = new LayoutHelper();
		}

        //-------------------------------------------------------------------------------------------------

        void _ttTimer_Tick(object sender, EventArgs e)
        {
            if (this._tooltip.Active)
            {
                this._tooltip.Active = false;
                _ttTimer.Stop();
            }
        }
        //-------------------------------------------------------------------------------------------------
		/// <summary> 
		/// Nettoyage des ressources utilisées.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}
        //-------------------------------------------------------------------------------------------------
		#region Code généré par le Concepteur de composants
		/// <summary> 
		/// Méthode requise pour la prise en charge du concepteur - ne modifiez pas 
		/// le contenu de cette méthode avec l'éditeur de code.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            
            this.SuspendLayout();
            // 
            // TypePanel
            // 
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Name = "TypePanel";
            this.Size = new System.Drawing.Size(224, 3200);
            this.DoubleClick += new System.EventHandler(this.TypePanel_DoubleClick);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.TypePanel_MouseMove);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.TypePanel_MouseClick);
            this.ResumeLayout(false);

		}
		#endregion

        //-------------------------------------------------------------------------------------------------
		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

            if (Controller != null && Controller.MatrixModel != null )
            {
                using (Graphics g = e.Graphics)
                {
                    _layout.Clear();

                    using (Bitmap image = new Bitmap(this.ViewRectangle.Width, ViewRectangle.Height))
                    {
                        using (Graphics g1 = Graphics.FromImage(image))
                        {
                            g1.Clip = new Region(this.ViewRectangle);

                            Draw(g1);

                            g.DrawImageUnscaled( image, 0,0 ); //already clipped 
                        }
                    }
                }
            }
		}
        //-------------------------------------------------------------------------------------------------
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            if (Controller == null || Controller.MatrixModel == null)
            {
                base.OnPaintBackground(e);
                string str = "No matrix is currently loaded";
                SizeF strSize = e.Graphics.MeasureString( str, Font );
                e.Graphics.DrawString(str, this.Font, SystemBrushes.GrayText, 
                    ((int)this.Width / 2) - ((int)strSize.Width /2), 16);
            }
        }
        //-------------------------------------------------------------------------------------------------
        void Draw(Graphics g)
        {
            int y = -Controller.OffsetY + Controller.DisplayOptions.RootHeight;

            TreeIterator<Module> iterator = new TreeIterator<Module>(Controller.MatrixModel.Hierarchy);
            Tree<Module>.Node node = iterator.Next();

            while (node != null)
            {
                Module module = node.NodeValue;

                if (node.IsHidden == false )
                {
                    if (node.IsCollapsed == false)
                    {
                        Rectangle bounds = new Rectangle(
                            node.Depth * Controller.DisplayOptions.CellHeight,
                            y,
                            Controller.DisplayOptions.CellHeight,
                            Controller.CountNbDisplayableNested(node) * Controller.DisplayOptions.CellHeight);

                        if (g.Clip.IsVisible(bounds))
                        {
                            DrawPanel(g, bounds, node);
                            _layout.Add(new NodePanel(node, bounds));
                        }

                        // y position does not change for next node

                        node = iterator.Next();

                    }
                    else
                    {
                        // node is collapsed - draw the module at position
                        // if the node has childrenskip them
                        int x = node.Depth * Controller.DisplayOptions.CellHeight;

                        Rectangle bounds = 
                            new Rectangle(x, y, Size.Width - x, Controller.DisplayOptions.CellHeight);

                        if (g.Clip.IsVisible(bounds))
                        {
                            DrawPanel(g, bounds, node);
                            _layout.Add(new NodePanel(node, bounds));
                        }

                        // position for next panel
                        y += Controller.DisplayOptions.CellHeight;

                        node = iterator.Skip();
                    }
                }
                else
                {
                    node = iterator.Next();
                }
            }

            this.Size = new Size(Size.Width, y + Controller.OffsetY);
            g.DrawRectangle(_borderPen, new Rectangle(0, 0, this.Width - 1, this.Height - 1));

            Rectangle rootBounds = new Rectangle(0, 0, Size.Width - 1, Controller.DisplayOptions.RootHeight - 2);
            DrawRootPanel(g, rootBounds);

        }
        //-------------------------------------------------------------------------------------------------
        void DrawPanel(Graphics g, Rectangle bounds, Tree<Module>.Node node )
        {
            Tcdev.Dsm.Model.Module module = node.NodeValue;
            
            if ( Controller.ProviderNode == node )
            {
                g.FillRectangle( Brushes.White, bounds );
            }
            else
            {
                g.FillRectangle(Controller.GetBackgroundColour(node, null), bounds);
            }

            //g.FillPolygon(
            //    Brushes.Red,
            //   new Point[] { 
            //       new Point(bounds.Right - 10, bounds.Top), 
            //       new Point( bounds.Right, bounds.Top ),
            //       new Point( bounds.Right, bounds.Top +10)
            //   }
            //    );
            
            if ( node.HasChildren )
            {
                if ( node.IsCollapsed )
                {
                    g.DrawImage( _imgCollapsed, bounds.Left + 4, bounds.Top + 4 );
                    g.DrawString( module.Name  + " - " + module.Id, 
                        GetNodeFont(node), Brushes.Black, bounds.Left + 18, bounds.Top + 2 );
                }
                else
                {
                    g.DrawImage( _imgExpanded,bounds.Left + 4, bounds.Top + 4 );
                    g.DrawString( module.Name , GetNodeFont(node), Brushes.Black, 
                        bounds.Left, bounds.Top + 18, _vStringFormat);
                }
            }
            else
            {
                g.DrawString(module.Name + " - " + module.Id, GetNodeFont(node), Brushes.Black, 
                    bounds.Left + 2, bounds.Top + 2);
            }

            

            g.DrawRectangle( _borderPen, bounds );
        }

        //-------------------------------------------------------------------------------------------------
        
        void DrawRootPanel( Graphics g, Rectangle bounds )
        {
            g.FillRectangle(SystemBrushes.ControlLight, bounds);

            Tree<Module>.Node sn = Controller.MatrixModel.SelectedNode;
            string text;
            if (sn != null)
            {
                text = sn.NodeValue.FullName;
            }
            else
            {
                text = "<No module currently selected>";
            }

            g.DrawString(text, Controller.DisplayOptions.TextFont, 
                Brushes.CornflowerBlue, bounds.X + 1, bounds.Y + 3 );

            g.DrawRectangle(_borderPen, bounds);

            g.DrawLine(new Pen(Brushes.White, 1),
                bounds.Left, bounds.Bottom + 1, bounds.Right, bounds.Bottom + 1);

        }
        //-------------------------------------------------------------------------------------------------
        void TypePanel_DoubleClick(object sender, EventArgs e)
        {
            if (this.Controller.Enabled)
            {
                Point pos = this.PointToClient(Control.MousePosition);

                if (pos.Y > Controller.DisplayOptions.RootHeight)
                {
                    Controller.ExpandSelectedNode();
                }
            }
        }
        
        //-------------------------------------------------------------------------------------------------
        void TypePanel_MouseClick(object sender, MouseEventArgs e)
        {
            if (this.Controller.Enabled)
            {
                NodePanel nodePanel = _layout.LocatePanel(e.Location);

                if (nodePanel != null)
                {
                    Controller.SelectNode(nodePanel.Node);
                    
                    if (e.Button == MouseButtons.Right)
                    {
                        if (Controller.ContextMenuIsVisible)
                        {
                            Controller.HideContextMenu();
                            this.Invalidate();
                        }
                        else
                        {
                            Controller.ShowContextMenu(this.PointToScreen(e.Location));
                        }
                    }
                }
            }
        }
        //-------------------------------------------------------------------------------------------------
        Font GetNodeFont(Tree<Module>.Node node)
        {
            if (node == Controller.MatrixModel.SelectedNode)
            {
                return new Font(Controller.DisplayOptions.TextFont, FontStyle.Bold);
            }
            else
            {
                return Controller.DisplayOptions.TextFont;
            }
        }

        //-------------------------------------------------------------------------------------------------

        void DoTooltipAfterMouseMove(Point p)
        {
            NodePanel current = _layout.LocatePanel(p);

            if( LayoutHelper.MovedTest(_nodePanel, current, p ) )
            {
                _ttTimer.Stop();
                
                if ( current == null ) return;
                
                //Controller.ProviderModule = current.Node.NodeValue;
                Controller.SetCurrentModules( current.Node, Controller.ConsumerNode );
                
                _nodePanel = current;

                if (current == null)
                {
                    _tooltip.SetToolTip(this, String.Empty);
                }
                else
                {
                    _tooltip.SetToolTip(this, current.Node.NodeValue.FullName);
                }
                _tooltip.Active = true;
                _ttTimer.Start();
            }
        }

        //-------------------------------------------------------------------------------------------------

        private void TypePanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.Controller.Enabled)
            {
                DoTooltipAfterMouseMove(e.Location);
            }
        }
        //-------------------------------------------------------------------------------------------------
     }
}

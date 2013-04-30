using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

using Tcdev.Dsm.Model;

using Tcdev.Collections.Generic;

namespace Tcdev.Dsm.View
{
	/// <summary>
	/// This is the Matrix control showing the weights between the different types
	/// </summary>
	public class MatrixPanel : System.Windows.Forms.UserControl
	{
		//private System.ComponentModel.IContainer components;

        // TODO Correct public fields
		public bool HideClosedSets = false;
        public MatrixControl Controller;
        public Rectangle ViewRectangle;

        Pen          _borderPen;
        Pen          _fcPen;
        Brush        _fcBrush;
        StringFormat _vStringFormat;

        LayoutHelper _hLayout;  //list of horizontal panels
        LayoutHelper _vLayout;  // list of vertical panels
        
        //NodePanel    _hPanel; // current horizontal panel
        //NodePanel    _vPanel; // current vertical panel

        ToolTip _tooltip;
        Timer   _ttTimer;
        
        Image _imgExpanded;
        Image _imgCollapsed;

        //-------------------------------------------------------------------------------------------------
        /// <summary>
        /// Contstructor
        /// </summary>
		public MatrixPanel( )
		{
			// Cet appel est requis par le Concepteur de formulaires Windows.Forms.
			InitializeComponent();

            Font sysFont = SystemFonts.MessageBoxFont;
            this.Font = new Font(sysFont.Name, sysFont.SizeInPoints, sysFont.Style);

            _borderPen     = new Pen(Brushes.DarkGray, 1);
            _fcPen         = new Pen(Brushes.Black, 1);
            _fcBrush       = Brushes.Black;
            _vStringFormat = new StringFormat(StringFormatFlags.DirectionVertical);

            _tooltip = new ToolTip();
            _ttTimer = new Timer();
            _ttTimer.Interval = 4000;
            _ttTimer.Tick += new EventHandler(_ttTimer_Tick);

            _hLayout = new LayoutHelper();
            _vLayout = new LayoutHelper();

            _imgExpanded = Tcdev.Dsm.Properties.Resources.Expanded;
            _imgCollapsed = Tcdev.Dsm.Properties.Resources.Collpased;
  
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
            //if( disposing )
            //{
            //    if(components != null)
            //    {
            //        components.Dispose();
            //    }
            //}
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
            this.SuspendLayout();
            // 
            // MatrixPanel
            // 
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Name = "MatrixPanel";
            this.Size = new System.Drawing.Size(669, 366);
            this.DoubleClick += new System.EventHandler(this.MatrixPanel_DoubleClick);
            this.MouseLeave += new System.EventHandler(this.MatrixPanel_MouseLeave);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.MatrixPanel_MouseMove);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.MatrixPanel_MouseClick);
            this.ResumeLayout(false);

		}
		#endregion

        //-------------------------------------------------------------------------------------------------
		
		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

            if (Controller != null && Controller.MatrixModel != null)
            {
                using( Graphics g = e.Graphics ) // FIXME - is using necessary ?
                {
                    _hLayout.Clear();
                    _vLayout.Clear();

                    using (Bitmap image = new Bitmap(this.ViewRectangle.Width, ViewRectangle.Height))
                    {
                        using (Graphics g1 = Graphics.FromImage(image))
                        {
                            g1.Clip = new Region(ViewRectangle);
                            
                            try
                            {
                            Draw(g1);
}
catch(Exception ex )
{
    MessageBox.Show( ex.StackTrace );
}
                            g.DrawImageUnscaled(image, 0,0);
                        }
                    }
                }
            }
		}
        //-------------------------------------------------------------------------------------------------
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            if (Controller == null || Controller.MatrixModel == null) // for designer
            {
                base.OnPaintBackground(e);
            }
        }
        //-------------------------------------------------------------------------------------------------
        void Draw(Graphics g)
        {
            int y = -Controller.OffsetY + Controller.DisplayOptions.RootHeight;

            TreeIterator<Module> iterator = new TreeIterator<Module>(Controller.MatrixModel.Hierarchy);

            Tree<Module>.Node node = iterator.Next();

            //
            // Root has priority - so it is painted last but its panel is saved first in the hList
            if (node != null)
            {
                Rectangle rootBounds =new Rectangle(
                    -Controller.OffsetX, 0, 
                    Size.Width, Controller.DisplayOptions.RootHeight);

                // note  for root hpanel node of nodepanel is null
                _hLayout.Add(new NodePanel(null, rootBounds));
            }

            while (node != null )
            {
                Module module = node.NodeValue;

                if (node.IsCollapsed == false || node.IsHidden )
                {
                    node = iterator.Next();
                }
                else
                {
                    DrawPanel(g, node, y);
                    y += Controller.DisplayOptions.CellHeight;

                    node = iterator.Skip();
                }
            }

            this.Size = new Size(y + Controller.OffsetY - Controller.DisplayOptions.RootHeight + 1, y + Controller.OffsetY + 1);

            DrawGroupingSquares(g);
            
            DrawRootPanel(g);
            
        }

        //-------------------------------------------------------------------------------------------------
        void DrawGroupingSquares( Graphics g )
        {
            TreeIterator<Module> iterator = new TreeIterator<Module>(Controller.MatrixModel.Hierarchy);
            Tree<Module>.Node node = iterator.Next();

            int xPos = -Controller.OffsetX +1;
            int yPos = -Controller.OffsetY + Controller.DisplayOptions.RootHeight + 1;
            while (node != null)
            {
                if (node.IsHidden == false)
                {
                    if (node.IsCollapsed == false)
                    {
                        int width = Controller.CountNbDisplayableNested(node) * Controller.DisplayOptions.CellHeight;
                        Rectangle rect = new Rectangle(xPos, yPos, width - 1, width - 1);

                        if (g.Clip.IsVisible(rect))
                        {
                            g.DrawRectangle(new Pen(Brushes.DarkGray, 2), rect);
                        }
                        node = iterator.Next();
                    }
                    else
                    {
                        xPos += Controller.DisplayOptions.CellHeight;
                        yPos += Controller.DisplayOptions.CellHeight;
                        node = iterator.Skip();
                    }
                }
                else
                {
                    node = iterator.Next();
                }

            }
        }
        //-------------------------------------------------------------------------------------------------
        private void DrawRootPanel(Graphics g)
        {
            int stateDisplay = 0;  // tri-state optimisation 0 not started dispaying, 1 currently displaying
                                   // 2 finished displaying and can therefore break out of the loop

            int x = -Controller.OffsetX;

            Rectangle rootBounds = new Rectangle(x, 0, Size.Width, Controller.DisplayOptions.RootHeight);
                    
            TreeIterator<Module> iterator = new TreeIterator<Module>(Controller.MatrixModel.Hierarchy);
            Tree<Module>.Node node = iterator.Next();
            while (node != null && stateDisplay != 2)
            {
                Module module = node.NodeValue;

                if ( node.IsCollapsed == false ||  node.IsHidden )
                {
                    node = iterator.Next();
                }
                else
                {
                    Rectangle cell = new Rectangle(x, 0,
                        Controller.DisplayOptions.CellHeight, Controller.DisplayOptions.RootHeight - 2);

                    if (g.Clip.IsVisible(cell))
                    {
                        // for each visible cell we create a vertical panel in vLayout of
                        // height of _matrix
                        Rectangle vPanelRec =
                            new Rectangle(x, 0, Controller.DisplayOptions.CellHeight, Size.Height);
                        _vLayout.Add( new NodePanel( node, vPanelRec));

                        if ( Controller.ConsumerNode == node)
                        {
                            g.FillRectangle( Brushes.White, cell );
                        }
                        else
                        {
                            g.FillRectangle(Controller.GetBackgroundColour(node, null), cell);
                        }
                        
                        if ( node.HasChildren )
                        {
                            g.DrawImage( _imgCollapsed, cell.Left + ( cell.Width / 2.0f ) - 4, cell.Top + 2 );
                        }
                        
                        g.DrawString(module.Id.ToString(), Controller.DisplayOptions.TextFont, 
                            _fcBrush, x, 10, _vStringFormat);
                            
                        g.DrawRectangle(_borderPen, cell);
                        g.DrawLine(new Pen(Brushes.White, 1),
                            cell.Left, cell.Bottom + 1, cell.Right, cell.Bottom + 1);

                        if (stateDisplay == 0) 
                            stateDisplay++;
                    }
                    else if (stateDisplay == 1) 
                        stateDisplay++; // finished displaying

                    x += Controller.DisplayOptions.CellHeight;
                    
                    node = iterator.Skip();
                }
            }
        }
        //-------------------------------------------------------------------------------------------------
        void DrawPanel(Graphics g, Tree<Module>.Node rowNode, int y )
        {
            Rectangle rowBounds = new Rectangle(0, y, Size.Width, Controller.DisplayOptions.CellHeight);

            if (g.Clip.IsVisible(rowBounds))
            {
                _hLayout.Add( new NodePanel( rowNode, rowBounds));

                // can draw some of the row
                int x = -Controller.OffsetX;

                TreeIterator<Module> iterator = new TreeIterator<Module>(Controller.MatrixModel.Hierarchy);

                Tree<Module>.Node node = iterator.Next();

                Module rowModule = rowNode.NodeValue;

                int stateDisplay = 0;  
                // trisstate optimisation 0 not started dispaying, 1 currently displaying
                // 2 finished displaying and can therefore break out of the loop

                while (node != null && stateDisplay != 2)
                {
                    Module module = node.NodeValue;

                    if (node.IsCollapsed == false || node.IsHidden )
                    {
                        node = iterator.Next();
                    }
                    else
                    {
                        Rectangle cell = new Rectangle(
                            x, y, Controller.DisplayOptions.CellHeight, Controller.DisplayOptions.CellHeight);

                        if (g.Clip.IsVisible(cell))
                        {
                            if (node == rowNode) // the diagonal
                            {
                                g.FillRectangle(Controller.GetBackgroundColour(rowNode, node), cell); // TODO - OK
                            }
                            else
                            {
                                if (Controller.DisplayOptions.ShowCyclicRelations &&
                                    Model.DsmModel.HasCyclicRelation(rowModule, module))
                                {
                                    g.FillRectangle(Brushes.Yellow, cell);
                                }
                                //else if ( (_hPanel != null && _hPanel.Node == rowNode ) ||
                                //    (_vPanel != null && _vPanel.Node == node ) )
                                else if ( Controller.ProviderNode == rowNode || 
                                    Controller.ConsumerNode == node )
                                {
                                    g.FillRectangle( Brushes.White, cell );
                                }
                                else
                                {
                                    g.FillRectangle( Controller.GetBackgroundColour( rowNode, node ), cell );
                                    //if (rowNode.Parent == node.Parent )
                                    //{
                                    //    g.FillRectangle(
                                    //        Controller.GetBackgroundColour(rowNode.Parent), cell); // TODO - OK
                                    //}
                                    //else
                                    //{
                                    //    //g.FillRectangle(Brushes.AliceBlue, cell);
                                    //    g.FillRectangle(
                                    //       Controller.GetBackgroundColour( node ), cell );
                                    //}
                                }
                                DrawWeight(g, rowModule, module, cell);
                            }
                            
                            g.DrawRectangle(_borderPen, cell);
                            
                            if (stateDisplay == 0) 
                                stateDisplay++;
                        }
                        else if (stateDisplay == 1) 
                            stateDisplay++;

                        x += Controller.DisplayOptions.CellHeight;
                        node = iterator.Skip();
                    }
                }
            }
        }

        //-------------------------------------------------------------------------------------------------
        void DrawWeight(Graphics g, Module provider, Module consumer, Rectangle cell)
        {
            Relation relation = provider.GetRelation(consumer);

            int weight = 0;
            if (relation != null) weight = relation.Weight;

            if (weight > 0)
            {
                g.DrawString(weight.ToString(), Controller.DisplayOptions.WeightFont,
                    Brushes.Black, cell.Left + 1, cell.Top + 1);
            }
        }

        //-------------------------------------------------------------------------------------------------

        private void MatrixPanel_MouseMove(object sender, MouseEventArgs e)
        {
        try
        
        {
            // TODO review null values etc
            
            if (Controller.Enabled)
            {
                NodePanel hCurrent = _hLayout.LocatePanel( e.Location );
                NodePanel vCurrent = _vLayout.LocatePanel( e.Location );
                
                //if ( HasMouseMovedCell( e.Location, hCurrent, vCurrent ) )
                
                //if ( Controller.RowNode != hCurrent.Node || Controller.ColNode != vCurrent.Node )
                if ( ( hCurrent != null && hCurrent.Node != null && Controller.ProviderNode != hCurrent.Node ) ||
                    ( vCurrent != null && vCurrent.Node != null && Controller.ConsumerNode != vCurrent.Node ) )
                {
                    //_vPanel = vCurrent;
                    //_hPanel = hCurrent;
                    
                    // change in position
                    //Controller.ProviderModule = (hCurrent == null ) ? null : hCurrent.Node.NodeValue;
                    //Controller.ConsumerModule = (vCurrent == null ) ? null : vCurrent.Node.NodeValue;

                    Controller.SetCurrentModules( 
                        ( hCurrent == null || hCurrent.Node == null) ? null : hCurrent.Node,
                        ( vCurrent == null || vCurrent.Node == null) ? null : vCurrent.Node);

                    DoTooltipAfterMouseMove(hCurrent, vCurrent );
                }
            }
            
            }catch(Exception ex )
            {
                MessageBox.Show( "mp mouse move: " + ex.StackTrace );
            }
        }

        //-------------------------------------------------------------------------------------------------

        string TooltipString(Module mod)
        {
            if ( mod == null ) MessageBox.Show("string tooltip" );
            
            return string.Format("{1} [{0}]", mod.Id, mod.FullName);
        }

        //-------------------------------------------------------------------------------------------------
        
        //bool HasMouseMovedCell( Point p, NodePanel h, NodePanel v )
        //{
        //    return LayoutHelper.MovedTest( Controll, h, p ) || LayoutHelper.MovedTest(_vPanel, h, p );
        //}

        //-------------------------------------------------------------------------------------------------
        void DoTooltipAfterMouseMove(NodePanel hCurrent, NodePanel vCurrent )
        {
        try
        {
            _ttTimer.Stop();

            if ( hCurrent == null && vCurrent == null )
                return;

            if (hCurrent != null && vCurrent != null)
            {
                if (hCurrent.Node == null)
                {
                    // header
                    _tooltip.SetToolTip(this, TooltipString(vCurrent.Node.NodeValue) );
                }
                else // _matrix
                {
                    
                    if (vCurrent.Node == hCurrent.Node)
                    {
                        _tooltip.SetToolTip(this, TooltipString(vCurrent.Node.NodeValue) );
                    }
                    else
                    {
                        Relation rel = 
                            hCurrent.Node.NodeValue.GetRelation(vCurrent.Node.NodeValue);

                        int weight = 0;

                        if ( rel != null ) weight = rel.Weight;

                        _tooltip.SetToolTip(this,
                            "Provider: " + TooltipString(hCurrent.Node.NodeValue ) +
                            Environment.NewLine +
                            "Consumer: " + TooltipString(vCurrent.Node.NodeValue ) +
                            Environment.NewLine +
                            "Weight: " + weight);
                    }
                }
            }
            else
            {
                _tooltip.SetToolTip(this, String.Empty);
            }

            this.Invalidate();
            _tooltip.Active = true;
            _ttTimer.Start();
            }
            catch(Exception e )
            {
                MessageBox.Show( e.StackTrace );
            }
        }

        private void MatrixPanel_MouseLeave( object sender, EventArgs e )
        {
            //_hPanel = null;
            //_vPanel = null;
            //Controller.ProviderModule = null;
            //Controller.ConsumerModule = null;
            
            //this.Invalidate();
            
            try
            {
            
            Controller.SetCurrentModules( null, null );
            
            }
            catch( Exception ex )
            {
                MessageBox.Show( ex.StackTrace );
            }
        }

        private void MatrixPanel_DoubleClick( object sender, EventArgs e )
        {
            if (this.Controller.Enabled)
            {
                Controller.ExpandCurrentNode( );
            }
                //Point pos = this.PointToClient(Control.MousePosition);

                //if (pos.Y > Controller.DisplayOptions.RootHeight)
                //{
                //    Controller.ExpandSelectedNode();
                //}
            //}
        }

        private void MatrixPanel_MouseClick(object sender, MouseEventArgs e)
        {
            if (this.Controller.Enabled)
            {
                NodePanel providerNodePanel = _hLayout.LocatePanel(e.Location);
                NodePanel consumerNodePanel = _vLayout.LocatePanel(e.Location);

                if (providerNodePanel != null && consumerNodePanel != null )
                {
                    Controller.SelectProviderNode(providerNodePanel.Node);
                    Controller.SelectConsumerNode(consumerNodePanel.Node);

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
	}
}

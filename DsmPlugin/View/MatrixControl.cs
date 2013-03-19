using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

using Tcdev.Dsm.Model;
using Tcdev.Collections.Generic;
using Tcdev.Dsm.Commands;
using Tcdev.Outil;


namespace Tcdev.Dsm.View
{
	/// <summary>
	/// The matrix control which is the immediate parent of the TypePanel and MatrixPanels coordinating
    /// them in response to scrolling and resizing operations etc.
	/// </summary>
    public class MatrixControl : System.Windows.Forms.UserControl
    {
        private IContainer components;
        
        private SplitContainer   _splitContainer;
        private MatrixPanel      _matrix;
        private TypePanel        _selector;
        private HScrollBar       _hScrollBar;
        private VScrollBar       _vScrollBar;
        private ContextMenuStrip _cntxtMenuStrip;
        private DsmDisplayOptions _displayOptions;

        // TODO Correct pblic fields
        public ToolStripMenuItem cntxtItemMoveUp;
        public ToolStripMenuItem cntxtItemMoveDown;

        private Brush _brush1 = Brushes.LightCyan;
        private Brush _brush2 = Brushes.BlanchedAlmond;
        private Brush _brush3 = Brushes.Lavender;
        private Brush _brush4 = Brushes.MistyRose;

        public Tcdev.Dsm.Model.DsmModel MatrixModel;
        
        //internal Tree<Module>.Node RowNode = null;
        //internal Tree<Module>.Node ColNode = null;
        
        //private Module _providerModule;
        //private Module _consumerModule;
        //internal Node ProviderModule
        //{
        //    get { return _providerModule; }
        //    private set { _providerModule = value; }
        //}
        
        //internal Module ConsumerModule
        //{
        //    get { return _consumerModule; }
        //    private set { _consumerModule = value; }
        //}
        
        internal Tree<Module>.Node ProviderNode { get; set; }
        internal Tree<Module>.Node ConsumerNode { get; set; }

        internal void SetCurrentModules( Tree<Module>.Node provider, Tree<Module>.Node consumer )
        {
            ProviderNode = provider;
            ConsumerNode = consumer;
            
            _selector.Invalidate();
            _matrix.Invalidate();
        }
        

        //-------------------------------------------------------------------------------------------

        public int OffsetY = 0;
        private ToolStripMenuItem paritionToolStripMenuItem;
        public int OffsetX = 0;

        //-------------------------------------------------------------------------------------------
        public MatrixControl()
        {         
            // Cet appel est requis par le Concepteur de formulaires Windows.Forms.
            InitializeComponent();

            _displayOptions = new DsmDisplayOptions(this);

            _selector.Controller = this;
            _matrix.Controller   = this;
            
            this._matrix.SizeChanged += new EventHandler(Matrix_SizeChanged);

            _hScrollBar.Value = 0;
            _hScrollBar.Minimum = 0;
            _hScrollBar.SmallChange = _displayOptions.CellHeight;
 
            _vScrollBar.Value = 0;
            _vScrollBar.Minimum = 0;
            _vScrollBar.SmallChange = _displayOptions.CellHeight;
        }

        //-------------------------------------------------------------------------------------------

        internal DsmDisplayOptions DisplayOptions
        {
            get { return _displayOptions; }
        }

        //-------------------------------------------------------------------------------------------
        internal void NodeListModified(bool resize )
        {
            if (resize)
            {
                CalculatePanelSizes();
            }

            ResizeControl();

            _selector.ViewRectangle = this._splitContainer.Panel1.ClientRectangle;
            _selector.Invalidate();
            _matrix.ViewRectangle = this._splitContainer.Panel2.ClientRectangle;
            _matrix.Invalidate();

            Update();
        }

        //-------------------------------------------------------------------------------------------
        internal void CalculatePanelSizes()
        {
            int count = 0;

            if (MatrixModel != null && MatrixModel.Hierarchy != null)
            {
                TreeIterator<Module> iterator = new TreeIterator<Module>(MatrixModel.Hierarchy);
                Tree<Module>.Node node = iterator.Next();

                while (node != null)
                {
                    if (node.IsCollapsed)
                    {
                        count++;
                        node = iterator.Skip();
                    }
                    else
                    {
                        node = iterator.Next();
                    }
                }
            }

            this._selector.Size = new Size( this._selector.Width, 
                count * _displayOptions.CellHeight + _displayOptions.RootHeight);

            this._matrix.Size = new Size( count * _displayOptions.CellHeight,           
                count * _displayOptions.CellHeight + _displayOptions.RootHeight);
        }
        //-------------------------------------------------------------------------------------------

        internal int CountNbDisplayableNested(Tree<Module>.Node node)
        {
            int nb = 0;

            foreach (Tree<Module>.Node child in node.Children)
            {
                if (child.IsHidden == false)
                {
                    if (child.IsCollapsed)
                    {
                        nb++;
                    }
                    else
                    {
                        nb += CountNbDisplayableNested(child);
                    }
                }
            }

            return nb;
        }

        //-------------------------------------------------------------------------------------------
        internal System.Drawing.Brush GetBackgroundColour(Tree<Module>.Node rowNode, Tree<Module>.Node colNode )
        {
            int depth = 1;
            if ( colNode == null )
                depth = rowNode.Depth;
            else
            {
                if ( rowNode == colNode )
                    depth = rowNode.Depth;
                else if ( rowNode != null && rowNode.Parent != null && rowNode.Parent.NodeValue != null && rowNode.Parent == colNode.Parent )
                    depth = rowNode.Parent.Depth;
                else if ( rowNode.Parent != null && rowNode.Parent.NodeValue != null 
                    && colNode.Parent != null && colNode.Parent.NodeValue != null)
                    depth = Math.Min(rowNode.Parent.Depth, colNode.Parent.Depth );
            }

            switch (Math.Abs(depth) % 4)
            {
                case 0: return _brush1;
                case 1: return _brush2;
                case 2: return _brush3;
                case 3: return _brush4;
                default:
                    throw new ApplicationException("Logic error in GetBackgroundColour");
            }
        }

        //-------------------------------------------------------------------------------------------------

        internal void SelectNode(Tree<Module>.Node node)
        {
            MatrixModel.SelectedNode = node;
            NodeListModified(false);
            EnableButtons();
        }

        //-------------------------------------------------------------------------------------------------

        internal void ExpandSelectedNode()
        {
            Tree<Module>.Node node = MatrixModel.SelectedNode;
            
            if (node != null && node.HasChildren)
            {
                //Module module = MatrixModel.SelectedNode.NodeValue;
                node.IsCollapsed = !node.IsCollapsed;
                NodeListModified(true);
            }
        
        }
        
        internal void ExpandCurrentNode( )
        {
            //MessageBox.Show(ProviderModule == null ? "null" : "notnull" );
            //MessageBox.Show( ConsumerModule == null ? "null" : "notnull" );
            if ( ProviderNode == null && ConsumerNode != null)
            {
                // TODO need to know if current nodes are expandable !!
                
                if ( ConsumerNode.HasChildren )
                {
                    ConsumerNode.IsCollapsed = !ConsumerNode.IsCollapsed;
                    NodeListModified( true );
                }
            }
        }

        //-------------------------------------------------------------------------------------------------
        internal void MoveSelectedNodeUp()
        {
            if (MatrixModel.MoveUp())
            {
                NodeListModified(false);
                EnableButtons();
                MatrixModel.Modified = true;
            }
        }

        //-------------------------------------------------------------------------------------------------
        internal void MoveSelectedNodeDown()
        {
            if (MatrixModel.MoveDown())
            {
                NodeListModified(false);
                EnableButtons();
                MatrixModel.Modified = true;
            }
        }

        //-------------------------------------------------------------------------------------------
        internal void ShowContextMenu(Point position)
        {
            this._cntxtMenuStrip.Show(position);
        }

        //-------------------------------------------------------------------------------------------
        internal void HideContextMenu()
        {
            this._cntxtMenuStrip.Hide();
        }
        //-------------------------------------------------------------------------------------------
        internal bool ContextMenuIsVisible
        {
            get { return this._cntxtMenuStrip.Visible; }
        }

        //-------------------------------------------------------------------------------------------

        internal void HandleKeyEvent(KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Down:
                    if (e.Control)
                    {
                        ScrollTo(-1, _vScrollBar.Value + _vScrollBar.LargeChange, false);
                    }
                    else
                    {
                        ScrollTo(-1, _vScrollBar.Value + _vScrollBar.SmallChange, false);
                    }
                    break;
                case Keys.Up:
                    if (e.Control)
                    {
                        ScrollTo(-1, Math.Max(0,_vScrollBar.Value - _vScrollBar.LargeChange),false);
                    }
                    else
                    {
                        ScrollTo(-1, Math.Max(0,_vScrollBar.Value - _vScrollBar.SmallChange), false);
                    }
                    break;
                case Keys.Left:
                    if (e.Control)
                    {
                        ScrollTo( Math.Max(0,_hScrollBar.Value - _hScrollBar.LargeChange), -1, false);
                    }
                    else
                    {
                        ScrollTo( Math.Max(0,_hScrollBar.Value - _hScrollBar.SmallChange), -1, false);
                    }
                    break;
                case Keys.Right:
                    if (e.Control)
                    {
                        ScrollTo(_hScrollBar.Value + _hScrollBar.LargeChange, -1, false);
                    }
                    else
                    {
                        ScrollTo(_hScrollBar.Value + _hScrollBar.SmallChange, -1, false);
                    }
                    break;
                case Keys.PageDown:
                    ScrollTo(-1, _vScrollBar.Value + _vScrollBar.LargeChange, false);
                    break;
                case Keys.PageUp:

                    ScrollTo(-1, Math.Max( 0, _vScrollBar.Value - _vScrollBar.LargeChange), false);
                    break;
                case Keys.Home:
                    ScrollTo(0, 0, false);
                    break;
                    
                case Keys.End:
                    ScrollTo(_hScrollBar.Maximum - _hScrollBar.LargeChange, _vScrollBar.Maximum - _vScrollBar.LargeChange, false);
                    break;
                default:
                    break;
            }
        }
        
        //-------------------------------------------------------------------------------------------
        /// <summary> 
        /// Nettoyage des ressources utilisées.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }
        //-------------------------------------------------------------------------------------------
        #region Code généré par le Concepteur de composants
        /// <summary> 
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas 
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MatrixControl));
            this._splitContainer = new System.Windows.Forms.SplitContainer();
            this._hScrollBar = new System.Windows.Forms.HScrollBar();
            this._vScrollBar = new System.Windows.Forms.VScrollBar();
            this._cntxtMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.cntxtItemMoveUp = new System.Windows.Forms.ToolStripMenuItem();
            this.cntxtItemMoveDown = new System.Windows.Forms.ToolStripMenuItem();
            this.paritionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._selector = new Tcdev.Dsm.View.TypePanel();
            this._matrix = new Tcdev.Dsm.View.MatrixPanel();
            this._splitContainer.Panel1.SuspendLayout();
            this._splitContainer.Panel2.SuspendLayout();
            this._splitContainer.SuspendLayout();
            this._cntxtMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // _splitContainer
            // 
            this._splitContainer.BackColor = System.Drawing.SystemColors.ControlLight;
            this._splitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this._splitContainer.Location = new System.Drawing.Point(0, 0);
            this._splitContainer.Name = "_splitContainer";
            // 
            // _splitContainer.Panel1
            // 
            this._splitContainer.Panel1.BackColor = System.Drawing.SystemColors.Control;
            this._splitContainer.Panel1.Controls.Add(this._selector);
            // 
            // _splitContainer.Panel2
            // 
            this._splitContainer.Panel2.BackColor = System.Drawing.SystemColors.Control;
            this._splitContainer.Panel2.Controls.Add(this._matrix);
            this._splitContainer.Size = new System.Drawing.Size(825, 289);
            this._splitContainer.SplitterDistance = 265;
            this._splitContainer.SplitterWidth = 3;
            this._splitContainer.TabIndex = 2;
            this._splitContainer.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.splitContainer1_SplitterMoved);
            // 
            // _hScrollBar
            // 
            this._hScrollBar.Location = new System.Drawing.Point(270, 292);
            this._hScrollBar.Name = "_hScrollBar";
            this._hScrollBar.Size = new System.Drawing.Size(555, 17);
            this._hScrollBar.TabIndex = 1;
            this._hScrollBar.Scroll += new System.Windows.Forms.ScrollEventHandler(this.hScrollBar1_Scroll);
            // 
            // _vScrollBar
            // 
            this._vScrollBar.Location = new System.Drawing.Point(828, 0);
            this._vScrollBar.Maximum = 160;
            this._vScrollBar.Name = "_vScrollBar";
            this._vScrollBar.Size = new System.Drawing.Size(17, 289);
            this._vScrollBar.SmallChange = 10;
            this._vScrollBar.TabIndex = 3;
            this._vScrollBar.Value = 16;
            this._vScrollBar.Scroll += new System.Windows.Forms.ScrollEventHandler(this.vScrollBar1_Scroll);
            // 
            // _cntxtMenuStrip
            // 
            this._cntxtMenuStrip.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._cntxtMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cntxtItemMoveUp,
            this.cntxtItemMoveDown,
            this.paritionToolStripMenuItem});
            this._cntxtMenuStrip.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this._cntxtMenuStrip.Name = "contextMenuStrip1";
            this._cntxtMenuStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this._cntxtMenuStrip.Size = new System.Drawing.Size(142, 70);
            // 
            // cntxtItemMoveUp
            // 
            this.cntxtItemMoveUp.Enabled = false;
            this.cntxtItemMoveUp.Image = global::Tcdev.Dsm.Properties.Resources.UpArrow1;
            this.cntxtItemMoveUp.Name = "cntxtItemMoveUp";
            this.cntxtItemMoveUp.Size = new System.Drawing.Size(141, 22);
            this.cntxtItemMoveUp.Text = "Move Up";
            this.cntxtItemMoveUp.Click += new System.EventHandler(this.cntxtItemMoveUp_Click);
            // 
            // cntxtItemMoveDown
            // 
            this.cntxtItemMoveDown.Enabled = false;
            this.cntxtItemMoveDown.Image = global::Tcdev.Dsm.Properties.Resources.DownArrow;
            this.cntxtItemMoveDown.Name = "cntxtItemMoveDown";
            this.cntxtItemMoveDown.Size = new System.Drawing.Size(141, 22);
            this.cntxtItemMoveDown.Text = "Move Down";
            this.cntxtItemMoveDown.Click += new System.EventHandler(this.cntxtItemMoveDown_Click);
            // 
            // paritionToolStripMenuItem
            // 
            this.paritionToolStripMenuItem.Enabled = false;
            this.paritionToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("paritionToolStripMenuItem.Image")));
            this.paritionToolStripMenuItem.Name = "paritionToolStripMenuItem";
            this.paritionToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this.paritionToolStripMenuItem.Text = "Partition";
            this.paritionToolStripMenuItem.Click += new System.EventHandler(this.paritionToolStripMenuItem_Click);
            // 
            // _selector
            // 
            this._selector.BackColor = System.Drawing.SystemColors.Control;
            this._selector.ForeColor = System.Drawing.SystemColors.ControlText;
            this._selector.Location = new System.Drawing.Point(0, 3);
            this._selector.Name = "_selector";
            this._selector.Size = new System.Drawing.Size(265, 216);
            this._selector.TabIndex = 0;
            // 
            // _matrix
            // 
            this._matrix.AllowDrop = true;
            this._matrix.BackColor = System.Drawing.SystemColors.Control;
            this._matrix.Location = new System.Drawing.Point(1, 3);
            this._matrix.Name = "_matrix";
            this._matrix.Size = new System.Drawing.Size(501, 216);
            this._matrix.TabIndex = 0;
            // 
            // MatrixControl
            // 
            this.Controls.Add(this._vScrollBar);
            this.Controls.Add(this._splitContainer);
            this.Controls.Add(this._hScrollBar);
            this.DoubleBuffered = true;
            this.Enabled = false;
            this.Name = "MatrixControl";
            this.Size = new System.Drawing.Size(848, 315);
            this.Resize += new System.EventHandler(this.MatrixControl_Resize);
            this.EnabledChanged += new System.EventHandler(this.MatrixControl_EnabledChanged);
            this._splitContainer.Panel1.ResumeLayout(false);
            this._splitContainer.Panel2.ResumeLayout(false);
            this._splitContainer.ResumeLayout(false);
            this._cntxtMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion

        //-------------------------------------------------------------------------------------------
        void Matrix_SizeChanged(object sender, EventArgs e)
        {
            ResizeControl();
        }
        //-------------------------------------------------------------------------------------------
        void splitContainer1_SplitterMoved(object sender, SplitterEventArgs e)
        {
            if (this.Enabled)
            {
                _selector.Size = new Size(e.SplitX, this.Size.Height);

                ResizeControl();

                _selector.ViewRectangle = this._splitContainer.Panel1.ClientRectangle;
                _selector.Invalidate();
                _matrix.ViewRectangle = this._splitContainer.Panel2.ClientRectangle;
                _matrix.Invalidate();

                Update();
            }

        }
        //-------------------------------------------------------------------------------------------
        void MatrixControl_Resize(object sender, EventArgs e)
        {
            ResizeControl();

            _selector.ViewRectangle = this._splitContainer.Panel1.ClientRectangle;
            _selector.Invalidate();
            _matrix.ViewRectangle = this._splitContainer.Panel2.ClientRectangle;
            _matrix.Invalidate();
            this.Invalidate();
            Update();
        }
        //-------------------------------------------------------------------------------------------
        void ResizeControl()
        {
            this._splitContainer.Size = new Size(
                this.Width  - _vScrollBar.Width -1, 
                this.Height - _hScrollBar.Height-1);

            int w = this._splitContainer.Panel2.Width;
            int h = this._splitContainer.Panel2.Height;

            if (w < 0 || w > _matrix.Width)
            {
                _hScrollBar.Visible = false;

                OffsetX = 0;
            }
            else
            {
                this._hScrollBar.Location = new Point(this._splitContainer.Panel2.Left, this._splitContainer.Height);
                this._hScrollBar.Size = new Size(this._splitContainer.Panel2.Width, this._hScrollBar.Height);
                this._hScrollBar.Maximum = _matrix.Width;
                 
                this._hScrollBar.LargeChange = Math.Max(this._splitContainer.Panel2.Width, 0 );
                this._hScrollBar.SmallChange = _displayOptions.CellHeight;

                // adjust offset  so that last scroll does not go past end
                if ( OffsetX + w > _matrix.Width)
                {
                    OffsetX -= (OffsetX + w - _matrix.Width);
                }

                if (OffsetX < 0) OffsetX = 0;

                this._hScrollBar.Value = OffsetX;

                this._hScrollBar.Visible = true;
            }

            if (h < 0 || h > _matrix.Height)
            {
                _vScrollBar.Visible = false;

                OffsetY = 0;
            }
            else
            {
                
                _vScrollBar.Location = new Point(this._splitContainer.Width, _displayOptions.RootHeight +1 );
                _vScrollBar.Size = new Size(_vScrollBar.Width, this._splitContainer.Height - _displayOptions.RootHeight);
                this._vScrollBar.Maximum = _matrix.Height-_displayOptions.RootHeight;
                this._vScrollBar.LargeChange = Math.Max(0, this._splitContainer.Panel2.Height - _displayOptions.RootHeight);
                this._vScrollBar.SmallChange = _displayOptions.CellHeight;

                if (OffsetY + h > _matrix.Height)
                {
                    OffsetY -= (OffsetY + h - _matrix.Height);
                }

                if (OffsetY < 0) OffsetY = 0;

                this._vScrollBar.Value = OffsetY;   
                this._vScrollBar.Visible = true;
            }

            this._vScrollBar.Invalidate();
            this._hScrollBar.Invalidate();
        }

        //-------------------------------------------------------------------------------------------
        void vScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {       
            ScrollTo( -1, e.NewValue, true);
        }
        //-------------------------------------------------------------------------------------------
        void hScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            ScrollTo(e.NewValue, -1, true);
        }
        //-------------------------------------------------------------------------------------------
        void ScrollTo(int x, int y, bool wasScrollBar)
        {
            bool scroll = false;

            if (x >= 0)
            {
                OffsetX = x;
                int w = this._splitContainer.Panel2.Width;
               
                if (OffsetX + w > _matrix.Width)
                {
                    OffsetX -= (OffsetX + w - _matrix.Width);
                }

                if (OffsetX < 0) OffsetX = 0;

                _matrix.ViewRectangle = this._splitContainer.Panel2.ClientRectangle;
                _matrix.Invalidate();

                scroll = true;

                if (!wasScrollBar)
                {
                    _hScrollBar.Value = OffsetX;
                }
            }

            if (y >= 0)
            {
                OffsetY = y;

                int h = this._splitContainer.Panel2.Height;

                if (OffsetY + h >  _matrix.Height)
                {
                    OffsetY -= (OffsetY + h - _matrix.Height);
                }

                if (OffsetY < 0) OffsetY = 0;

                _matrix.ViewRectangle = this._splitContainer.Panel2.ClientRectangle;
                _matrix.Invalidate();

                _selector.ViewRectangle = this._splitContainer.Panel1.ClientRectangle;
                _selector.Invalidate();

                scroll = true;

                if (!wasScrollBar)
                {
                    _vScrollBar.Value = OffsetY;                   
                }
            }

            if (scroll) 
                Update();
        }
 
        //-------------------------------------------------------------------------------------------------
        void EnableButtons()
        {
            bool down = MatrixModel.CanMoveNodeDown();
            bool up   = MatrixModel.CanMoveNodeUp();

            this.cntxtItemMoveDown.Enabled = down;
            this.cntxtItemMoveUp.Enabled   = up;

            MainControl main = this.Parent.Parent.Parent.Parent as MainControl;
            main.EnableNodeDownButton = down;
            main.EnableNodeUpButton = up;

            bool canPartition = (MatrixModel.SelectedNode != null && 
                MatrixModel.SelectedNode.Children.Count > 1);

            this.paritionToolStripMenuItem.Enabled = canPartition;
            main.EnablePartitionButton = canPartition;
        }

        //-------------------------------------------------------------------------------------------

        void cntxtItemMoveUp_Click(object sender, EventArgs e)
        {
            MoveSelectedNodeUp();
        }

        //-------------------------------------------------------------------------------------------

        void cntxtItemMoveDown_Click(object sender, EventArgs e)
        {
            MoveSelectedNodeDown();
        }

        //-------------------------------------------------------------------------------------------

        private void MatrixControl_EnabledChanged(object sender, EventArgs e)
        {
            if ( this.Enabled && this.MatrixModel == null )
            {
                this.Enabled = false;
            }
            else if (this.Enabled)
            {
                this._cntxtMenuStrip.Enabled = true;
            }
        }

        //-------------------------------------------------------------------------------------------

        private void paritionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ICommand cmd = new CommandPartition( MatrixModel );
            CursorStateHelper csh = new CursorStateHelper(this, Cursors.WaitCursor);
            try
            {
                cmd.Execute();

               this.Invalidate();
                
            }
            catch( Exception ex )
            {
                ErrorDialog errdlg = new ErrorDialog(ex.ToString());
                errdlg.ShowDialog();
                errdlg.Dispose();
            }
            finally
            {
                csh.Reset();
            }        
        }
        //-------------------------------------------------------------------------------------------
    }
}

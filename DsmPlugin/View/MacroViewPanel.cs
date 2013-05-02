using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tcdev.Collections.Generic;
using Tcdev.Dsm.Model;

namespace Tcdev.Dsm.View
{
    public partial class MacroViewPanel : UserControl
    {
        Pen emptyCell = new Pen(Brushes.LightGray, 2);
        Pen diagonal = new Pen(Brushes.Gray, 2);
        Pen relation = new Pen(Brushes.Blue, 2);
        Pen cyclic = new Pen(Brushes.Red, 2);
        Timer _ttTimer;
        public MacroViewPanel()
        {
            InitializeComponent();
             _ttTimer = new Timer();
            _ttTimer.Interval = 4000;
            _ttTimer.Tick += new EventHandler(_ttTimer_Tick);

        }

        public Dsm.Model.DsmModel Model
        {
            get;
            set;
        }

        //-------------------------------------------------------------------------------------------------

        void _ttTimer_Tick(object sender, EventArgs e)
        {
            if (this.toolTip1.Active)
            {
                this.toolTip1.Active = false;
                _ttTimer.Stop();
            }
        }

        Bitmap _image;

        IDictionary<Point, Relation > _relationMap;

        public void BuildImage()
        {
            _image  = new Bitmap(2000, 2000);
            _relationMap = new Dictionary<Point, Relation>();

            // optimise - run through list to determine the points to display build the tooltip map
            // this allows us to calculate the size of the bitmap before creating it
            // then when drawing the bitmap we only need to draw the points from the map

            using (ModelessMessageBox msg = new ModelessMessageBox("Building image"))
            {

                using (Graphics g1 = Graphics.FromImage(_image))
                {
                    int xPos = 0;
                    int yPos = 0;
                    TreeIterator<Module> iteratorX = new TreeIterator<Module>(Model.Hierarchy);
                    Tree<Module>.Node nodeX = iteratorX.Next();

                    while (nodeX != null)
                    {
                        yPos = 0;

                        if (nodeX.HasChildren == false)
                        {
                            TreeIterator<Module> iteratorY = new TreeIterator<Module>(Model.Hierarchy);
                            Tree<Module>.Node nodeY = iteratorY.Next();

                            if (xPos % 50 == 0)
                            {
                                msg.UpdateProgress(xPos / 2000, nodeX.NodeValue.FullName);
                            }

                            while (nodeY != null)
                            {
                                if (nodeY.HasChildren == false)
                                {
                                    Pen pen = emptyCell;
                                    if (nodeX == nodeY)
                                    {
                                        pen = diagonal;
                                    }
                                    else if (DsmModel.HasCyclicRelation(nodeX.NodeValue, nodeY.NodeValue))
                                    {
                                        pen = cyclic;
                                    }
                                    else
                                    {
                                        Relation rel = nodeX.NodeValue.GetRelation(nodeY.NodeValue);
                                        if (rel != null && rel.Weight > 0)
                                        {
                                            pen = relation;
                                            _relationMap[new Point(xPos, yPos)] = rel;
                                        }
                                    }

                                    g1.DrawRectangle(pen, xPos, yPos, 2, 2); yPos += 2;
                                } 
                                nodeY = iteratorY.Next();

                            }xPos +=2;
                        }
                        
                        nodeX = iteratorX.Next();

                    }
                }
                
            }
            //this.pictureBox1.Image = _image;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            using (Graphics g = e.Graphics)
            {
                g.DrawImage(_image, 0, 0 );
            }
        }


        private void MacroViewPanel_MouseMove(object sender, MouseEventArgs e)
        {
            
            if (_relationMap.ContainsKey(e.Location))
            {_ttTimer.Stop();
                Relation r = _relationMap[e.Location];

                toolTip1.SetToolTip(this, r.Consumer.FullName);

                toolTip1.Active = true;
                _ttTimer.Start();
            }
            //else
            //{
            //    toolTip1.Active = false;
            //}
        }


    }
}

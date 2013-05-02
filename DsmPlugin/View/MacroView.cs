using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tcdev.Dsm.Model;

namespace Tcdev.Dsm.View
{
    public partial class MacroView : Form
    {
        public MacroView()
        {
            InitializeComponent();
        }

        public DsmModel Model {
            set
            {
                 this.macroViewPanel1.Model = value;
            }
            get 
            { 
                return this.macroViewPanel1.Model;
            }
        }

        public void Build()
        {
            this.macroViewPanel1.BuildImage();
        }
    }
}

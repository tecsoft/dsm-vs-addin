using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using System.Resources;

namespace ResourcesTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            ResourceManager rm =
                new ResourceManager("test.resources", this.GetType().Assembly);

            this.label1.Text = rm.GetString("STR");
        }
    }
}
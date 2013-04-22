using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tcdev.Dsm.Adapters;
using System.IO;

namespace VisualStudioTestDouble
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            _adapter = new VisualStudioAdapter();
        }

        VisualStudioAdapter _adapter = null;


        private void button1_Click(object sender, EventArgs e)
        {
                FileInfo fi = new FileInfo("./NoProject.dsm");

                FileInfo[] testdll = fi.Directory.GetFiles("Tcdev.DsmPlugin.dll");

                _adapter.LoadAssembly(testdll[0].FullName, false);

                _adapter.Open(fi.Directory.FullName, fi.Name );
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FileInfo fi = new FileInfo("./Project.dsm");

            FileInfo[] testdll = fi.Directory.GetFiles("Tcdev.DsmPlugin.dll");

            _adapter.LoadAssembly(testdll[0].FullName, false);

            _adapter.Open(fi.Directory.FullName, fi.Name);


        }

        private void button3_Click(object sender, EventArgs e)
        {
            _adapter.Reanalyser();
        }
    }
}

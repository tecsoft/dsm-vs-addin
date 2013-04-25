using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tcdev.Dsm.Model;
using Tcdev.Collections.Generic;
using Tcdev.Dsm.Model.DependencyRules;

namespace Tcdev.Dsm.View
{
    public partial class ModifyRuleDialog : Form
    {
        DsmModel _model;
        public ModifyRuleDialog( DsmModel model)
        {
            InitializeComponent();

            Font sysFont = SystemFonts.MessageBoxFont;
            this.Font = new Font(sysFont.Name, sysFont.SizeInPoints, sysFont.Style);

            btnOk.Enabled = false;

            _model = model;

            lblSourceName.Text = model.SelectedNode.NodeValue.FullName;

            InitialiseTree();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click_1(object sender, EventArgs e)
        {

        }

        void AddChildren(TreeNode parent, Tree<Module>.Node moduleNode)
        {
            foreach (var childNode in moduleNode.Children)
            {
                Module m = childNode.NodeValue;
                TreeNode node = parent.Nodes.Add( m.FullName, m.Name );
                AddChildren(node, childNode);
            }
        }

        void InitialiseTree()
        {
            Tree<Module>.Node parentModule = _model.Hierarchy.Root;

            foreach (var moduleNode in parentModule.Children)
            {
                Module m = moduleNode.NodeValue;
                TreeNode node = targetTree.Nodes.Add(m.FullName, m.Name);
                AddChildren(node, moduleNode);
            }
        }

        private void consumerTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            this.lblTargetName.Text = e.Node.TreeView.SelectedNode.Name;
            btnOk.Enabled = true;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            Module provider = _model.FindNode(lblSourceName.Text).NodeValue;
            Module consumer = _model.FindNode(lblTargetName.Text).NodeValue;
            var rule = new CannotUseRule(provider, consumer);
            _model.RuleManager.Add(rule);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        
    }
}

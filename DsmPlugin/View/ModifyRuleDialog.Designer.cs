namespace Tcdev.Dsm.View
{
    partial class ModifyRuleDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblSourceName = new System.Windows.Forms.Label();
            this.targetTree = new System.Windows.Forms.TreeView();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lblTargetName = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.RuleType = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.Target = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Search = new System.Windows.Forms.DataGridViewButtonColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // lblSourceName
            // 
            this.lblSourceName.AutoSize = true;
            this.lblSourceName.Location = new System.Drawing.Point(9, 143);
            this.lblSourceName.Name = "lblSourceName";
            this.lblSourceName.Size = new System.Drawing.Size(81, 13);
            this.lblSourceName.TabIndex = 1;
            this.lblSourceName.Text = "source fullname";
            this.lblSourceName.Click += new System.EventHandler(this.label1_Click_1);
            // 
            // targetTree
            // 
            this.targetTree.Location = new System.Drawing.Point(106, 225);
            this.targetTree.Name = "targetTree";
            this.targetTree.Size = new System.Drawing.Size(441, 80);
            this.targetTree.TabIndex = 3;
            this.targetTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.consumerTree_AfterSelect);
            // 
            // btnOk
            // 
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Location = new System.Drawing.Point(367, 311);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 4;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(449, 311);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // lblTargetName
            // 
            this.lblTargetName.AutoSize = true;
            this.lblTargetName.Location = new System.Drawing.Point(304, 20);
            this.lblTargetName.Name = "lblTargetName";
            this.lblTargetName.Size = new System.Drawing.Size(154, 13);
            this.lblTargetName.TabIndex = 6;
            this.lblTargetName.Text = "Select target from tree below ...\r\n";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(129, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Create a dependency rule";
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "Can use",
            "Cannot use"});
            this.comboBox1.Location = new System.Drawing.Point(12, 170);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(78, 21);
            this.comboBox1.TabIndex = 8;
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToResizeRows = false;
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.RuleType,
            this.Target,
            this.Search});
            this.dataGridView1.Location = new System.Drawing.Point(15, 49);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(523, 142);
            this.dataGridView1.TabIndex = 9;
            // 
            // RuleType
            // 
            this.RuleType.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.RuleType.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.ComboBox;
            this.RuleType.Frozen = true;
            this.RuleType.HeaderText = "Rule";
            this.RuleType.Items.AddRange(new object[] {
            "Can Use",
            "Cannot Use"});
            this.RuleType.MinimumWidth = 100;
            this.RuleType.Name = "RuleType";
            this.RuleType.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.RuleType.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // Target
            // 
            this.Target.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.Target.Frozen = true;
            this.Target.HeaderText = "Target";
            this.Target.MinimumWidth = 100;
            this.Target.Name = "Target";
            this.Target.Width = 240;
            // 
            // Search
            // 
            this.Search.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.Search.HeaderText = "...";
            this.Search.MinimumWidth = 16;
            this.Search.Name = "Search";
            this.Search.ReadOnly = true;
            this.Search.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Search.Text = "...";
            this.Search.ToolTipText = "Browse for target";
            this.Search.UseColumnTextForButtonValue = true;
            this.Search.Width = 22;
            // 
            // ModifyRuleDialog
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(550, 355);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lblTargetName);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.targetTree);
            this.Controls.Add(this.lblSourceName);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "ModifyRuleDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Edit Dependency Rules";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblSourceName;
        private System.Windows.Forms.TreeView targetTree;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label lblTargetName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewComboBoxColumn RuleType;
        private System.Windows.Forms.DataGridViewTextBoxColumn Target;
        private System.Windows.Forms.DataGridViewButtonColumn Search;
    }
}
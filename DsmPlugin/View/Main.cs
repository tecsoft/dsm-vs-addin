using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using Tcdev.Dsm.Model;
using Reflector;
using Reflector.CodeModel;
using Tcdev.Dsm.Engine;
using System.IO;
using Tcdev.Outil;
using Tcdev.Dsm.Commands;
using Tcdev.Dsm.Adapters;
namespace Tcdev.Dsm.View
{
	/// <summary>
	/// The MainControl is the collection of controls which make up the plugin
	/// </summary>
	public class MainControl : UserControl, IDsmParentControl
    {
        private IContainer components;

        private System.Windows.Forms.TabControl tabControl;
		private System.Windows.Forms.TabPage pageResults;
        private TabPage pageAssemblies;
        private Button btnAnalyse;
        private Panel panel1;
        private ToolStrip toolStrip1;
        private Panel panel2;
        private MatrixControl matrixControl1;
        private ToolStripButton btnSave;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripButton btnMoveUp;
        private ToolStripButton btnMoveDown;
        private ToolStripButton btnHighlightCyclic;
        private ToolStripSplitButton btnZoom;
        private ToolStripMenuItem itmZoom1;
        private ToolStripMenuItem itmZoom2;
        private ToolStripMenuItem itmZoom3;
        private CheckedListBox checkedListBox1;
        private ToolStripMenuItem itmZoom4;
        private ToolStripMenuItem itmZoom5;
        private Button btnBrowse;
        private Label label1;
        private GroupBox groupBox2;
        private RadioButton radioButton2;
        private RadioButton radioButton1;
        private Label lblStatus;
        private ToolStripButton btnReports;
        private CheckBox chkExCompName;
        private GroupBox groupBox3;
        private CheckBox chkExGlobal;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripButton btnPartition;
        private ToolStripSeparator toolStripSeparator3;
        private CheckBox chkHideNested;
        private ToolTip toolTip1;
        Model.DsmModel _model;
        private GroupBox groupBox1;
        private Label label2;
        private ToolStripButton btnAddRule;
        private ToolStripButton btnMacroView;
        IAdapter _adapter;

        //-------------------------------------------------------------------------------------------------
        /// <summary>
        /// delegate for commands to update progress bar. 
        /// </summary>
        /// <param name="percentageComplete"></param>
        /// <param name="statusText"></param>
        public delegate void ProgressUpdateDelegate(int percentageComplete, string statusText);

        //-------------------------------------------------------------------------------------------------

        public MainControl()
        {
            InitializeComponent();
            Font sysFont = SystemFonts.MessageBoxFont;
            this.Font = new Font(sysFont.Name, sysFont.SizeInPoints, sysFont.Style);
            _adapter = null;
        }

        //-------------------------------------------------------------------------------------------------
        /// <summary>
        /// Get or set the parent adapter
        /// </summary>
        public IAdapter Adapter
        {
            get { return _adapter; }
            set { _adapter = value; }
        }

        //-------------------------------------------------------------------------------------------------
		/// <summary>
		/// Nettoyage des ressources utilisées.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if( components != null )
					components.Dispose();
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainControl));
            this.tabControl = new System.Windows.Forms.TabControl();
            this.pageAssemblies = new System.Windows.Forms.TabPage();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.checkedListBox1 = new System.Windows.Forms.CheckedListBox();
            this.lblStatus = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnAnalyse = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.chkHideNested = new System.Windows.Forms.CheckBox();
            this.chkExGlobal = new System.Windows.Forms.CheckBox();
            this.chkExCompName = new System.Windows.Forms.CheckBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.pageResults = new System.Windows.Forms.TabPage();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnSave = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnMoveUp = new System.Windows.Forms.ToolStripButton();
            this.btnMoveDown = new System.Windows.Forms.ToolStripButton();
            this.btnAddRule = new System.Windows.Forms.ToolStripButton();
            this.btnPartition = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.btnHighlightCyclic = new System.Windows.Forms.ToolStripButton();
            this.btnZoom = new System.Windows.Forms.ToolStripSplitButton();
            this.itmZoom1 = new System.Windows.Forms.ToolStripMenuItem();
            this.itmZoom2 = new System.Windows.Forms.ToolStripMenuItem();
            this.itmZoom3 = new System.Windows.Forms.ToolStripMenuItem();
            this.itmZoom4 = new System.Windows.Forms.ToolStripMenuItem();
            this.itmZoom5 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.btnReports = new System.Windows.Forms.ToolStripButton();
            this.btnMacroView = new System.Windows.Forms.ToolStripButton();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.matrixControl1 = new Tcdev.Dsm.View.MatrixControl();
            this.tabControl.SuspendLayout();
            this.pageAssemblies.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.pageResults.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.pageAssemblies);
            this.tabControl.Controls.Add(this.pageResults);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabControl.HotTrack = true;
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(848, 558);
            this.tabControl.TabIndex = 0;
            this.tabControl.KeyUp += new System.Windows.Forms.KeyEventHandler(this.tabControl_KeyUp);
            // 
            // pageAssemblies
            // 
            this.pageAssemblies.AutoScroll = true;
            this.pageAssemblies.BackColor = System.Drawing.SystemColors.Control;
            this.pageAssemblies.Controls.Add(this.label2);
            this.pageAssemblies.Controls.Add(this.groupBox1);
            this.pageAssemblies.Controls.Add(this.lblStatus);
            this.pageAssemblies.Controls.Add(this.label1);
            this.pageAssemblies.Controls.Add(this.btnAnalyse);
            this.pageAssemblies.Controls.Add(this.groupBox3);
            this.pageAssemblies.Controls.Add(this.btnBrowse);
            this.pageAssemblies.Controls.Add(this.groupBox2);
            this.pageAssemblies.Location = new System.Drawing.Point(4, 24);
            this.pageAssemblies.Name = "pageAssemblies";
            this.pageAssemblies.Size = new System.Drawing.Size(840, 530);
            this.pageAssemblies.TabIndex = 0;
            this.pageAssemblies.Text = "Start Page";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(19, 49);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(326, 15);
            this.label2.TabIndex = 13;
            this.label2.Text = "To start a new analysis select at least one assembly and click:";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.checkedListBox1);
            this.groupBox1.Location = new System.Drawing.Point(16, 77);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(510, 438);
            this.groupBox1.TabIndex = 12;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Assemblies";
            // 
            // checkedListBox1
            // 
            this.checkedListBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.checkedListBox1.CheckOnClick = true;
            this.checkedListBox1.Location = new System.Drawing.Point(6, 19);
            this.checkedListBox1.Name = "checkedListBox1";
            this.checkedListBox1.Size = new System.Drawing.Size(486, 400);
            this.checkedListBox1.Sorted = true;
            this.checkedListBox1.TabIndex = 4;
            this.checkedListBox1.ThreeDCheckBoxes = true;
            // 
            // lblStatus
            // 
            this.lblStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblStatus.AutoSize = true;
            this.lblStatus.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lblStatus.Location = new System.Drawing.Point(532, 329);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(0, 15);
            this.lblStatus.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(19, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(185, 15);
            this.label1.TabIndex = 7;
            this.label1.Text = "Open a previously saved analysis :\r\n";
            // 
            // btnAnalyse
            // 
            this.btnAnalyse.Location = new System.Drawing.Point(378, 44);
            this.btnAnalyse.Name = "btnAnalyse";
            this.btnAnalyse.Size = new System.Drawing.Size(113, 24);
            this.btnAnalyse.TabIndex = 1;
            this.btnAnalyse.Text = "Run Analysis";
            this.btnAnalyse.Click += new System.EventHandler(this.btnAnalyse_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.chkHideNested);
            this.groupBox3.Controls.Add(this.chkExGlobal);
            this.groupBox3.Controls.Add(this.chkExCompName);
            this.groupBox3.Location = new System.Drawing.Point(532, 77);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(260, 101);
            this.groupBox3.TabIndex = 10;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Other options";
            // 
            // chkHideNested
            // 
            this.chkHideNested.AutoSize = true;
            this.chkHideNested.Location = new System.Drawing.Point(11, 66);
            this.chkHideNested.Name = "chkHideNested";
            this.chkHideNested.Size = new System.Drawing.Size(128, 19);
            this.chkHideNested.TabIndex = 11;
            this.chkHideNested.Text = "Hide nested classes";
            this.toolTip1.SetToolTip(this.chkHideNested, "Hide nested classes from view in the matrix.\r\n(Dependencies are however included " +
                    "in the weights for the parent class)");
            this.chkHideNested.UseVisualStyleBackColor = true;
            // 
            // chkExGlobal
            // 
            this.chkExGlobal.AutoSize = true;
            this.chkExGlobal.Checked = true;
            this.chkExGlobal.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkExGlobal.Location = new System.Drawing.Point(11, 43);
            this.chkExGlobal.Name = "chkExGlobal";
            this.chkExGlobal.Size = new System.Drawing.Size(165, 19);
            this.chkExGlobal.TabIndex = 10;
            this.chkExGlobal.Text = "Exclude global namespace";
            this.toolTip1.SetToolTip(this.chkExGlobal, "Classes (e.g. AssemblyInfo) defined in the global namespace are excluded from the" +
                    " analysis");
            this.chkExGlobal.UseVisualStyleBackColor = true;
            // 
            // chkExCompName
            // 
            this.chkExCompName.AutoSize = true;
            this.chkExCompName.Checked = true;
            this.chkExCompName.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkExCompName.Location = new System.Drawing.Point(11, 18);
            this.chkExCompName.Name = "chkExCompName";
            this.chkExCompName.Size = new System.Drawing.Size(240, 19);
            this.chkExCompName.TabIndex = 9;
            this.chkExCompName.Text = "Exclude compiler generated namespaces";
            this.toolTip1.SetToolTip(this.chkExCompName, "Certain compiler generated classes \r\n(such as <CppImplementationDetails>) \r\nare e" +
                    "xcluded from the analysis");
            this.chkExCompName.UseVisualStyleBackColor = true;
            // 
            // btnBrowse
            // 
            this.btnBrowse.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBrowse.Location = new System.Drawing.Point(236, 9);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(67, 25);
            this.btnBrowse.TabIndex = 8;
            this.btnBrowse.Text = "Browse";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.radioButton2);
            this.groupBox2.Controls.Add(this.radioButton1);
            this.groupBox2.Enabled = false;
            this.groupBox2.Location = new System.Drawing.Point(550, 406);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(260, 92);
            this.groupBox2.TabIndex = 8;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Model Type";
            this.groupBox2.Visible = false;
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Enabled = false;
            this.radioButton2.Location = new System.Drawing.Point(11, 43);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(127, 19);
            this.radioButton2.TabIndex = 8;
            this.radioButton2.Text = "Deployment model";
            this.toolTip1.SetToolTip(this.radioButton2, "Classes in the matrix are organised by assembly");
            this.radioButton2.UseVisualStyleBackColor = true;
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Checked = true;
            this.radioButton1.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.radioButton1.Location = new System.Drawing.Point(11, 19);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(127, 19);
            this.radioButton1.TabIndex = 7;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "Architecture model";
            this.toolTip1.SetToolTip(this.radioButton1, "Classes in the matrix are organised by namespace");
            this.radioButton1.UseVisualStyleBackColor = true;
            // 
            // pageResults
            // 
            this.pageResults.BackColor = System.Drawing.SystemColors.Control;
            this.pageResults.Controls.Add(this.panel2);
            this.pageResults.Controls.Add(this.panel1);
            this.pageResults.Location = new System.Drawing.Point(4, 24);
            this.pageResults.Name = "pageResults";
            this.pageResults.Size = new System.Drawing.Size(840, 530);
            this.pageResults.TabIndex = 1;
            this.pageResults.Text = "Matrix";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.matrixControl1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 30);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(840, 500);
            this.panel2.TabIndex = 8;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.toolStrip1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(840, 30);
            this.panel1.TabIndex = 7;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnSave,
            this.toolStripSeparator1,
            this.btnMoveUp,
            this.btnMoveDown,
            this.btnAddRule,
            this.btnPartition,
            this.toolStripSeparator2,
            this.btnHighlightCyclic,
            this.btnZoom,
            this.toolStripSeparator3,
            this.btnReports,
            this.btnMacroView});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStrip1.Size = new System.Drawing.Size(840, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            this.toolStrip1.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.toolStrip1_ItemClicked);
            // 
            // btnSave
            // 
            this.btnSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnSave.Image = ((System.Drawing.Image)(resources.GetObject("btnSave.Image")));
            this.btnSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(23, 22);
            this.btnSave.Text = "toolStripButton1";
            this.btnSave.ToolTipText = "Save current project";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // btnMoveUp
            // 
            this.btnMoveUp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnMoveUp.Enabled = false;
            this.btnMoveUp.Image = global::Tcdev.Dsm.Properties.Resources.UpArrow1;
            this.btnMoveUp.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnMoveUp.Name = "btnMoveUp";
            this.btnMoveUp.Size = new System.Drawing.Size(23, 22);
            this.btnMoveUp.Text = "Move Up";
            // 
            // btnMoveDown
            // 
            this.btnMoveDown.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnMoveDown.Enabled = false;
            this.btnMoveDown.Image = global::Tcdev.Dsm.Properties.Resources.DownArrow;
            this.btnMoveDown.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnMoveDown.Name = "btnMoveDown";
            this.btnMoveDown.Size = new System.Drawing.Size(23, 22);
            this.btnMoveDown.Text = "toolStripButton1";
            this.btnMoveDown.ToolTipText = "Move Down";
            // 
            // btnAddRule
            // 
            this.btnAddRule.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnAddRule.Image = ((System.Drawing.Image)(resources.GetObject("btnAddRule.Image")));
            this.btnAddRule.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnAddRule.Name = "btnAddRule";
            this.btnAddRule.Size = new System.Drawing.Size(23, 22);
            this.btnAddRule.Text = "toolStripButton1";
            this.btnAddRule.ToolTipText = "Add rule for selected node";
            this.btnAddRule.Visible = false;
            // 
            // btnPartition
            // 
            this.btnPartition.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnPartition.Enabled = false;
            this.btnPartition.Image = global::Tcdev.Dsm.Properties.Resources.Partition;
            this.btnPartition.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnPartition.Name = "btnPartition";
            this.btnPartition.Size = new System.Drawing.Size(23, 22);
            this.btnPartition.Text = "toolStripButton1";
            this.btnPartition.ToolTipText = "Partition modules of current selection";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // btnHighlightCyclic
            // 
            this.btnHighlightCyclic.CheckOnClick = true;
            this.btnHighlightCyclic.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnHighlightCyclic.Image = ((System.Drawing.Image)(resources.GetObject("btnHighlightCyclic.Image")));
            this.btnHighlightCyclic.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnHighlightCyclic.Name = "btnHighlightCyclic";
            this.btnHighlightCyclic.Size = new System.Drawing.Size(23, 22);
            this.btnHighlightCyclic.Text = "toolStripButton1";
            this.btnHighlightCyclic.ToolTipText = "Show/Hide Cyclic Dependencies";
            // 
            // btnZoom
            // 
            this.btnZoom.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnZoom.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.itmZoom1,
            this.itmZoom2,
            this.itmZoom3,
            this.itmZoom4,
            this.itmZoom5});
            this.btnZoom.Image = global::Tcdev.Dsm.Properties.Resources.Zoom1;
            this.btnZoom.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnZoom.Name = "btnZoom";
            this.btnZoom.Size = new System.Drawing.Size(32, 22);
            this.btnZoom.Text = "toolStripSplitButton1";
            this.btnZoom.ToolTipText = "Zoom";
            this.btnZoom.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.btnZoom_DropDownItemClicked);
            // 
            // itmZoom1
            // 
            this.itmZoom1.CheckOnClick = true;
            this.itmZoom1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.itmZoom1.Name = "itmZoom1";
            this.itmZoom1.Size = new System.Drawing.Size(119, 22);
            this.itmZoom1.Text = "Smallest";
            // 
            // itmZoom2
            // 
            this.itmZoom2.CheckOnClick = true;
            this.itmZoom2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.itmZoom2.Name = "itmZoom2";
            this.itmZoom2.Size = new System.Drawing.Size(119, 22);
            this.itmZoom2.Text = "Smaller";
            // 
            // itmZoom3
            // 
            this.itmZoom3.Checked = true;
            this.itmZoom3.CheckOnClick = true;
            this.itmZoom3.CheckState = System.Windows.Forms.CheckState.Checked;
            this.itmZoom3.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.itmZoom3.Name = "itmZoom3";
            this.itmZoom3.Size = new System.Drawing.Size(119, 22);
            this.itmZoom3.Text = "Medium";
            // 
            // itmZoom4
            // 
            this.itmZoom4.CheckOnClick = true;
            this.itmZoom4.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.itmZoom4.Name = "itmZoom4";
            this.itmZoom4.Size = new System.Drawing.Size(119, 22);
            this.itmZoom4.Text = "Larger";
            // 
            // itmZoom5
            // 
            this.itmZoom5.CheckOnClick = true;
            this.itmZoom5.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.itmZoom5.Name = "itmZoom5";
            this.itmZoom5.Size = new System.Drawing.Size(119, 22);
            this.itmZoom5.Text = "Largest";
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // btnReports
            // 
            this.btnReports.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnReports.Image = global::Tcdev.Dsm.Properties.Resources.Reports1;
            this.btnReports.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnReports.Name = "btnReports";
            this.btnReports.Size = new System.Drawing.Size(23, 22);
            this.btnReports.Text = "Generate Report";
            // 
            // btnMacroView
            // 
            this.btnMacroView.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnMacroView.Image = ((System.Drawing.Image)(resources.GetObject("btnMacroView.Image")));
            this.btnMacroView.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnMacroView.Name = "btnMacroView";
            this.btnMacroView.Size = new System.Drawing.Size(23, 22);
            this.btnMacroView.Text = "macro view";
            this.btnMacroView.Visible = false;
            // 
            // matrixControl1
            // 
            this.matrixControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.matrixControl1.Enabled = false;
            this.matrixControl1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.matrixControl1.Location = new System.Drawing.Point(0, 0);
            this.matrixControl1.Name = "matrixControl1";
            this.matrixControl1.Size = new System.Drawing.Size(840, 500);
            this.matrixControl1.TabIndex = 7;
            // 
            // MainControl
            // 
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.tabControl);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "MainControl";
            this.Size = new System.Drawing.Size(848, 558);
            this.tabControl.ResumeLayout(false);
            this.pageAssemblies.ResumeLayout(false);
            this.pageAssemblies.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.pageResults.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);

		}
		#endregion

        //-------------------------------------------------------------------------------------------------
        /// <summary>
        /// Add an a preloaded assembly to the assembly list
        /// </summary>
        /// <param name="assembly"></param>
        public void AddAssembly( Target assembly )
        {
            //TabPage assemblyPage = this.tabControl.TabPages[0];
            this.checkedListBox1.Items.Add(assembly);
        }

        //-------------------------------------------------------------------------------------------------
        /// <summary>
        /// Add an a preloaded assembly to the assembly list
        /// </summary>
        /// <param name="assembly"></param>
        public void AddAssembly(Target assembly, bool check)
        {
            //TabPage assemblyPage = this.tabControl.TabPages[0];
            this.checkedListBox1.Items.Add(assembly, check );
        }

        //-------------------------------------------------------------------------------------------------
        /// <summary>
        /// Clear the assembly list
        /// </summary>
        public void ClearAssemblies()
        {
            this.checkedListBox1.Items.Clear();
        }
        
        //-------------------------------------------------------------------------------------------------
        /// <summary>
        /// Enabled state of the NodeUp menu _button
        /// </summary>
        internal bool EnableNodeUpButton
        {
            get { return this.btnMoveUp.Enabled; }
            set { this.btnMoveUp.Enabled = value; }
        }

        //-------------------------------------------------------------------------------------------------
        /// <summary>
        /// Enabled state of the NodeDown menu _button
        /// </summary>
        internal bool EnableNodeDownButton
        {
            get { return this.btnMoveDown.Enabled; }
            set { this.btnMoveDown.Enabled = value; }
        }
        //-------------------------------------------------------------------------------------------------
        /// <summary>
        /// Set enabled state of partition button
        /// </summary>
        internal bool EnablePartitionButton
        {
            get { return this.btnPartition.Enabled; }
            set { this.btnPartition.Enabled = value; }
        }

        public void ReAnalyse()
        {
            IAnalyser analyser = _adapter.GetAnalyser();
           // {
                TabPage assemblyPage = this.tabControl.TabPages[0];

                // Set assemblies to be analysed
                foreach (Target assembly in this.checkedListBox1.CheckedItems)
                {
                    analyser.IncludeAssembly(assembly);
                }

                if (_model == null)
                    _model = new DsmModel();

                ICommand cmd = new CommandAnalyse(analyser, _model);

                ModelessMessageBox msg = new ModelessMessageBox("Analysing");
                try
                {

                    cmd.Execute(msg.UpdateProgress);

                    if (cmd.Completed)
                    {
                        matrixControl1.Size = new Size(
                            this.tabControl.ClientSize.Width,
                            this.tabControl.ClientSize.Height - this.toolStrip1.Height);

                        SetModel(_model);

                        this.pageAssemblies.Hide();
                        this.tabControl.SelectedTab = this.pageResults;
                    }
                }
                finally
                {
                    msg.Dispose();
                }

                this.Refresh();
            //}
        }

        public void btnAnalyse_Click(object sender, System.EventArgs e)
        {
            ReAnalyse();
        }

        //-------------------------------------------------------------------------------------------------
        /// <summary>
        /// Run the analysis
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void btnAnalyse_Click2(object sender, System.EventArgs e)
		{
            CursorStateHelper csh = new CursorStateHelper( this, Cursors.WaitCursor );

            try
            {
                if (checkedListBox1.CheckedItems.Count == 0)
                {
                    MessageBox.Show("At least one assembly must be selected", "DSM",
                        MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }
                else
                {
                    if (ConfirmModelSaved())
                    {
                        this.Refresh();

                        IAnalyser analyser = _adapter.GetAnalyser();
                        //{
                            TabPage assemblyPage = this.tabControl.TabPages[0];

                            // Set assemblies to be analysed
                            foreach (Target assembly in this.checkedListBox1.CheckedItems)
                            {
                                analyser.IncludeAssembly(assembly);
                            }

                            // Set options for model and analyser
                            DsmOptions options = new DsmOptions();

                            options.DsmModelType= (radioButton1.Checked) ?
                                DsmOptions.ModelType.Logical : DsmOptions.ModelType.Physical;
                            options.ExcludeCompilerNamespaces = chkExCompName.Checked;
                            options.ExcludeGlobalNamespace    = chkExGlobal.Checked;
                            options.HideNestedClasses         = chkHideNested.Checked;

                            DsmModel newModel = new Tcdev.Dsm.Model.DsmModel();
                            newModel.Options = options;
                            analyser.Options = options;

                            ModelessMessageBox msg = new ModelessMessageBox("Analysing");
                            msg.Show();
                            try
                            {


                                ICommand cmd = new CommandAnalyse(analyser, newModel);

                                cmd.Execute(null);

                                if (cmd.Completed)
                                {
                                    matrixControl1.Size = new Size(
                                        this.tabControl.ClientSize.Width,
                                        this.tabControl.ClientSize.Height - this.toolStrip1.Height);

                                    SetModel(newModel);

                                    this.pageAssemblies.Hide();
                                    this.tabControl.SelectedTab = this.pageResults;
                                }
                            }
                            finally { msg.Dispose(); }

                            this.Refresh();
                        //}
                    }
                }
                
            }
            catch (Exception ex)
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

        //-----------------------------------------------------------------------------------------

        private void SetModel( DsmModel model )
        {
            _model = model;
            this.matrixControl1.MatrixModel = _model;
            this.matrixControl1.Enabled = true;
            this.toolStrip1.Enabled = true;

            
        }
        

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// Handle a click on one of the menu bttons
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void toolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            try
            {
                ToolStripItem item = e.ClickedItem;
                if (item == this.btnMoveDown)
                {
                    matrixControl1.MoveSelectedNodeDown();
                }
                else if ( item == this.btnMoveUp )
                {
                    matrixControl1.MoveSelectedNodeUp();
                }
                else if (item == this.btnHighlightCyclic)
                {
                    matrixControl1.DisplayOptions.ShowCyclicRelations = 
                        !(matrixControl1.DisplayOptions.ShowCyclicRelations);
                }
                else if (item == this.btnSave)
                {
                    DoProjectSave();
                }
                else if (item == this.btnReports)
                {
                    DoShowReport();
                }
                else if (item == this.btnPartition)
                {
                    DoPartitioning();
                }
                else if (item == this.btnAddRule)
                {
                    matrixControl1.AddRule();
                }
                else if (item == this.btnMacroView)
                {
                    MacroView macroView = new MacroView();
                    macroView.Model = _model;
                    macroView.Build();
                    macroView.Show();
                    macroView.Refresh();
                }
            }
            catch (Exception ex)
            {
                ErrorDialog dlg = new ErrorDialog(ex.ToString());
                dlg.ShowDialog();
                dlg.Dispose();
            }
        }

        //-------------------------------------------------------------------------------------------------
        void DoShowReport()
        {
            ICommand cmd = new CommandReport(_model);

            try
            {
                cmd.Execute(null);
            }
            catch (DsmException dsmEx)
            {
                MessageBox.Show("Unable to create the report for the following reason: " +
                    Environment.NewLine + Environment.NewLine + dsmEx.ToString(), 
                    "Dependency Structure Matrix",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation); 
            }
            catch (Exception e)
            {
                ErrorDialog dlg = new ErrorDialog(e.ToString());
                dlg.ShowDialog();
                dlg.Dispose();
            }
        }

        //-------------------------------------------------------------------------------------------------
        public void DoProjectOpen(FileInfo dsmFile )
        {
            if ( ConfirmModelSaved() )
            {
                DsmModel newModel = new DsmModel();
                ICommand cmd = new CommandOpen(newModel, dsmFile );

                CursorStateHelper csh = new CursorStateHelper(this, Cursors.WaitCursor);

                Refresh();

                ModelessMessageBox msg = new ModelessMessageBox("Loading project file");
                try
                {
                    cmd.Execute( msg.UpdateProgress );

                    if (cmd.Completed)
                    {
                        SetModel(newModel);

                        matrixControl1.Size = new Size(this.tabControl.ClientSize.Width,
                               this.tabControl.ClientSize.Height - this.toolStrip1.Height);

                        this.pageAssemblies.Hide();
                        this.tabControl.SelectedTab = this.pageResults;
                    }
                }
                catch (DsmException dsmEx)
                {
                    MessageBox.Show("Unable to load DSM fom file for the following reason: " +
                        Environment.NewLine + Environment.NewLine + dsmEx.ToString(), 
                        "Dependency Structure Matrix",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                catch (Exception ex)
                {
                    ErrorDialog errdlg = new ErrorDialog(ex.ToString());
                    errdlg.ShowDialog();
                    errdlg.Dispose();
                }
                finally
                {
                    msg.Dispose();
                    csh.Reset();
                }
            }
        }
        //-------------------------------------------------------------------------------------------------
        private void DoProjectSave()
        {
            ICommand cmd = new CommandSave( _model );

            CursorStateHelper csh = new CursorStateHelper(this, Cursors.WaitCursor);

            Refresh();

            try
            {
                cmd.Execute(null);
            }
            catch (Exception ex)
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

        //-------------------------------------------------------------------------------------------------
        
        void DoPartitioning()
        {
            ICommand cmd = new CommandPartition(_model);
            CursorStateHelper csh = new CursorStateHelper(this, Cursors.WaitCursor);

            Refresh();

            try
            {
                cmd.Execute(null);

               this.matrixControl1.Invalidate();
                
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
        
        //-------------------------------------------------------------------------------------------------
        private void btnZoom_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            ToolStripItem item = e.ClickedItem;

            this.itmZoom1.Checked = false;
            this.itmZoom2.Checked = false;
            this.itmZoom3.Checked = false;
            this.itmZoom4.Checked = false;
            this.itmZoom5.Checked = false;

            if (item == this.itmZoom1)
            {
                this.matrixControl1.DisplayOptions.SetZoomLevel(1);
            }
            else if (item == itmZoom2)
            {
                this.matrixControl1.DisplayOptions.SetZoomLevel(2);
            }
            else if ( item == itmZoom3 )
            {
                this.matrixControl1.DisplayOptions.SetZoomLevel(3);
            }
            else if (item == itmZoom4)
            {
                this.matrixControl1.DisplayOptions.SetZoomLevel(4);
            }
            else
            {
                this.matrixControl1.DisplayOptions.SetZoomLevel(5);
            }
        }
        //-------------------------------------------------------------------------------------------------
        private void btnBrowse_Click(object sender, EventArgs e)
        {
            DoProjectOpen(null);
        }

        ////-------------------------------------------------------------------------------------------------
        //public void UpdateProgress(int val, string message)
        //{
        //    this.lblStatus.Text = message;
        //    this.progressBar1.Value = val;

        //    this.lblStatus.Refresh();
        //    this.progressBar1.Refresh();
        //}

        //-------------------------------------------------------------------------------------------------

        bool ConfirmModelSaved()
        {
            bool cont = true;

            if (_model != null && _model.IsModified)
            {
                string text = "The current model has some unsaved changes." +
                    Environment.NewLine + Environment.NewLine +
                    "Do you wish to save the model before continuing ?";

                DialogResult dr = MessageBox.Show(text, "Unsaved changes",
                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                if (dr.Equals(DialogResult.Yes))
                {
                    ICommand cmd = new CommandSave(_model);
                    cmd.Execute(null);
                }
                else if (dr.Equals(DialogResult.Cancel))
                {
                    cont = false;
                }
                // else No - cont  true;
            }

            return cont;
        }

        //-------------------------------------------------------------------------------------------------

        public bool OnClosing()
        {
            bool closed = true;

            if (_model != null && _model.IsModified)
            {
                string text = "The current model has some unsaved changes." +
                    Environment.NewLine + Environment.NewLine +
                    "Do you wish to save your changes now ? " +
                    Environment.NewLine + Environment.NewLine +
                    "They will be lost if you click No";

                DialogResult dr = MessageBox.Show(text, "DSM PlugIn - Saved changes ?",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (dr.Equals(DialogResult.Yes))
                {
                    ICommand cmd = new CommandSave(_model);
                    cmd.Execute(null);
                }
                else
                {
                    closed = false;
                }
            }

            return closed;
        }

        private void tabControl_KeyUp(object sender, KeyEventArgs e)
        {
            // Process key at this level if necessary then pass to DSM

            matrixControl1.HandleKeyEvent(e);
        }

        //-------------------------------------------------------------------------------------------------
    }
}

namespace Tcdev.Dsm.View
{
    partial class MacroView
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
            this.macroViewPanel1 = new Tcdev.Dsm.View.MacroViewPanel();
            this.SuspendLayout();
            // 
            // macroViewPanel1
            // 
            this.macroViewPanel1.AutoScroll = true;
            this.macroViewPanel1.Location = new System.Drawing.Point(0, 0);
            this.macroViewPanel1.Model = null;
            this.macroViewPanel1.Name = "macroViewPanel1";
            this.macroViewPanel1.Size = new System.Drawing.Size(900, 679);
            this.macroViewPanel1.TabIndex = 0;
            // 
            // MacroView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(900, 679);
            this.Controls.Add(this.macroViewPanel1);
            this.Name = "MacroView";
            this.Text = "MacroView";
            this.ResumeLayout(false);

        }

        #endregion

        private MacroViewPanel macroViewPanel1;
    }
}
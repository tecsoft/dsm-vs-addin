namespace Tcdev.Dsm.View
{
    partial class ErrorDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager( typeof( ErrorDialog ) );
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.txtBoxError = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // richTextBox1
            // 
            this.richTextBox1.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.richTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBox1.Location = new System.Drawing.Point( 12, 12 );
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size( 410, 65 );
            this.richTextBox1.TabIndex = 0;
            this.richTextBox1.Text = "Sorry, but an unexpected error has occurred. \n\nPlease report the error text displ" +
                "ayed below to tom.e.carter@gmail.com, \nexplaining what you were trying to do pri" +
                "or to this dialog box being displayed.";
            // 
            // txtBoxError
            // 
            this.txtBoxError.AcceptsReturn = true;
            this.txtBoxError.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.txtBoxError.Location = new System.Drawing.Point( 12, 83 );
            this.txtBoxError.Multiline = true;
            this.txtBoxError.Name = "txtBoxError";
            this.txtBoxError.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtBoxError.Size = new System.Drawing.Size( 410, 252 );
            this.txtBoxError.TabIndex = 1;
            // 
            // ErrorDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size( 434, 352 );
            this.Controls.Add( this.txtBoxError );
            this.Controls.Add( this.richTextBox1 );
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ( (System.Drawing.Icon)( resources.GetObject( "$this.Icon" ) ) );
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ErrorDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "DSM Plugin - Unexpected Error";
            this.ResumeLayout( false );
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.TextBox txtBoxError;
    }
}
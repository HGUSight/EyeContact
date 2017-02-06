namespace Renewal
{
    partial class magnifier
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
            this.SuspendLayout();
            // 
            // magnifier
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(282, 253);
            this.ControlBox = false;
            this.DoubleBuffered = true;
            this.MaximizeBox = false;
            this.Name = "magnifier";
            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.magnifier_FormClosing);
            this.Load += new System.EventHandler(this.magnifier_Load);
          //  this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.magnifier_MouseDown);
          //  this.MouseLeave += new System.EventHandler(this.magnifier_MouseLeave);
            //this.MouseHover += new System.EventHandler(this.magnifier_MouseHover);
            //this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.magnifier_MouseMove);
            this.ResumeLayout(false);

        }

        #endregion
    }
}
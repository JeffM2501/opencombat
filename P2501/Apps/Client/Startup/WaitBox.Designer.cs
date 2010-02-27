namespace P2501Client
{
    partial class WaitBox
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
            this.StatusLine = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.SuspendLayout();
            // 
            // StatusLine
            // 
            this.StatusLine.AutoSize = true;
            this.StatusLine.Location = new System.Drawing.Point(3, 9);
            this.StatusLine.Name = "StatusLine";
            this.StatusLine.Size = new System.Drawing.Size(37, 13);
            this.StatusLine.TabIndex = 0;
            this.StatusLine.Text = "Status";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(6, 35);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(294, 23);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBar1.TabIndex = 1;
            // 
            // WaitBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(305, 67);
            this.ControlBox = false;
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.StatusLine);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "WaitBox";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "WaitBox";
            this.Load += new System.EventHandler(this.WaitBox_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label StatusLine;
        private System.Windows.Forms.ProgressBar progressBar1;
    }
}
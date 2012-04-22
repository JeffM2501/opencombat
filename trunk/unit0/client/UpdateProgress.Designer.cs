namespace Client
{
    partial class UpdateProgress
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
            this.components = new System.ComponentModel.Container();
            this.Cancel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.UpdateStatusLabel = new System.Windows.Forms.Label();
            this.UpdateStatusBar = new System.Windows.Forms.ProgressBar();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // Cancel
            // 
            this.Cancel.Location = new System.Drawing.Point(361, 9);
            this.Cancel.Name = "Cancel";
            this.Cancel.Size = new System.Drawing.Size(75, 23);
            this.Cancel.TabIndex = 0;
            this.Cancel.Text = "Cancel";
            this.Cancel.UseVisualStyleBackColor = true;
            this.Cancel.Click += new System.EventHandler(this.Cancel_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(108, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Checking for updates";
            // 
            // UpdateStatusLabel
            // 
            this.UpdateStatusLabel.AutoSize = true;
            this.UpdateStatusLabel.Location = new System.Drawing.Point(12, 34);
            this.UpdateStatusLabel.Name = "UpdateStatusLabel";
            this.UpdateStatusLabel.Size = new System.Drawing.Size(130, 13);
            this.UpdateStatusLabel.TabIndex = 2;
            this.UpdateStatusLabel.Text = "Contacting Update Server";
            // 
            // UpdateStatusBar
            // 
            this.UpdateStatusBar.Location = new System.Drawing.Point(12, 50);
            this.UpdateStatusBar.Name = "UpdateStatusBar";
            this.UpdateStatusBar.Size = new System.Drawing.Size(424, 23);
            this.UpdateStatusBar.TabIndex = 3;
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // UpdateProgress
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(448, 86);
            this.ControlBox = false;
            this.Controls.Add(this.UpdateStatusBar);
            this.Controls.Add(this.UpdateStatusLabel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Cancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "UpdateProgress";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "UpdateProgress";
            this.Load += new System.EventHandler(this.UpdateProgress_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button Cancel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label UpdateStatusLabel;
        private System.Windows.Forms.ProgressBar UpdateStatusBar;
        private System.Windows.Forms.Timer timer1;
    }
}
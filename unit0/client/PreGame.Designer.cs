namespace Client
{
    partial class PreGame
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
            this.Play_BN = new System.Windows.Forms.Button();
            this.WebFrame = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // Play_BN
            // 
            this.Play_BN.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Play_BN.Location = new System.Drawing.Point(715, 284);
            this.Play_BN.Name = "Play_BN";
            this.Play_BN.Size = new System.Drawing.Size(75, 23);
            this.Play_BN.TabIndex = 0;
            this.Play_BN.Text = "Play";
            this.Play_BN.UseVisualStyleBackColor = true;
            this.Play_BN.Click += new System.EventHandler(this.Play_BN_Click);
            // 
            // WebFrame
            // 
            this.WebFrame.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.WebFrame.Location = new System.Drawing.Point(0, 12);
            this.WebFrame.Name = "WebFrame";
            this.WebFrame.Size = new System.Drawing.Size(799, 266);
            this.WebFrame.TabIndex = 1;
            this.WebFrame.Resize += new System.EventHandler(this.WebFrame_Resize);
            // 
            // PreGame
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(802, 319);
            this.Controls.Add(this.WebFrame);
            this.Controls.Add(this.Play_BN);
            this.Name = "PreGame";
            this.Text = "Pre-Game";
            this.Load += new System.EventHandler(this.PreGame_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.PreGame_Paint);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button Play_BN;
        private System.Windows.Forms.Panel WebFrame;
    }
}


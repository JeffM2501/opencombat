namespace Client
{
    partial class FailoverBrowser
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FailoverBrowser));
            if (UseBrowser)
                this.Browser = new System.Windows.Forms.WebBrowser();
            else
            {
                this.ImageBox = new System.Windows.Forms.PictureBox();
                ((System.ComponentModel.ISupportInitialize)(this.ImageBox)).BeginInit();
            }
            this.SuspendLayout();
         
            if (UseBrowser)
            {
            // 
                // Browser
                // 

                this.Browser.Dock = System.Windows.Forms.DockStyle.Fill;
                this.Browser.Location = new System.Drawing.Point(0, 0);
                this.Browser.MinimumSize = new System.Drawing.Size(20, 20);
                this.Browser.Name = "Browser";
                this.Browser.Size = new System.Drawing.Size(456, 346);
                this.Browser.TabIndex = 0;
            }
            else
            {
                // 
                // ImageBox
                // 
                this.ImageBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
                | System.Windows.Forms.AnchorStyles.Left) 
                | System.Windows.Forms.AnchorStyles.Right)));
                this.ImageBox.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("ImageBox.BackgroundImage")));
                this.ImageBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
                this.ImageBox.Location = new System.Drawing.Point(0, 0);
                this.ImageBox.Name = "ImageBox";
                this.ImageBox.Size = new System.Drawing.Size(453, 346);
                this.ImageBox.TabIndex = 1;
                this.ImageBox.TabStop = false;
            }
            // 
            // FailoverBrowser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            if (!UseBrowser) 
                this.Controls.Add(this.ImageBox);
            else
                this.Controls.Add(this.Browser);
            this.Name = "FailoverBrowser";
            this.Size = new System.Drawing.Size(456, 346);
            if (!UseBrowser) 
                ((System.ComponentModel.ISupportInitialize)(this.ImageBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.WebBrowser Browser;
        private System.Windows.Forms.PictureBox ImageBox;
    }
}

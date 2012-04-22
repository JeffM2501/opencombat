namespace Editor.Dialogs
{
    partial class TextureManagement
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.InfoBox = new System.Windows.Forms.GroupBox();
            this.EndIDTE = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.StartIDTE = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.BlockY = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.BlockX = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.Remove = new System.Windows.Forms.Button();
            this.Add = new System.Windows.Forms.Button();
            this.TextureList = new System.Windows.Forms.ListBox();
            this.PreviewBox = new System.Windows.Forms.PictureBox();
            this.panel1.SuspendLayout();
            this.InfoBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.BlockY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.BlockX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PreviewBox)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.InfoBox);
            this.panel1.Controls.Add(this.Remove);
            this.panel1.Controls.Add(this.Add);
            this.panel1.Controls.Add(this.TextureList);
            this.panel1.Location = new System.Drawing.Point(447, 2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(166, 402);
            this.panel1.TabIndex = 0;
            // 
            // InfoBox
            // 
            this.InfoBox.Controls.Add(this.EndIDTE);
            this.InfoBox.Controls.Add(this.label4);
            this.InfoBox.Controls.Add(this.StartIDTE);
            this.InfoBox.Controls.Add(this.label3);
            this.InfoBox.Controls.Add(this.BlockY);
            this.InfoBox.Controls.Add(this.label2);
            this.InfoBox.Controls.Add(this.BlockX);
            this.InfoBox.Controls.Add(this.label1);
            this.InfoBox.Location = new System.Drawing.Point(3, 156);
            this.InfoBox.Name = "InfoBox";
            this.InfoBox.Size = new System.Drawing.Size(156, 191);
            this.InfoBox.TabIndex = 3;
            this.InfoBox.TabStop = false;
            this.InfoBox.Text = "Info";
            // 
            // EndIDTE
            // 
            this.EndIDTE.Location = new System.Drawing.Point(9, 164);
            this.EndIDTE.Name = "EndIDTE";
            this.EndIDTE.ReadOnly = true;
            this.EndIDTE.Size = new System.Drawing.Size(89, 20);
            this.EndIDTE.TabIndex = 7;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 148);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(40, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "End ID";
            // 
            // StartIDTE
            // 
            this.StartIDTE.Location = new System.Drawing.Point(9, 125);
            this.StartIDTE.Name = "StartIDTE";
            this.StartIDTE.ReadOnly = true;
            this.StartIDTE.Size = new System.Drawing.Size(89, 20);
            this.StartIDTE.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 109);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(43, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Start ID";
            // 
            // BlockY
            // 
            this.BlockY.Location = new System.Drawing.Point(9, 76);
            this.BlockY.Name = "BlockY";
            this.BlockY.Size = new System.Drawing.Size(86, 20);
            this.BlockY.TabIndex = 3;
            this.BlockY.ValueChanged += new System.EventHandler(this.Block_ValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 60);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(92, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Blocks Per Height";
            // 
            // BlockX
            // 
            this.BlockX.Location = new System.Drawing.Point(9, 32);
            this.BlockX.Name = "BlockX";
            this.BlockX.Size = new System.Drawing.Size(86, 20);
            this.BlockX.TabIndex = 1;
            this.BlockX.ValueChanged += new System.EventHandler(this.Block_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Blocks Per Width";
            // 
            // Remove
            // 
            this.Remove.Location = new System.Drawing.Point(84, 127);
            this.Remove.Name = "Remove";
            this.Remove.Size = new System.Drawing.Size(75, 23);
            this.Remove.TabIndex = 2;
            this.Remove.Text = "Remove";
            this.Remove.UseVisualStyleBackColor = true;
            this.Remove.Click += new System.EventHandler(this.Remove_Click);
            // 
            // Add
            // 
            this.Add.Location = new System.Drawing.Point(3, 127);
            this.Add.Name = "Add";
            this.Add.Size = new System.Drawing.Size(75, 23);
            this.Add.TabIndex = 1;
            this.Add.Text = "Add";
            this.Add.UseVisualStyleBackColor = true;
            this.Add.Click += new System.EventHandler(this.Add_Click);
            // 
            // TextureList
            // 
            this.TextureList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TextureList.FormattingEnabled = true;
            this.TextureList.Location = new System.Drawing.Point(3, 0);
            this.TextureList.Name = "TextureList";
            this.TextureList.Size = new System.Drawing.Size(160, 121);
            this.TextureList.TabIndex = 0;
            this.TextureList.SelectedIndexChanged += new System.EventHandler(this.TextureList_SelectedIndexChanged);
            // 
            // PreviewBox
            // 
            this.PreviewBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PreviewBox.Location = new System.Drawing.Point(2, 2);
            this.PreviewBox.Name = "PreviewBox";
            this.PreviewBox.Size = new System.Drawing.Size(439, 399);
            this.PreviewBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.PreviewBox.TabIndex = 1;
            this.PreviewBox.TabStop = false;
            // 
            // TextureManagement
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(616, 407);
            this.Controls.Add(this.PreviewBox);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TextureManagement";
            this.Text = "TextureManagement";
            this.Load += new System.EventHandler(this.TextureManagement_Load);
            this.panel1.ResumeLayout(false);
            this.InfoBox.ResumeLayout(false);
            this.InfoBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.BlockY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.BlockX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PreviewBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox InfoBox;
        private System.Windows.Forms.TextBox EndIDTE;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox StartIDTE;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown BlockY;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown BlockX;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button Remove;
        private System.Windows.Forms.Button Add;
        private System.Windows.Forms.ListBox TextureList;
        private System.Windows.Forms.PictureBox PreviewBox;
    }
}
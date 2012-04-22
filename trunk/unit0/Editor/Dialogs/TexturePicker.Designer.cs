namespace Editor.Dialogs
{
    partial class TexturePicker
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
            this.TextureSet = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.TextureIndex = new System.Windows.Forms.TextBox();
            this.ImageBox = new System.Windows.Forms.PictureBox();
            this.Done = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.ImageBox)).BeginInit();
            this.SuspendLayout();
            // 
            // TextureSet
            // 
            this.TextureSet.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.TextureSet.FormattingEnabled = true;
            this.TextureSet.Location = new System.Drawing.Point(80, 12);
            this.TextureSet.Name = "TextureSet";
            this.TextureSet.Size = new System.Drawing.Size(247, 21);
            this.TextureSet.TabIndex = 0;
            this.TextureSet.SelectedIndexChanged += new System.EventHandler(this.TextureSet_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Texture Set";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(333, 15);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(72, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Texture Index";
            // 
            // TextureIndex
            // 
            this.TextureIndex.Location = new System.Drawing.Point(411, 12);
            this.TextureIndex.Name = "TextureIndex";
            this.TextureIndex.ReadOnly = true;
            this.TextureIndex.Size = new System.Drawing.Size(100, 20);
            this.TextureIndex.TabIndex = 3;
            // 
            // ImageBox
            // 
            this.ImageBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ImageBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ImageBox.Location = new System.Drawing.Point(12, 38);
            this.ImageBox.Name = "ImageBox";
            this.ImageBox.Size = new System.Drawing.Size(642, 435);
            this.ImageBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.ImageBox.TabIndex = 4;
            this.ImageBox.TabStop = false;
            this.ImageBox.Click += new System.EventHandler(this.ImageBox_Click);
            this.ImageBox.Paint += new System.Windows.Forms.PaintEventHandler(this.ImageBox_Paint);
            this.ImageBox.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ImageBox_MouseClick);
            // 
            // Done
            // 
            this.Done.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Done.Location = new System.Drawing.Point(579, 5);
            this.Done.Name = "Done";
            this.Done.Size = new System.Drawing.Size(75, 23);
            this.Done.TabIndex = 5;
            this.Done.Text = "Done";
            this.Done.UseVisualStyleBackColor = true;
            // 
            // TexturePicker
            // 
            this.AcceptButton = this.Done;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(666, 485);
            this.Controls.Add(this.Done);
            this.Controls.Add(this.ImageBox);
            this.Controls.Add(this.TextureIndex);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.TextureSet);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TexturePicker";
            this.Text = "Texture Picker";
            this.Load += new System.EventHandler(this.TexturePicker_Load);
            ((System.ComponentModel.ISupportInitialize)(this.ImageBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox TextureSet;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox TextureIndex;
        private System.Windows.Forms.PictureBox ImageBox;
        private System.Windows.Forms.Button Done;

    }
}
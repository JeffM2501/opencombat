namespace Editor.Dialogs
{
    partial class FillZRange
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
            this.MaxZ = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.MinZ = new System.Windows.Forms.NumericUpDown();
            this.OK = new System.Windows.Forms.Button();
            this.Cancel = new System.Windows.Forms.Button();
            this.BlockList = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.FillMap = new System.Windows.Forms.CheckBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.MinY = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.MaxY = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.MinX = new System.Windows.Forms.NumericUpDown();
            this.Label2223 = new System.Windows.Forms.Label();
            this.MaxX = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.GeometryTypes = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.MaxZ)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.MinZ)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.MinY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.MaxY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.MinX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.MaxX)).BeginInit();
            this.SuspendLayout();
            // 
            // MaxZ
            // 
            this.MaxZ.Location = new System.Drawing.Point(55, 12);
            this.MaxZ.Name = "MaxZ";
            this.MaxZ.Size = new System.Drawing.Size(93, 20);
            this.MaxZ.TabIndex = 0;
            this.MaxZ.ValueChanged += new System.EventHandler(this.ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(37, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Max Z";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 40);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(34, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Min Z";
            // 
            // MinZ
            // 
            this.MinZ.Location = new System.Drawing.Point(55, 38);
            this.MinZ.Name = "MinZ";
            this.MinZ.Size = new System.Drawing.Size(93, 20);
            this.MinZ.TabIndex = 2;
            this.MinZ.ValueChanged += new System.EventHandler(this.ValueChanged);
            // 
            // OK
            // 
            this.OK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.OK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OK.Location = new System.Drawing.Point(267, 203);
            this.OK.Name = "OK";
            this.OK.Size = new System.Drawing.Size(75, 23);
            this.OK.TabIndex = 4;
            this.OK.Text = "OK";
            this.OK.UseVisualStyleBackColor = true;
            this.OK.Click += new System.EventHandler(this.OK_Click);
            // 
            // Cancel
            // 
            this.Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Cancel.Location = new System.Drawing.Point(186, 203);
            this.Cancel.Name = "Cancel";
            this.Cancel.Size = new System.Drawing.Size(75, 23);
            this.Cancel.TabIndex = 5;
            this.Cancel.Text = "Cancel";
            this.Cancel.UseVisualStyleBackColor = true;
            // 
            // BlockList
            // 
            this.BlockList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.BlockList.FormattingEnabled = true;
            this.BlockList.Location = new System.Drawing.Point(234, 11);
            this.BlockList.Name = "BlockList";
            this.BlockList.Size = new System.Drawing.Size(108, 21);
            this.BlockList.TabIndex = 6;
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(167, 14);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(61, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Block Type";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.FillMap);
            this.groupBox1.Controls.Add(this.panel1);
            this.groupBox1.Location = new System.Drawing.Point(15, 66);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(327, 131);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Bounds";
            // 
            // FillMap
            // 
            this.FillMap.AutoSize = true;
            this.FillMap.Location = new System.Drawing.Point(13, 19);
            this.FillMap.Name = "FillMap";
            this.FillMap.Size = new System.Drawing.Size(92, 17);
            this.FillMap.TabIndex = 1;
            this.FillMap.Text = "Fill Entire Map";
            this.FillMap.UseVisualStyleBackColor = true;
            this.FillMap.CheckedChanged += new System.EventHandler(this.FillMap_CheckedChanged);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.MinY);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.MaxY);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.MinX);
            this.panel1.Controls.Add(this.Label2223);
            this.panel1.Controls.Add(this.MaxX);
            this.panel1.Location = new System.Drawing.Point(13, 42);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(301, 71);
            this.panel1.TabIndex = 0;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(145, 38);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(34, 13);
            this.label5.TabIndex = 11;
            this.label5.Text = "Min Y";
            // 
            // MinY
            // 
            this.MinY.Location = new System.Drawing.Point(188, 36);
            this.MinY.Maximum = new decimal(new int[] {
            499,
            0,
            0,
            0});
            this.MinY.Minimum = new decimal(new int[] {
            500,
            0,
            0,
            -2147483648});
            this.MinY.Name = "MinY";
            this.MinY.Size = new System.Drawing.Size(93, 20);
            this.MinY.TabIndex = 10;
            this.MinY.ValueChanged += new System.EventHandler(this.ValueChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(145, 12);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(37, 13);
            this.label6.TabIndex = 9;
            this.label6.Text = "Max Y";
            // 
            // MaxY
            // 
            this.MaxY.Location = new System.Drawing.Point(188, 10);
            this.MaxY.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.MaxY.Minimum = new decimal(new int[] {
            499,
            0,
            0,
            -2147483648});
            this.MaxY.Name = "MaxY";
            this.MaxY.Size = new System.Drawing.Size(93, 20);
            this.MaxY.TabIndex = 8;
            this.MaxY.ValueChanged += new System.EventHandler(this.ValueChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 38);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(34, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Min X";
            // 
            // MinX
            // 
            this.MinX.Location = new System.Drawing.Point(46, 36);
            this.MinX.Maximum = new decimal(new int[] {
            499,
            0,
            0,
            0});
            this.MinX.Minimum = new decimal(new int[] {
            500,
            0,
            0,
            -2147483648});
            this.MinX.Name = "MinX";
            this.MinX.Size = new System.Drawing.Size(93, 20);
            this.MinX.TabIndex = 6;
            this.MinX.ValueChanged += new System.EventHandler(this.ValueChanged);
            // 
            // Label2223
            // 
            this.Label2223.AutoSize = true;
            this.Label2223.Location = new System.Drawing.Point(3, 12);
            this.Label2223.Name = "Label2223";
            this.Label2223.Size = new System.Drawing.Size(37, 13);
            this.Label2223.TabIndex = 5;
            this.Label2223.Text = "Max X";
            // 
            // MaxX
            // 
            this.MaxX.Location = new System.Drawing.Point(46, 10);
            this.MaxX.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.MaxX.Minimum = new decimal(new int[] {
            499,
            0,
            0,
            -2147483648});
            this.MaxX.Name = "MaxX";
            this.MaxX.Size = new System.Drawing.Size(93, 20);
            this.MaxX.TabIndex = 4;
            this.MaxX.ValueChanged += new System.EventHandler(this.ValueChanged);
            // 
            // label7
            // 
            this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(167, 40);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(52, 13);
            this.label7.TabIndex = 10;
            this.label7.Text = "Geometry";
            // 
            // GeometryTypes
            // 
            this.GeometryTypes.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.GeometryTypes.FormattingEnabled = true;
            this.GeometryTypes.Location = new System.Drawing.Point(234, 37);
            this.GeometryTypes.Name = "GeometryTypes";
            this.GeometryTypes.Size = new System.Drawing.Size(108, 21);
            this.GeometryTypes.TabIndex = 9;
            // 
            // FillZRange
            // 
            this.AcceptButton = this.OK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.Cancel;
            this.ClientSize = new System.Drawing.Size(354, 238);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.GeometryTypes);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.BlockList);
            this.Controls.Add(this.Cancel);
            this.Controls.Add(this.OK);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.MinZ);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.MaxZ);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FillZRange";
            this.Text = "FillZRange";
            this.Load += new System.EventHandler(this.FillZRange_Load);
            ((System.ComponentModel.ISupportInitialize)(this.MaxZ)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.MinZ)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.MinY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.MaxY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.MinX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.MaxX)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NumericUpDown MaxZ;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown MinZ;
        private System.Windows.Forms.Button OK;
        private System.Windows.Forms.Button Cancel;
        private System.Windows.Forms.ComboBox BlockList;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox FillMap;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label Label2223;
        private System.Windows.Forms.NumericUpDown MaxX;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown MinY;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown MaxY;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown MinX;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox GeometryTypes;
    }
}
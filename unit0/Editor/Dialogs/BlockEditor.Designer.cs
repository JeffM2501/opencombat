namespace Editor.Dialogs
{
    partial class BlockEditor
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
            this.glControl1 = new OpenTK.GLControl();
            this.panel1 = new System.Windows.Forms.Panel();
            this.InfoGroup = new System.Windows.Forms.GroupBox();
            this.BottomID = new System.Windows.Forms.NumericUpDown();
            this.BottomEnabled = new System.Windows.Forms.CheckBox();
            this.WestID = new System.Windows.Forms.NumericUpDown();
            this.WestEnabled = new System.Windows.Forms.CheckBox();
            this.EastID = new System.Windows.Forms.NumericUpDown();
            this.EastEnabled = new System.Windows.Forms.CheckBox();
            this.SouthID = new System.Windows.Forms.NumericUpDown();
            this.SouthEnabled = new System.Windows.Forms.CheckBox();
            this.NorthID = new System.Windows.Forms.NumericUpDown();
            this.NorthEnabled = new System.Windows.Forms.CheckBox();
            this.TopID = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.Remove = new System.Windows.Forms.Button();
            this.Add = new System.Windows.Forms.Button();
            this.BlockList = new System.Windows.Forms.ListBox();
            this.GeometryList = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.BottomSet = new System.Windows.Forms.Button();
            this.WestSet = new System.Windows.Forms.Button();
            this.EastSet = new System.Windows.Forms.Button();
            this.SouthSet = new System.Windows.Forms.Button();
            this.NorthSet = new System.Windows.Forms.Button();
            this.TopSet = new System.Windows.Forms.Button();
            this.Trans = new System.Windows.Forms.CheckBox();
            this.Rename = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.InfoGroup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.BottomID)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.WestID)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.EastID)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.SouthID)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NorthID)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.TopID)).BeginInit();
            this.SuspendLayout();
            // 
            // glControl1
            // 
            this.glControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.glControl1.BackColor = System.Drawing.Color.Black;
            this.glControl1.Location = new System.Drawing.Point(2, 24);
            this.glControl1.Name = "glControl1";
            this.glControl1.Size = new System.Drawing.Size(508, 540);
            this.glControl1.TabIndex = 0;
            this.glControl1.VSync = false;
            this.glControl1.Load += new System.EventHandler(this.glControl1_Load);
            this.glControl1.Paint += new System.Windows.Forms.PaintEventHandler(this.glControl1_Paint);
            this.glControl1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.glControl1_MouseDown);
            this.glControl1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.glControl1_MouseMove);
            this.glControl1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.glControl1_MouseUp);
            this.glControl1.Resize += new System.EventHandler(this.glControl1_Resize);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.Rename);
            this.panel1.Controls.Add(this.InfoGroup);
            this.panel1.Controls.Add(this.Remove);
            this.panel1.Controls.Add(this.Add);
            this.panel1.Controls.Add(this.BlockList);
            this.panel1.Location = new System.Drawing.Point(516, 2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(172, 562);
            this.panel1.TabIndex = 1;
            // 
            // InfoGroup
            // 
            this.InfoGroup.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.InfoGroup.Controls.Add(this.Trans);
            this.InfoGroup.Controls.Add(this.BottomSet);
            this.InfoGroup.Controls.Add(this.BottomID);
            this.InfoGroup.Controls.Add(this.BottomEnabled);
            this.InfoGroup.Controls.Add(this.WestSet);
            this.InfoGroup.Controls.Add(this.WestID);
            this.InfoGroup.Controls.Add(this.WestEnabled);
            this.InfoGroup.Controls.Add(this.EastSet);
            this.InfoGroup.Controls.Add(this.EastID);
            this.InfoGroup.Controls.Add(this.EastEnabled);
            this.InfoGroup.Controls.Add(this.SouthSet);
            this.InfoGroup.Controls.Add(this.SouthID);
            this.InfoGroup.Controls.Add(this.SouthEnabled);
            this.InfoGroup.Controls.Add(this.NorthSet);
            this.InfoGroup.Controls.Add(this.NorthID);
            this.InfoGroup.Controls.Add(this.TopSet);
            this.InfoGroup.Controls.Add(this.NorthEnabled);
            this.InfoGroup.Controls.Add(this.TopID);
            this.InfoGroup.Controls.Add(this.label2);
            this.InfoGroup.Location = new System.Drawing.Point(3, 172);
            this.InfoGroup.Name = "InfoGroup";
            this.InfoGroup.Size = new System.Drawing.Size(166, 387);
            this.InfoGroup.TabIndex = 3;
            this.InfoGroup.TabStop = false;
            this.InfoGroup.Text = "Details";
            // 
            // BottomID
            // 
            this.BottomID.Enabled = false;
            this.BottomID.Location = new System.Drawing.Point(9, 289);
            this.BottomID.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.BottomID.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            this.BottomID.Name = "BottomID";
            this.BottomID.Size = new System.Drawing.Size(120, 20);
            this.BottomID.TabIndex = 17;
            this.BottomID.ValueChanged += new System.EventHandler(this.BottomID_ValueChanged);
            // 
            // BottomEnabled
            // 
            this.BottomEnabled.AutoSize = true;
            this.BottomEnabled.Location = new System.Drawing.Point(9, 266);
            this.BottomEnabled.Name = "BottomEnabled";
            this.BottomEnabled.Size = new System.Drawing.Size(59, 17);
            this.BottomEnabled.TabIndex = 16;
            this.BottomEnabled.Text = "Bottom";
            this.BottomEnabled.UseVisualStyleBackColor = true;
            this.BottomEnabled.CheckedChanged += new System.EventHandler(this.BottomEnabled_CheckedChanged);
            // 
            // WestID
            // 
            this.WestID.Enabled = false;
            this.WestID.Location = new System.Drawing.Point(9, 240);
            this.WestID.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.WestID.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            this.WestID.Name = "WestID";
            this.WestID.Size = new System.Drawing.Size(120, 20);
            this.WestID.TabIndex = 14;
            this.WestID.ValueChanged += new System.EventHandler(this.WestID_ValueChanged);
            // 
            // WestEnabled
            // 
            this.WestEnabled.AutoSize = true;
            this.WestEnabled.Location = new System.Drawing.Point(9, 217);
            this.WestEnabled.Name = "WestEnabled";
            this.WestEnabled.Size = new System.Drawing.Size(51, 17);
            this.WestEnabled.TabIndex = 13;
            this.WestEnabled.Text = "West";
            this.WestEnabled.UseVisualStyleBackColor = true;
            this.WestEnabled.CheckedChanged += new System.EventHandler(this.WestEnabled_CheckedChanged);
            // 
            // EastID
            // 
            this.EastID.Enabled = false;
            this.EastID.Location = new System.Drawing.Point(9, 191);
            this.EastID.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.EastID.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            this.EastID.Name = "EastID";
            this.EastID.Size = new System.Drawing.Size(120, 20);
            this.EastID.TabIndex = 11;
            this.EastID.ValueChanged += new System.EventHandler(this.EastID_ValueChanged);
            // 
            // EastEnabled
            // 
            this.EastEnabled.AutoSize = true;
            this.EastEnabled.Location = new System.Drawing.Point(9, 168);
            this.EastEnabled.Name = "EastEnabled";
            this.EastEnabled.Size = new System.Drawing.Size(47, 17);
            this.EastEnabled.TabIndex = 10;
            this.EastEnabled.Text = "East";
            this.EastEnabled.UseVisualStyleBackColor = true;
            this.EastEnabled.CheckedChanged += new System.EventHandler(this.EastEnabled_CheckedChanged);
            // 
            // SouthID
            // 
            this.SouthID.Enabled = false;
            this.SouthID.Location = new System.Drawing.Point(9, 142);
            this.SouthID.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.SouthID.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            this.SouthID.Name = "SouthID";
            this.SouthID.Size = new System.Drawing.Size(120, 20);
            this.SouthID.TabIndex = 8;
            this.SouthID.ValueChanged += new System.EventHandler(this.SouthID_ValueChanged);
            // 
            // SouthEnabled
            // 
            this.SouthEnabled.AutoSize = true;
            this.SouthEnabled.Location = new System.Drawing.Point(9, 119);
            this.SouthEnabled.Name = "SouthEnabled";
            this.SouthEnabled.Size = new System.Drawing.Size(54, 17);
            this.SouthEnabled.TabIndex = 7;
            this.SouthEnabled.Text = "South";
            this.SouthEnabled.UseVisualStyleBackColor = true;
            this.SouthEnabled.CheckedChanged += new System.EventHandler(this.SouthEnabled_CheckedChanged);
            // 
            // NorthID
            // 
            this.NorthID.Enabled = false;
            this.NorthID.Location = new System.Drawing.Point(9, 93);
            this.NorthID.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.NorthID.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            this.NorthID.Name = "NorthID";
            this.NorthID.Size = new System.Drawing.Size(120, 20);
            this.NorthID.TabIndex = 5;
            this.NorthID.ValueChanged += new System.EventHandler(this.NorthID_ValueChanged);
            // 
            // NorthEnabled
            // 
            this.NorthEnabled.AutoSize = true;
            this.NorthEnabled.Location = new System.Drawing.Point(9, 70);
            this.NorthEnabled.Name = "NorthEnabled";
            this.NorthEnabled.Size = new System.Drawing.Size(52, 17);
            this.NorthEnabled.TabIndex = 3;
            this.NorthEnabled.Text = "North";
            this.NorthEnabled.UseVisualStyleBackColor = true;
            this.NorthEnabled.CheckedChanged += new System.EventHandler(this.NorthEnabled_CheckedChanged);
            // 
            // TopID
            // 
            this.TopID.Location = new System.Drawing.Point(9, 40);
            this.TopID.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.TopID.Name = "TopID";
            this.TopID.Size = new System.Drawing.Size(120, 20);
            this.TopID.TabIndex = 2;
            this.TopID.ValueChanged += new System.EventHandler(this.TopID_ValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 24);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(26, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Top";
            // 
            // Remove
            // 
            this.Remove.Location = new System.Drawing.Point(113, 143);
            this.Remove.Name = "Remove";
            this.Remove.Size = new System.Drawing.Size(56, 23);
            this.Remove.TabIndex = 2;
            this.Remove.Text = "Remove";
            this.Remove.UseVisualStyleBackColor = true;
            this.Remove.Click += new System.EventHandler(this.Remove_Click);
            // 
            // Add
            // 
            this.Add.Location = new System.Drawing.Point(3, 143);
            this.Add.Name = "Add";
            this.Add.Size = new System.Drawing.Size(40, 23);
            this.Add.TabIndex = 1;
            this.Add.Text = "Add";
            this.Add.UseVisualStyleBackColor = true;
            this.Add.Click += new System.EventHandler(this.Add_Click);
            // 
            // BlockList
            // 
            this.BlockList.FormattingEnabled = true;
            this.BlockList.Location = new System.Drawing.Point(3, 3);
            this.BlockList.Name = "BlockList";
            this.BlockList.Size = new System.Drawing.Size(158, 134);
            this.BlockList.TabIndex = 0;
            this.BlockList.SelectedIndexChanged += new System.EventHandler(this.BlockList_SelectedIndexChanged);
            // 
            // GeometryList
            // 
            this.GeometryList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.GeometryList.FormattingEnabled = true;
            this.GeometryList.Location = new System.Drawing.Point(60, 2);
            this.GeometryList.Name = "GeometryList";
            this.GeometryList.Size = new System.Drawing.Size(187, 21);
            this.GeometryList.TabIndex = 2;
            this.GeometryList.SelectedIndexChanged += new System.EventHandler(this.GeometryList_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(2, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(52, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Geometry";
            // 
            // BottomSet
            // 
            this.BottomSet.Enabled = false;
            this.BottomSet.Image = global::Editor.Properties.Resources.kcoloredit;
            this.BottomSet.Location = new System.Drawing.Point(134, 287);
            this.BottomSet.Name = "BottomSet";
            this.BottomSet.Size = new System.Drawing.Size(24, 24);
            this.BottomSet.TabIndex = 18;
            this.BottomSet.UseVisualStyleBackColor = true;
            this.BottomSet.Click += new System.EventHandler(this.BottomSet_Click);
            // 
            // WestSet
            // 
            this.WestSet.Enabled = false;
            this.WestSet.Image = global::Editor.Properties.Resources.kcoloredit;
            this.WestSet.Location = new System.Drawing.Point(134, 238);
            this.WestSet.Name = "WestSet";
            this.WestSet.Size = new System.Drawing.Size(24, 24);
            this.WestSet.TabIndex = 15;
            this.WestSet.UseVisualStyleBackColor = true;
            this.WestSet.Click += new System.EventHandler(this.WestSet_Click);
            // 
            // EastSet
            // 
            this.EastSet.Enabled = false;
            this.EastSet.Image = global::Editor.Properties.Resources.kcoloredit;
            this.EastSet.Location = new System.Drawing.Point(134, 189);
            this.EastSet.Name = "EastSet";
            this.EastSet.Size = new System.Drawing.Size(24, 24);
            this.EastSet.TabIndex = 12;
            this.EastSet.UseVisualStyleBackColor = true;
            this.EastSet.Click += new System.EventHandler(this.EastSet_Click);
            // 
            // SouthSet
            // 
            this.SouthSet.Enabled = false;
            this.SouthSet.Image = global::Editor.Properties.Resources.kcoloredit;
            this.SouthSet.Location = new System.Drawing.Point(134, 140);
            this.SouthSet.Name = "SouthSet";
            this.SouthSet.Size = new System.Drawing.Size(24, 24);
            this.SouthSet.TabIndex = 9;
            this.SouthSet.UseVisualStyleBackColor = true;
            this.SouthSet.Click += new System.EventHandler(this.SouthSet_Click);
            // 
            // NorthSet
            // 
            this.NorthSet.Enabled = false;
            this.NorthSet.Image = global::Editor.Properties.Resources.kcoloredit;
            this.NorthSet.Location = new System.Drawing.Point(134, 91);
            this.NorthSet.Name = "NorthSet";
            this.NorthSet.Size = new System.Drawing.Size(24, 24);
            this.NorthSet.TabIndex = 6;
            this.NorthSet.UseVisualStyleBackColor = true;
            this.NorthSet.Click += new System.EventHandler(this.NorthSet_Click);
            // 
            // TopSet
            // 
            this.TopSet.Image = global::Editor.Properties.Resources.kcoloredit;
            this.TopSet.Location = new System.Drawing.Point(134, 38);
            this.TopSet.Name = "TopSet";
            this.TopSet.Size = new System.Drawing.Size(24, 24);
            this.TopSet.TabIndex = 4;
            this.TopSet.UseVisualStyleBackColor = true;
            this.TopSet.Click += new System.EventHandler(this.TopSet_Click);
            // 
            // Trans
            // 
            this.Trans.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Trans.AutoSize = true;
            this.Trans.Location = new System.Drawing.Point(9, 324);
            this.Trans.Name = "Trans";
            this.Trans.Size = new System.Drawing.Size(89, 17);
            this.Trans.TabIndex = 19;
            this.Trans.Text = "Transparent  ";
            this.Trans.UseVisualStyleBackColor = true;
            this.Trans.CheckedChanged += new System.EventHandler(this.Trans_CheckedChanged);
            // 
            // Rename
            // 
            this.Rename.Location = new System.Drawing.Point(49, 143);
            this.Rename.Name = "Rename";
            this.Rename.Size = new System.Drawing.Size(58, 23);
            this.Rename.TabIndex = 4;
            this.Rename.Text = "Rename";
            this.Rename.UseVisualStyleBackColor = true;
            this.Rename.Click += new System.EventHandler(this.Rename_Click);
            // 
            // BlockEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(688, 570);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.GeometryList);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.glControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "BlockEditor";
            this.Text = "BlockEditor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.BlockEditor_FormClosing);
            this.panel1.ResumeLayout(false);
            this.InfoGroup.ResumeLayout(false);
            this.InfoGroup.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.BottomID)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.WestID)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.EastID)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.SouthID)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NorthID)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.TopID)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private OpenTK.GLControl glControl1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox InfoGroup;
        private System.Windows.Forms.Button Remove;
        private System.Windows.Forms.Button Add;
        private System.Windows.Forms.ListBox BlockList;
        private System.Windows.Forms.ComboBox GeometryList;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button TopSet;
        private System.Windows.Forms.CheckBox NorthEnabled;
        private System.Windows.Forms.NumericUpDown TopID;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button NorthSet;
        private System.Windows.Forms.NumericUpDown NorthID;
        private System.Windows.Forms.Button SouthSet;
        private System.Windows.Forms.NumericUpDown SouthID;
        private System.Windows.Forms.CheckBox SouthEnabled;
        private System.Windows.Forms.Button EastSet;
        private System.Windows.Forms.NumericUpDown EastID;
        private System.Windows.Forms.CheckBox EastEnabled;
        private System.Windows.Forms.Button BottomSet;
        private System.Windows.Forms.NumericUpDown BottomID;
        private System.Windows.Forms.CheckBox BottomEnabled;
        private System.Windows.Forms.Button WestSet;
        private System.Windows.Forms.NumericUpDown WestID;
        private System.Windows.Forms.CheckBox WestEnabled;
        private System.Windows.Forms.CheckBox Trans;
        private System.Windows.Forms.Button Rename;
    }
}
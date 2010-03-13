﻿namespace GUIElementConstructor
{
    partial class Editor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Editor));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.GLView = new OpenTK.GLControl();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.NewElement = new System.Windows.Forms.Button();
            this.ElementTree = new System.Windows.Forms.TreeView();
            this.TreeImages = new System.Windows.Forms.ImageList(this.components);
            this.ObjectDataPannel = new System.Windows.Forms.GroupBox();
            this.YPos = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.XPos = new System.Windows.Forms.NumericUpDown();
            this.XPosLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.ComponentList = new System.Windows.Forms.ListBox();
            this.UpdateTimer = new System.Windows.Forms.Timer(this.components);
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.YSize = new System.Windows.Forms.NumericUpDown();
            this.XSize = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.Options = new System.Windows.Forms.ListView();
            this.OptionName = new System.Windows.Forms.ColumnHeader();
            this.OptionValue = new System.Windows.Forms.ColumnHeader();
            this.label5 = new System.Windows.Forms.Label();
            this.BGColorPanel = new System.Windows.Forms.Panel();
            this.FGColorPanel = new System.Windows.Forms.Panel();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.ValueName = new System.Windows.Forms.TextBox();
            this.Add = new System.Windows.Forms.Button();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.ObjectDataPannel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.YPos)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.XPos)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.YSize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.XSize)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.GLView);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(1049, 499);
            this.splitContainer1.SplitterDistance = 661;
            this.splitContainer1.TabIndex = 0;
            // 
            // GLView
            // 
            this.GLView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.GLView.BackColor = System.Drawing.Color.Black;
            this.GLView.Location = new System.Drawing.Point(3, 3);
            this.GLView.Name = "GLView";
            this.GLView.Size = new System.Drawing.Size(655, 493);
            this.GLView.TabIndex = 0;
            this.GLView.VSync = false;
            this.GLView.Load += new System.EventHandler(this.GLView_Load);
            this.GLView.Paint += new System.Windows.Forms.PaintEventHandler(this.GLView_Paint);
            this.GLView.Resize += new System.EventHandler(this.GLView_Resize);
            // 
            // splitContainer2
            // 
            this.splitContainer2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer2.Location = new System.Drawing.Point(3, 3);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.NewElement);
            this.splitContainer2.Panel1.Controls.Add(this.ElementTree);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.Add);
            this.splitContainer2.Panel2.Controls.Add(this.ObjectDataPannel);
            this.splitContainer2.Panel2.Controls.Add(this.label1);
            this.splitContainer2.Panel2.Controls.Add(this.ComponentList);
            this.splitContainer2.Size = new System.Drawing.Size(381, 496);
            this.splitContainer2.SplitterDistance = 162;
            this.splitContainer2.TabIndex = 4;
            // 
            // NewElement
            // 
            this.NewElement.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.NewElement.Location = new System.Drawing.Point(294, 136);
            this.NewElement.Name = "NewElement";
            this.NewElement.Size = new System.Drawing.Size(75, 23);
            this.NewElement.TabIndex = 3;
            this.NewElement.Text = "New";
            this.NewElement.UseVisualStyleBackColor = true;
            this.NewElement.Click += new System.EventHandler(this.NewElement_Click);
            // 
            // ElementTree
            // 
            this.ElementTree.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.ElementTree.FullRowSelect = true;
            this.ElementTree.HideSelection = false;
            this.ElementTree.ImageIndex = 1;
            this.ElementTree.ImageList = this.TreeImages;
            this.ElementTree.Indent = 15;
            this.ElementTree.LabelEdit = true;
            this.ElementTree.Location = new System.Drawing.Point(3, 5);
            this.ElementTree.Name = "ElementTree";
            this.ElementTree.SelectedImageIndex = 0;
            this.ElementTree.Size = new System.Drawing.Size(375, 125);
            this.ElementTree.TabIndex = 2;
            this.ElementTree.AfterLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.ElementTree_AfterLabelEdit);
            this.ElementTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.ElementTree_AfterSelect);
            // 
            // TreeImages
            // 
            this.TreeImages.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("TreeImages.ImageStream")));
            this.TreeImages.TransparentColor = System.Drawing.Color.Transparent;
            this.TreeImages.Images.SetKeyName(0, "list.png");
            this.TreeImages.Images.SetKeyName(1, "field.png");
            // 
            // ObjectDataPannel
            // 
            this.ObjectDataPannel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.ObjectDataPannel.Controls.Add(this.ValueName);
            this.ObjectDataPannel.Controls.Add(this.label7);
            this.ObjectDataPannel.Controls.Add(this.FGColorPanel);
            this.ObjectDataPannel.Controls.Add(this.label6);
            this.ObjectDataPannel.Controls.Add(this.BGColorPanel);
            this.ObjectDataPannel.Controls.Add(this.label5);
            this.ObjectDataPannel.Controls.Add(this.Options);
            this.ObjectDataPannel.Controls.Add(this.groupBox2);
            this.ObjectDataPannel.Controls.Add(this.groupBox1);
            this.ObjectDataPannel.Location = new System.Drawing.Point(3, 3);
            this.ObjectDataPannel.Name = "ObjectDataPannel";
            this.ObjectDataPannel.Size = new System.Drawing.Size(218, 324);
            this.ObjectDataPannel.TabIndex = 2;
            this.ObjectDataPannel.TabStop = false;
            this.ObjectDataPannel.Text = "Info";
            // 
            // YPos
            // 
            this.YPos.Location = new System.Drawing.Point(108, 14);
            this.YPos.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.YPos.Minimum = new decimal(new int[] {
            2000,
            0,
            0,
            -2147483648});
            this.YPos.Name = "YPos";
            this.YPos.Size = new System.Drawing.Size(56, 20);
            this.YPos.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(88, 17);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(14, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Y";
            // 
            // XPos
            // 
            this.XPos.Location = new System.Drawing.Point(26, 14);
            this.XPos.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.XPos.Minimum = new decimal(new int[] {
            2000,
            0,
            0,
            -2147483648});
            this.XPos.Name = "XPos";
            this.XPos.Size = new System.Drawing.Size(56, 20);
            this.XPos.TabIndex = 1;
            // 
            // XPosLabel
            // 
            this.XPosLabel.AutoSize = true;
            this.XPosLabel.Location = new System.Drawing.Point(6, 16);
            this.XPosLabel.Name = "XPosLabel";
            this.XPosLabel.Size = new System.Drawing.Size(14, 13);
            this.XPosLabel.TabIndex = 0;
            this.XPosLabel.Text = "X";
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(227, 96);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(66, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Components";
            // 
            // ComponentList
            // 
            this.ComponentList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.ComponentList.FormattingEnabled = true;
            this.ComponentList.Location = new System.Drawing.Point(227, 116);
            this.ComponentList.Name = "ComponentList";
            this.ComponentList.Size = new System.Drawing.Size(151, 212);
            this.ComponentList.TabIndex = 0;
            // 
            // UpdateTimer
            // 
            this.UpdateTimer.Interval = 10;
            this.UpdateTimer.Tick += new System.EventHandler(this.UpdateTimer_Tick);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.XPosLabel);
            this.groupBox1.Controls.Add(this.YPos);
            this.groupBox1.Controls.Add(this.XPos);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(6, 19);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(174, 42);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Position";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.YSize);
            this.groupBox2.Controls.Add(this.XSize);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Location = new System.Drawing.Point(6, 64);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(174, 42);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Size";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 16);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(14, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "X";
            // 
            // YSize
            // 
            this.YSize.Location = new System.Drawing.Point(108, 15);
            this.YSize.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.YSize.Minimum = new decimal(new int[] {
            2000,
            0,
            0,
            -2147483648});
            this.YSize.Name = "YSize";
            this.YSize.Size = new System.Drawing.Size(56, 20);
            this.YSize.TabIndex = 3;
            // 
            // XSize
            // 
            this.XSize.Location = new System.Drawing.Point(26, 14);
            this.XSize.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.XSize.Minimum = new decimal(new int[] {
            2000,
            0,
            0,
            -2147483648});
            this.XSize.Name = "XSize";
            this.XSize.Size = new System.Drawing.Size(56, 20);
            this.XSize.TabIndex = 1;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(88, 17);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(14, 13);
            this.label4.TabIndex = 2;
            this.label4.Text = "Y";
            // 
            // Options
            // 
            this.Options.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.Options.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.OptionName,
            this.OptionValue});
            this.Options.GridLines = true;
            this.Options.Location = new System.Drawing.Point(6, 211);
            this.Options.Name = "Options";
            this.Options.Size = new System.Drawing.Size(206, 104);
            this.Options.TabIndex = 6;
            this.Options.UseCompatibleStateImageBehavior = false;
            this.Options.View = System.Windows.Forms.View.Details;
            this.Options.SelectedIndexChanged += new System.EventHandler(this.Options_SelectedIndexChanged);
            this.Options.DoubleClick += new System.EventHandler(this.Options_DoubleClick);
            // 
            // OptionName
            // 
            this.OptionName.Text = "Name";
            this.OptionName.Width = 86;
            // 
            // OptionValue
            // 
            this.OptionValue.Text = "Option";
            this.OptionValue.Width = 116;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 123);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(92, 13);
            this.label5.TabIndex = 7;
            this.label5.Text = "Background Color";
            // 
            // BGColorPanel
            // 
            this.BGColorPanel.Location = new System.Drawing.Point(99, 119);
            this.BGColorPanel.Name = "BGColorPanel";
            this.BGColorPanel.Size = new System.Drawing.Size(28, 24);
            this.BGColorPanel.TabIndex = 8;
            this.BGColorPanel.Click += new System.EventHandler(this.BGColorPanel_Click);
            // 
            // FGColorPanel
            // 
            this.FGColorPanel.Location = new System.Drawing.Point(99, 149);
            this.FGColorPanel.Name = "FGColorPanel";
            this.FGColorPanel.Size = new System.Drawing.Size(28, 24);
            this.FGColorPanel.TabIndex = 10;
            this.FGColorPanel.Click += new System.EventHandler(this.FGColorPanel_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 153);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(88, 13);
            this.label6.TabIndex = 9;
            this.label6.Text = "Foreground Color";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 188);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(34, 13);
            this.label7.TabIndex = 11;
            this.label7.Text = "Value";
            // 
            // ValueName
            // 
            this.ValueName.Location = new System.Drawing.Point(46, 185);
            this.ValueName.Name = "ValueName";
            this.ValueName.Size = new System.Drawing.Size(166, 20);
            this.ValueName.TabIndex = 12;
            // 
            // Add
            // 
            this.Add.Location = new System.Drawing.Point(230, 12);
            this.Add.Name = "Add";
            this.Add.Size = new System.Drawing.Size(75, 23);
            this.Add.TabIndex = 3;
            this.Add.Text = "Add";
            this.Add.UseVisualStyleBackColor = true;
            this.Add.Click += new System.EventHandler(this.Add_Click);
            // 
            // Editor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1049, 499);
            this.Controls.Add(this.splitContainer1);
            this.Name = "Editor";
            this.Text = "Form1";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.Panel2.PerformLayout();
            this.splitContainer2.ResumeLayout(false);
            this.ObjectDataPannel.ResumeLayout(false);
            this.ObjectDataPannel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.YPos)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.XPos)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.YSize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.XSize)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private OpenTK.GLControl GLView;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox ComponentList;
        private System.Windows.Forms.Timer UpdateTimer;
        private System.Windows.Forms.TreeView ElementTree;
        private System.Windows.Forms.Button NewElement;
        private System.Windows.Forms.ImageList TreeImages;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.GroupBox ObjectDataPannel;
        private System.Windows.Forms.NumericUpDown YPos;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown XPos;
        private System.Windows.Forms.Label XPosLabel;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown YSize;
        private System.Windows.Forms.NumericUpDown XSize;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ListView Options;
        private System.Windows.Forms.ColumnHeader OptionName;
        private System.Windows.Forms.ColumnHeader OptionValue;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Panel BGColorPanel;
        private System.Windows.Forms.Panel FGColorPanel;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox ValueName;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button Add;
    }
}

namespace GUIElementConstructor
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.GLView = new OpenTK.GLControl();
            this.ComponentList = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.UpdateTimer = new System.Windows.Forms.Timer(this.components);
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
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
            this.splitContainer1.Panel2.Controls.Add(this.treeView1);
            this.splitContainer1.Panel2.Controls.Add(this.label1);
            this.splitContainer1.Panel2.Controls.Add(this.ComponentList);
            this.splitContainer1.Size = new System.Drawing.Size(1032, 484);
            this.splitContainer1.SplitterDistance = 651;
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
            this.GLView.Size = new System.Drawing.Size(645, 478);
            this.GLView.TabIndex = 0;
            this.GLView.VSync = false;
            this.GLView.Load += new System.EventHandler(this.GLView_Load);
            this.GLView.Paint += new System.Windows.Forms.PaintEventHandler(this.GLView_Paint);
            this.GLView.Resize += new System.EventHandler(this.GLView_Resize);
            // 
            // ComponentList
            // 
            this.ComponentList.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ComponentList.FormattingEnabled = true;
            this.ComponentList.Location = new System.Drawing.Point(214, 24);
            this.ComponentList.Name = "ComponentList";
            this.ComponentList.Size = new System.Drawing.Size(151, 121);
            this.ComponentList.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(214, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(66, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Components";
            // 
            // UpdateTimer
            // 
            this.UpdateTimer.Interval = 10;
            this.UpdateTimer.Tick += new System.EventHandler(this.UpdateTimer_Tick);
            // 
            // treeView1
            // 
            this.treeView1.Location = new System.Drawing.Point(3, 150);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(151, 322);
            this.treeView1.TabIndex = 2;
            // 
            // Editor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1032, 484);
            this.Controls.Add(this.splitContainer1);
            this.Name = "Editor";
            this.Text = "Form1";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private OpenTK.GLControl GLView;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox ComponentList;
        private System.Windows.Forms.Timer UpdateTimer;
        private System.Windows.Forms.TreeView treeView1;
    }
}


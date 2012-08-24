namespace PlanEdit
{
	partial class _3dView
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
			this.SuspendLayout();
			// 
			// glControl1
			// 
			this.glControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.glControl1.BackColor = System.Drawing.Color.Black;
			this.glControl1.Location = new System.Drawing.Point(0, 1);
			this.glControl1.Name = "glControl1";
			this.glControl1.Size = new System.Drawing.Size(602, 450);
			this.glControl1.TabIndex = 0;
			this.glControl1.VSync = false;
			this.glControl1.Load += new System.EventHandler(this.glControl1_Load);
			this.glControl1.Paint += new System.Windows.Forms.PaintEventHandler(this.glControl1_Paint);
			this.glControl1.Resize += new System.EventHandler(this.glControl1_Resize);
			// 
			// panel1
			// 
			this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.panel1.Location = new System.Drawing.Point(608, 1);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(121, 450);
			this.panel1.TabIndex = 1;
			// 
			// _3dView
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(732, 463);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.glControl1);
			this.Name = "_3dView";
			this.Text = "_3dView";
			this.ResumeLayout(false);

		}

		#endregion

		private OpenTK.GLControl glControl1;
		private System.Windows.Forms.Panel panel1;
	}
}
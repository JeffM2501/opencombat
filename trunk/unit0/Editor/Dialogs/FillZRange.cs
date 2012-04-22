using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using GridWorld;

namespace Editor.Dialogs
{
    public partial class FillZRange : Form
    {
        public List<string> BlockNames = new List<string>();
        public int SelectedBlock = -1;

        public int LowerZ = -1;
        public int UpperZ = -1;

        public int LowerX = -1;
        public int UpperX = -1;

        public int LowerY = -1;
        public int UpperY = -1;

        public bool FillEntireMap = false;

        public Cluster.Block.Geometry SelectedGeometry = Cluster.Block.Geometry.Solid;

        public bool Loading = false;

        public FillZRange()
        {
            InitializeComponent();

            foreach (Cluster.Block.Geometry geos in Enum.GetValues(typeof(Cluster.Block.Geometry)))
                GeometryTypes.Items.Add(geos);

            GeometryTypes.SelectedIndex = 1;

            Loading = true;

            MinX.Increment = Cluster.XYSize;
            MaxX.Increment = Cluster.XYSize;
            MinY.Increment = Cluster.XYSize;
            MaxY.Increment = Cluster.XYSize;

            MinX.Minimum = Cluster.XYSize * -100;
            MinY.Minimum = Cluster.XYSize * -100;

            MinX.Maximum = Cluster.XYSize * 99;
            MinY.Maximum = Cluster.XYSize * 99;

            MinX.Minimum = Cluster.XYSize * -99;
            MaxX.Minimum = Cluster.XYSize * -99;

            MinX.Maximum = Cluster.XYSize * 100;
            MaxX.Maximum = Cluster.XYSize * 100;

            Loading = false;
        }

        private void FillZRange_Load(object sender, EventArgs e)
        {
            MinZ.Minimum = 0;
            MinZ.Maximum = Cluster.ZSize - 2;

            MaxZ.Minimum = 0;
            MaxZ.Maximum = Cluster.ZSize - 1;

            MaxZ.Value = 1;
            MinZ.Value = 0;

            foreach (string s in BlockNames)
                BlockList.Items.Add(s);

            BlockList.SelectedIndex = 0;
        }

        private void ValueChanged(object sender, EventArgs e)
        {
            if (Loading)
                return;

            if (MaxZ.Value <= MinZ.Value)
                MaxZ.Value = MinZ.Value + 1;

            if (MaxX.Value <= MinX.Value)
                MaxX.Value = MinX.Value + 1;

            if (MaxY.Value <= MinY.Value)
                MaxY.Value = MinY.Value + 1;
        }

        private void OK_Click(object sender, EventArgs e)
        {
            SelectedBlock = BlockList.SelectedIndex;

            LowerZ = (int)MinZ.Value;
            UpperZ = (int)MaxZ.Value;

            LowerX = (int)MinX.Value;
            UpperX = (int)MaxX.Value;

            LowerY = (int)MinY.Value;
            UpperY = (int)MaxY.Value;

            FillEntireMap = FillMap.Checked;

            SelectedGeometry = (Cluster.Block.Geometry)GeometryTypes.SelectedItem;
        }

        private void FillMap_CheckedChanged(object sender, EventArgs e)
        {
            panel1.Enabled = !FillMap.Checked;
        }
    }
}

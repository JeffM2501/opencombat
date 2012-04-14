using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using OpenTK;
using GridWorld;
using FileLocations;

namespace Editor
{
    public class EditorConfig
    {
        public string ResourcesPath = string.Empty;
    }

    public class Edit
    {
        public event EventHandler<EventArgs> SelectionChanged;
        public event EventHandler<EventArgs> RegenerateAllGeometry;
        public event EventHandler<EventArgs> BlockDefsChanged;
        public event EventHandler<EventArgs> SelectedBlockChanged;
      
        public event EventHandler<EventArgs> WorldInfoChanged;

        public delegate void WorldObjectChangeCB(GridWorld.World world);
        public event WorldObjectChangeCB WorldObjectChanged;

        EditorConfig Config = new EditorConfig();

        public World TheWorld = new World();

        public DirectoryInfo DataPath = null;

        public int SelectedBlockDef { get; protected set; }

        public void SelectBlockDef(int ID)
        {
            if (ID > TheWorld.BlockDefs.Count)
                ID = -1;

            SelectedBlockDef = ID;
            if (SelectedBlockChanged != null)
                SelectedBlockChanged(this, EventArgs.Empty);
        }

        public void DefsUpdated()
        {
            if (BlockDefsChanged != null)
                BlockDefsChanged(this, EventArgs.Empty);

            RebuildStaticGeometry();
        }

        public Edit(EditorConfig config)
        {
            Config = config;

            DataPath = new DirectoryInfo(Config.ResourcesPath);
            Locations.DataPathOveride = DataPath.FullName;

            TheWorld = WorldBuilder.NewWorld(string.Empty, null);
            TheWorld.Finailize();

            SelectedBlockDef = -1;
        }

        public void OpenWorldFile(string filename)
        {
            TheWorld = World.ReadWorldWithGeometry(new FileInfo(filename));
            TheWorld.Finailize();

            SelectedItems.Clear();
            SelectedBlockDef = -1;
            if (WorldObjectChanged != null)
                WorldObjectChanged(TheWorld);
        }

        public void SaveWorldFile(string filename)
        {
            TheWorld.SaveWorldWithGeometry(new FileInfo(filename));
        }

        public List<Viewer.Selection> SelectedItems = new List<Viewer.Selection>();

        public List<Viewer.Selection> GetSelections()
        {
            return SelectedItems;
        }

        public void ProjectionClick(Viewer.Selection selection, bool additve)
        {
            if (!additve)
                SelectedItems.Clear();

            if (selection.Valid)
                SelectedItems.Add(selection);

            if (SelectionChanged != null)
                SelectionChanged(this, EventArgs.Empty);
        }

        public void ClearMap()
        {

            TheWorld.Clusters.Clear();

            if (RegenerateAllGeometry != null)
                RegenerateAllGeometry(this, EventArgs.Empty);
            
        }

        public void RebuildStaticGeometry()
        {
            TheWorld.SetupTextureInfos(); 

            ClusterGeometry.BuildGeometry(TheWorld);
            if (RegenerateAllGeometry != null)
                RegenerateAllGeometry(this, EventArgs.Empty);
        }

        public void FillZRange(int blockID, Cluster.Block.Geometry geo, Vector3 min, Vector3 max, bool allBlocks)
        {
            if (allBlocks)
            {
                foreach (Cluster cluster in TheWorld.Clusters.Values)
                {
                    cluster.ClearAllBlocks();
                    for (int i = (int)min.Z; i < (int)max.Z; i++)
                        WorldBuilder.FlatBuilder.FillClusterZWithBlock(cluster, i, blockID, geo);
                }
            }
            else
            {
                for (int x = (int)min.X; x < (int)max.X; x += Cluster.XYSize)
                {
                    for (int y = (int)min.Y; y < (int)max.Y; y += Cluster.XYSize)
                    {
                        Cluster.ClusterPos pos = new Cluster.ClusterPos(x, y);
                        Cluster cluster = null;
                        if (TheWorld.Clusters.ContainsKey(pos))
                            cluster = TheWorld.Clusters[pos];
                        else
                        {
                            cluster = new Cluster();
                            cluster.Origin = pos;
                            TheWorld.Clusters.Add(pos, cluster);
                        }

                        for (int i = (int)min.Z; i < (int)max.Z; i++)
                            WorldBuilder.FlatBuilder.FillClusterZWithBlock(cluster, i, blockID, geo);
                    }
                }
            }

            // it's such a big change just rebuild it all
            RebuildStaticGeometry();
        }

        public void WorldInfoUpdated()
        {
            TheWorld.SetupTextureInfos(); 

            if (WorldInfoChanged != null)
                WorldInfoChanged(this, EventArgs.Empty);
            
            if (RegenerateAllGeometry != null)
                RegenerateAllGeometry(this, EventArgs.Empty);
        }

        public class BlockEdit
        {
            public Vector3 BlockLocation = Vector3.Zero;
            public int NewDefID = World.BlockDef.EmptyID;
            public Cluster.Block.Geometry NewGeometry = GridWorld.Cluster.Block.Geometry.Empty;
        }

        public void EditBlocks(List<BlockEdit> edits)
        {
            List<Cluster> ClustersThatNeedReuild = new List<Cluster>();

            foreach (BlockEdit edit in edits)
            {
                // find the cluster
                Cluster cluster = TheWorld.ClusterFromPosition(edit.BlockLocation);
                if (cluster != null && edit.NewDefID >= 0 && edit.NewDefID < TheWorld.BlockDefs.Count)
                {
                    Cluster.Block block = TheWorld.BlockFromPosition(edit.BlockLocation);
                    block.DefID = edit.NewDefID;
                    block.Geom = edit.NewGeometry;

                    if (!ClustersThatNeedReuild.Contains(cluster))
                        ClustersThatNeedReuild.Add(cluster);

                    Cluster.ClusterPos relPos = cluster.GetPositionRelative(edit.BlockLocation);

                    // is this block near an edge
                    if (relPos.X == 0)
                    {
                        Cluster c = TheWorld.ClusterFromPosition(cluster.Origin.X-Cluster.XYSize,cluster.Origin.Y,0);
                        if (c != null && !ClustersThatNeedReuild.Contains(c))
                            ClustersThatNeedReuild.Add(c);
                    }

                    if (relPos.Y == 0)
                    {
                        Cluster c = TheWorld.ClusterFromPosition(cluster.Origin.X,cluster.Origin.Y-Cluster.XYSize,0);
                        if (c != null && !ClustersThatNeedReuild.Contains(c))
                            ClustersThatNeedReuild.Add(c);
                    }

                    if (relPos.X == Cluster.XYSize-1)
                    {
                        Cluster c = TheWorld.ClusterFromPosition(cluster.Origin.X+Cluster.XYSize,cluster.Origin.Y,0);
                        if (c != null && !ClustersThatNeedReuild.Contains(c))
                            ClustersThatNeedReuild.Add(c);
                    }

                     if (relPos.Y == Cluster.XYSize-1)
                    {
                        Cluster c = TheWorld.ClusterFromPosition(cluster.Origin.X,cluster.Origin.Y+Cluster.XYSize,0);
                        if (c != null && !ClustersThatNeedReuild.Contains(c))
                            ClustersThatNeedReuild.Add(c);
                    }
                }

                foreach (Cluster dirtyCluster in ClustersThatNeedReuild)
                {
                    ClusterGeometry.BuildGeometry(TheWorld, dirtyCluster);
                }

                if (RegenerateAllGeometry != null)
                    RegenerateAllGeometry(this, EventArgs.Empty);
            }
        }
    }
}

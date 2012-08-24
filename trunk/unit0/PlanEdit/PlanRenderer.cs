using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

using OpenTK;

using GridWorld;

namespace PlanEdit
{
	public class PlanRenderer : IDisposable
	{
		public World TheWorld = null;
		PictureBox PictureFrame = null;
		string DataDir = string.Empty;

        public int PixelScale = 32;
        public Point DrawOffset = Point.Empty;
        public int ZLevel = 31;

        ImageCache Cache = null;

		public PlanRenderer(PictureBox frame, World world, Prefs prefs)
		{
            Cache = new ImageCache(prefs.DataPath);

            TheWorld = world;
            PictureFrame = frame;
            DataDir = prefs.DataPath;

            DrawOffset = new Point(0, PictureFrame.Height);
            PictureFrame.Paint += new PaintEventHandler(PictureFrame_Paint);
            PictureFrame.Resize += new EventHandler(PictureFrame_Resize);
		}

        public void Dispose()
        {
            Cache.Dispose();
        }

        void PictureFrame_Resize(object sender, EventArgs e)
        {
            PictureFrame.Update();
        }

        Image GetTopImageForBlock(Cluster.Block block)
        {
            int texture = TheWorld.BlockTextureToTextureID(TheWorld.BlockDefs[block.DefID].Top);
            return Cache.GetImage(TheWorld.Info.Textures[texture].FileName);
        }

        void PictureFrame_Paint(object sender, PaintEventArgs e)
        {
            try
            {
                foreach (Cluster cluster in TheWorld.Clusters.Values)
                {
                    // make sure the cluster is visible
                    Point scaledOrigin = new Point((cluster.Origin.X * PixelScale) - DrawOffset.X, (cluster.Origin.Y * PixelScale) - DrawOffset.Y);

                    Rectangle clusterBounds = new Rectangle(scaledOrigin, new Size(Cluster.XYSize * PixelScale, Cluster.XYSize * PixelScale));
                    Rectangle frameBounds = PictureFrame.Bounds;
                    if (!frameBounds.Contains(clusterBounds) && !frameBounds.IntersectsWith(clusterBounds))
                        continue;

                    for (int x = cluster.Origin.X; x < cluster.Origin.X + Cluster.XYSize; x++)
                    {
                        for (int y = cluster.Origin.Y; y < cluster.Origin.Y + Cluster.XYSize; y++)
                        {
                            int foundZ = 0;
                            // find the block under the Z level
                            Cluster.Block block = Cluster.Block.Empty;
                            for (int z = ZLevel; z >= 0; z--)
                            {
                                Cluster.Block b = cluster.GetBlockAbs(x, y, z);
                                if (b.DefID >= 0 && b != Cluster.Block.Empty && b != Cluster.Block.Invalid && !TheWorld.BlockDefs[b.DefID].Transperant)
                                {
                                    block = b;
                                    foundZ = z;
                                    break;
                                }
                            }

                            if (block != Cluster.Block.Empty)
                            {
                                Image img = GetTopImageForBlock(block);
                                int offset = TheWorld.BlockTextureToTextureOffset(TheWorld.BlockDefs[block.DefID].Top);
                                int texture = TheWorld.BlockTextureToTextureID(TheWorld.BlockDefs[block.DefID].Top);
                                Vector2[] offsets = ClusterGeometry.GeometryBuilder.GetUVsForOffset(offset,texture,TheWorld);

                                Point start = new Point((int)(offsets[0].X * img.Width),(int)(offsets[0].Y * img.Height));
                                Size bounds = new Size((int)(offsets[2].X * img.Width)-start.X,(int)(offsets[2].Y * img.Height)-start.Y);

                                Rectangle sourceRect = new Rectangle(start,bounds);
                                Rectangle destRect = new Rectangle(DrawOffset.X + x*PixelScale,DrawOffset.Y + (-1 * y * PixelScale),PixelScale,PixelScale);
                                e.Graphics.DrawImage(img,destRect,sourceRect,GraphicsUnit.Pixel);

                                // shade it based on Z and slope

                                int minFade = 64;
                                float param = foundZ / (float)(ZLevel);
                                param = 1.0f - param;
                                param *= minFade;

                                int alpha = (int)param;
    
                                Brush brush = new SolidBrush(Color.FromArgb(alpha, Color.White));
                                e.Graphics.FillRectangle(brush, destRect);
                                brush.Dispose();
                            }
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {

            }
        }
	}
}

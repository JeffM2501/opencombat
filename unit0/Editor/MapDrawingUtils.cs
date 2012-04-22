using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

using GridWorld;
using FileLocations;

using OpenTK;

namespace Editor
{
    public class MapDrawingUtils
    {
        public static Size BitmapSize = new Size(100, 100);

        public static Image BuildBlockDefImage(int BlockDefID, World world)
        {
            Bitmap bitmap = new Bitmap(BitmapSize.Width, BitmapSize.Height, PixelFormat.Format24bppRgb);
            Graphics graphics = Graphics.FromImage(bitmap);

            Brush brush = Brushes.White;
            graphics.FillRectangle(Brushes.White,new Rectangle(0,0,bitmap.Width,bitmap.Height));

            World.BlockDef def = world.BlockDefs[BlockDefID];

            if (def.Top >= 0)
            {
                Bitmap image = new Bitmap(Locations.FindDataFile(world.Info.Textures[world.BlockTextureToTextureID(def.Top)].FileName));
                Vector2[] offsets = ClusterGeometry.GeometryBuilder.GetUVsForOffset(world.BlockTextureToTextureOffset(def.Top), world.BlockTextureToTextureID(def.Top), world);

                Point min = new Point((int)(offsets[0].X * image.Size.Width),(int)(offsets[0].Y * image.Size.Height));
                Point max = new Point((int)(offsets[2].X * image.Size.Width), (int)(offsets[2].Y * image.Size.Height));

                graphics.DrawImage(image, new Rectangle(0, 0, bitmap.Width, bitmap.Height), new Rectangle(min.X, min.Y, max.X - min.X, max.Y - min.Y), GraphicsUnit.Pixel);
            }

            graphics.Dispose();

            return bitmap;
        }
    }
}

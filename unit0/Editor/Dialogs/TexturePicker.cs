using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using FileLocations;
using GridWorld;

namespace Editor.Dialogs
{
    public partial class TexturePicker : Form
    {
       // World TheWorld = null;
        public int TextureID = -1;
        public int TextureOffset = -1;

        protected int localStart;

        protected int countX = 0;
        protected int countY = 0;

        protected Dictionary<string, Bitmap> BitmapCache = new Dictionary<string, Bitmap>();

        public TexturePicker( World world )
        {
            InitializeComponent();

            //TheWorld = world;

            foreach (World.TextureInfo info in world.Info.Textures)
            {
                TextureSet.Items.Add(info);
                BitmapCache.Add(info.FileName, new Bitmap(Locations.FindDataFile(info.FileName)));
            }
        }

        private void TexturePicker_Load(object sender, EventArgs e)
        {
            TextureSet.SelectedIndex = TextureID;
            TextureIndex.Text = TextureOffset.ToString();
        }

        private void ImageBox_Click(object sender, EventArgs e)
        {
          
        }

        private void ImageBox_MouseClick(object sender, MouseEventArgs e)
        {
            ImageScaleInfo info = ImageScaleInfo.Get(ImageBox);

            Point relative = new Point(e.X - info.originX, e.Y - info.originY);

            if (relative.X < 0 || relative.Y < 0 || relative.X > info.actualX || relative.Y > info.actualY)
                return;

            relative.X = (int)(relative.X / info.scale);
            relative.Y = (int)(relative.Y / info.scale);

            int gridY = 0;
            if (relative.Y != 0)
                gridY = relative.Y / ( ImageBox.Image.Size.Width / countX );

            int gridX = 0;
            if (relative.X != 0)
                gridX = relative.X / (ImageBox.Image.Size.Height / countY );

            int offset = gridY * countY + gridX;
            TextureOffset = localStart + offset;
            TextureIndex.Text = TextureOffset.ToString();

            ImageBox.Invalidate();
        }

        protected class ImageScaleInfo
        {
            public float scale = 1;

            public int originX = 0;
            public int originY = 0;

            public int actualX = 0;
            public int actualY = 0;

            public static ImageScaleInfo Get(PictureBox ImageBox)
            {
                ImageScaleInfo info = new ImageScaleInfo();
                // find out the origin of each axis

                // try X and see
                float thisScale = (float)ImageBox.Size.Width / (float)ImageBox.Image.Size.Width;
                int thisY = (int)(ImageBox.Image.Size.Height * thisScale);

                if (thisY <= ImageBox.Size.Height)
                    info.scale = thisScale;
                else
                    info.scale = (float)ImageBox.Size.Height / (float)ImageBox.Image.Size.Height;

                info.actualX = (int)(info.scale * ImageBox.Image.Width);

                if (info.actualX < ImageBox.Size.Width)
                {
                    int delta = ImageBox.Size.Width - info.actualX;
                    info.originX = delta / 2;
                }

                info.actualY = (int)(info.scale * ImageBox.Image.Height);

                if (info.actualY < ImageBox.Size.Height)
                {
                    int delta = ImageBox.Size.Height - info.actualY;
                    info.originY = delta / 2;
                }

                return info;
            }
        }

        private void ImageBox_Paint(object sender, PaintEventArgs e)
        {
            Pen outlinePen = new Pen(Color.Red, 3);
            Pen innerPen = new Pen(Color.White, 1);
            Pen gridPen = new Pen(Color.DarkGray, 2);

            ImageScaleInfo info = ImageScaleInfo.Get(ImageBox);

            int gridOffsetX = (int)((ImageBox.Image.Width / countX) * info.scale);
            int gridOffsetY = (int)((ImageBox.Image.Height / countY) * info.scale);

            int localOffset = TextureOffset - localStart;
            int y = localOffset / countY;
            int x = localOffset - (y * countY);

            // draw the grid
            // outer rectangle
            e.Graphics.DrawRectangle(gridPen, new Rectangle(info.originX, info.originY, info.actualX, info.actualY));

            for (int i = 1; i < countX; i++)
                e.Graphics.DrawLine(gridPen, info.originX + (gridOffsetX * i), info.originY, info.originX + (gridOffsetX * i), info.originY + info.actualY);
         
            for (int i = 1; i < countY; i++)
                e.Graphics.DrawLine(gridPen, info.originX, info.originY + (gridOffsetY * i), info.originX + info.actualX, info.originY + (gridOffsetY * i));

                // draw the selection
            e.Graphics.DrawRectangle(outlinePen, new Rectangle(info.originX + (x * gridOffsetX), info.originY + (y * gridOffsetY), gridOffsetX, gridOffsetY));
            e.Graphics.DrawRectangle(innerPen, new Rectangle(info.originX + (x * gridOffsetX), info.originY + (y * gridOffsetY), gridOffsetX, gridOffsetY));

            outlinePen.Dispose();
            innerPen.Dispose();
            gridPen.Dispose();
        }

        private void TextureSet_SelectedIndexChanged(object sender, EventArgs e)
        {
            World.TextureInfo info = TextureSet.SelectedItem as World.TextureInfo;

            if (info == null)
                return;

            ImageBox.Image = BitmapCache[info.FileName];
            localStart = info.Start;

            countX = info.XCount;
            countY = info.YCount;
        }
    }
}

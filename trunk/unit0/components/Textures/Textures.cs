using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Textures
{
    public class Texture
    {
        public static bool CacheFilesToImage = false;
        public static bool UseAnisometricFiltering = false;

        public static void ContextCreated (int contextID)
        {
            AddContext(contextID);
        }

        public static void ContextDestroyed (int contextID)
        {
            RemoveContext(contextID);
        }

        public static void ContextChanged (int contextID)
        {
            SetContext(contextID);
        }

        public static void ContextUnload (int contextID)
        {
            FlushGL();
        }

        public static void ContextReload(int contextID)
        {

        }

        public static void Unbind()
        {
            LastBoundIDs[currentContextID] = UnboundID;
        }

        public static void PreloadAll()
        {
            foreach (KeyValuePair<string,Texture> texture in TextureCache())
                texture.Value.Load();
        }

        public static Texture Get ( string imageFile )
        {
            return Get(imageFile, true, false);
        }

        public static Texture Get(string imageFile, bool miped, bool clamped)
        {
            string name = (string)imageFile.Clone();
            name += "M" + miped.ToString() + "C" + clamped.ToString();
            lock (TextureCaches)
            {
                if (TextureCache().ContainsKey(name))
                    return TextureCache()[name];
            }

            return new Texture(name, new FileInfo(imageFile), miped, clamped);
        }

        public static Texture Get(Image image)
        {
            return Get(image, image.GetHashCode().ToString());
        }

        public static Texture Get(Image image, string name)
        {
            return Get(image, name, true, false);
        }

        public static Texture Get(Image image, string name, bool miped, bool clamped)
        {
            name += "M" + miped.ToString() + "C" + clamped.ToString();
            lock (TextureCaches)
            {
                if (TextureCache().ContainsKey(name))
                    return TextureCache()[name];
            }
            return new Texture(name, image, miped, clamped);
        }

        public static void FlushGL()
        {
            foreach (KeyValuePair<string, Texture> item in TextureCache())
                item.Value.Invalidate();
        }

        protected static int InvalidID = -1;
        protected static int UnboundID = -2;

        protected static int lastContextID = 0;
        protected static int currentContextID = 0;

        public static int NewContext()
        {
            lastContextID++;
            AddContext(lastContextID);
            return lastContextID;
        }

        protected static void AddContext( int id )
        {
            lock (TextureCaches)
            {
                TextureCaches.Add(lastContextID, new Dictionary<string, Texture>());
                LastBoundIDs.Add(id, UnboundID);
                currentContextID = id;
            }
        }

        public static void SetContext(int context)
        {
            currentContextID = context;
        }

        public static void RemoveContext(int context)
        {
            if (context == 0)
                return;

            currentContextID = context;
            FlushGL();

            currentContextID = 0;

            lock (TextureCaches)
            {
                TextureCaches.Remove(context);
                LastBoundIDs.Remove(context);
            }
        }

        protected static Dictionary<string, Texture> TextureCache()
        {
            lock (TextureCaches)
                return TextureCaches[currentContextID];
        }

        protected static int LastBound()
        {
            lock (TextureCaches)
                return LastBoundIDs[currentContextID];
        }

        protected static Dictionary<int, Dictionary<string, Texture>> InitCache()
        {
            Dictionary<int, Dictionary<string, Texture>> cache = new Dictionary<int, Dictionary<string, Texture>>();
            cache.Add(0, new Dictionary<string, Texture>());

            return cache;
        }

        protected static Dictionary<int, int> InitLastUsed()
        {
            Dictionary<int, int> cache = new Dictionary<int, int>();
            cache.Add(0, UnboundID);
            return cache;
        }

        protected static Dictionary<int, Dictionary<string, Texture>> TextureCaches = InitCache();
        protected static Dictionary<int, int> LastBoundIDs = InitLastUsed();

        protected int BoundID = InvalidID;
        protected bool mipMapped = true;
        protected bool clamped = false;
        protected Size imageSize = Size.Empty;
        protected Vector2 imageBounds = Vector2.Zero;

        protected FileInfo file = null;
        protected Image image = null;

        public String Name = string.Empty;

        public bool Loaded
        {
            get { return BoundID != InvalidID; }
        }

        public bool MipMapped
        {
            get { return mipMapped; }
        }

        public bool Clamped
        {
            get { return clamped; }
        }

        public int Width
        {
            get
            {
                CheckSize();
                return imageSize.Width;
            }
        }

        public int Height
        {
            get
            {
                CheckSize();
                return imageSize.Height;
            }
        }

        public Vector2 Bounds
        {
            get
            {
                CheckSize();
                return imageBounds;
            }
        }

        public Size Size
        {
            get
            {
                CheckSize();
                return imageSize;
            }
        }

        protected Texture(string name, FileInfo info, bool mip, bool clamp)
        {
            Name = name;
            file = info;
            mipMapped = mip;
            clamped = clamp;
            lock (TextureCaches)
            {
                TextureCache().Add(name, this);
            }
        }

        protected Texture(string name, Image img, bool mip, bool clamp)
        {
            Name = name;
            image = img;
            mipMapped = mip;
            clamped = clamp; 
            lock (TextureCaches)
            {
                TextureCache().Add(name, this);
            }
        }

        public void Invalidate()
        {
            if (BoundID != -1)
                GL.DeleteTexture(BoundID);
            BoundID = InvalidID;
        }

        public void Load ()
        {
            GL.Enable(EnableCap.Texture2D);
            Bitmap bitmap;

            if (image != null)
                bitmap = new Bitmap(image);
            else
            {
                if (file == null || !file.Exists)
                    return;

                bitmap = new Bitmap(file.FullName);

                if (CacheFilesToImage)
                    image = bitmap;
            }

            if (BoundID != InvalidID)
                Invalidate();

            GL.GenTextures(1, out BoundID);
            GL.BindTexture(TextureTarget.Texture2D, BoundID);

            BitmapData data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

            if (mipMapped)
            {
                if (UseAnisometricFiltering)
                {
                    //GL.TexParameter(TextureTarget.Texture2D, (TextureParameterName)0x84FF, 2f);

                    float maxAniso;
                    GL.GetFloat((GetPName)ExtTextureFilterAnisotropic.MaxTextureMaxAnisotropyExt, out maxAniso);
                    GL.TexParameter(TextureTarget.Texture2D, (TextureParameterName)ExtTextureFilterAnisotropic.TextureMaxAnisotropyExt, maxAniso);
                }
                GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
            }
            
            bitmap.UnlockBits(data);
            bitmap.Dispose();

            if (!mipMapped)
            {
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            }
            else
            {
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearClipmapLinearSgix);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.LinearDetailSgis);
            }

            if (clamped)
            {
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
            }
            else
            {
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
            }

            GL.BindTexture(TextureTarget.Texture2D, BoundID);
        }

        public void Bind()
        {
            if (LastBound() == BoundID)
                return;

            if (BoundID == InvalidID)
                Load();

            if (BoundID != InvalidID)
            {
                GL.Enable(EnableCap.Texture2D);
                GL.BindTexture(TextureTarget.Texture2D, BoundID);
            }
            else
                GL.Disable(EnableCap.Texture2D);
        }

        protected void CheckSize()
        {
            if (imageSize == Size.Empty)
            {
                if (image != null)
                {
                    imageSize = new Size(image.Width, image.Height);
                    imageBounds = new Vector2(image.Width, image.Height);
                }
                else
                {
                    if (CacheFilesToImage)
                        Load();
                    else
                    {
                        Image pic = Image.FromFile(file.FullName);
                        if (pic != null)
                        {
                            imageSize = new Size(pic.Width, pic.Height);
                            imageBounds = new Vector2(image.Width, image.Height);
                        }
                    }
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using System.Drawing;

namespace PlanEdit
{
    public class ImageCache : IDisposable
    {
        public string RootFolder = string.Empty;

        protected Dictionary<string, Image> Cache = new Dictionary<string, Image>();

        public ImageCache(string dir)
        {
            RootFolder = dir;
        }

        protected string GetRelativePath(string path)
        {
            return path.Substring(RootFolder.Length, path.Length - RootFolder.Length);
        }

        public List<string> Subfolders(string folder)
        {
            List<string> subs = new List<string>();
            DirectoryInfo dir = new DirectoryInfo(Path.Combine(RootFolder, folder));
            foreach (DirectoryInfo subdir in dir.GetDirectories())
                subs.Add(GetRelativePath(subdir.FullName));
            return subs;
        }

        public List<string> Files(string folder)
        {
            List<string> subs = new List<string>();
            DirectoryInfo dir = new DirectoryInfo(Path.Combine(RootFolder, folder));
            foreach (FileInfo subfile in dir.GetFiles())
                subs.Add(GetRelativePath(subfile.FullName));
            return subs;
        }

        public void Dispose()
        {
            foreach (KeyValuePair<string, Image> img in Cache)
                img.Value.Dispose();

            Cache.Clear();
        }

        public Image GetImage(string path)
        {
            if (Cache.ContainsKey(path))
                return Cache[path];

            string fullpath = Path.Combine(RootFolder, path);
            if (File.Exists(fullpath))
            {
                Image img = Bitmap.FromFile(fullpath);
                Cache.Add(path, img);
                return img;
            }
    
            return null;
        }
    }
}

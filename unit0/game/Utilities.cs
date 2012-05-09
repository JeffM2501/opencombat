using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace Game
{
    public class Utilities
    {
        public static string GetMD5Hash( byte[] buffer )
        {
            MD5 md5 = MD5.Create();
            byte[] hash = md5.ComputeHash(buffer);
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
                sBuilder.Append(hash[i].ToString("x2"));

            return sBuilder.ToString();
        }

        public static string GetMD5Hash(string buffer)
        {
            return GetMD5Hash(Encoding.UTF8.GetBytes(buffer));
        }

        public static string GetMD5Hash(FileInfo file)
        {
            FileStream fs = file.OpenRead();
            byte[] buffer = new byte[file.Length];
            fs.Read(buffer, 0, (int)file.Length);
            fs.Close();
            return GetMD5Hash(buffer);
        }

        public static string GetMD5Hash(DirectoryInfo directory)
        {
            byte[] buffer = new byte[0];
            int offset = 0;
            foreach (FileInfo file in directory.GetFiles())
            {
                FileStream fs = file.OpenRead();
                Array.Resize(ref buffer, (int)(buffer.Length + file.Length));
                fs.Read(buffer, offset, (int)file.Length);
                offset = buffer.Length;
                fs.Close();
            }

            return GetMD5Hash(buffer);
        }

        public static string ReadMD5HashFromFile(string hashFile)
        {
            FileInfo file = new FileInfo(hashFile);
            if (!file.Exists)
                return string.Empty;

            TextReader reader = file.OpenText();
            string hash = reader.ReadLine();
            reader.Close();
            return hash;
        }

        public static void WriteMD5HashToFile(string hashFile, string hash)
        {
            FileInfo outfile = new FileInfo(hashFile);
            FileStream fs = outfile.OpenWrite();
            StreamWriter tw = new StreamWriter(fs);
            tw.WriteLine(hash);
            tw.Close();
            fs.Close();
        }

        public static string GetMD5Hash(string file, string hashFile)
        {
            string hash = string.Empty;
            if (hashFile != string.Empty)
                hash = ReadMD5HashFromFile(hashFile);

            if (hash != string.Empty)
                return hash;

            hash = GetMD5Hash(new FileInfo(file));
            if (hashFile != null)
                WriteMD5HashToFile(hashFile, hash);

            return hash;
        }

        public enum PlatformType
        {
            Windows,
            Linux,
            MacOSX
        }

        public static PlatformType GetRealPlatform()
        {
            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.Unix:
                    // Well, there are chances MacOSX is reported as Unix instead of MacOSX.
                    // Instead of platform check, we'll do a feature checks (Mac specific root folders)
                    if (Directory.Exists("/Applications")
                        & Directory.Exists("/System")
                        & Directory.Exists("/Users")
                        & Directory.Exists("/Volumes"))
                        return PlatformType.MacOSX;
                    else
                        return PlatformType.Linux;

                case PlatformID.MacOSX:
                    return PlatformType.MacOSX;

                default:
                    return PlatformType.Windows;
            }
        }
    }
}

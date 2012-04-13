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
            for (int i = 0; i < buffer.Length; i++)
                sBuilder.Append(buffer[i].ToString("x2"));

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
            fs.Read(buffer, buffer.Length, 1);
            fs.Close();
            return GetMD5Hash(buffer);
        }
    }
}

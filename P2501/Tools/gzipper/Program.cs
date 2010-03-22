﻿/*
    Open Combat/Projekt 2501
    Copyright (C) 2010  Jeffery Allen Myers

    This library is free software; you can redistribute it and/or
    modify it under the terms of the GNU Lesser General Public
    License as published by the Free Software Foundation; either
    version 2.1 of the License, or (at your option) any later version.

    This library is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
    Lesser General Public License for more details.

    You should have received a copy of the GNU Lesser General Public
    License along with this library; if not, write to the Free Software
    Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;

namespace gzipper
{
    class Program
    {
        static void Main(string[] args)
        {
            DirectoryInfo inDir = new DirectoryInfo(args[0]);
            if (!inDir.Exists)
                return;

            string baseURL = string.Empty;
            if (args.Length > 1)
                baseURL = args[1];

            DirectoryInfo outdir = new DirectoryInfo(Path.Combine(Path.GetDirectoryName(inDir.FullName), inDir.Name + "_out"));
            outdir.Create();

            FileInfo outlog = new FileInfo(Path.Combine(outdir.FullName,"files.txt"));
            Stream logStream = outlog.OpenWrite();
            StreamWriter logWriter = new StreamWriter(logStream);

            ProcessDir(inDir, outdir, string.Empty, baseURL, logWriter);

            logWriter.Close();
            logStream.Close();
        }

        static void ProcessDir ( DirectoryInfo inDir, DirectoryInfo outDir, string path, string baseURL, StreamWriter log )
        {
            foreach (FileInfo file in inDir.GetFiles())
            {
                Console.WriteLine(file.FullName);

                CopyCompressedFile(file, outDir);

                string localpath = Path.Combine(path, file.Name);
                localpath = localpath.Replace('\\', '/');

                string url = baseURL + localpath;


                string md5 = ComputeMD5(file);

                log.WriteLine(localpath + "\t" + url + "\t" + md5);
            }

            foreach (DirectoryInfo dir in inDir.GetDirectories())
            {
                if (dir.Name[0] == '.')
                    continue;
                DirectoryInfo newDir = new DirectoryInfo(Path.Combine(outDir.FullName,dir.Name));
                newDir.Create();

                string d = Path.Combine(path, dir.Name);
                ProcessDir(dir,newDir,d,baseURL,log);
            }
        }

        static string ComputeMD5 ( FileInfo file )
        {
            Stream f = file.OpenRead();
            MD5 md5 = MD5.Create();
            byte[] inputHash = md5.ComputeHash(f);
            f.Close();

            string hash = "";
            foreach (byte b in inputHash)
                hash += b.ToString("x2");

            return hash;
        }

        static void CopyCompressedFile (FileInfo file, DirectoryInfo outDir )
        {
            FileInfo outputFile = new FileInfo(Path.Combine(outDir.FullName,file.Name));
            Stream outStream = outputFile.OpenWrite();
            GZipStream gz = new GZipStream(outStream, CompressionMode.Compress);

            Stream instream = file.OpenRead();

            bool done = false;
            while (!done)
            {
                byte[] buffer = new byte[512];
                int read = instream.Read(buffer, 0, 512);
                if (read == 0)
                    done = true;
                else
                {
                    gz.Write(buffer, 0, read);
                    done = read != 512;
                }
            }
            instream.Close();
            gz.Flush();
            gz.Close();
            outStream.Close();
        }
    }
}

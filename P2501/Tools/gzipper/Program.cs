using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;

namespace gzipper
{
    class Program
    {
        static void Main(string[] args)
        {
            DirectoryInfo inDir = new DirectoryInfo(args[0]);
            if (!inDir.Exists)
                return;

            DirectoryInfo outdir = new DirectoryInfo(Path.Combine(Path.GetDirectoryName(inDir.FullName), inDir.Name + "_out"));
            outdir.Create();

            ProcessDir(inDir,outdir);
           
        }

        static void ProcessDir ( DirectoryInfo inDir, DirectoryInfo outDir )
        {
            foreach ( FileInfo file in inDir.GetFiles() )
                CopyCompressedFile(file,outDir);

            foreach (DirectoryInfo dir in inDir.GetDirectories())
            {
                if (dir.Name[0] == '.')
                    continue;
                DirectoryInfo newDir = new DirectoryInfo(Path.Combine(outDir.FullName,dir.Name));
                newDir.Create();
                ProcessDir(dir,newDir);
            }
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

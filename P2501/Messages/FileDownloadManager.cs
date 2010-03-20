using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Security.Cryptography;

namespace Messages
{
    public class FileDownloadEventArgs : EventArgs
    {
        public int ID = -1;

        public FileDownloadEventArgs ( int i )
        {
            ID = i;
        }
    }

    public delegate void FileEventHandler ( object sender, FileDownloadEventArgs args );

    public class FileDownloadManager
    {
        protected static int lastFileID = 0;

        public class DownladedFile
        {
            public int ID = 0;
            public string Checksum = string.Empty;

            public byte[][] Buffers = null;

            public event FileEventHandler Complete = null;

            public void Done ()
            {
                if (Complete != null)
                    Complete(this, new FileDownloadEventArgs(ID));
            }
        }

        public static DirectoryInfo CacheFileDIr
        {
            set
            {
                lock(CacheDir)
                {
                    CacheDir = value;
                }
            }
        }

        protected static DirectoryInfo CacheDir = null;

        protected static List<DownladedFile> files = new List<DownladedFile>();

        public static int MaxMessageSize = -1;

        public static int ChacheFile ( FileInfo file )
        {
            if (!file.Exists)
                return -1;

            return ChacheFile(file.OpenRead());
        }

        public static int ChacheFile ( Stream file )
        {
            byte[] readbuffer = new byte[32768];
            byte[] buffer = null;

            using (MemoryStream ms = new MemoryStream())
            {
                while (true)
                {
                    int read = file.Read(readbuffer, 0, readbuffer.Length);
                    if (read <= 0)
                        break;
                    ms.Write(readbuffer, 0, read);
                }
                file.Close();
                buffer = ms.ToArray();
            }

            if (buffer.Length == 0)
                return -1;

            DownladedFile memFile = new DownladedFile();
            memFile.ID = lastFileID++;

            // for now just make one big ass buffer
            if (MaxMessageSize < 0)
            {
                memFile.Buffers = new byte[1][];
                memFile.Buffers[0] = buffer;
            }
            else
            {
                List<byte[]> temp = new List<byte[]>();
                int pos = 0;
                while (pos < buffer.Length)
                {
                    int size = MaxMessageSize;
                    if (buffer.Length - pos < size)
                        size = buffer.Length - pos;

                    byte[] b = new byte[size];
                    Array.Copy(buffer, pos, b, 0, size);
                    temp.Add(b);
                }

                memFile.Buffers = temp.ToArray();
            }

            memFile.Checksum = "";
            foreach (byte b in MD5.Create().ComputeHash(buffer))
                memFile.Checksum += b.ToString("x2");

            lock (files)
            {
                files.Add(memFile);
            }

            return memFile.ID;
        }

        public static FileTransfter[] GetMessages(int fileID, int clientID)
        {
            lock (files)
            {
                List<FileTransfter> msgs = new List<FileTransfter>();

                DownladedFile file = null;

                foreach (DownladedFile f in files)
                {
                    if (f.ID == fileID)
                    {
                        file = f;
                        break;
                    }
                }

                if (file != null)
                {
                    int i = 1;
                    foreach (byte[] b in file.Buffers)
                    {
                        FileTransfter msg = new FileTransfter();
                        msg.ID = clientID;
                        msg.Total = file.Buffers.Length;
                        msg.Chunk = i++;
                        msg.Size = b.Length;
                        msg.Data = b;
                        msgs.Add(msg);
                    }
                }

                return msgs.ToArray();
            }
        }

        public static bool FileExist ( int id )
        {
            lock (files)
            {
                foreach (DownladedFile f in files)
                {
                    if (f.ID == id)
                        return true;
                }
            }
            return false;
        }

        public static string GetFileChecksum (int id)
        {
            lock (files)
            {
                foreach (DownladedFile f in files)
                {
                    if (f.ID == id)
                        return f.Checksum;
                }
                return string.Empty;
            }
        }

        public static Stream GetFile( int ID )
        {
            lock (files)
            {
                foreach (DownladedFile file in files)
                {
                    if (file.ID == ID && file.Checksum != string.Empty && file.Buffers != null)
                        return new MemoryStream(file.Buffers[0]);
                }
            }
            return null;
        }

        public static Stream GetFile ( string checksum )
        {
            lock (files)
            {
                foreach ( DownladedFile file in files )
                {
                    if (file.Checksum == checksum && file.Buffers!= null)
                        return new MemoryStream(file.Buffers[0]);
                }
            }

            lock (CacheDir)
            {
                if (CacheDir == null)
                    return null;
            }

            FileInfo f = new FileInfo(Path.Combine(CacheDir.FullName, checksum));
            if (!f.Exists)
            {
                FileStream stream = f.OpenRead();
                byte[] buffer = new byte[f.Length];
                stream.Read(buffer, 0, (int)f.Length);
                stream.Close();
                return new MemoryStream(buffer);
            }
            return null;
        }

        public static int GetDownloadID ( FileEventHandler complete )
        {
            lock(files)
            {
                DownladedFile file = new DownladedFile();
                file.ID = lastFileID++;
                file.Complete += complete;
                files.Add(file);
                return file.ID;
            }
        }

        public static bool AddFileData ( FileTransfter msg )
        {
            DownladedFile file = null;
            lock (files)
            {
                foreach(DownladedFile f in files)
                {
                    if (f.ID == msg.ID)
                    {
                        file = f;
                        break;
                    }
                }

                if (file == null)
                {
                    file = new DownladedFile();
                    file.ID = msg.ID;
                    files.Add(file);
                }

                if (file.Buffers == null || msg.Total > file.Buffers.Length)
                {
                    byte[][] newbuffer = new byte[msg.Total][];
                    if (file.Buffers != null)
                        file.Buffers.CopyTo(newbuffer, 0);

                    file.Buffers = newbuffer;
                }

                file.Buffers[msg.Chunk-1] = msg.Data;

                // see if it we are done
                if (msg.Chunk != msg.Total)
                    return false;

                byte[][] fullbuffer = new byte[1][];

                int size = 0;
                foreach (byte[] b in file.Buffers)
                    size += b.Length;

                fullbuffer[0] = new byte[size];

                size = 0;
                foreach (byte[] b in file.Buffers)
                {
                    b.CopyTo(fullbuffer[0], size);
                    size += b.Length;
                }

                file.Buffers = fullbuffer;

                file.Checksum = "";
                foreach (byte b in MD5.Create().ComputeHash(file.Buffers[0]))
                    file.Checksum += b.ToString("x2");

                // ok try to cache this thing out
                if (CacheDir != null)
                {
                    lock (CacheDir)
                    {
                        FileInfo cachefile = new FileInfo(Path.Combine(CacheDir.FullName, file.Checksum));
                        try
                        {
                            Stream s = cachefile.OpenWrite();
                            s.Write(file.Buffers[0], 0, file.Buffers[0].Length);
                            s.Close();
                        }
                        catch (System.Exception /*ex*/)
                        {
                            CacheDir = null;// the cache dir is crap, don't try to cache anymore stuff
                        }
                    }
                }
               
            }
            file.Done();
            return true;
        }
    }
}

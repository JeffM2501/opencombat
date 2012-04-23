using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Web;

using FileLocations;

namespace Client
{
    class Patcher
    {
        Hasher DataHashes = new Hasher();
        Hasher BinHases = new Hasher();

        Hasher RemoteDataHashes = null;
        // Use later
        //Hasher RemoteBinHashes = null;

        WebClient WebConnection = null;

        protected bool Done = false;
        protected bool NeedToDie = false;
        protected bool ErrorOnUpdate = false;

        public enum Status
        {
            Unknown,
            BuldingHashes,
            GettingDataList,
            GettingDataFiles,
            GettingBinList,
            GettingBinFiles,
        }

        protected Status ProgressStatus = Status.Unknown;

        protected int TotalFilesToGet = 0;
        protected int TotalFilesGot = 0;

        public static string DataUpdateURL = "http://www.opencombat.net/updates/data.php";
        public static string BinUpdateURL = "http://www.opencombat.net/updates/bin.php";

        protected List<string> OutOfDateDataFiles = null;

        protected object Locker = new object();

        public void BuildHashTables()
        {
            lock(Locker)
                ProgressStatus = Status.BuldingHashes;
            DataHashes.BuildDataHashes(false);
        }

        public void StartUpdate()
        {
            if (WebConnection != null)
                return;

            Done = false;
            NeedToDie = false;
            ErrorOnUpdate = false;
            RemoteDataHashes = null;
           // RemoteBinHashes = null;

            if (DataHashes == null)
                BuildHashTables();

            lock(Locker)
                ProgressStatus = Status.GettingDataList;

            WebConnection = new WebClient();
            WebConnection.DownloadStringCompleted += new DownloadStringCompletedEventHandler(WebConnection_DownloadStringCompleted);
            WebConnection.DownloadDataCompleted +=new DownloadDataCompletedEventHandler(WebConnection_DownloadDataCompleted);

            WebConnection.DownloadStringAsync(new Uri(DataUpdateURL + "?action=gethash"));
        }

        void  WebConnection_DownloadDataCompleted(object sender, DownloadDataCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                lock (Locker)
                    ErrorOnUpdate = Done = true;
                return;
            }

            if (OutOfDateDataFiles != null)
            {
                lock (Locker)
                {
                    ProgressStatus = Status.GettingDataFiles;
                    TotalFilesGot++;
                }

                bool LastOne = GetNextDataFile();

                if (LastOne)
                    StartBinUpdate();

                if (e.UserState.GetType() != typeof(string))
                    return;

                string fileName = e.UserState as string;

                string localPath = FindLocalLocationForDownload(fileName);
                if (localPath == string.Empty)
                    return;

                if (DataHashes.HashTable.ContainsKey(fileName))
                    DataHashes.HashTable[fileName].Hash = RemoteDataHashes.HashTable[fileName].Hash;
                else
                    DataHashes.HashTable.Add(fileName, RemoteDataHashes.HashTable[fileName]);

                FileInfo file = new FileInfo(localPath);
                FileStream fs = file.OpenWrite();
                fs.Write(e.Result, 0, e.Result.Length);
                fs.Close();

                if (LastOne)
                    DataHashes.SaveDataHases();
            }
            else // it's bins so do what we have to for them
            {

            }
        }

        protected string FindLocalLocationForDownload(string fileName)
        {
            string dir = Locations.GetWritableDataFolder(fileName);

            if (!Locations.CreateSubDirsForFile(dir, fileName))
                return string.Empty;

            return new FileInfo(Path.Combine(dir, fileName)).FullName;
        }

        void WebConnection_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                lock (Locker)
                    ErrorOnUpdate = Done = true;
                return;
            }

            if (RemoteDataHashes == null)
            {
                string[] hashes = e.Result.Split("\r\n".ToCharArray());

                RemoteDataHashes = new Hasher();
                foreach (string hash in hashes)
                {
                    if (hash != string.Empty)
                        RemoteDataHashes.HashFromText(hash);
                }

                OutOfDateDataFiles = GetOutOfDateFiles(RemoteDataHashes, DataHashes);

                lock (Locker)
                {
                    ProgressStatus = Status.GettingDataFiles;
                    TotalFilesToGet = OutOfDateDataFiles.Count;
                    TotalFilesGot = 0;
                }

                if (GetNextDataFile())
                    StartBinUpdate();
            }
            else
            {
                // this will be the bin update hash
            }
           
        }

        protected void StartBinUpdate()
        {
            lock (Locker)
                ProgressStatus = Status.GettingBinList;

            lock (Locker)
                Done = true;
        }

        protected bool GetNextDataFile()
        {
            if (OutOfDateDataFiles == null)
                return true;

            if (OutOfDateDataFiles.Count == 0)
            {
                OutOfDateDataFiles = null;
                return true;
            }

            string thisFile = OutOfDateDataFiles[0];
            OutOfDateDataFiles.RemoveAt(0);
            WebConnection.DownloadDataAsync(new Uri(DataUpdateURL + "?action=getfile&file=" + HttpUtility.UrlEncode(thisFile)),thisFile);

            return false;
        }


        public List<string> GetOutOfDateFiles(Hasher remoteHashes, Hasher localHashes)
        {
            List<string> outOfDateFiles = new List<string>();

            foreach (Hasher.Entry entry in remoteHashes.HashTable.Values)
            {
                if (!localHashes.HashTable.ContainsKey(entry.LocalPath) || localHashes.HashTable[entry.LocalPath].Hash != entry.Hash)
                    outOfDateFiles.Add(entry.LocalPath);
            }

            return outOfDateFiles;
        }

        public bool UpdateDone()
        {
            lock (Locker)
                return Done;
        }

        public bool NeedToExit()
        {
            lock (Locker)
                return NeedToDie;
        }

        public bool UpdateError()
        {
            lock (Locker)
                return ErrorOnUpdate;
        }

        public Status GetStatus()
        {
            lock (Locker)
                return ProgressStatus;
        }

        public void GetProgress (out int total, out int done )
        {
            lock (Locker)
            {
                total = TotalFilesToGet;
                done = TotalFilesGot;
            }
        }

        public void KillUpdate()
        {
            if (WebConnection != null && WebConnection.IsBusy)
                WebConnection.CancelAsync();

            WebConnection = null;
            Done = true;
            NeedToDie = false;
        }
    }
}

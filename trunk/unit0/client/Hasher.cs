using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.IO;
using System.Security.Cryptography;

using FileLocations;

namespace Client
{
    public class Hasher
    {
        public class Entry
        {
            public string Hash = string.Empty;
            public string LocalPath = string.Empty;

            public Entry() { }
            public Entry(string line) { ReadConfigLine(line); }

            public void ReadConfigLine(string line)
            {
                string[] parts = line.Split(";".ToCharArray());
                if (parts.Length > 0)
                    Hash = parts[0];
                if (parts.Length > 1)
                    LocalPath = parts[1];
            }

            public string WriteConfigLine()
            {
                return  Hash + ";" + LocalPath;
            }
        }

        public Dictionary<string, Entry> HashTable = new Dictionary<string, Entry>();

        public void ReadHashtable(string path)
        {
            FileInfo file = new FileInfo(path);
            StreamReader reader = file.OpenText();

            HashTable.Clear();
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                Entry e = new Entry();
                e.ReadConfigLine(line);
                if (HashTable.ContainsKey(e.LocalPath))
                    HashTable[e.LocalPath] = e;
                else
                    HashTable.Add(e.LocalPath, e);
            }
            reader.Close();
        }

        public void WriteHashtable(string path)
        {
            FileInfo file = new FileInfo(path);
            FileStream fs = file.OpenWrite();
            StreamWriter writer = new StreamWriter(fs);

            foreach (Entry e in HashTable.Values)
                writer.WriteLine(e.WriteConfigLine());

            writer.Close();
            fs.Close();
        }

        public void HashFromText(string line)
        {
            Entry e = new Entry(line);
            if (HashTable.ContainsKey(e.LocalPath))
                HashTable[e.LocalPath] = e;
            else
                HashTable.Add(e.LocalPath, e);
        }

        protected List<DirectoryInfo> DirsToHash = new List<DirectoryInfo>();

        public void BuildDataHashes( bool forceRebuild )
        {
            // see if we have a saved hashtable.
            string hashPath = Path.Combine(Locations.GetLowestConfigDir(),"data.contents.list");

            if (!forceRebuild && File.Exists(hashPath))
                ReadHashtable(hashPath);
            else
            {
                // walk the install dir

                string installDir = Locations.GetApplicationDataDir();

                DirsToHash.Add(new DirectoryInfo(installDir));
                while (DirsToHash.Count > 0)
                {
                    DirectoryInfo dir = DirsToHash[0];
                    DirsToHash.RemoveAt(0);
                    AddFilesToHashTable(dir, installDir, true);
                }

                string commonDir = Locations.GetCommonDataDir();

                if (commonDir != string.Empty && Directory.Exists(commonDir))
                {
                    DirsToHash.Add(new DirectoryInfo(commonDir));
                    while (DirsToHash.Count > 0)
                    {
                        DirectoryInfo dir = DirsToHash[0];
                        DirsToHash.RemoveAt(0);
                        AddFilesToHashTable(dir, commonDir, true);
                    }
                }

                string userDir = Locations.GetUserDataDir();

                if (userDir != string.Empty && Directory.Exists(userDir))
                {
                    DirsToHash.Add(new DirectoryInfo(userDir));
                    while (DirsToHash.Count > 0)
                    {
                        DirectoryInfo dir = DirsToHash[0];
                        DirsToHash.RemoveAt(0);
                        AddFilesToHashTable(dir, userDir, true);
                    }
                }

                WriteHashtable(hashPath);
            }
        }

        public void SaveDataHases()
        {
            string hashPath = Path.Combine(Locations.GetLowestConfigDir(), "data.contents.list");
            WriteHashtable(hashPath);
        }

        public bool BuildBinHashes()
        {
            // see if our running dir has a hashTable
            string hashFile = "bin.contents.list";
            string hashPath = Path.Combine(Locations.ThisExeConfigPath, hashFile);

            if (!File.Exists(hashPath))
                hashPath = Path.Combine(Locations.GetLowestConfigDir(), hashFile);

            if (File.Exists(hashPath))
                ReadHashtable(hashPath);
            else
            {
                // if this doesn't work we'll have to make a small app that can build the hashtable and call it
                HashTable.Clear();
                DirectoryInfo dir = new DirectoryInfo(Locations.ThisExePath);

                bool OneDidntWork = false;

                foreach (FileInfo file in dir.GetFiles())
                {
                    Entry entry = new Entry();
                    try
                    {
                          entry.Hash = GetMD5HashFromFile(file.FullName);
                    }
                    catch (System.Exception /*ex*/)
                    {
                        OneDidntWork = true;
                    }

                    entry.LocalPath = file.Name;
                    HashTable.Add(file.Name, entry);
                }

                WriteHashtable(hashPath);

                if (OneDidntWork)
                {
                    Locations.ExcuteExe(Path.Combine(Locations.ThisExePath, "hashbins.exe"), hashPath + "," + Locations.ThisExePath);
                    Application.Exit();
                    return false;
                }
            }
            return true;
        }

        protected string GetMD5HashFromFile(string fileName)
        {
            FileStream file = new FileStream(fileName, FileMode.Open);
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] retVal = md5.ComputeHash(file);
            file.Close();

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < retVal.Length; i++)
            {
                sb.Append(retVal[i].ToString("x2"));
            }
            return sb.ToString();
        }

        protected void AddFilesToHashTable(DirectoryInfo dir, string rootPath, bool recursive)
        {
            foreach (FileInfo file in dir.GetFiles())
            {
                Entry e = new Entry();
                e.Hash = GetMD5HashFromFile(file.FullName);
                e.LocalPath =  file.FullName.Remove(0, rootPath.Length+1).Replace('\\','/');

                if (HashTable.ContainsKey(e.LocalPath))
                    HashTable[e.LocalPath] = e;
                else
                    HashTable.Add(e.LocalPath, e);
            }

            if (!recursive)
                return;

            foreach (DirectoryInfo d in dir.GetDirectories())
                DirsToHash.Add(d);
        } 
    }
}

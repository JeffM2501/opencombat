using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Web;
using System.Net;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace Patcher
{
    public partial class PatchForm : Form
    {
        object marker = new object();
        Thread worker = null;

        DirectoryInfo rootDir;
        FileInfo CurrentVersion;

        bool done = false;
        int count = -1;
        int current = 0;

        int lastCurrent = 0;

        string currentFile = string.Empty;

        public PatchForm( FileInfo currentVersionFile )
        {
            InitializeComponent();
            UpdateLabel.Text = "Checking for updates";
            progressBar1.Visible = false;

            CurrentVersion = currentVersionFile;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            lock(marker)
            {
                if (done)
                {
                    timer1.Stop();
                    DialogResult = DialogResult.OK;
                    if (count == -1)
                        UpdateLabel.Text = "Content up to date";
                    else
                        UpdateLabel.Text = "Update Complete";
                }
                else
                {
                    if (count == 0)
                    {
                        UpdateLabel.Text = "Comparing Files";
                    }
                    else if ( count > 1)
                    {
                        UpdateLabel.Text = "Updating:" + currentFile;
                        progressBar1.Visible = true;
                        progressBar1.Maximum = count;
                        progressBar1.Increment(current - progressBar1.Step);
                    }
                }
            }
            Application.DoEvents();
        }

        private void PatchForm_Load(object sender, EventArgs e)
        {
            rootDir = new DirectoryInfo(Path.GetDirectoryName(CurrentVersion.FullName));
            FileInfo test = new FileInfo(Path.Combine(rootDir.FullName,"temp.test"));
            try
            {
                Stream s = test.OpenWrite();
                if (s != null)
                {
                    s.Close();
                    test.Delete();
                }
                else
                    test = null;
            }
            catch (System.Exception /*ex*/)
            {
            	test = null;
            }

            if (test == null)
            {
                DirectoryInfo AppSettingsDir = new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Projekt2501"));
                rootDir = new DirectoryInfo(Path.Combine(AppSettingsDir.FullName,"Updates"));
                test = new FileInfo(Path.Combine(rootDir.FullName,"temp.test"));
                try
                {
                    Stream s = test.OpenWrite();
                    if (s != null)
                    {
                        s.Close();
                        test.Delete();
                    }
                    else
                        test = null;
                }
                catch (System.Exception /*ex*/)
                {
            	    test = null;
                }
            }
            if (test == null)
            {
                MessageBox.Show("Unable to write update data");
                DialogResult = DialogResult.Cancel;
                Close();
                return;
            }

            timer1.Start();

            worker = new Thread(new ThreadStart(RunUpdate));
            worker.Start();
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            if (worker != null)
            {
                worker.Abort();
                worker = null;
            }
        }

        protected void RunUpdate()
        {
            UpdateList thisVersion = new UpdateList(CurrentVersion);

            WebClient client = new WebClient();
            Stream stream = client.OpenRead("http://www.opencombat.net/updates/client/version.txt");
            StreamReader reader = new StreamReader(stream);
            UpdateList remoteVersion = new UpdateList(reader);
            stream.Close();

            if (thisVersion.Version == remoteVersion.Version)
            {
                lock(marker)
                {
                    done = true;
                    return;
                }
            }

            lock(marker)
            {
                count = 0;
            }
            List<UpdateList.UpdateItem> DownloadItems = new List<UpdateList.UpdateItem>();
            List<UpdateList.UpdateItem> DeleteItems = new List<UpdateList.UpdateItem>();

            // walk the list of files that are in the new version and see if we have to download them
            foreach(KeyValuePair<string,UpdateList.UpdateItem> item in remoteVersion.Items)
            {
                if (thisVersion.Items.ContainsKey(item.Key))
                {
                    if (thisVersion.Items[item.Key].MD5Hash != item.Value.MD5Hash)
                        DownloadItems.Add(item.Value);
                    thisVersion.Items.Remove(item.Key);
                }
                else
                    DownloadItems.Add(item.Value);
            }

            foreach (KeyValuePair<string, UpdateList.UpdateItem> item in thisVersion.Items)
                DeleteItems.Add(item.Value);

            lock(marker)
            {
                count = DownloadItems.Count + DeleteItems.Count;
            }

            foreach (UpdateList.UpdateItem newItem in DownloadItems)
            {
                lock(marker)
                {
                    current++;
                    currentFile = newItem.LocalPath;
                }

                DownalodFile(newItem,client.OpenRead(newItem.URL));
            }

            foreach (UpdateList.UpdateItem deleteItem in DeleteItems)
            {
                lock (marker)
                {
                    current++;
                    currentFile = deleteItem.LocalPath;
                }
                BuildPath(deleteItem.LocalPath).Delete();
            }

            // write the new version file

            try
            {
                CurrentVersion.Delete();
            }
            catch (System.Exception /*ex*/)
            {
                DirectoryInfo AppSettingsDir = new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Projekt2501"));
                rootDir = new DirectoryInfo(Path.Combine(AppSettingsDir.FullName, "Updates"));
                CurrentVersion = new FileInfo(Path.Combine(rootDir.FullName, "client_version.txt"));
            }

            Stream vers = CurrentVersion.OpenWrite();
            remoteVersion.Write(new StreamWriter(vers));
            vers.Close();

            done = true;
        }

        protected FileInfo BuildPath ( string p )
        {
            string[] nugs = p.Split("/".ToCharArray());
            DirectoryInfo dir = new DirectoryInfo(rootDir.FullName);
            for (int i = 0; i < nugs.Length-1; i++)
                dir = dir.CreateSubdirectory(nugs[i]);

            return new FileInfo(Path.Combine(dir.FullName, nugs[nugs.Length - 1]));
        }

        protected void DownalodFile(UpdateList.UpdateItem item, Stream stream )
        {
            FileInfo outfile = BuildPath(item.LocalPath);
            GZipStream gz = new GZipStream(stream, CompressionMode.Decompress);

            Stream outstream = outfile.OpenWrite();
            if (outstream == null)
                return;

            bool filedone = false;
            while (!filedone)
            {
                byte[] buffer = new byte[512];
                int read = gz.Read(buffer, 0, 512);
                if (read == 0)
                    filedone = true;
                else
                {
                    outstream.Write(buffer, 0, read);
                    filedone = read != 512;
                }
            }
            gz.Close();
            stream.Close();
            outstream.Close();
        }
    }

    public class UpdateList
    {
        public class UpdateItem
        {
            public string LocalPath = string.Empty;
            public string URL = string.Empty;
            public string MD5Hash = string.Empty;

            public UpdateItem( string[] a)
            {
                LocalPath = a[0];
                URL = a[1];
                MD5Hash = a[2];
            }
        }

        public Dictionary<string, UpdateItem> Items = new Dictionary<string,UpdateItem>();
        public string Version = string.Empty;

        public UpdateList ( FileInfo file )
        {
            if (!file.Exists)
                return;

            Stream stream = file.OpenRead();
            Read(new StreamReader(stream));
            stream.Close();
        }

        public UpdateList ( StreamReader reader )
        {
            Read(reader);
        }

        protected void Read ( StreamReader reader )
        {
            Version = reader.ReadLine();

            string line = reader.ReadLine();
            while (line != null)
            {
                if (line == string.Empty)
                    continue;

                string[] nugs = line.Split("\t".ToCharArray());
                if (nugs.Length > 1)
                    Items.Add(nugs[0],new UpdateItem(nugs));

                line = reader.ReadLine();
            }
            reader.Close();
        }

        public void Write ( StreamWriter writer )
        {
            foreach(KeyValuePair<string,UpdateItem> item in Items)
                writer.WriteLine(item.Value.LocalPath + "\t" + item.Value.URL + "\t" + item.Value.MD5Hash);

            writer.Close();
        }
    }
}

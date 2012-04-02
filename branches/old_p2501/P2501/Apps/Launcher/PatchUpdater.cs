/*
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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Net;
using System.Web;
using System.IO;
using System.IO.Compression;

namespace Launcher
{
    public partial class PatchUpdater : Form
    {
        public string existingVers = string.Empty;

        Thread worker = null;

        object marker = new object();

        bool done = false;

        int count = 0;
        int current = 0;

        public PatchUpdater()
        {
            InitializeComponent();
            UpdateLabel.Text = "Checking for updates";
            progressBar1.Visible = false;
        }

        private void PatchUpdater_Shown(object sender, EventArgs e)
        {

        }

        private void PatchUpdater_Load(object sender, EventArgs e)
        {
            timer1.Start();

            worker = new Thread(new ThreadStart(RunUpdate));
            worker.Start();
        }

        protected void RunUpdate ()
        {
            WebClient client = new WebClient();

            Stream stream = client.OpenRead("http://www.opencombat.net/updates/patcher/version.txt");
            StreamReader reader = new StreamReader(stream);
            string vers = reader.ReadLine();

            if (existingVers != string.Empty && vers == existingVers)
            {
                reader.Close();
                stream.Close();
                lock(marker)
                {
                    done = true;
                    return;
                }
            }

            // time to download
            List<string> files = new List<string>();
            string temp = reader.ReadLine();
            while ( temp != null)
            {
                files.Add(temp);
                temp = reader.ReadLine();
            }
            reader.Close();
            stream.Close();


            lock (marker)
            {
                count = files.Count;
            }
           
            DirectoryInfo AppSettingsDir = new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Projekt2501"));
            if (!AppSettingsDir.Exists)
                AppSettingsDir.Create();

            DirectoryInfo updateDir = AppSettingsDir.CreateSubdirectory("Updates");

            foreach (string file in files)
            {
                string[] nugs = file.Split("\t".ToCharArray());
                if (nugs.Length < 2)
                    continue;

                FileInfo outputFile = new FileInfo(Path.Combine(updateDir.FullName, nugs[0]));

                Stream outstream = outputFile.OpenWrite();
                if (outstream == null)
                    continue;

                stream = client.OpenRead(nugs[1]);
                GZipStream gz = new GZipStream(stream,CompressionMode.Decompress);

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

                outstream.Close();
                gz.Close();
                stream.Close();

                lock (marker)
                {
                    current++;
                }
            }

            lock (marker)
                done = true;
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

        private void timer1_Tick(object sender, EventArgs e)
        {
            lock (marker)
            {
                if (done)
                {
                    timer1.Stop();
                    DialogResult = DialogResult.OK;
                    if (count == 0)
                        UpdateLabel.Text = "Patch System up to date";
                    else
                        UpdateLabel.Text = "Update Complete";
                }
                else
                {
                    if (count > 0)
                    {
                        UpdateLabel.Text = "Updating Patch System";
                        progressBar1.Maximum = count;
                        progressBar1.Visible = true;
                        progressBar1.Step = current;
                    }
                }
            }
            Application.DoEvents();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;
using System.Net;
using System.Web;
using System.Diagnostics;
using System.Threading;
using System.IO;

using Clients;
using Lidgren.Network;

using Auth;

namespace P2501Client
{
    class Login
    {
        public UInt64 UID = 0;
        public UInt64 Token = 0;

        CryptoClient client = null; 

        public bool Connect (string email, string password )
        {
            WaitBox box = new WaitBox("Logon");
            box.Show();
            box.Update(10, "Contacting secure host");

            if (client != null)
                client.Kill();

            // CryptoClient client = new CryptoClient("www.awesomelaser.com", 4111);
            client = new CryptoClient("localhost", 4111);

            Stopwatch timer = new Stopwatch();
            timer.Start();

            int timeout = 120;

            bool done = false;
            bool connected = false;

            while (!done)
            {
                if (!connected && client.IsConnected)
                    box.Update(20, "Connection established");
                if (connected && !client.IsConnected)
                    done = true;

                if (!done)
                {
                    NetBuffer buffer = client.GetPentMessage();
                    while (buffer != null)
                    {
                        int name = buffer.ReadInt32();

                        if (name == AuthMessage.Hail)
                        {
                            RequestAuth msg = new RequestAuth();
                            msg.email = email;
                            msg.password = password;

                            box.Update(75, "Sending credentials");
                            client.SendMessage(msg.Pack(), msg.Channel());
                        }
                        else if (name == AuthMessage.AuthOK)
                        {
                            AuthOK ok = new AuthOK();
                            ok.Unpack(ref buffer);
                            box.Update(100, "Login complete");
                            box.Close();
                            UID = ok.ID;
                            Token = ok.Token;
                            return true;
                        }
                        else if (name == AuthMessage.AuthBadCred)
                        {
                            client.Kill();
                            client = null;
                            box.Close();
                            MessageBox.Show("Login Failed");
                            return false;
                        }
                        else
                        {
                            done = true;
                            connected = false;
                        }

                        if (!done)
                            buffer = client.GetPentMessage();
                        else
                            buffer = null;
                    }

                    if (timer.ElapsedMilliseconds / 1000 > timeout)
                    {
                        done = true;
                        connected = false;
                    }
                    Application.DoEvents();
                    Thread.Sleep(100);
                }
            }
            box.Close();

            client.Kill();
            client = null;
            MessageBox.Show("The login server could not be contacted");

            return false;
        }

        static public bool CheckName ( string name )
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://www.awesomelaser.com/p2501/Auth/callsigncheck.php?name=" + HttpUtility.UrlEncode(name));
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            Stream resStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(resStream);
            string ret = reader.ReadToEnd();
            reader.Close();
            resStream.Close();

            return ret == "OK";
        }

        public bool AddCharacter ( string name )
        {
            if (client == null || !CheckName(name))
                return false;

            WaitBox box = new WaitBox("Adding Callsign");
            box.Update("Contacting Server");

            RequestAddCharacter data = new RequestAddCharacter();
            data.callsign = name;
            client.SendMessage(data.Pack(), data.Channel());
           
            Stopwatch timer = new Stopwatch();
            timer.Start();

            int timeout = 30;

            while (timer.ElapsedMilliseconds / 1000 < timeout)
            {
                NetBuffer buffer = client.GetPentMessage();
                while (buffer != null)
                {
                    int msgCode = buffer.ReadInt32();

                    if (msgCode == AuthMessage.CharacterAddOK)
                    {
                        client.Kill();
                        client = null;
                        box.Close();
                        return true;
                    }
                    else
                    {
                        box.Close();
                        client.Kill();
                        client = null;
                        return false;      
                    }
                   // buffer = client.GetPentMessage();
                }
                Application.DoEvents();
                Thread.Sleep(100);
            }
            box.Close();
            client.Kill();
            client = null;
            return false;
        }

        public Dictionary<UInt64,string> GetCharacterList ()
        {
            if (client == null)
                return null;

            WaitBox box = new WaitBox("Callsigns");
            box.Show();
            box.Update("Requesting list");
            NetBuffer buffer = new NetBuffer();
            buffer.Write(AuthMessage.RequestCharacterList);
            client.SendMessage(buffer, NetChannel.ReliableInOrder1);

            Stopwatch timer = new Stopwatch();
            timer.Start();

            int timeout = 30;

            while (timer.ElapsedMilliseconds / 1000 < timeout)
            {
                buffer = client.GetPentMessage();
                while (buffer != null)
                {
                    int name = buffer.ReadInt32();

                    if (name == AuthMessage.CharacterList)
                    {
                        Dictionary<UInt64, string> list = new Dictionary<UInt64, string>();

                        CharacterList cList = new CharacterList();
                        cList.Unpack(ref buffer);

                        foreach (CharacterList.CharacterInfo info in cList.Characters)
                            list.Add(info.CID, info.Name);

                        client.Kill();
                        client = null;
                        box.Close();
                        return list;
                    }
                    else
                    {
                        box.Close();
                        client.Kill();
                        client = null;
                        return null;      
                    }
                   // buffer = client.GetPentMessage();
                } 
                Application.DoEvents();
                Thread.Sleep(100);
            }
            box.Close();
            client.Kill();
            client = null;
            return null;      
        }
    }
}

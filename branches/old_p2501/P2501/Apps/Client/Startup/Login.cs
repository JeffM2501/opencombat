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
using System.Windows.Forms;
using System.Text;
using System.Net;
using System.Web;
using System.Net.Security;
using System.Diagnostics;
using System.Threading;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

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

        static bool useUDP = false;

        WebClient webClient = null;

        public bool Connect (string email, string password )
        {
            if (useUDP)
                return ConnectUDP(email, password);

            return ConnectHTML(email, password);
        }

        protected bool ConnectHTML(string email, string password)
        {
            WaitBox box = new WaitBox("Logon");
            box.Show();
            box.Update("Contacting secure server");
            SetSLL();

            UID = 0;
            Token = 0;

            string url = "https://secure.opencombat.net/authserver.php?action=login&email=" + HttpUtility.UrlEncode(email) + "&password=" + HttpUtility.UrlEncode(password);
            webClient = new WebClient();

            Stream stream = webClient.OpenRead(url);
            StreamReader reader = new StreamReader(stream);

            string code = reader.ReadLine();
            if (code != "ok")
            {
                reader.Close();
                stream.Close();
                webClient = null;
                box.Close();
                return false;
            }

            UID = UInt64.Parse(reader.ReadLine());
            Token = UInt64.Parse(reader.ReadLine());

            reader.Close();
            stream.Close();

            box.Update("Login complete");
            box.Close();
            return true;
        }

        protected bool ConnectUDP(string email, string password)
        {
            WaitBox box = new WaitBox("Logon");
            box.Show();
            box.Update("Contacting secure host");

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

                            box.Update("Sending credentials");
                            client.SendMessage(msg.Pack(), msg.Channel());
                        }
                        else if (name == AuthMessage.AuthOK)
                        {
                            AuthOK ok = new AuthOK();
                            ok.Unpack(ref buffer);
                            box.Update("Login complete");
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
            SetSLL();

            WebClient request = new WebClient();
            Stream resStream = request.OpenRead("https://secure.opencombat.net/callsigncheck.php?name=" + HttpUtility.UrlEncode(name));
            StreamReader reader = new StreamReader(resStream);
            string ret = reader.ReadLine();
            reader.Close();
            resStream.Close();

            return ret == "OK";
        }

        public enum RegisterCode
        {
            OK,
            BadEmail,
            BadCallsign,
            Error
        }

        public static RegisterCode Register ( string email, string password, string character )
        {
            if (useUDP)
                return RegisterUDP(email, password, character);

            return RegisterHTML(email, password, character);
        }

        protected static void SetSLL ()
        {
             ServicePointManager.ServerCertificateValidationCallback +=  
                delegate(  
                object sender,  
                X509Certificate certificate,  
                X509Chain chain,  
                SslPolicyErrors sslPolicyErrors)  
                {  
                   return true;  
                };  
        }

        protected static RegisterCode RegisterHTML(string email, string password, string character)
        {
            SetSLL();

            string url = "https://secure.opencombat.net/authserver.php?action=adduser&email=" + HttpUtility.UrlEncode(email) + "&password=" + HttpUtility.UrlEncode(password) + "&character=" + HttpUtility.UrlEncode(character) ;

            WebClient request = new WebClient();
            Stream stream = request.OpenRead(url);
            StreamReader reader = new StreamReader(stream);

            string code = reader.ReadLine();
            if (code == "authbademail")
                return RegisterCode.BadEmail;
            if (code == "authbadcallsign")
                return RegisterCode.BadCallsign;
            if (code != "ok")
                return RegisterCode.Error;

            reader.Close();
            stream.Close();

            return RegisterCode.OK;
        }

        protected static RegisterCode RegisterUDP ( string email, string password, string character )
        {
            // CryptoClient client = new CryptoClient("www.awesomelaser.com", 4111);
            CryptoClient client = new CryptoClient("localhost", 4111);

            Stopwatch timer = new Stopwatch();
            timer.Start();

            int timeout = 120;

            bool done = false;
            bool connected = false;

            while (!done)
            {
                if (connected && !client.IsConnected)
                {
                    client.Kill();
                    return RegisterCode.Error;
                }

                NetBuffer buffer = client.GetPentMessage();
                while (buffer != null)
                {
                    int name = buffer.ReadInt32();

                    if (name == AuthMessage.Hail)
                    {
                        RequestAdd msg = new RequestAdd();
                        msg.email = email;
                        msg.password = password;
                        msg.callsign = character;

                        client.SendMessage(msg.Pack(), msg.Channel());
                    }
                    else if (name == AuthMessage.AddOK)
                    {
                        client.Kill();
                        done = true;
                    }
                    else if (name == AuthMessage.AddBadCallsign)
                    {
                        client.Kill();
                        return RegisterCode.BadCallsign;
                    }
                    else if (name == AuthMessage.AddBadEmail)
                    {
                        client.Kill();
                        return RegisterCode.BadEmail;

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
                    client.Kill();
                    return RegisterCode.Error;
                }
                Application.DoEvents();
                Thread.Sleep(100);
            }
            client.Kill();

            return RegisterCode.OK;
        }

        public bool AddCharacter ( string name )
        {
            if (useUDP)
                return AddCharacterUDP(name);

            return AddCharacterHTML(name);
        }

        public bool AddCharacterHTML ( string name )
        {
            if (webClient == null)
                return false;

            WaitBox box = new WaitBox("Adding Callsign");
            box.Update("Contacting Server");
            
            SetSLL();

            string url = "https://secure.opencombat.net/authserver.php?action=addchar&UID=" + UID.ToString() + "&callsign=" + HttpUtility.UrlEncode(name);
            Stream stream = webClient.OpenRead(url);
            StreamReader reader = new StreamReader(stream);

            webClient = null;
            string code = reader.ReadLine();
            reader.Close();
            stream.Close();
            box.Close();

            return code == "ok";
        }

        public bool AddCharacterUDP ( string name )
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
            if (useUDP)
                return GetCharacterListUDP();
            return GetCharacterListHTML();
        }

        protected Dictionary<UInt64,string> GetCharacterListHTML ()
        {
            if (webClient == null)
                return null;

            Dictionary<UInt64, string> list = new Dictionary<UInt64, string>();

            SetSLL();
            WaitBox box = new WaitBox("Callsigns");
            box.Show();
            box.Update("Requesting list");

            string url = "https://secure.opencombat.net/authserver.php?action=listchar&uid=" + UID.ToString() + "&token=" + Token.ToString();
            Stream stream = webClient.OpenRead(url);
            StreamReader reader = new StreamReader(stream);

            string code = reader.ReadLine();
            int count = 0;
            if (int.TryParse(code,out count))
            {
                for (int i = 0; i < count; i++)
                {
                    string item = reader.ReadLine();
                    string[] nugs = item.Split("\t".ToCharArray());

                    list.Add(UInt64.Parse(nugs[0]), nugs[1]);
                }
            }
         
            webClient = null;
            reader.Close();
            stream.Close();
            box.Close();

            return list;
        }

        protected Dictionary<UInt64,string> GetCharacterListUDP ()
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

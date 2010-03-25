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
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Diagnostics;

using Lidgren.Network;
using OpenTK;
using Simulation;

namespace Messages
{
    public class MessageMapper
    {
        protected Dictionary<int, Type> MessageTypes = new Dictionary<int, Type>();

        public MessageMapper()
        {
            Assembly myModule = Assembly.GetExecutingAssembly();
            foreach (Type t in myModule.GetTypes())
            {
                if (t.BaseType == typeof(MessageClass))
                {
                    MessageClass m = (MessageClass)Activator.CreateInstance(t);

                    MessageTypes.Add(m.Name, t);
                }
            }
        }

        public MessageClass MessageFromID ( int id )
        {
            if (MessageTypes.ContainsKey(id))
                return (MessageClass)Activator.CreateInstance(MessageTypes[id]);
            return null;
        }
    }

    public class MessageProtcoll
    {
        public static int Version = 1;
    }

    public class MessageClass
    {
        public Int32 Name = InvalidMessage;

        public static int InvalidMessage = -1;

        public static int Ping = 10;
        public static int Pong = 20;

        public static int Hail = 100;
        public static int Disconnect = 110;
        public static int WhatTimeIsIt = 180;
        public static int TheTimeIsNow = 185;

        public static int Login = 200;
        public static int LoginAccept = 210;
        public static int InstanceSelect = 220;
        public static int InstanceSelectFailed = 222;
        public static int InstanceSettings = 225;
        public static int SetTeamPreference = 230;

        public static int RequestServerVersInfo = 300;
        public static int ServerVersInfo = 305;
        public static int RequestInstanceList = 350;
        public static int InstanceList = 355;

        public static int PlayerInfo = 400;
        public static int PlayerListDone = 410;

        public static int RequestMapInfo = 500;

        public static int ChatMessage = 600;

        public static int AllowSpawn = 700;
        public static int RequestSpawn = 710;
        public static int PlayerSpawn = 720;

        public static int FileTransfter = 10000;

        static int GetName ( ref NetBuffer  buffer )
        {
            return buffer.ReadInt32();
        }

        public static MessageClass NoDataMessage( Int32 name )
        {
            Temp.Name = name;
            return Temp;
        }

        private static MessageClass Temp = new MessageClass();

        public virtual NetBuffer Pack ()
        {
            NetBuffer buffer = new NetBuffer();
            buffer.Write(Name);
            return buffer;
        }

        public virtual bool Unpack(ref NetBuffer buffer)
        {
            return true;
        }

        public virtual NetChannel Channel ()
        {
            return NetChannel.ReliableInOrder1;
        }

        protected void PackClass (ref NetBuffer buffer, object obj)
        {
            BinaryFormatter formater = new BinaryFormatter();
            MemoryStream stream = new MemoryStream();
            formater.Serialize(stream,obj);

            byte[] b = stream.ToArray();
            stream.Close();
            buffer.Write(b.Length);
            buffer.Write(b);
        }

        protected object UnpackClass ( ref NetBuffer buffer )
        {
            Int32 size = buffer.ReadInt32();
            byte[] b = buffer.ReadBytes(size);

            MemoryStream stream = new MemoryStream(b);

            BinaryFormatter formater = new BinaryFormatter();
            object obj = formater.Deserialize(stream);
            stream.Close();
            return obj;
        }

        protected void PackCompressedClass(ref NetBuffer buffer, object obj)
        {
            BinaryFormatter formater = new BinaryFormatter();
            MemoryStream stream = new MemoryStream();
            GZipStream gstream = new GZipStream(stream, CompressionMode.Compress);
            formater.Serialize(gstream, obj);

            byte[] b = stream.ToArray();
            gstream.Close();
            stream.Close();
            buffer.Write(b.Length);
            buffer.Write(b);
        }

        protected object UnpackCompressedClass(ref NetBuffer buffer)
        {
            Int32 size = buffer.ReadInt32();
            byte[] b = buffer.ReadBytes(size);

            MemoryStream stream = new MemoryStream(b);
            GZipStream gstream = new GZipStream(stream, CompressionMode.Decompress);

            BinaryFormatter formater = new BinaryFormatter();
            object obj = formater.Deserialize(gstream);
            gstream.Close();
            stream.Close();
            return obj;
        }

        protected void PackObjectState (ref NetBuffer buffer, ObjectState state )
        {
            buffer.Write(state.Position.X);
            buffer.Write(state.Position.Y);
            buffer.Write(state.Position.Z);

            buffer.Write(state.Movement.X);
            buffer.Write(state.Movement.Y);
            buffer.Write(state.Movement.Z);

            buffer.Write(state.Rotation);
            buffer.Write(state.Spin);
        }

        protected ObjectState UnpackObjectState(ref NetBuffer buffer)
        {
            ObjectState state = new ObjectState();
            state.Position = new Vector3(buffer.ReadFloat(), buffer.ReadFloat(), buffer.ReadFloat());
            state.Movement = new Vector3(buffer.ReadFloat(), buffer.ReadFloat(), buffer.ReadFloat());
            state.Rotation = buffer.ReadFloat();
            state.Spin = buffer.ReadFloat();

            return state;
        }
    }

    public class Ping : MessageClass
    {
        public UInt64 ID = 0;
        public Ping()
        {
            Name = MessageClass.Ping;
        }

        public override NetBuffer Pack()
        {
            NetBuffer buffer = base.Pack();
            buffer.Write(ID);
            return buffer;
        }

        public override bool Unpack(ref NetBuffer buffer)
        {
            if (!base.Unpack(ref buffer))
                return false;

            ID = buffer.ReadUInt64();
            return true;
        }

        public override NetChannel Channel()
        {
            return NetChannel.Unreliable;
        }
    }

    public class Pong : MessageClass
    {
        public UInt64 ID = 0;
        public Pong()
        {
            Name = MessageClass.Pong;
        }

        public Pong( UInt64 id)
        {
            Name = MessageClass.Pong;
            ID = id;
        }

        public override NetBuffer Pack()
        {
            NetBuffer buffer = base.Pack();
            buffer.Write(ID);
            return buffer;
        }

        public override bool Unpack(ref NetBuffer buffer)
        {
            if (!base.Unpack(ref buffer))
                return false;

            ID = buffer.ReadUInt64();
            return true;
        }

        public override NetChannel Channel()
        {
            return NetChannel.Unreliable;
        }
    }

    public class Disconnect : MessageClass
    {
        public UInt64 ID = 0;

        public Disconnect()
        {
            Name = MessageClass.Disconnect;
        }

        public override NetBuffer Pack()
        {
            NetBuffer buffer = base.Pack();
            buffer.Write(ID);
            return buffer;
        }

        public override bool Unpack(ref NetBuffer buffer)
        {
            if (!base.Unpack(ref buffer))
                return false;

            ID = buffer.ReadUInt64();
            return true;
        }
    }

    public class Login : MessageClass
    {
        public UInt64 UID = 0;
        public UInt64 Token = 0;
        public UInt64 CID = 0;
        public Int32 Version = MessageProtcoll.Version;

        public Int32 Major = 0;
        public Int32 Minor = 0;
        public Int32 Revision = 0;
        public Int32 Bin = 0;

        public string OS = string.Empty;


        public Login()
        {
            Name = MessageClass.Login;
        }

        public override NetBuffer Pack()
        {
            NetBuffer buffer = base.Pack();
            buffer.Write(UID);
            buffer.Write(Token);
            buffer.Write(CID);
            buffer.Write(Version);
            buffer.Write(Major);
            buffer.Write(Minor);
            buffer.Write(Revision);
            buffer.Write(Bin);

            OS = Environment.OSVersion.ToString();

            buffer.Write(OS);
            return buffer;
        }

        public override bool Unpack(ref NetBuffer buffer)
        {
            if (!base.Unpack(ref buffer))
                return false;

            UID = buffer.ReadUInt64();
            Token = buffer.ReadUInt64();
            CID = buffer.ReadUInt64();
            Version = buffer.ReadInt32();
            Major = buffer.ReadInt32();
            Minor = buffer.ReadInt32();
            Revision = buffer.ReadInt32();
            Bin = buffer.ReadInt32();
            OS = buffer.ReadString();
            return true;
        }
    }

    public class LoginAccept : MessageClass
    {
        public UInt64 PlayerID = 0;
        public string Callsign = string.Empty;

        public LoginAccept()
        {
            Name = MessageClass.LoginAccept;
        }

        public override NetBuffer Pack()
        {
            NetBuffer buffer = base.Pack();
            buffer.Write(PlayerID);
            buffer.Write(Callsign);
            return buffer;
        }

        public override bool Unpack(ref NetBuffer buffer)
        {
            if (!base.Unpack(ref buffer))
                return false;

            PlayerID = buffer.ReadUInt64();
            Callsign = buffer.ReadString();
            return true;
        }
    }

    public class ServerVersInfo : MessageClass
    {
        public int Major = 0;
        public int Minor = 0;
        public int Rev = 0;

        public int Protocoll = MessageProtcoll.Version;

        public ServerVersInfo()
        {
            Name = MessageClass.ServerVersInfo;
        }

        public override NetBuffer Pack()
        {
            NetBuffer buffer = base.Pack();
            buffer.Write(Major);
            buffer.Write(Minor);
            buffer.Write(Rev);
            buffer.Write(Protocoll);
            return buffer;
        }

        public override bool Unpack(ref NetBuffer buffer)
        {
            if (!base.Unpack(ref buffer))
                return false;

            Major = buffer.ReadInt32();
            Minor = buffer.ReadInt32();
            Rev = buffer.ReadInt32();
            Protocoll = buffer.ReadInt32();
            return true;
        }
    }

    public class InstanceList : MessageClass
    {
        public class InstanceDescription
        {
            public Int32 ID = -1;
            public string Description = string.Empty;
        }

        public List<InstanceDescription> Instances = new List<InstanceDescription>();

        public InstanceList()
        {
            Name = MessageClass.InstanceList;
        }

        public void Add ( int id, string desc )
        {
            InstanceDescription d = new InstanceDescription();
            d.ID = id;
            d.Description = desc;
            Instances.Add(d);
        }

        public override NetBuffer Pack()
        {
            NetBuffer buffer = base.Pack();
            buffer.Write(Instances.Count);

            foreach(InstanceDescription i in Instances)
            {
                buffer.Write(i.ID);
                buffer.Write(i.Description);
            }
            return buffer;
        }

        public override bool Unpack(ref NetBuffer buffer)
        {
            if (!base.Unpack(ref buffer))
                return false;

            Instances.Clear();
            Int32 count = buffer.ReadInt32();
            for (Int32 i = 0; i < count; i++)
            {
                InstanceDescription desc = new InstanceDescription();
                int id = buffer.ReadInt32();
                string d = buffer.ReadString();
                Add(id, d);
            }
            return true;
        }
    }

    public class InstanceSelect : MessageClass
    {
        public int ID = -1;

        public InstanceSelect()
        {
            Name = MessageClass.InstanceSelect;
        }

        public override NetBuffer Pack()
        {
            NetBuffer buffer = base.Pack();
            buffer.Write(ID);
            return buffer;
        }

        public override bool Unpack(ref NetBuffer buffer)
        {
            if (!base.Unpack(ref buffer))
                return false;

            ID = buffer.ReadInt32();

            return true;
        }
    }

    public class InstanceSettings : MessageClass
    {
        public int ID = -1;
        public SimSettings Settings = new SimSettings();
        public string MapChecksum = string.Empty;
        public string[] TeamNames;

        public InstanceSettings()
        {
            Name = MessageClass.InstanceSettings;
        }

        public override NetBuffer Pack()
        {
            NetBuffer buffer = base.Pack();
            PackClass(ref buffer, Settings);
            buffer.Write(MapChecksum);
            buffer.Write(TeamNames.Length);
            foreach (string s in TeamNames)
                buffer.Write(s);
            return buffer;
        }

        public override bool Unpack(ref NetBuffer buffer)
        {
            if (!base.Unpack(ref buffer))
                return false;

            Settings = (SimSettings)UnpackClass(ref buffer);
            MapChecksum = buffer.ReadString();

            int count = buffer.ReadInt32();
            TeamNames = new string[count];
            for (int i = 0; i < count; i++)
                TeamNames[i] = buffer.ReadString();

            return true;
        }
    }

    public class PlayerInfo : MessageClass
    {
        public UInt64 PlayerID = 0;
        public string Callsign = string.Empty;
        public int Team = -1;
        public Int32 Score = -1;
        public string Avatar = string.Empty;
        public PlayerStatus Status = PlayerStatus.Despawned;

        public PlayerInfo()
        {
            Name = MessageClass.PlayerInfo;
        }

        public PlayerInfo( Player player)
        {
            Name = MessageClass.PlayerInfo;

            PlayerID = player.ID;
            Callsign = player.Callsign;
            Score = player.Score;
            Status = player.Status;
            Avatar = player.Avatar;
            Team = player.Team;
        }

        public override NetBuffer Pack()
        {
            NetBuffer buffer = base.Pack();
            buffer.Write(PlayerID);
            buffer.Write(Callsign);
            buffer.Write(Score);
            buffer.Write(Team);
            buffer.Write(Avatar);
            buffer.Write((byte)Status);
            return buffer;
        }

        public override bool Unpack(ref NetBuffer buffer)
        {
            if (!base.Unpack(ref buffer))
                return false;

            PlayerID = buffer.ReadUInt64();
            Callsign = buffer.ReadString();
            Score = buffer.ReadInt32();
            Team = buffer.ReadInt32();
            Avatar = buffer.ReadString();
            Status = (PlayerStatus)Enum.ToObject(typeof(PlayerStatus), buffer.ReadByte());
            return true;
        }

        public override NetChannel Channel()
        {
            return NetChannel.ReliableInOrder2;
        }
    }

    public class SetTeamPreference : MessageClass
    {
        public int Team = -1;

        public SetTeamPreference()
        {
            Name = MessageClass.SetTeamPreference;
        }

        public override NetBuffer Pack()
        {
            NetBuffer buffer = base.Pack();
            buffer.Write(Team);
            return buffer;
        }

        public override bool Unpack(ref NetBuffer buffer)
        {
            if (!base.Unpack(ref buffer))
                return false;

            Team = buffer.ReadInt32();
            return true;
        }
    }

    public class RequestMapInfo : MessageClass
    {
        public int ID = 0;

        public RequestMapInfo()
        {
            Name = MessageClass.RequestMapInfo;
        }

        public override NetBuffer Pack()
        {
            NetBuffer buffer = base.Pack();
            buffer.Write(ID);
            return buffer;
        }

        public override bool Unpack(ref NetBuffer buffer)
        {
            if (!base.Unpack(ref buffer))
                return false;

            ID = buffer.ReadInt32();
            return true;
        }
    }

    public class FileTransfter : MessageClass
    {
        public FileTransfter()
        {
            Name = MessageClass.FileTransfter;
        }

        public int ID = 0;
        public int Size = 0;
        public int Chunk = 0;
        public int Total = 0;
        public byte[] Data = null;

        public FileTransfter(byte[] b, int count, int i, int id)
        {
            Name = MessageClass.FileTransfter;
            Data = b;
            Size = b.Length;
            Chunk = i;
            Total = count;
            ID = id;
        }

        public override NetBuffer Pack()
        {
            NetBuffer buffer = base.Pack();
            buffer.Write(ID);
            buffer.Write(Chunk);
            buffer.Write(Total);
            buffer.Write(Size);
            buffer.Write(Data);
            return buffer;
        }

        public override bool Unpack(ref NetBuffer buffer)
        {
            if (!base.Unpack(ref buffer))
                return false;

            ID = buffer.ReadInt32();
            Chunk = buffer.ReadInt32();
            Total = buffer.ReadInt32();
            Size = buffer.ReadInt32();
            Data = buffer.ReadBytes(Size);
            return true;
        }
    }

    public class ChatMessage : MessageClass
    {
        public string ChatChannel = string.Empty;
        public string From = string.Empty;
        public string Message = string.Empty;

        public ChatMessage()
        {
            Name = MessageClass.ChatMessage;
        }

        public override NetBuffer Pack()
        {
            NetBuffer buffer = base.Pack();
            buffer.Write(ChatChannel);
            buffer.Write(From);
            buffer.Write(Message);
            return buffer;
        }

        public override bool Unpack(ref NetBuffer buffer)
        {
            if (!base.Unpack(ref buffer))
                return false;

            ChatChannel = buffer.ReadString();
            From = buffer.ReadString();
            Message = buffer.ReadString();
           return true;
        }
    }

    public class PlayerSpawn : MessageClass
    {
        public UInt64 PlayerID = 0;
        public ObjectState PlayerState;
        public double Time = -1;

        public PlayerSpawn()
        {
            Name = MessageClass.PlayerSpawn;
        }

        public PlayerSpawn ( Player player )
        {
            Name = MessageClass.PlayerSpawn;
            PlayerID = player.ID;
            PlayerState = player.LastUpdateState;
            Time = player.LastUpdateTime;  
        }

        public override NetBuffer Pack()
        {
            NetBuffer buffer = base.Pack();
            buffer.Write(PlayerID);
            PackObjectState(ref buffer, PlayerState);
            buffer.Write(Time);
            return buffer;
        }

        public override bool Unpack(ref NetBuffer buffer)
        {
            if (!base.Unpack(ref buffer))
                return false;

            PlayerID = buffer.ReadUInt64();
            PlayerState = UnpackObjectState(ref buffer);
            Time = buffer.ReadDouble();
            return true;
        }
    }

    public class WhatTimeIsIt : MessageClass
    {
        public UInt64 ID = 0;

        public WhatTimeIsIt()
        {
            Name = MessageClass.WhatTimeIsIt;
        }

        public override NetBuffer Pack()
        {
            NetBuffer buffer = base.Pack();
            buffer.Write(ID);
            return buffer;
        }

        public override bool Unpack(ref NetBuffer buffer)
        {
            if (!base.Unpack(ref buffer))
                return false;

            ID = buffer.ReadUInt64();
            return true;
        }

        public override NetChannel Channel()
        {
            return NetChannel.Unreliable;
        }
    }

    public class TheTimeIsNow : MessageClass
    {
        public double Time = -1;
        public UInt64 ID = 0;

        public TheTimeIsNow()
        {
            Name = MessageClass.TheTimeIsNow;
        }

        public override NetBuffer Pack()
        {
            NetBuffer buffer = base.Pack();
            buffer.Write(ID);
            buffer.Write(Time);
            return buffer;
        }

        public override bool Unpack(ref NetBuffer buffer)
        {
            if (!base.Unpack(ref buffer))
                return false;

            ID = buffer.ReadUInt64();
            Time = buffer.ReadDouble();
            return true;
        }

        public override NetChannel Channel()
        {
            return NetChannel.Unreliable;
        }
    }
}

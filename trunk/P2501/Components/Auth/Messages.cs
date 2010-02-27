using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;

namespace Auth
{
    public class RequestAdd : AuthMessage
    {
        public string email = string.Empty;
        public string password = string.Empty;
        public string callsign = string.Empty;

        public RequestAdd()
        {
            Name = AuthMessage.RequestAdd;
        }

        public override NetBuffer Pack()
        {
            NetBuffer buffer = base.Pack();
            buffer.Write(email);
            buffer.Write(password);
            buffer.Write(callsign);
            return buffer;
        }

        public override bool Unpack(ref NetBuffer buffer)
        {
            if (!base.Unpack(ref buffer))
                return false;

            email = buffer.ReadString();
            password = buffer.ReadString();
            callsign = buffer.ReadString();
            return true;
        }
    }

    public class RequestAddCharacter : AuthMessage
    {
        public string callsign = string.Empty;

        public RequestAddCharacter()
        {
            Name = AuthMessage.RequestAddCharacter;
        }

        public override NetBuffer Pack()
        {
            NetBuffer buffer = base.Pack();
            buffer.Write(callsign);
            return buffer;
        }

        public override bool Unpack(ref NetBuffer buffer)
        {
            if (!base.Unpack(ref buffer))
                return false;

            callsign = buffer.ReadString();
            return true;
        }
    }
    public class RequestAuth : AuthMessage
    {
        public string email = string.Empty;
        public string password = string.Empty;

        public RequestAuth()
        {
            Name = AuthMessage.RequestAuth;
        }

        public override NetBuffer Pack()
        {
            NetBuffer buffer = base.Pack();
            buffer.Write(email);
            buffer.Write(password);
            return buffer;
        }

        public override bool Unpack(ref NetBuffer buffer)
        {
            if (!base.Unpack(ref buffer))
                return false;

            email = buffer.ReadString();
            password = buffer.ReadString();
            return true;
        }
    }

    public class AuthOK : AuthMessage
    {
        public UInt64 ID = 0;
        public UInt64 Token = 0;

        public AuthOK()
        {
            Name = AuthMessage.AuthOK;
        }

        public AuthOK( UInt64 id, UInt64 token )
        {
            Name = AuthMessage.AuthOK;
            ID = id;
            Token = token;
        }

        public override NetBuffer Pack()
        {
            NetBuffer buffer = base.Pack();
            buffer.Write(ID);
            buffer.Write(Token);
            return buffer;
        }

        public override bool Unpack(ref NetBuffer buffer)
        {
            if (!base.Unpack(ref buffer))
                return false;

            ID = buffer.ReadUInt64();
            Token = buffer.ReadUInt64();
            return true;
        }
    }

    public class CharacterList : AuthMessage
    {
        public class CharacterInfo
        {
            public string Name = string.Empty;
            public UInt64 CID = 0;

            public CharacterInfo( string n, UInt64 i)
            {
                Name = n;
                CID = i;
            }
        }

        public List<CharacterInfo> Characters = new List<CharacterInfo>();

        public CharacterList()
        {
            Name = AuthMessage.CharacterList;
        }

        public override NetBuffer Pack()
        {
            NetBuffer buffer = base.Pack();

            buffer.Write(Characters.Count);
            foreach (CharacterInfo c in Characters)
            {
                buffer.Write(c.Name);
                buffer.Write(c.CID);
            }
            return buffer;
        }

        public override bool Unpack(ref NetBuffer buffer)
        {
            if (!base.Unpack(ref buffer))
                return false;

            Characters.Clear();
            int count = buffer.ReadInt32();
            for (int i = 0; i < count; i++)
                Characters.Add(new CharacterInfo(buffer.ReadString(),buffer.ReadUInt64()));

            return true;
        }
    }
}

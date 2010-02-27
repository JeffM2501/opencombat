using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.IO;
using System.IO.Compression;

using Lidgren.Network;

namespace Auth
{
    public class AuthMessage
    {
        public static int Hail = 100;
        public static int Error = 0;
        
        public static int RequestAdd = 200;
        public static int AddOK = 210;
        public static int AddBadEmail = 220;
        public static int AddBadCallsign = 230;
        public static int AddBadPass = 240;
        public static int AddBadBan = 250;

        public static int RequestAuth = 300;
        public static int AuthOK = 310;
        public static int AuthBadCred = 320;
        public static int AuthBadBan = 330;

        public static int RequestChangePass = 400;
        public static int PassChangeBadNoAuth = 410;
        public static int PassChangeBadNewPass = 420;
        public static int PassChangeBadBan = 430;

        public static int RequestCharacterList = 500;
        public static int CharacterList = 510;
        public static int CharacterListBadNoAuth = 520;
        public static int CharacterListBadBan = 530;

        public static int RequestAddCharacter = 600;
        public static int CharacterAddOK = 610;
        public static int CharacterAddBadNoAuth = 620;
        public static int CharacterAddBadName = 630;
        public static int CharacterAddBadBan = 640;
        public static int CharacterAddBadTooMany = 650;

        public static int RequestDelCharacter = 700;
        public static int CharacterDelOK = 710;
        public static int CharacterDelBadNoAuth = 720;
        public static int CharacterDelBadBan = 730;
        public static int CharacterDelBadNoChar = 740;

        public static int RequestSelCharacter = 800;
        public static int CharacterSellOK = 810;
        public static int CharacterSellBadNoAuth = 820;
        public static int CharacterSellBadBan = 830;

        public Int32 Name = -1;

        static int GetName ( ref NetBuffer  buffer )
        {
            return buffer.ReadInt32();
        }

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
    }
}

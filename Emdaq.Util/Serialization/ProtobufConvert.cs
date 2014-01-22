using System;
using System.IO;
using ProtoBuf;

namespace Emdaq.Util.Serialization
{
    public class ProtobufConvert
    {
        public static byte[] Serialize(object o)
        {
            using (var ms = new MemoryStream())
            {
                Serializer.Serialize(ms, o);
                ms.Flush();
                ms.Seek(0, SeekOrigin.Begin);
                return ms.ToArray();
            }
        }

        public static T Deserialize<T>(byte[] bytes)
        {
            using (var ms = new MemoryStream(bytes))
            {
                ms.Flush();
                ms.Seek(0, SeekOrigin.Begin);
                return Serializer.Deserialize<T>(ms);
            }
        }

        public static string SerializeToString(object o)
        {
            var bytes = Serialize(o);
            return Convert.ToBase64String(bytes);
        }

        public static T DeserializeFromString<T>(string s)
        {
            var bytes = Convert.FromBase64String(s);
            return Deserialize<T>(bytes);
        }
    }
}

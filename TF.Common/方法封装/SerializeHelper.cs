using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;

namespace TF.Common
{
    public class SerializeHelper
    {

        public static byte[] BinarySerialize(object obj)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(ms, obj);
                ms.Position = 0;
                byte[] bytes = new byte[ms.Length];
                ms.Read(bytes, 0, bytes.Length);
                return bytes;
            }
        }

        public static byte[] XmlSerialize(object obj)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                XmlSerializer formatter = new XmlSerializer(obj.GetType());
                formatter.Serialize(ms, obj);
                ms.Position = 0;
                byte[] bytes = new byte[ms.Length];
                ms.Read(bytes, 0, bytes.Length);
                return bytes;
            }
        }

        public static T BinaryDeserialize<T>(byte[] bytes) where T : class
        {
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                BinaryFormatter xml = new BinaryFormatter();
                return xml.Deserialize(ms) as T;
            }
        }

        public static T XmlDeserialize<T>(byte[] bytes) where T : class
        {
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                XmlSerializer xml = new XmlSerializer(typeof(T));
                return xml.Deserialize(ms) as T;
            }
        }
    }
}
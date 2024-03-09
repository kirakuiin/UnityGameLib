using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace GameLib.Common
{
    /// <summary>
    /// 序列化工具。
    /// </summary>
    public static class SerializeTool
    {
        /// <summary>
        /// 序列化对象。
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static byte[] Serialize<T>(T obj) where T : struct
        {
            var stream = new MemoryStream();
            var formatter = new BinaryFormatter();
            formatter.Serialize(stream, obj);
            return stream.GetBuffer();
        }

        /// <summary>
        /// 反序列化对象。
        /// </summary>
        /// <param name="bytes"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Deserialize<T>(byte[] bytes)
        {
            var stream = new MemoryStream(bytes);
            var formatter = new BinaryFormatter();
            return (T)formatter.Deserialize(stream);
        }
    }
}
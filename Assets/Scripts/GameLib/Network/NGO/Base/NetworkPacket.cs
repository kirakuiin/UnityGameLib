using System.Collections.Generic;
using System.Text;
using Unity.Netcode;
using Newtonsoft.Json;

// ReSharper disable once CheckNamespace
namespace GameLib.Network.NGO
{
    /// <summary>
    /// 网络数据传输包。
    /// </summary>
    /// <remarks>子类必须实现将内部数据打包为二进制流的逻辑。</remarks>
    public abstract class BinaryNetworkPacket : INetworkSerializable
    {
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            if (serializer.IsReader)
            {
                var reader = serializer.GetFastBufferReader();
                reader.ReadValueSafe(out int size);
                var stream = new byte[size];
                reader.ReadBytesSafe(ref stream, size);
                ReadBytes(stream);
            }

            if (serializer.IsWriter)
            {
                var writer = serializer.GetFastBufferWriter();
                var data = WriteBytes();
                writer.WriteValueSafe(data.Length);
                writer.WriteBytesSafe(data);
            }
        }

        protected abstract byte[] WriteBytes();

        protected abstract void ReadBytes(byte[] stream);

        protected JsonBinarySerializer GetJsonWriter()
        {
            return new JsonBinarySerializer();
        }

        protected JsonBinaryDeserializer GetJsonReader(byte[] stream)
        {
            return new JsonBinaryDeserializer(stream);
        }
    }

    /// <summary>
    /// json序列化构造器。
    /// </summary>
    public class JsonBinarySerializer
    {
        private readonly List<string> _buildList = new();

        /// <summary>
        /// 序列化一个新的对象。
        /// </summary>
        /// <param name="obj"></param>
        public void Serialize(object obj)
        {
            _buildList.Add(JsonConvert.SerializeObject(obj));
        }

        /// <summary>
        /// 获得序列化字节流。
        /// </summary>
        /// <returns></returns>
        public byte[] GetBytes()
        {
            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(_buildList));
        }
    }

    /// <summary>
    /// 对Json序列化结果进行反序列化。
    /// </summary>
    public class JsonBinaryDeserializer
    {
        private readonly List<string> _resultList;
        private int _index = 0;
        
        public JsonBinaryDeserializer(byte[] stream)
        {
            var jsonStr = Encoding.UTF8.GetString(stream);
            _resultList = JsonConvert.DeserializeObject<List<string>>(jsonStr);
        }

        private T Current<T>()
        {
            return JsonConvert.DeserializeObject<T>(_resultList[_index-1]);
        }

        private bool MoveNext()
        {
            _index += 1;
            return _index <= _resultList.Count;
        }

        /// <summary>
        /// 在整个流中反序列化流里的第一个对象。每调用一次会推进一次。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Deserialize<T>()
        {
            MoveNext();
            return Current<T>();
        }
        
        public void Deserialize<T>(ref T value)
        {
            MoveNext();
            value = Current<T>();
        }

        public void Reset()
        {
            _index = 0;
        }
    }
}
using System.Collections.Generic;
using GameLib.Network.NGO;
using NUnit.Framework;
using UnityEngine;

namespace Tests.Editor
{
    [TestFixture]
    public class NetworkPacketUt
    {
        [Test]
        public void EqualityTest()
        {
            var data = new TestClass()
            {
                P1 = new Dictionary<int, string>(){{1, "hello"}},
                P2 = new List<float>() {1, 2, 3},
                P3 = false,
                P4 = new TestClass.CustomStruct() {Vec = -1},
            };
            var anoData = new TestClass();
            
            anoData.Read(data.Write());
            
            Assert.AreEqual(data.P1[1], anoData.P1[1]);
            Assert.AreEqual(data.P2[2], anoData.P2[2]);
            Assert.AreEqual(data.P3, anoData.P3);
            Assert.AreEqual(data.P4, anoData.P4);
        }

    }
    
    public class TestClass : BinaryNetworkPacket
    {
        public Dictionary<int, string> P1;

        public List<float> P2;

        public bool P3;

        public CustomStruct P4;

        public byte[] Write()
        {
            return WriteBytes();
        }

        public void Read(byte[] stream)
        {
            ReadBytes(stream);
        }
        
        protected override byte[] WriteBytes()
        {
            var writer = GetJsonWriter();
            writer.Serialize(P1);
            writer.Serialize(P2);
            writer.Serialize(P3);
            writer.Serialize(P4);
            var result = writer.GetBytes();
            return result;
        }

        protected override void ReadBytes(byte[] stream)
        {
            var reader = GetJsonReader(stream);
            P1 = reader.Deserialize<Dictionary<int, string>>();
            P2 = reader.Deserialize<List<float>>();
            P3 = reader.Deserialize<bool>();
            P4 = reader.Deserialize<CustomStruct>();
        }

        public struct CustomStruct
        {
            public int Vec;
        }
    }
}
using System;
using System.Net;
using GameLib.Network;
using NUnit.Framework;
using UnityEngine;

namespace Tests.Editor
{
    [Serializable]
    public struct Message
    {
        public int value;
        public string name;

        public override string ToString()
        {
            return $"{name},{value}";
        }
    }
    
    [TestFixture]
    public class BroadcastUnitTest
    {
        private const ushort Port = 13131;
        private readonly TimedBroadcaster<Message> _sender = new(Port);
        private readonly BroadcastListener<Message> _receiver = new(Port);
        private readonly Message _sentMsg = new () {value = 1, name = "hello"};

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _receiver.StartListen();
            _receiver.OnReceivedBroadcast += OnReceivedBroadcast;
            _sender.StartBroadcast(_sentMsg);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            _receiver.StopListen();
            _sender.StopBroadcast();
            _receiver.Dispose();
            _sender.Dispose();
        }

        void OnReceivedBroadcast(IPAddress addr, Message msg)
        {
            Debug.Log(addr.ToString());
            Debug.Log(msg.ToString());
        }

        [Test]
        [Timeout(100)]
        public void TestIsSending()
        {
            Assert.IsFalse(_sender.IsSending == false);
        }
        
        [Test]
        public void TestIsListening()
        {
            Assert.IsFalse(_receiver.IsListening == false);
        }

        [Test]
        public void TestStopListening()
        {
            _receiver.StopListen();
            
            Assert.IsFalse(_receiver.IsListening);
        }
        
        [Test]
        public void TestStopSending()
        {
            _sender.StopBroadcast();
            
            Assert.IsFalse(_sender.IsSending);
        }

        [Test]
        public void TestReceiverDispose()
        {
            const ushort port = 1241;

            var listener = new BroadcastListener<Message>(port);
            listener.StartListen();
            listener.Dispose();
            listener = new BroadcastListener<Message>(port);
            listener.StartListen();
            listener.Dispose();
        }
        

        [Test]
        public void TestSenderDispose()
        {
            const ushort port = 1245;

            var sender = new TimedBroadcaster<Message>(port);
            sender.StartBroadcast(_sentMsg);
            sender.Dispose();
            sender = new TimedBroadcaster<Message>(port);
            sender.StartBroadcast(_sentMsg);
            sender.Dispose();
        }
    }
}
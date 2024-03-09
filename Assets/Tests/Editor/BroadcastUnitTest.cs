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
        private readonly TimedBroadcaster<Message> _sender = new();
        private readonly BroadcastListener<Message> _receiver = new();
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
            _receiver.StartListen();
            _sender.StopBroadcast();
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
    }
}
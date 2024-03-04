using System.Net;
using GameLib.Network;
using NUnit.Framework;

namespace Tests.Editor
{
    [TestFixture]
    public class BroadcastUnitTest
    {
        private readonly TimedBroadcaster _sender = new TimedBroadcaster();
        private readonly BroadcastListener _receiver = new BroadcastListener();
        private const string Message = "hello world!";

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _receiver.StartListen();
            _receiver.OnReceivedBroadcast += OnReceivedBroadcast;
            _sender.StartBroadcast(Message);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            _receiver.StartListen();
            _sender.StopBroadcast();
        }

        void OnReceivedBroadcast(IPAddress addr, string msg)
        {
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
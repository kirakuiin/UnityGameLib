using System.Net;
using GameLib.Network;
using NUnit.Framework;

namespace UnitTest.Editor
{
    [TestFixture]
    public class BroadcastUnitTest
    {
        private readonly TimedBroadcaster _sender = new TimedBroadcaster();
        private readonly BroadcastListener _receiver = new BroadcastListener();
        private const string Message = "hello world!";

        public BroadcastUnitTest()
        {
            _receiver.StartListen();
            _receiver.OnReceivedBroadcast += OnReceivedBroadcast;
            _sender.StartBroadcast(Message);
        }

        void OnReceivedBroadcast(IPAddress addr, string msg)
        {
        }

        [Test]
        public void TestIsSending()
        {
            if (_sender.IsSending == false)
            {
                throw new UnitTestException("广播未启动！");
            }
        }
        
        [Test]
        public void TestIsListening()
        {
            if (_receiver.IsListening == false)
            {
                throw new UnitTestException("监听未启动！");
            }
        }

        [Test]
        public void TestStopListening()
        {
            _receiver.StopListen();
            
            if (_receiver.IsListening == true)
            {
                throw new UnitTestException("监听未关闭！");
            }
        }
        
        [Test]
        public void TestStopSending()
        {
            _sender.StopBroadcast();
            
            if (_sender.IsSending == true)
            {
                throw new UnitTestException("广播未关闭！");
            }
        }
    }
}
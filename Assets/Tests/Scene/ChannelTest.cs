using System;
using UnityEngine;
using GameLib.Network.NGO.Channel;
using Unity.Collections;
using Unity.Netcode;

namespace Tests.Scene 
{
    /// <summary>
    /// 测试<c>NetworkedMessageChannel</c>
    /// </summary>
    public class ChannelTest : MonoBehaviour
    {
        private NetworkedMessageChannel<TestMessage> _channel;
        private IDisposable _sub;
        private int _index = 0;
        private TestMessage? _recvMessage;
        private string _sendMessage;
        
        void Start()
        {
            _channel = new NetworkedMessageChannel<TestMessage>(NetworkManager.Singleton);
            _sub = _channel.Subscribe(OnReceiveMessage);
        }

        void OnReceiveMessage(TestMessage message)
        {
            _recvMessage = message;
        }

        void OnGUI()
        {
            GUILayout.BeginArea(new Rect(10, 10, 300, 300));
            if (NetworkManager.Singleton.IsServer)
            {
                StartSendMessage();
            }
            ShowMessage();
            GUILayout.EndArea();
        }

        void StartSendMessage()
        {
            _sendMessage = GUILayout.TextField(_sendMessage);
            if (GUILayout.Button("发送"))
            {
                _index += 1;
                _channel.Publish(new TestMessage(){Message = _sendMessage, Value = _index});
            }
        }

        void ShowMessage()
        {
            if (_recvMessage != null)
            {
                GUILayout.Label(_recvMessage.ToString());
            }
        }
    }

    struct TestMessage : INetworkSerializeByMemcpy
    {
        public FixedString32Bytes Message;
        public int Value;

        public override string ToString()
        {
            return $"msg={Message}, val={Value}";
        }
    }
}

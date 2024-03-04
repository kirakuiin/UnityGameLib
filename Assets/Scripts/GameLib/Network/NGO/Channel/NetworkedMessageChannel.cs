using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace GameLib.Network.NGO.Channel
{
    /// <summary>
    /// 网络信道使得发布的消息可以同时被本地和客户端收到。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class NetworkedMessageChannel<T> : MessageChannel<T> where T : unmanaged, INetworkSerializeByMemcpy
    {
        private readonly string _channelName = $"{nameof(T)}Channel";

        private readonly NetworkManager _networkManager;

        public NetworkedMessageChannel(NetworkManager manager)
        {
            _networkManager = manager;
            InitRegister();
        }

        private void InitRegister()
        {
            _networkManager.OnClientConnectedCallback += OnClientConnected;
            if (_networkManager.IsListening)
            {
                RegisterHandler();
            }
        }

        private void OnClientConnected(ulong obj)
        {
            RegisterHandler();
        }

        private void RegisterHandler()
        {
            if (!_networkManager.IsServer)
            {
                _networkManager.CustomMessagingManager.RegisterNamedMessageHandler(_channelName, ReceiveMessageThroughNetwork);
            }
        }

        private void ReceiveMessageThroughNetwork(ulong clientID, FastBufferReader reader)
        {
            reader.ReadValueSafe(out T message);
            base.Publish(message);
        }

        public override void Publish(T message)
        {
            if (_networkManager.IsServer)
            {
                SendMessageThroughNetwork(message);
                base.Publish(message);
            }
            else
            {
                Debug.LogError("仅服务端可以发布消息");
            }
        }

        private void SendMessageThroughNetwork(T message)
        {
            if (!IsNetworkWorking()) return;

            var writer = new FastBufferWriter(FastBufferWriter.GetWriteSize<T>(), Allocator.Temp);
            writer.WriteValueSafe(message);
            _networkManager.CustomMessagingManager.SendNamedMessageToAll(_channelName, writer);
        }

        private bool IsNetworkWorking()
        {
            return _networkManager != null && _networkManager.CustomMessagingManager != null;
        }

        protected override void Dispose(bool isDisposing)
        {
            if (IsDisposed) return;
            if (!IsNetworkWorking()) return;
            
            _networkManager.CustomMessagingManager.UnregisterNamedMessageHandler(_channelName);
            _networkManager.OnClientConnectedCallback -= OnClientConnected;
            
            base.Dispose(isDisposing);
        }
    }
}
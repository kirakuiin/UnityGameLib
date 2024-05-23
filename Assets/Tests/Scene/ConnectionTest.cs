using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameLib.Common.Extension;
using GameLib.Common.Pattern;
using GameLib.Network;
using UnityEngine;
using GameLib.Network.NGO.Channel;
using GameLib.Network.NGO.ConnectionManagement;
using Unity.Netcode;

namespace Tests.Scene
{
    public class ConnectionTest : MonoBehaviour
    {
        [SerializeField] private string ipAddr;

        [SerializeField] private ushort port;

        private IDisposable _handler;

        private bool _recvRequest;

        private CustomHosting _host;
        
        private void Start()
        {
            InitService();
            InitConnectionState();
        }

        void InitService()
        {
            ServiceLocator.Instance.Register<IPublisher<ConnectInfo>>(new MessageChannel<ConnectInfo>());
            _handler = ServiceLocator.Instance.Get<ISubscriber<ConnectInfo>>().Subscribe(OnStatusChange);
        }

        void OnStatusChange(ConnectInfo status)
        {
            Debug.Log($"网络状态为：{status}");
        }
        
        void InitConnectionState()
        {
            var connectMethod = new DirectIPConnectionMethod(Address.GetIPEndPoint(ipAddr, port));
            _host = new CustomHosting(OnConnect);
            ConnectionManager.Instance.AddState(new OfflineState());
            ConnectionManager.Instance.AddState(new StartHostingState(connectMethod));
            ConnectionManager.Instance.AddState<HostingState>(_host);
            ConnectionManager.Instance.AddState(new ClientConnectingState(connectMethod));
            ConnectionManager.Instance.AddState(new ClientConnectedState());
            ConnectionManager.Instance.AddState(new ClientReconnectingState(connectMethod));
            Debug.Log(ConnectionManager.Instance.GetStatesByInterface<IConnectionResettable>().Count());
        }

        private void OnConnect()
        {
            _recvRequest = true;
        }

        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(10, 10, 300, 300));
            if (!NetworkManager.Singleton.IsListening)
            {
                ShowSelection();
            }
            else
            {
                if (NetworkManager.Singleton.IsServer)
                {
                    ShowServerOption();
                }
                else
                {
                    ShowClientOption();
                }
            }
            GUILayout.EndArea();
        }

        void ShowSelection()
        {
            if (GUILayout.Button("创建主机"))
            {
                ConnectionManager.Instance.StartHost();
            }
            if (GUILayout.Button("加入服务端"))
            {
                ConnectionManager.Instance.StartClient();
            }
        }

        void ShowServerOption()
        {
            if (GUILayout.Button("关闭主机"))
            {
                ConnectionManager.Instance.UserRequestShutdown();
            }
            if (GUILayout.Button("移动位置"))
            {
                RandomPos(NetworkManager.Singleton.LocalClient.PlayerObject);
            }

            if (_recvRequest)
            {
                if (GUILayout.Button("允许加入"))
                {
                    _host.SetAllowConnection(true);
                    _recvRequest = false;
                }

                if (GUILayout.Button("不允许加入"))
                {
                    _host.SetAllowConnection(false);
                    _recvRequest = false;
                }
            }
        }

        void ShowClientOption()
        {
            if (GUILayout.Button("断开连接"))
            {
                ConnectionManager.Instance.UserRequestShutdown();
            }
            if (GUILayout.Button("移动位置"))
            {
                RandomPos(NetworkManager.Singleton.LocalClient.PlayerObject);
            }
        }

        void RandomPos(NetworkObject obj)
        {
            var posList = new List<int>() {-2, -1, 0, 1, 2};
            var random = new System.Random();
            obj.gameObject.transform.position = new Vector3(random.Choice(posList), random.Choice(posList));
        }
            
    }

    public class CustomHosting : HostingState
    {
        private readonly Action _callback;

        private bool _isSet;

        private bool _isAllow;
        
        public CustomHosting(Action callback)
        {
            _callback = callback;
        }
        
        protected override async void SetResponse(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
        {
            response.Pending = true;
            _isSet = false;
            _callback();
            await WaitForDecision(response);
        }

        public void SetAllowConnection(bool isAllow)
        {
            _isAllow = isAllow;
            _isSet = true;
        }

        private async Task WaitForDecision(NetworkManager.ConnectionApprovalResponse response)
        {
            await TaskExtension.Wait(() => _isSet);
            if (_isAllow)
            {
                response.Approved = true;
                response.CreatePlayerObject = true;
            }
            else
            {
                response.Approved = false;
                response.Reason = JsonUtility.ToJson(ConnectInfo.Create(ConnectStatus.UserDefined, "大咩"));
            }

            response.Pending = false;
        }
    }
}
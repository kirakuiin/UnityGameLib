using System;
using System.Collections.Generic;
using System.Net;
using GameLib.Common;
using GameLib.Common.Extension;
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
        
        private void Start()
        {
            InitService();
            InitConnectionState();
        }

        void InitService()
        {
            ServiceLocator.Instance.Register<IPublisher<ConnectStatus>>(new MessageChannel<ConnectStatus>());
            _handler = ServiceLocator.Instance.Get<ISubscriber<ConnectStatus>>().Subscribe(OnStatusChange);
        }

        void OnStatusChange(ConnectStatus status)
        {
            Debug.Log($"网络状态为：{status}");
        }
        
        void InitConnectionState()
        {
            var connectMethod = new DirectIPConnectionMethod(Address.GetIPEndPoint(ipAddr, port));
            ConnectionManager.Instance.AddState(new OfflineState());
            ConnectionManager.Instance.AddState(new StartHostingState(connectMethod));
            ConnectionManager.Instance.AddState(new HostingState());
            ConnectionManager.Instance.AddState(new ClientConnectingState(connectMethod));
            ConnectionManager.Instance.AddState(new ClientConnectedState());
            ConnectionManager.Instance.AddState(new ClientReconnectingState(connectMethod));
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
            var endPoint = Address.GetIPEndPoint(ipAddr, port);
            if (GUILayout.Button("创建主机"))
            {
                ConnectionManager.Instance.StartHost(endPoint);
            }
            if (GUILayout.Button("加入服务端"))
            {
                ConnectionManager.Instance.StartClient(endPoint);
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
}
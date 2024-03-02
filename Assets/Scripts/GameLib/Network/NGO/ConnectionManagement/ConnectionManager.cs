using System;
using System.Collections.Generic;
using System.Net;
using GameLib.Common;
using Unity.Netcode;
using UnityEngine;

namespace GameLib.Network.NGO.ConnectionManagement
{
    [Serializable]
    public struct ConnectionConfig
    {
        /// <summary>
        /// 最大同时连接玩家数量。
        /// </summary>
        [Tooltip("最大同时连接玩家数")]
        public int maxConnectedPlayerNum;

        /// <summary>
        /// 重连尝试次数。
        /// </summary>
        [Tooltip("重连尝试次数")]
        public int reconnectAttemptNum;
    }
    
    public class ConnectionManager : PersistentMonoSingleton<ConnectionManager>
    {
        /// <summary>
        /// 连接设置数据。
        /// </summary>
        [SerializeField]
        public ConnectionConfig config;
        
        private ConnectionState _currentState;

        private Dictionary<string, ConnectionState> _statesInfo;

        private void Start()
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;
            NetworkManager.Singleton.OnServerStarted += OnServerStarted;
            NetworkManager.Singleton.OnServerStopped += OnServerStopped;
            NetworkManager.Singleton.ConnectionApprovalCallback += OnApproveCheck;
            NetworkManager.Singleton.OnTransportFailure += OnTransportFailure;
        }

        private void OnClientConnected(ulong clientID)
        {
            _currentState.OnClientConnected(clientID);
        }

        private void OnClientDisconnect(ulong clientID)
        {
            _currentState.OnClientDisconnected(clientID);
        }

        private void OnServerStarted()
        {
            _currentState.OnServerStarted();
        }

        private void OnServerStopped(bool isHost)
        {
            _currentState.OnServerStopped();
        }

        private void OnApproveCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
        {
            _currentState.ApprovalCheck(request, response);
        }

        private void OnTransportFailure()
        {
            _currentState.OnTransportFailure();
        }


        private void OnDestroy()
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnect;
            NetworkManager.Singleton.OnServerStarted -= OnServerStarted;
            NetworkManager.Singleton.OnServerStopped -= OnServerStopped;
            NetworkManager.Singleton.ConnectionApprovalCallback -= OnApproveCheck;
            NetworkManager.Singleton.OnTransportFailure -= OnTransportFailure;
        }

        /// <summary>
        /// 新增一个状态(用以被转换)。
        /// </summary>
        /// <remarks>其他接口必须要状态添加完毕之后再进行调用。</remarks>
        /// <param name="state">状态对象</param>
        /// <typeparam name="T">状态的类型</typeparam>
        public void AddState<T>(T state) where T : ConnectionState
        {
            _statesInfo[state.GetStateType()] = state;
            if (state is OfflineState)
            {
                _currentState = state;
            }
        }

        /// <summary>
        /// 转换至下一个状态
        /// </summary>
        /// <typeparam name="T">状态的类型</typeparam>
        public void ChangeState<T>()
        {
            if (_currentState.GetStateType() == GetTypeName<T>()) return;
            
            Debug.Log($"From {nameof(_currentState)} to {GetTypeName<T>()}");
            
            _currentState?.Exit();
            _currentState = GetState<T>();
            _currentState.Enter();
        }
        
        private string GetTypeName<T>()
        {
            return typeof(T).Name;
        }

        private ConnectionState GetState<T>()
        {
            var key = GetTypeName<T>();
            if (_statesInfo.TryGetValue(key, out var state))
            {
                return state;
            }
            throw new NotExistConnectionStateException(key);
        }

        /// <summary>
        /// 玩家主动要求关闭连接时调用。
        /// </summary>
        public void UserRequestShutdown()
        {
            _currentState.OnUserRequestShutdown();
        }

        /// <summary>
        /// 启动主机。
        /// </summary>
        /// <param name="address">服务端IP地址</param>
        /// <param name="port">服务端端口号</param>
        public void StartHost(IPAddress address, ushort port)
        {
            _currentState.StartHost();
        }

        /// <summary>
        /// 启动客户端。
        /// </summary>
        /// <param name="address">服务端IP地址</param>
        /// <param name="port">服务端端口号</param>
        public void StartClient(IPAddress address, ushort port)
        {
            _currentState.StartClient(address, port);
        }
    }
}
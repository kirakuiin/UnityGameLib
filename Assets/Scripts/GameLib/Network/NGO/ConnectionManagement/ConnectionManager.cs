using System;
using System.Collections.Generic;
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

        private readonly Dictionary<string, ConnectionState> _statesInfo = new();

        private void Start()
        {
            NetworkManager.Singleton.OnConnectionEvent += OnConnectionEvent;
            NetworkManager.Singleton.OnServerStarted += OnServerStarted;
            NetworkManager.Singleton.OnServerStopped += OnServerStopped;
            NetworkManager.Singleton.ConnectionApprovalCallback += OnApproveCheck;
            NetworkManager.Singleton.OnTransportFailure += OnTransportFailure;
        }

        private void OnConnectionEvent(NetworkManager manager, ConnectionEventData e)
        {
            var clientID = e.ClientId;
            switch (e.EventType)
            {
                case ConnectionEvent.ClientConnected:
                    Debug.Log($"客户端{clientID}连接。");
                    _currentState.OnClientConnected(clientID);
                    break;
                case ConnectionEvent.ClientDisconnected:
                    Debug.Log($"客户端{clientID}断开连接。");
                    _currentState.OnClientDisconnected(clientID);
                    break;
            }
        }


        private void OnServerStarted()
        {
            Debug.Log("服务端启动。");
            _currentState.OnServerStarted();
        }

        private void OnServerStopped(bool isHost)
        {
            Debug.Log($"{(isHost ? "主机端" : "服务端")}关闭。");
            _currentState.OnServerStopped();
        }

        private void OnApproveCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
        {
            Debug.Log($"客户端{request.ClientNetworkId}进行认证。");
            _currentState.ApprovalCheck(request, response);
        }

        private void OnTransportFailure()
        {
            Debug.LogWarning($"传输失败。");
            _currentState.OnTransportFailure();
        }

        protected override void OnDestroy()
        {
            if (NetworkManager.Singleton != null)
            {
                NetworkManager.Singleton.OnConnectionEvent -= OnConnectionEvent;
                NetworkManager.Singleton.OnServerStarted -= OnServerStarted;
                NetworkManager.Singleton.OnServerStopped -= OnServerStopped;
                NetworkManager.Singleton.ConnectionApprovalCallback -= OnApproveCheck;
                NetworkManager.Singleton.OnTransportFailure -= OnTransportFailure;
            }
            base.OnDestroy();
        }

        /// <summary>
        /// 新增一个状态(用以被转换)。
        /// </summary>
        /// <remarks>1. 如果T和实例类型不一致，调用时最好显式设置<c>T</c>来指定代表的状态类型。</remarks>
        /// <remarks>2. 其他接口必须要状态添加完毕之后再进行调用。</remarks>
        /// <param name="state">状态对象</param>
        /// <typeparam name="T">状态的类型</typeparam>
        public void AddState<T>(T state) where T : ConnectionState
        {
            _statesInfo[GetTypeName<T>()] = state;
            if (state is OfflineState)
            {
                _currentState = state;
            }
        }
        
        private string GetTypeName<T>()
        {
            return typeof(T).Name;
        }

        /// <summary>
        /// 转换至下一个状态
        /// </summary>
        /// <typeparam name="T">状态的类型</typeparam>
        public void ChangeState<T>()
        {
            Debug.Log($"状态转换：{_currentState?.GetType().Name} => {typeof(T).Name}");
            if (_currentState != null && _currentState.Equals(GetState<T>())) return;
            
            _currentState?.Exit();
            _currentState = GetState<T>();
            _currentState.Enter();
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
        /// 获得实现了指定接口地状态。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IEnumerable<T> GetStatesByInterface<T>()
        {
            foreach (var state in _statesInfo.Values)
            {
                if (state is T tState)
                {
                    yield return tState;
                }
            }
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
        public void StartHost()
        {
            _currentState.StartHost();
        }

        /// <summary>
        /// 启动客户端。
        /// </summary>
        public void StartClient()
        {
            _currentState.StartClient();
        }
    }
}
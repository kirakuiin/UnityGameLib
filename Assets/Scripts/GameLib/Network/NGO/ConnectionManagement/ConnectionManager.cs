using System;
using System.Collections.Generic;
using GameLib.Common;
using UnityEngine;

namespace GameLib.Network.NGO.ConnectionManagement
{
    [Serializable]
    public struct ConnectionConfig
    {
        /// <summary>
        /// 最大同时连接玩家数量。
        /// </summary>
        public int maxConnectedPlayerNum;

        /// <summary>
        /// 重连尝试次数。
        /// </summary>
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

        /// <summary>
        /// 新增一个状态(用以被转换)。
        /// </summary>
        /// <param name="state">状态对象</param>
        /// <typeparam name="T">状态的类型</typeparam>
        public void AddState<T>(T state) where T : ConnectionState
        {
            _statesInfo[state.GetStateType()] = state;
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
    }
}
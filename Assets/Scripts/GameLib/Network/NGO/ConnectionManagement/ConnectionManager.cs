using System.Collections.Generic;
using GameLib.Common;
using UnityEngine;

namespace GameLib.Network.NGO.ConnectionManagement
{
    public class ConnectionManager : PersistentMonoSingleton<ConnectionManager>
    {
        private ConnectionState _currentState;

        private Dictionary<string, ConnectionState> _statesInfo;

        /// <summary>
        /// 新增一个状态(用以被转换)。
        /// </summary>
        /// <param name="state">状态对象</param>
        /// <typeparam name="T">状态的类型</typeparam>
        public void AddState<T>(T state) where T : ConnectionState
        {
            _statesInfo[GetTypeName<T>()] = state;
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
            Debug.Log($"From {nameof(_currentState)} to {GetTypeName<T>()}");
            
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
    }

    /// <summary>
    /// 表示某个连接状态不存在的异常。
    /// </summary>
    internal class NotExistConnectionStateException : LibException
    {
        public NotExistConnectionStateException(string stateType) : base($"State: {stateType} not exist!")
        {
        }
    }
}
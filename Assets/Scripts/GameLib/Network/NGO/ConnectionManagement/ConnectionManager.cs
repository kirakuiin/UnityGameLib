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
            _statesInfo[state.GetStateType()] = state;
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
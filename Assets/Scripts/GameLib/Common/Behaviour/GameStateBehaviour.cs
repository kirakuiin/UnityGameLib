using System;
using UnityEngine;

namespace GameLib.Common.Behaviour
{
    /// <summary>
    /// 游戏状态的基类，代表游戏当前所处的模式。
    /// </summary>
    /// <remarks>同一时间只有一个状态。</remarks>
    /// <typeparam name="T">游戏状态的枚举量</typeparam>
    public abstract class GameStateBehaviour<T> : MonoBehaviour where T : Enum
    {
        /// <summary>
        /// 是否可以持续存在。
        /// </summary>
        private bool IsPersist { get; } = false;

        /// <summary>
        /// 游戏当前的状态。
        /// </summary>
        public abstract T State { get; }

        private static GameObject _activeStateGameObject;

        protected void Start()
        {
            if (CleanPrevState())
            {
                SetNewState();
            }
        }

        private bool CleanPrevState()
        {
            if (_activeStateGameObject == null) return true;
            if (_activeStateGameObject == gameObject) return false;

            var previousState = _activeStateGameObject.GetComponent<GameStateBehaviour<T>>();
            if (previousState.IsPersist && previousState.State.Equals(State))
            {
                Destroy(gameObject);
                return false;
            }
            
            Destroy(_activeStateGameObject);
            return true;
        }

        private void SetNewState()
        {
            _activeStateGameObject = gameObject;
            if (IsPersist)
            {
                DontDestroyOnLoad(gameObject);
            }
        }
    }
}
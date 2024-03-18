using GameLib.Common;
using Unity.Netcode;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace GameLib.Network.NGO
{
    /// <summary>
    /// 继承于<c>NetworkBehaviour</c>的单例模式。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class NetworkSingleton<T> : NetworkBehaviour, ISingleton where T : NetworkSingleton<T>
    {
        private static T _instance;

        private SingletonInitializationStatus _status = SingletonInitializationStatus.Uninitialize;

        /// <summary>
        /// 返回单例实例
        /// </summary>
        /// <value><c>T</c>.</value>
        public static T Instance => _instance;

        /// <summary>
        /// 判断单例是否已经初始化
        /// </summary>
        /// <returns>如果初始化完毕的话返回<c>true</c></returns>
        public virtual bool IsInitialized => _status == SingletonInitializationStatus.Initialized;

        protected virtual void Awake()
        {
            if (_instance is null)
            {
                _instance = this as T;
                Initialize();
            }
            else
            {
                if (Application.isPlaying)
                {
                    Destroy(gameObject);
                }
                else
                {
                    DestroyImmediate(gameObject);
                }
            }
        }
        
        public virtual void Initialize()
        {
            if (_status != SingletonInitializationStatus.Uninitialize) return;

            _status = SingletonInitializationStatus.Initializing;
            OnInitializing();
            _status = SingletonInitializationStatus.Initialized;
            OnInitialized();
        }

        /// <summary>
        /// 初始化过程中调用
        /// </summary>
        protected virtual void OnInitializing()
        {
        }
        
        /// <summary>
        /// 初始化完毕调用
        /// </summary>
        protected virtual void OnInitialized()
        {
        }
        
        public virtual void Clear()
        {
        }

        /// <summary>
        /// 销毁对象。
        /// </summary>
        public static void Destroy()
        {
            if (_instance is null) return;
            
            _instance.Clear();
            _instance = default;
        }

        public override void OnNetworkDespawn()
        {
            Destroy();
        }
    }
}
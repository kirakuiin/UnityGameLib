using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameLib.Network.NGO
{
    /// <summary>
    /// 通过封装API来加载场景，使用<see cref="SceneManager"/>来加载场景。
    /// 或者是使用<see cref="NetworkManager.SceneManager"/>来加载场景。
    /// </summary>
    public class SceneLoader : NetworkSingleton<SceneLoader>
    {
        /// <summary>
        /// 场景加载时触发。
        /// </summary>
        public event Action<Scene, LoadSceneMode> OnSceneLoaded;

        /// <summary>
        /// 触发场景事件。
        /// </summary>
        public event Action<SceneEvent> OnSceneEvent;

        private bool _isInitialized;
        
        private bool IsNetworkSceneManagementEnable =>
            (NetworkManager != null
             && NetworkManager.SceneManager != null
             && NetworkManager.NetworkConfig.EnableSceneManagement);

        /// <summary>
        /// 通过网络场景管理器来异步地加载场景。
        /// </summary>
        /// <param name="sceneName">场景名称</param>
        /// <param name="mode">加载模式</param>
        public void LoadSceneByNet(string sceneName, LoadSceneMode mode = LoadSceneMode.Single)
        {
            if (IsSpawned && IsNetworkSceneManagementEnable && !NetworkManager.ShutdownInProgress)
            {
                if (NetworkManager.IsServer)
                {
                    NetworkManager.SceneManager.LoadScene(sceneName, mode);
                }
            }
        }

        /// <summary>
        /// 通过Unity的场景管理器异步地加载场景。
        /// </summary>
        /// <param name="sceneName">场景名称</param>
        /// <param name="mode">加载模式</param>
        /// <returns>任务状态</returns>
        public AsyncOperation LoadScene(string sceneName, LoadSceneMode mode = LoadSceneMode.Single)
        {
            return SceneManager.LoadSceneAsync(sceneName, mode);
        }

        public virtual void Start()
        {
            SceneManager.sceneLoaded += OnSceneManagerSceneLoaded;
            NetworkManager.OnServerStarted += OnNetworkSessionStarted;
            NetworkManager.OnClientStarted += OnNetworkSessionStarted;
            NetworkManager.OnServerStopped += OnNetworkSessionEnded;
            NetworkManager.OnClientStopped += OnNetworkSessionEnded;
        }
        
        private void OnSceneManagerSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            OnSceneLoaded?.Invoke(scene, mode);
        }

        private void OnNetworkSessionStarted()
        {
            if (!_isInitialized && IsNetworkSceneManagementEnable)
            {
                NetworkManager.SceneManager.OnSceneEvent += OnSceneManagerSceneEvent;
            }

            _isInitialized = true;
        }

        private void OnSceneManagerSceneEvent(SceneEvent sceneEvent)
        {
            switch (sceneEvent.SceneEventType)
            {
                // 服务端通知客户端开始加载场景，仅主机端和客户端执行。
                case SceneEventType.Load:
                    break;
                // 服务端通知客户端所有客户端加载完毕，仅在主机端和客户端执行。
                case SceneEventType.LoadComplete:
                    break;
                // 服务端通知客户端开始同步场景，仅在客户端执行。
                case SceneEventType.Synchronize:
                    Synchronize();
                    break;
                // 客户端通知服务端同步完毕，仅在服务端执行。
                case SceneEventType.SynchronizeComplete:
                    break;
            }
            OnSceneEvent?.Invoke(sceneEvent);
        }
        
        private void Synchronize()
        {
            // 处在这种情况下时，NGO不会主动卸载额外场景来保证和服务器同步，所以手动卸载。
            if (IsPureClient() && NetworkManager.SceneManager.ClientSynchronizationMode == LoadSceneMode.Single)
            {
                UnloadAddictiveScenes();
            }
        }

        private bool IsPureClient()
        {
            return NetworkManager.IsClient && !NetworkManager.IsHost;
        }

        private void UnloadAddictiveScenes()
        {
            var activeScene = SceneManager.GetActiveScene();
            for (var i = 0; i < SceneManager.sceneCount; ++i)
            {
                var scene = SceneManager.GetSceneAt(i);
                if (scene.isLoaded && scene != activeScene)
                {
                    SceneManager.UnloadSceneAsync(scene);
                }
            }
        }

        private void OnNetworkSessionEnded(bool isHost)
        {
            if (_isInitialized && IsNetworkSceneManagementEnable)
            {
                NetworkManager.SceneManager.OnSceneEvent -= OnSceneManagerSceneEvent;
            }

            _isInitialized = false;
        }

        public override void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneManagerSceneLoaded;
            if (NetworkManager != null)
            {
                NetworkManager.OnServerStarted -= OnNetworkSessionStarted;
                NetworkManager.OnClientStarted -= OnNetworkSessionStarted;
                NetworkManager.OnServerStopped -= OnNetworkSessionEnded;
                NetworkManager.OnClientStopped -= OnNetworkSessionEnded;
            }
            base.OnDestroy();
        }

        protected override void OnInitializing()
        {
            base.OnInitializing();
            if (Application.isPlaying)
            {
                DontDestroyOnLoad(gameObject);
            }
        }
    }
}
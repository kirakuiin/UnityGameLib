using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace GameLib.Network.NGO
{

    /// <summary>
    /// 网络事件同步管理器。
    /// 当每个客户端都发出过同步请求之后，视为整个事件同步完毕。
    /// </summary>
    public class NetworkSyncManager : NetworkSingleton<NetworkSyncManager>
    {
        struct EventInfo
        {
            public string EventKey;
            public Action OnDone;
        }

        private readonly Queue<EventInfo> _pendingEvents = new ();
        private readonly Dictionary<string, Action> _callbackContainer = new();
        private readonly Dictionary<string, bool> _syncResult = new();
        
        // 服务器专用。
        private readonly Dictionary<string, HashSet<ulong>> _eventCollections = new();
        
        /// <summary>
        /// 添加一个带完毕回调的同步事件。
        /// </summary>
        /// <param name="e"></param>
        /// <param name="onDone"></param>
        /// <typeparam name="T"></typeparam>
        public void AddSyncEvent<T>(T e, Action onDone=default) where T : Enum
        {
            Debug.Log($"添加网络同步事件{e}");
            _pendingEvents.Enqueue(new EventInfo() {EventKey = EventToStr(e), OnDone = onDone});
        }
        
        private string EventToStr<T>(T e) where T : Enum
        {
            return $"{typeof(T).FullName}|{e.GetHashCode()}";
        }

        /// <summary>
        /// 查询某个事件是否同步完毕。
        /// </summary>
        /// <param name="e"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public bool HasBeenSyncDone<T>(T e) where T : Enum
        {
            var eventKey = EventToStr(e);
            return _syncResult.ContainsKey(eventKey) && _syncResult[eventKey];
        }

        /// <summary>
        /// 重置全部事件(仅服务器)。
        /// </summary>
        public void ResetAll()
        {
            if (!IsServer) return;
            Debug.Log($"重置全部网络同步事件");
            _eventCollections.Clear();
            ResetAllClientRpc();
        }

        [Rpc(SendTo.ClientsAndHost)]
        private void ResetAllClientRpc()
        {
            _pendingEvents.Clear();
            _callbackContainer.Clear();
            _syncResult.Clear();
        }

        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                NetworkManager.OnClientDisconnectCallback += RecheckEvent;
            }
        }

        private void RecheckEvent(ulong clientID)
        {
            Debug.Log("触发重新检查");
            var curConnectedIds = new List<ulong>(NetworkManager.ConnectedClientsIds);
            curConnectedIds.Remove(clientID);
            foreach (var eventKey in _eventCollections.Keys)
            {
                CheckSync(eventKey, curConnectedIds);
            }
        }

        public override void OnNetworkDespawn()
        {
            if (NetworkManager && IsServer)
            {
                NetworkManager.OnClientDisconnectCallback -= RecheckEvent;
            }
        }

        private void Update()
        {
            while (_pendingEvents.Count > 0 && IsSpawned)
            {
                var eventInfo = _pendingEvents.Dequeue();
                _callbackContainer[eventInfo.EventKey] = eventInfo.OnDone;
                _syncResult.TryAdd(eventInfo.EventKey, false);
                SyncEventServerRpc(eventInfo.EventKey);
            }
        }

        [Rpc(SendTo.Server)]
        private void SyncEventServerRpc(string eventKey, RpcParams rpcParams=default)
        {
            UpdateEvent(eventKey, rpcParams.Receive.SenderClientId);
        }

        private void UpdateEvent(string eventKey, ulong clientID)
        {
            _eventCollections.TryAdd(eventKey, new HashSet<ulong>());
            _eventCollections[eventKey].Add(clientID);
            CheckSync(eventKey, NetworkManager.ConnectedClientsIds);
        }

        private void CheckSync(string eventKey, IEnumerable<ulong> curConnectedIds)
        {
            if (_eventCollections[eventKey].IsSupersetOf(curConnectedIds))
            {
                SyncCompleteClientRpc(eventKey);
            }
        }

        [Rpc(SendTo.ClientsAndHost)]
        private void SyncCompleteClientRpc(string eventKey)
        {
            Debug.Log($"网络同步事件{eventKey}完毕");
            _syncResult[eventKey] = true;
            _callbackContainer[eventKey]?.Invoke();
            _callbackContainer[eventKey] = null;
        }
    }

}
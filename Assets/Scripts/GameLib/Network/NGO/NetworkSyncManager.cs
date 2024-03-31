using System;
using System.Collections.Generic;
using System.Linq;
using GameLib.Common.DataStructure;
using Unity.Netcode;
using UnityEngine;

namespace GameLib.Network.NGO
{
    public struct SyncEvent : INetworkSerializeByMemcpy
    {
        /// <summary>
        /// 事件ID代表一个唯一事件。
        /// </summary>
        public int EventID;

        /// <summary>
        /// 通过枚举的形式创建事件。
        /// </summary>
        /// <param name="val"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static SyncEvent Create<T>(T val) where T : Enum
        {
            return new SyncEvent() { EventID = val.GetHashCode() };
        }

        public override string ToString()
        {
            return $"SyncEvent{EventID}";
        }
    }
    
    /// <summary>
    /// 用来同步网络进度的管理器。
    /// </summary>
    public class NetworkSyncManager : NetworkSingleton<NetworkSyncManager>
    {
        private readonly Dictionary<SyncEvent, Counter<ulong>> _eventCounter = new();

        private readonly Dictionary<SyncEvent, Action<SyncEvent>> _eventAction = new();
        
        /// <summary>
        /// 发起一个网络间进度同步事件。
        /// </summary>
        /// <param name="syncEvent"></param>
        /// <param name="onSyncDone"></param>
        public void AddSyncEvent(SyncEvent syncEvent, Action<SyncEvent> onSyncDone=default)
        {
            if (!NetworkManager.IsServer || !IsSpawned) return;
            Debug.Log($"添加网络同步事件{syncEvent}");

            _eventCounter[syncEvent] = new Counter<ulong>();
            _eventAction[syncEvent] = onSyncDone;
        }

        /// <summary>
        /// 客户端通报客户端同步成功。
        /// </summary>
        /// <param name="syncEvent"></param>
        public void SyncDone(SyncEvent syncEvent)
        {
            if (!NetworkManager.IsClient || !IsSpawned) return;
            Debug.Log($"客户端{NetworkManager.LocalClientId}发起{syncEvent}同步。");
            SyncDoneServerRpc(syncEvent);
        }

        [ServerRpc(RequireOwnership = false)]
        private void SyncDoneServerRpc(SyncEvent syncEvent, ServerRpcParams param=default)
        {
            var clientID = param.Receive.SenderClientId;
            if (!_eventCounter.ContainsKey(syncEvent))
            {
                Debug.Log($"客户端{clientID}同步不存在事件{syncEvent}。");
                return;
            }
            
            _eventCounter[syncEvent][clientID] += 1;
            Debug.Log($"客户端{clientID}同步事件{syncEvent}完毕。");
            if (IsSyncComplete(syncEvent))
            {
                SyncComplete(syncEvent);
            }
        }

        /// <summary>
        /// 查询事件是否同步完毕。
        /// </summary>
        /// <param name="syncEvent"></param>
        /// <returns></returns>
        public bool IsSyncComplete(SyncEvent syncEvent)
        {
            if (!_eventCounter.ContainsKey(syncEvent)) return false;
            var counter = _eventCounter[syncEvent];
            var syncResult = from id in NetworkManager.ConnectedClientsIds
                select counter.ContainsKey(id);
            return syncResult.All(val => val);
        }

        private void SyncComplete(SyncEvent syncEvent)
        {
            Debug.Log($"事件{syncEvent}全部同步完毕。");
            _eventAction[syncEvent]?.Invoke(syncEvent);
            _eventAction.Remove(syncEvent);
        }
    }

}
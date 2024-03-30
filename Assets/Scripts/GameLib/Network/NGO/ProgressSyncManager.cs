﻿using System;
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
    /// 服务器用来同步多个客户端的各种进度事件。需要被继承来确定其具体类型。
    /// </summary>
    public class ProgressSyncManager : NetworkSingleton<ProgressSyncManager>
    {
        private readonly Dictionary<SyncEvent, Counter<ulong>> _eventCounter = new();

        private readonly Dictionary<SyncEvent, Action<SyncEvent>> _eventAction = new();
        
        /// <summary>
        /// 服务端发起一个进度同步事件，当所有客户端都同步之后触发注册的回调函数。
        /// </summary>
        /// <param name="syncEvent"></param>
        /// <param name="onSyncDone"></param>
        public void AddSyncEvent(SyncEvent syncEvent, Action<SyncEvent> onSyncDone)
        {
            if (!NetworkManager.IsServer || !IsSpawned) return;
            if (_eventCounter.ContainsKey(syncEvent)) return;
            Debug.Log($"添加同步事件{syncEvent}");

            _eventCounter[syncEvent] = new Counter<ulong>();
            _eventAction[syncEvent] = onSyncDone;
        }

        /// <summary>
        /// 客户端通报客户端同步成功。
        /// </summary>
        /// <param name="syncEvent"></param>
        public void ClientSyncDone(SyncEvent syncEvent)
        {
            if (!NetworkManager.IsClient || !IsSpawned) return;
            Debug.Log($"客户端{NetworkManager.LocalClientId}发起{syncEvent}同步。");
            ClientSyncDoneServerRpc(syncEvent);
        }

        [ServerRpc(RequireOwnership = false)]
        private void ClientSyncDoneServerRpc(SyncEvent syncEvent, ServerRpcParams param=default)
        {
            var clientID = param.Receive.SenderClientId;
            if (!_eventCounter.ContainsKey(syncEvent))
            {
                Debug.Log($"客户端{clientID}同步不存在事件{syncEvent}。");
                return;
            }
            
            _eventCounter[syncEvent][clientID] += 1;
            Debug.Log($"客户端{clientID}同步事件{syncEvent}完毕。");
            if (CheckSyncDone(syncEvent))
            {
                SyncDone(syncEvent);
            }
        }

        private bool CheckSyncDone(SyncEvent syncEvent)
        {
            var counter = _eventCounter[syncEvent];
            var syncResult = from id in NetworkManager.ConnectedClientsIds
                select counter.ContainsKey(id);
            return syncResult.All(val => val);
        }

        private void SyncDone(SyncEvent syncEvent)
        {
            Debug.Log($"事件{syncEvent}全部同步完毕。");
            _eventAction[syncEvent]?.Invoke(syncEvent);
            _eventCounter.Remove(syncEvent);
            _eventAction.Remove(syncEvent);
        }
    }
}
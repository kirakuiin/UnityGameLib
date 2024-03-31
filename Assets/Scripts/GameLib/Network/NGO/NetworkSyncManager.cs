using System;
using System.Collections.Generic;
using System.Linq;
using GameLib.Common.DataStructure;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameLib.Network.NGO
{
    /// <summary>
    /// 网络同步事件。
    /// </summary>
    public class NetSyncEvent : INetworkSerializable
    {
        /// <summary>
        /// 事件ID代表一个唯一事件。
        /// </summary>
        public int EventID;

        public String Name;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref EventID);
            serializer.SerializeValue(ref Name);
        }

        /// <summary>
        /// 通过枚举的形式创建事件。
        /// </summary>
        /// <param name="val"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static NetSyncEvent Create<T>(T val) where T : Enum
        {
            return new NetSyncEvent()
            {
                EventID = val.GetHashCode(),
                Name = $"{typeof(T).Name}.{val.ToString()}",
            };
        }

        public override string ToString()
        {
            return $"网络同步事件[{Name}]";
        }
    }
    
    /// <summary>
    /// 用来同步网络进度的管理器。
    /// </summary>
    public class NetworkSyncManager : NetworkSingleton<NetworkSyncManager>
    {
        private readonly Dictionary<NetSyncEvent, Counter<ulong>> _eventCounter = new();

        private readonly Dictionary<NetSyncEvent, Action<NetSyncEvent>> _eventAction = new();
        
        /// <summary>
        /// 发起一个网络间进度同步事件。
        /// </summary>
        /// <param name="netSyncEvent"></param>
        /// <param name="onSyncDone"></param>
        public void AddSyncEvent(NetSyncEvent netSyncEvent, Action<NetSyncEvent> onSyncDone=default)
        {
            if (!NetworkManager.IsServer || !IsSpawned) return;
            Debug.Log($"添加{netSyncEvent}");

            _eventCounter[netSyncEvent] = new Counter<ulong>();
            _eventAction[netSyncEvent] = onSyncDone;
        }

        /// <summary>
        /// 客户端通报客户端同步成功。
        /// </summary>
        /// <param name="netSyncEvent"></param>
        public void SyncDone(NetSyncEvent netSyncEvent)
        {
            if (!NetworkManager.IsClient || !IsSpawned) return;
            Debug.Log($"客户端{NetworkManager.LocalClientId}开始同步{netSyncEvent}。");
            SyncDoneServerRpc(netSyncEvent);
        }

        [ServerRpc(RequireOwnership = false)]
        private void SyncDoneServerRpc(NetSyncEvent netSyncEvent, ServerRpcParams param=default)
        {
            var clientID = param.Receive.SenderClientId;
            if (!_eventCounter.ContainsKey(netSyncEvent))
            {
                Debug.Log($"客户端{clientID}同步不存在的{netSyncEvent}。");
                return;
            }
            
            _eventCounter[netSyncEvent][clientID] += 1;
            Debug.Log($"客户端{clientID}同步{netSyncEvent}完毕。");
            if (IsSyncComplete(netSyncEvent))
            {
                SyncComplete(netSyncEvent);
            }
        }

        /// <summary>
        /// 查询事件是否同步完毕。
        /// </summary>
        /// <param name="netSyncEvent"></param>
        /// <returns></returns>
        public bool IsSyncComplete(NetSyncEvent netSyncEvent)
        {
            if (!_eventCounter.ContainsKey(netSyncEvent)) return false;
            var counter = _eventCounter[netSyncEvent];
            var syncResult = from id in NetworkManager.ConnectedClientsIds
                select counter.ContainsKey(id);
            return syncResult.All(val => val);
        }

        private void SyncComplete(NetSyncEvent netSyncEvent)
        {
            Debug.Log($"{netSyncEvent}全部同步完毕。");
            _eventAction[netSyncEvent]?.Invoke(netSyncEvent);
            _eventAction.Remove(netSyncEvent);
        }
    }

}
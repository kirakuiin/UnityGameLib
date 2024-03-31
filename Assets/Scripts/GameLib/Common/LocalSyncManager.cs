using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameLib.Common
{
    public struct SyncEvent
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
    /// 管理本地的进度同步事件。
    /// </summary>
    public class LocalSyncManager : Singleton<LocalSyncManager>
    {
        private readonly Dictionary<SyncEvent, int> _eventCounter = new();

        private readonly Dictionary<SyncEvent, Action<SyncEvent>> _eventAction = new();
        
        /// <summary>
        /// 发起一个同步事件。
        /// </summary>
        /// <param name="syncEvent"></param>
        /// <param name="needSyncNum"></param>
        /// <param name="onSyncDone"></param>
        public void AddSyncEvent(SyncEvent syncEvent, int needSyncNum, Action<SyncEvent> onSyncDone=default)
        {
            Debug.Log($"添加同步事件{syncEvent}({needSyncNum})");

            _eventCounter[syncEvent] = needSyncNum;
            _eventAction[syncEvent] = onSyncDone;
        }

        /// <summary>
        /// 通报同步成功。
        /// </summary>
        /// <param name="syncEvent"></param>
        public void SyncDone(SyncEvent syncEvent)
        {
            if (!_eventCounter.ContainsKey(syncEvent))
            {
                Debug.Log($"同步不存在事件{syncEvent}。");
                return;
            }
            
            _eventCounter[syncEvent] -= 1;
            Debug.Log($"同步事件{syncEvent}。");
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
            return _eventCounter.TryGetValue(syncEvent, out var value) && value <= 0;
        }

        private void SyncComplete(SyncEvent syncEvent)
        {
            Debug.Log($"事件{syncEvent}全部同步完毕。");
            _eventAction[syncEvent]?.Invoke(syncEvent);
            _eventAction.Remove(syncEvent);
        }
    }
}
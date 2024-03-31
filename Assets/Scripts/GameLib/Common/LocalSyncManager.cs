using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameLib.Common
{
    public struct LocalSyncEvent
    {
        /// <summary>
        /// 事件ID代表一个唯一事件。
        /// </summary>
        public int EventID;
        
        public String Name;

        /// <summary>
        /// 通过枚举的形式创建事件。
        /// </summary>
        /// <param name="val"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static LocalSyncEvent Create<T>(T val) where T : Enum
        {
            return new LocalSyncEvent()
            {
                EventID = val.GetHashCode(),
                Name = $"{typeof(T).Name}.{val.ToString()}",
            };
        }

        public override string ToString()
        {
            return $"本地同步事件{Name}";
        }
    }
    
    /// <summary>
    /// 管理本地的进度同步事件。
    /// </summary>
    public class LocalSyncManager : Singleton<LocalSyncManager>
    {
        private readonly Dictionary<LocalSyncEvent, int> _eventCounter = new();

        private readonly Dictionary<LocalSyncEvent, Action<LocalSyncEvent>> _eventAction = new();
        
        /// <summary>
        /// 发起一个同步事件。
        /// </summary>
        /// <param name="localSyncEvent"></param>
        /// <param name="needSyncNum"></param>
        /// <param name="onSyncDone"></param>
        public void AddSyncEvent(LocalSyncEvent localSyncEvent, int needSyncNum, Action<LocalSyncEvent> onSyncDone=default)
        {
            Debug.Log($"添加{localSyncEvent}(等待:{needSyncNum})");

            _eventCounter[localSyncEvent] = needSyncNum;
            _eventAction[localSyncEvent] = onSyncDone;
        }

        /// <summary>
        /// 通报同步成功。
        /// </summary>
        /// <param name="localSyncEvent"></param>
        public void SyncDone(LocalSyncEvent localSyncEvent)
        {
            if (!_eventCounter.ContainsKey(localSyncEvent))
            {
                Debug.Log($"同步不存在{localSyncEvent}。");
                return;
            }
            
            _eventCounter[localSyncEvent] -= 1;
            Debug.Log($"同步{localSyncEvent}。");
            if (IsSyncComplete(localSyncEvent))
            {
                SyncComplete(localSyncEvent);
            }
            
        }

        /// <summary>
        /// 查询事件是否同步完毕。
        /// </summary>
        /// <param name="localSyncEvent"></param>
        /// <returns></returns>
        public bool IsSyncComplete(LocalSyncEvent localSyncEvent)
        {
            return _eventCounter.TryGetValue(localSyncEvent, out var value) && value <= 0;
        }

        private void SyncComplete(LocalSyncEvent localSyncEvent)
        {
            Debug.Log($"{localSyncEvent}全部同步完毕。");
            _eventAction[localSyncEvent]?.Invoke(localSyncEvent);
            _eventAction.Remove(localSyncEvent);
        }
    }
}
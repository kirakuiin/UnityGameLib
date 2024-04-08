using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameLib.Common
{
    /// <summary>
    /// 管理本地的进度同步事件。
    /// </summary>
    public class LocalSyncManager : Singleton<LocalSyncManager>
    {
        private readonly Dictionary<string, int> _eventCounter = new();

        private readonly Dictionary<string, Action> _eventAction = new();
        
        /// <summary>
        /// 发起一个本地同步事件。
        /// </summary>
        /// <param name="e"></param>
        /// <param name="needSyncNum"></param>
        /// <param name="onSyncDone"></param>
        public void AddSyncEvent<T>(T e, int needSyncNum, Action onSyncDone=default) where T : Enum
        {
            Debug.Log($"添加本地事件{e}(等待:{needSyncNum})");

            var eventKey = EventToStr(e);
            _eventCounter[eventKey] = needSyncNum;
            _eventAction[eventKey] = onSyncDone;
        }
        
        private string EventToStr<T>(T e) where T : Enum
        {
            return $"{typeof(T).FullName}|{e.GetHashCode()}";
        }

        /// <summary>
        /// 通报同步成功。
        /// </summary>
        /// <param name="e"></param>
        public void SyncDone<T>(T e) where T : Enum
        {
            var eventKey = EventToStr(e);
            if (!_eventCounter.ContainsKey(eventKey))
            {
                Debug.Log($"同步不存在事件{e}。");
                return;
            }
            
            _eventCounter[eventKey] -= 1;
            Debug.Log($"同步{e}。剩余{_eventCounter[eventKey]}");
            if (HasBeenSyncDone(e))
            {
                SyncComplete(e);
            }
            
        }

        /// <summary>
        /// 查询事件是否同步完毕。
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public bool HasBeenSyncDone<T>(T e) where T : Enum
        {
            var eventKey = EventToStr(e);
            return _eventCounter.ContainsKey(eventKey) && _eventCounter[eventKey] <= 0;
        }

        private void SyncComplete<T>(T e) where T : Enum
        {
            Debug.Log($"{e}全部同步完毕。");
            var eventKey = EventToStr(e);
            _eventAction[eventKey]?.Invoke();
            _eventAction[eventKey] = null;
        }
    }
}
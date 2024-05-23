using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameLib.Common.Utility
{
    /// <summary>
    /// 某些对象要以低于<c>MonoBehaviour</c>Update速度来进行更新，比如说数据库的数据更新等。
    /// 或者某些对象不想与<c>GameObject</c>产生耦合。
    /// </summary>
    public class UpdateRunner : MonoBehaviour
    {
        /// <summary>
        /// 记录每个更新对象的相关数据。
        /// </summary>
        class SubscriberData
        {
            /// <summary>
            /// 更新周期
            /// </summary>
            public float Period;

            /// <summary>
            /// 下次调用的时间
            /// </summary>
            public float NextCallTime;

            /// <summary>
            /// 最近一次调用的时间
            /// </summary>
            public float LastCallTime;
        }

        private readonly Queue<Action> _pendingHandler = new();

        private readonly HashSet<Action<float>> _subscribers = new();
        
        private readonly Dictionary<Action<float>, SubscriberData> _subscriberData = new();

        public void OnDestroy()
        {
            _pendingHandler.Clear();
            _subscribers.Clear();
            _subscriberData.Clear();
        }

        /// <summary>
        /// 订阅更新，订阅后会以固定的频率调用订阅函数。默认每帧一次。
        /// </summary>
        /// <remarks>不要假定内部的订阅函数会以特定的数据进行更新。</remarks>
        /// <param name="onUpdate">订阅函数，参数为更新间隔</param>
        /// <param name="updatePeriod">更新周期，0代表每帧一次</param>
        public void Subscribe(Action<float> onUpdate, float updatePeriod = 0)
        {
            if (!IsValidFunc(onUpdate)) return;

            if (!_subscribers.Contains(onUpdate))
            {
                _pendingHandler.Enqueue(
                    () =>
                    {
                        if (_subscribers.Add(onUpdate))
                        {
                            _subscriberData.Add(onUpdate, new SubscriberData()
                            {
                                Period = updatePeriod,
                                LastCallTime = Time.time,
                                NextCallTime = 0,
                            });
                        }
                    }
                );
            }
        }

        private bool IsValidFunc(Action<float> onUpdate)
        {
            // 局部函数离开作用域后会被释放。
            if (onUpdate?.Target == null) return false;
            // 不能用匿名函数进行订阅
            if (onUpdate.Method.ToString().Contains("<")) return false;
            return true;
        }

        /// <summary>
        /// 取消订阅。
        /// </summary>
        /// <param name="onUpdate">被取消的订阅函数</param>
        public void UnSubscribe(Action<float> onUpdate)
        {
            _pendingHandler.Enqueue(
                () =>
                {
                    _subscribers.Remove(onUpdate);
                    _subscriberData.Remove(onUpdate);
                }
            );
        }

        private void Update()
        {
            PopulateUpdateAction();

            foreach (var subscriber in _subscribers)
            {
                var subscriberData = _subscriberData[subscriber];
                if (Time.time > subscriberData.NextCallTime)
                {
                    subscriber.Invoke(Time.time - subscriberData.LastCallTime);
                    subscriberData.LastCallTime = Time.time;
                    subscriberData.NextCallTime = Time.time + subscriberData.Period;
                }
            }
        }

        private void PopulateUpdateAction()
        {
            while (_pendingHandler.Count > 0)
            {
                _pendingHandler.Dequeue()?.Invoke();
            }
        }
    }
}
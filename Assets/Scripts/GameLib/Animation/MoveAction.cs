using System;
using System.Collections;
using UnityEngine;

namespace GameLib.Animation
{
    /// <summary>
    /// 位移行动。
    /// </summary>
    public class MoveAction : AnimationAction
    {
        /// <summary>
        /// 让目标移动到在指定时间内指定位置。
        /// </summary>
        /// <param name="target"></param>
        /// <param name="destination"></param>
        /// <param name="time"></param>
        /// <param name="onDone"></param>
        public void MoveTo(Transform target, Vector3 destination, float time, Action onDone=default)
        {
            StartCoroutine(MoveCoroutine(target, destination, time, onDone));
        }
        
        private IEnumerator MoveCoroutine(Transform obj, Vector3 to, float time, Action onDone=default)
        {
            var elapseTime = 0.0f;
            var from = obj.position;
            while (true)
            {
                elapseTime += Time.deltaTime;
                var progress = elapseTime / time;
                obj.position = Vector3.Lerp(from, to, progress);
                if (progress >= 1.0)
                    break;
                yield return null;
            }
            onDone?.Invoke();
        }
    }
}
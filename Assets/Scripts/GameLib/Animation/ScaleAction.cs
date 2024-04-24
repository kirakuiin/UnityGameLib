using System;
using System.Collections;
using UnityEngine;

namespace GameLib.Animation
{
    /// <summary>
    /// 缩放行动。
    /// </summary>
    public class ScaleAction : AnimationAction 
    {
        
        /// <summary>
        /// 让目标在指定时间缩放到指定大小。
        /// </summary>
        /// <param name="target"></param>
        /// <param name="destination"></param>
        /// <param name="time"></param>
        /// <param name="onDone"></param>
        public void ScaleTo(Transform target, Vector3 destination, float time, Action onDone=default)
        {
            StartCoroutine(ScaleCoroutine(target, destination, time, onDone));
        }

        private IEnumerator ScaleCoroutine(Transform obj, Vector3 to, float time, Action onDone=default)
        {
            var elapseTime = 0.0f;
            var from = obj.localScale;
            while (true)
            {
                elapseTime += Time.deltaTime;
                var progress = elapseTime / time;
                obj.localScale = Vector3.Lerp(from, to, progress);
                if (progress >= 1.0)
                    break;
                yield return null;
            }
            onDone?.Invoke();
        }
    }
}
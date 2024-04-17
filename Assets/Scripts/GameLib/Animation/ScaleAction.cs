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
        [Tooltip("缩放时间")]
        [SerializeField]
        public float time = 0.5f;

        /// <summary>
        /// 让目标缩放到指定大小。
        /// </summary>
        public void ScaleTo(Transform target, Vector3 destination, Action onDone=default)
        {
            StartCoroutine(ScaleCoroutine(target, destination, onDone));
        }

        private IEnumerator ScaleCoroutine(Transform obj, Vector3 to, Action onDone=default)
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
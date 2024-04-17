﻿using System;
using System.Collections;
using UnityEngine;

namespace GameLib.Animation
{
    /// <summary>
    /// 位移行动。
    /// </summary>
    public class MoveAction : AnimationAction
    {
        [Tooltip("移动时间")]
        [SerializeField]
        public float time = 0.5f;

        /// <summary>
        /// 让目标移动到指定位置。
        /// </summary>
        /// <param name="target"></param>
        /// <param name="destination"></param>
        /// <param name="onDone"></param>
        public void MoveTo(Transform target, Vector3 destination, Action onDone=default)
        {
            StartCoroutine(MoveCoroutine(target, destination, onDone));
        }
        
        private IEnumerator MoveCoroutine(Transform obj, Vector3 to, Action onDone=default)
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
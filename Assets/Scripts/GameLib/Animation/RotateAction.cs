using System;
using System.Collections;
using UnityEngine;

namespace GameLib.Animation
{
    /// <summary>
    /// 旋转行动。
    /// </summary>
    public class RotateAction: AnimationAction
    {
        [Tooltip("旋转时间")]
        [SerializeField]
        public float time = 0.5f;

        /// <summary>
        /// 让目标移动到指定位置。
        /// </summary>
        /// <param name="target"></param>
        /// <param name="destination"></param>
        /// <param name="onDone"></param>
        public void RotateTo(Transform target, Quaternion destination, Action onDone=default)
        {
            StartCoroutine(RotateCoroutine(target, destination, onDone));
        }

        private IEnumerator RotateCoroutine(Transform obj, Quaternion to, Action onDone=default)
        {
            var elapseTime = 0.0f;
            var baseTime = Quaternion.Angle(obj.rotation, to)/time;
            while (true)
            {
                elapseTime += Time.deltaTime;
                var progress = elapseTime / time;
                var curRot = obj.rotation;
                obj.rotation = Quaternion.RotateTowards(curRot, to, baseTime*Time.deltaTime);
                if (progress >= 1.0)
                    break;
                yield return null;
            }
            onDone?.Invoke();
        }
    }
}
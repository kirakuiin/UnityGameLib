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
        /// <summary>
        /// 让目标在指定时间内旋转到指定位置。
        /// </summary>
        /// <param name="target"></param>
        /// <param name="destination"></param>
        /// <param name="time"></param>
        /// <param name="onDone"></param>
        public void RotateTo(Transform target, Quaternion destination, float time, Action onDone=default)
        {
            StartCoroutine(RotateCoroutine(target, destination, time, onDone));
        }

        private IEnumerator RotateCoroutine(Transform obj, Quaternion to, float time, Action onDone=default)
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
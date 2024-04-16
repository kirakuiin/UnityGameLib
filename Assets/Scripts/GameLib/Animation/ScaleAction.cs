using System;
using System.Collections;
using UnityEngine;

namespace GameLib.Animation
{
    /// <summary>
    /// 缩放行动。
    /// </summary>
    public class ScaleAction: MonoBehaviour
    {
        [Tooltip("缩放时间")]
        [SerializeField]
        private float time = 0.5f;

        /// <summary>
        /// 让目标缩放到指定大小。
        /// </summary>
        public void ScaleTo(Transform target, Vector3 destination, Action onDone=default)
        {
            StartCoroutine(Scale(target, destination, onDone));
        }

        private IEnumerator Scale(Transform tr, Vector3 scale, Action onDone)
        {
            float t = 0;
            var startScale = tr.localScale;
            while (true)
            {
                t += Time.deltaTime;
                float a = t / time;
                tr.localScale = Vector3.Lerp(startScale, scale, a);
                if (a >= 1.0f)
                    break;
                yield return null;
            }
            onDone?.Invoke();
        }
    }
}
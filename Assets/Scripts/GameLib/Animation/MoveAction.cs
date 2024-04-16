using System;
using System.Collections;
using UnityEngine;

namespace GameLib.Animation
{
    /// <summary>
    /// 位移行动。
    /// </summary>
    public class MoveAction : MonoBehaviour
    {
        [Tooltip("移动时间")]
        [SerializeField]
        private float time = 0.5f;

        /// <summary>
        /// 让目标移动到指定位置。
        /// </summary>
        /// <param name="target"></param>
        /// <param name="destination"></param>
        /// <param name="onDone"></param>
        public void MoveTo(Transform target, Vector3 destination, Action onDone=default)
        {
            StartCoroutine(Move(target, destination, onDone));
        }

        private IEnumerator Move(Transform tr, Vector3 pos, Action onDone)
        {
            float t = 0;
            var startPos = tr.position;
            while (true)
            {
                t += Time.deltaTime;
                float a = t / time;
                tr.position = Vector3.Lerp(startPos, pos, a);
                if (a >= 1.0f)
                    break;
                yield return null;
            }
            onDone?.Invoke();
        }
    }
}
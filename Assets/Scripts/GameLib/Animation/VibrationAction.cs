using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace GameLib.Animation
{
    /// <summary>
    /// 震动行动。
    /// </summary>
    public class VibrationAction: AnimationAction
    {
        /// <summary>
        /// 简谐震动。
        /// </summary>
        /// <param name="target">震动目标</param>
        /// <param name="amplitude">振幅</param>
        /// <param name="frequency">振动频率</param>
        /// <param name="direction">振动方向</param>
        /// <param name="decay">每个振动周期振幅的缩小幅度</param>
        /// <param name="onDone"></param>
        public void SimpleShake(Transform target, Vector3 direction, float amplitude, float frequency, float decay, Action onDone=default)
        {
            StartCoroutine(ShakeCoroutine(target, direction, amplitude, frequency, decay, onDone));
        }
        
        private IEnumerator ShakeCoroutine(Transform target, Vector3 direction, float amplitude, float frequency, float decay, Action onDone=default)
        {
            var curAmp = amplitude;
            var normDir = direction.normalized;
            var prevOffset = 0f;
            var curTime = 0f;
            while (curAmp >= 0)
            {
                curTime += Time.fixedDeltaTime;
                var radian = frequency * curTime;
                var offset = curAmp * math.sin(radian);
                var diff = (offset - prevOffset) * normDir;
                prevOffset = offset;
                target.Translate(diff);
                if (radian >= 2 * math.PI)
                {
                    curAmp -= decay;
                    curTime -= (2 * math.PI) / frequency;
                }
                yield return null;
            }
            onDone?.Invoke();
        }
    }
}
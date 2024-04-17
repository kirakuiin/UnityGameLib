using System;
using System.Collections;
using UnityEngine;

namespace GameLib.Animation
{
    /// <summary>
    /// 动画动作基类。
    /// </summary>
    public abstract class AnimationAction : MonoBehaviour
    {
        /// <summary>
        /// 通知动画。
        /// </summary>
        public void Stop()
        {
            StopAllCoroutines();
        }
    }
}
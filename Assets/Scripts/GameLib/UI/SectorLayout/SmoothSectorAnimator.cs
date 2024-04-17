using System;
using GameLib.Animation;
using UnityEngine;

namespace GameLib.UI.SectorLayout
{
    /// <summary>
    /// 平滑动画
    /// </summary>
    [ExecuteAlways]
    [RequireComponent(typeof(MoveAction))]
    [RequireComponent(typeof(RotateAction))]
    public class SmoothSectorAnimator : SectorAnimator
    {
        private MoveAction _move;

        private RotateAction _rotate;
        
        private void Awake()
        {
            _move = GetComponent<MoveAction>();
            _rotate = GetComponent<RotateAction>();
            _move.time = animateTime;
            _rotate.time = animateTime;
        }
        
        /// <summary>
        /// 播放动画
        /// </summary>
        /// <param name="childTransform">节点的变换对象</param>
        /// <param name="targetPosition">节点的目标位置</param>
        /// <param name="targetRotation">节点的目标旋转</param>
        public override void Play(Transform childTransform, Vector3 targetPosition, Quaternion targetRotation)
        {
            _move.MoveTo(childTransform, targetPosition);
            _rotate.RotateTo(childTransform, targetRotation);
        }

        private void OnValidate()
        {
            if (_rotate != null)
            {
                _rotate.time = animateTime;
            }

            if (_move != null)
            {
                _move.time = animateTime;
            }
        }

        public override void Stop()
        {
            _move.Stop();
            _rotate.Stop();
        }
    }
}
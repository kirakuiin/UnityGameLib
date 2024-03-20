using UnityEngine;

namespace GameLib.UI.SectorLayout
{
    /// <summary>
    /// 平滑动画
    /// </summary>
    public class SmoothSectorAnimator : SectorAnimator
    {
        /// <summary>
        /// 播放动画
        /// </summary>
        /// <param name="childTransform">节点的变换对象</param>
        /// <param name="targetPosition">节点的目标位置</param>
        /// <param name="targetRotation">节点的目标旋转</param>
        public override void Play(Transform childTransform, Vector3 targetPosition, Quaternion targetRotation)
        {
            childTransform.position = Vector3.Lerp(childTransform.position, targetPosition, Time.deltaTime*animateSpeed);
            childTransform.rotation =
                Quaternion.RotateTowards(childTransform.rotation, targetRotation, Time.deltaTime * animateSpeed*30);
        }
    }
}
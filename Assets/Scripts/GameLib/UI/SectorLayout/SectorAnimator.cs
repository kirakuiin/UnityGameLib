using UnityEngine;

namespace GameLib.UI.SectorLayout
{
    /// <summary>
    /// 用来处理元素添加到扇形布局动画相关问题。
    /// </summary>
    public class SectorAnimator : MonoBehaviour
    {
        [SerializeField]
        protected float animateSpeed = 10;

        /// <summary>
        /// 播放动画
        /// </summary>
        /// <param name="childTransform">节点的变换对象</param>
        /// <param name="targetPosition">节点的目标位置</param>
        /// <param name="targetRotation">节点的目标旋转</param>
        public virtual void Play(Transform childTransform, Vector3 targetPosition, Quaternion targetRotation)
        {
            childTransform.position = targetPosition;
            childTransform.rotation = targetRotation;
        }
    }
}
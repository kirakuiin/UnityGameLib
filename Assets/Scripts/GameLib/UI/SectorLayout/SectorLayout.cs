using System.Collections;
using UnityEngine;
using System.Linq;
using GameLib.Common.Extension;

namespace GameLib.UI.SectorLayout
{
    /// <summary>
    /// 扇形布局。
    /// </summary>
    /// <remarks>如果通过<c>SetParent</c>来设置子节点，那么不会触发自动布局，需要调用<see cref="Rebuild"/>来触发重新布局。</remarks>
    [ExecuteInEditMode]
    public class SectorLayout : MonoBehaviour
    {
        [Tooltip("圆心位置")]
        [SerializeField]
        private float radius;

        [Tooltip("两个对象间角度间隔")]
        [SerializeField]
        private float angleInterval;

        [SerializeField]
        private SectorAnimator animator;

        private Vector3 _origin;

        private void Awake()
        {
            _origin = transform.position - new Vector3(0, radius, 0);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(_origin, 10f);
            Gizmos.DrawWireSphere(_origin, radius);
        }

# if UNITY_EDITOR
        private void Update()
        {
            if (Application.isPlaying) return;
            if (transform.childCount == 0) return;
            for (var i = 0; i < transform.childCount; ++i)
            {
                transform.GetChild(i).position = CalcChildPosition(i);
                transform.GetChild(i).rotation = CalcChildQuaternion(i);
            }
        }
#endif
        private Vector3 CalcChildPosition(int childIdx)
        {
            var targetRad = Mathf.Deg2Rad*CalcChildDegrees(childIdx);
            return _origin + new Vector3(-Mathf.Sin(targetRad), Mathf.Cos(targetRad)) * radius;
        }

        private float CalcChildDegrees(int childIdx)
        {
            var mostLeftAngle = (transform.childCount-1) * angleInterval / 2;
            return (mostLeftAngle - angleInterval * childIdx);
        }

        private Quaternion CalcChildQuaternion(int childIdx)
        {
            return Quaternion.Euler(0, 0, CalcChildDegrees(childIdx));
        }

        /// <summary>
        /// 增加一个子对象。
        /// </summary>
        /// <param name="childObj"></param>
        public void Add(GameObject childObj)
        {
            childObj.transform.SetParent(transform);
            Rebuild();
        }

        /// <summary>
        /// 重新布局。
        /// </summary>
        public void Rebuild()
        {
            StopAllCoroutines();
            StartCoroutine(RebuildCoroutine());
        }

        private IEnumerator RebuildCoroutine()
        {
            while (!IsAllUpdateDone())
            {
                for (var i = 0; i < transform.childCount; ++i)
                {
                    animator.Play(transform.GetChild(i), CalcChildPosition(i),
                        CalcChildQuaternion(i));
                }
                yield return null;
            }
        }

        private bool IsAllUpdateDone()
        {
            var query = from index in Enumerable.Range(0, transform.childCount)
                select new
                {
                    basePos = transform.GetChild(index).position,
                    baseRot = transform.GetChild(index).rotation,
                    targetPos = CalcChildPosition(index),
                    targetRot = CalcChildQuaternion(index)
                }
                into element
                where !MathExtension.Approximately(element.basePos, element.targetPos, 1E-02f)
                      || !MathExtension.Approximately(element.baseRot, element.targetRot, 1E-01F)
                select element;
            
            var isAllDone = query.ToArray().Length == 0;
            return isAllDone;
        }

        /// <summary>
        /// 移除一个子对象。
        /// </summary>
        /// <param name="childObj"></param>
        public void Remove(GameObject childObj)
        {
            childObj.transform.SetParent(null);
            Rebuild();
        }
    }

}
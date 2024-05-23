using UnityEngine;
using GameLib.UI.Extension;

namespace GameLib.UI.SectorLayout
{
    /// <summary>
    /// 扇形布局。
    /// </summary>
    /// <remarks>如果通过<c>SetParent</c>来设置子节点，那么不会触发自动布局，需要调用<see cref="Rebuild"/>来触发重新布局。</remarks>
    [ExecuteInEditMode]
    public class SectorLayout : MonoBehaviour
    {
        [Tooltip("每个对象的大小")]
        [SerializeField]
        private Vector2 cellSize;
        
        [Tooltip("圆心位置")]
        [SerializeField]
        private float radius;

        [Tooltip("两个对象间角度间隔")]
        [SerializeField]
        private float angleInterval;
        
        [Tooltip("对象添加动画播放器")]
        [SerializeField]
        private SectorAnimator animator;

        [Tooltip("最小的顺序值")]
        [SerializeField] private int minimumOrder = 1;
        
        private Vector3 Origin => transform.position - new Vector3(0, radius, 0);

        /// <summary>
        /// 设置每个元素的大小。
        /// </summary>
        public Vector2 CellSize
        {
            set
            {
                cellSize = value;
                UpdateChildSize();
            }
            get => cellSize;
        }

        /// <summary>
        /// 重设间隔角度。
        /// </summary>
        /// <param name="angle"></param>
        public void SetAngle(float angle)
        {
            angleInterval = angle;
            Rebuild();
        }

        private void UpdateChildSize()
        {
            for (var i = 0; i < transform.childCount; ++i)
            {
                transform.GetChild(i).GetComponent<RectTransform>().SetRectSize(cellSize);
            }
        }
        

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(Origin, 10f);
            Gizmos.DrawWireSphere(Origin, radius);
        }

# if UNITY_EDITOR
        private void Update()
        {
            if (Application.isPlaying) return;
            if (transform.childCount == 0) return;
            var origin = Origin;
            for (var i = 0; i < transform.childCount; ++i)
            {
                transform.GetChild(i).position = CalcChildPosition(i, origin);
                transform.GetChild(i).rotation = CalcChildQuaternion(i);
            }
        }
#endif
        private Vector3 CalcChildPosition(int childIdx, Vector3 origin)
        {
            var targetRad = Mathf.Deg2Rad*CalcChildDegrees(childIdx);
            return origin + new Vector3(-Mathf.Sin(targetRad), Mathf.Cos(targetRad)) * radius;
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
        /// <remarks>如果对象实现了<see cref="GameLib.UI.CanvasDrawOrder"/>接口，那么会对其显示顺序进行自动排序。</remarks>
        /// <param name="childObj"></param>
        public void Add(GameObject childObj)
        {
            childObj.transform.SetParent(transform);
            childObj.GetComponent<RectTransform>().SetRectSize(cellSize);
            Rebuild();
        }
        
        /// <summary>
        /// 重新布局。
        /// </summary>
        public void Rebuild()
        {
            var origin = Origin;
            animator.Stop();
            for (var i = 0; i < transform.childCount; ++i)
            {
                var childTransform = transform.GetChild(i);
                animator.Play(childTransform, CalcChildPosition(i, origin),
                    CalcChildQuaternion(i));
                if (childTransform.gameObject.TryGetComponent<IDrawOrder>(out var comp))
                {
                    comp.Order = i+minimumOrder;
                }
            }
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
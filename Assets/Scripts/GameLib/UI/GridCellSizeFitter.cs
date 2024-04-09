using UnityEngine;
using UnityEngine.UI;

namespace GameLib.UI
{
    /// <summary>
    /// 让GridLayoutGroup的CellSize属性可以自动适配父对象的大小。
    /// </summary>
    [ExecuteInEditMode]
    [RequireComponent(typeof(GridLayoutGroup))]
    public class GridCellSizeFitter: MonoBehaviour
    {
        private enum Axis { X, Y };
        private enum RatioMode { Free, Fixed };
 
        [Tooltip("采用X轴伸展还是Y轴伸展")]
        [SerializeField] Axis expand;
        
        [Tooltip("每个网格的大小是否保持固定比率")]
        [SerializeField] RatioMode ratioMode;
        
        [Tooltip("网格大小比率")]
        [SerializeField] float cellRatio = 1;
 
        private RectTransform _rectTransform;
        private GridLayoutGroup _gridLayout;
 
        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _gridLayout = GetComponent<GridLayoutGroup>();
        }
 
        void Start()
        {
            UpdateCellSize();
        }
 
        void OnRectTransformDimensionsChange()
        {
            UpdateCellSize();
        }
 
#if UNITY_EDITOR
        [ExecuteAlways]
        void Update()
        {
            if (Application.isPlaying) return;
            UpdateCellSize();
        }
#endif
 
        void OnValidate()
        {
            _rectTransform = GetComponent<RectTransform>();
            _gridLayout = GetComponent<GridLayoutGroup>();
            UpdateCellSize();
        }
 
        void UpdateCellSize()
        {
            if (_gridLayout is null) return;
            var count = _gridLayout.constraintCount;
            if (expand == Axis.X)
            {
                float spacing = (count - 1) * _gridLayout.spacing.x;
                float contentSize = _rectTransform.rect.width - _gridLayout.padding.left - _gridLayout.padding.right - spacing;
                float sizePerCell = contentSize / count;
                _gridLayout.cellSize = new Vector2(sizePerCell, ratioMode == RatioMode.Free ? _gridLayout.cellSize.y : sizePerCell * cellRatio);
         
            }
            else //if (expand == Axis.Y)
            {
                float spacing = (count - 1) * _gridLayout.spacing.y;
                float contentSize = _rectTransform.rect.height - _gridLayout.padding.top - _gridLayout.padding.bottom -spacing;
                float sizePerCell = contentSize / count;
                _gridLayout.cellSize = new Vector2(ratioMode == RatioMode.Free ? _gridLayout.cellSize.x : sizePerCell * cellRatio, sizePerCell);
            }
        }
    }
}
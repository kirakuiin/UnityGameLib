using UnityEngine;

namespace GameLib.UI.Fitter
{
    /// <summary>
    /// 让SectorLayout的CellSize属性可以自动适配父对象的大小。
    /// </summary>
    [ExecuteAlways]
    [RequireComponent(typeof(SectorLayout.SectorLayout))]
    public class SectorCellSizeFitter : SizeFitter
    {
        private enum Axis { X, Y };
        private enum RatioMode { Free, Fixed };
 
        [Tooltip("采用X轴伸展还是Y轴伸展")]
        [SerializeField] Axis expand;
        
        [Tooltip("每个网格的大小是否保持固定比率")]
        [SerializeField] RatioMode ratioMode;

        [Tooltip("网格初始大小")] [SerializeField]
        private Vector2 cellSize;

        private float CellRatio => expand == Axis.X ? cellSize.y / cellSize.x : cellSize.x / cellSize.y;
 
        private RectTransform _rectTransform;
        private SectorLayout.SectorLayout _sectorLayout;
        private Vector2 _initialRatio;
 
        protected override void InitComponent()
        {
            _rectTransform = GetComponent<RectTransform>();
            _sectorLayout = GetComponent<SectorLayout.SectorLayout>();
            var rectSize = _rectTransform.rect.size;
            _initialRatio = cellSize / rectSize;
        }
        
        protected override void UpdateSize()
        {
            if (_sectorLayout is null) return;
            var rectSize = _rectTransform.rect.size;
            var cell = _initialRatio * rectSize;
            if (expand == Axis.X)
            {
                _sectorLayout.CellSize = new Vector2(cell.x, ratioMode == RatioMode.Free ? cell.y : cell.x * CellRatio);
            }
            else
            {
                _sectorLayout.CellSize = new Vector2(ratioMode == RatioMode.Free ? cell.x : cell.y * CellRatio, cell.y);
            }
        }
    }
}
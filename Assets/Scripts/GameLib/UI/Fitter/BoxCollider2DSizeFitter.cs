using UnityEngine;

namespace GameLib.UI.Fitter
{
    /// <summary>
    /// 让<see cref="BoxCollider2D"/>同UI的大小一致的组件。
    /// </summary>
    [ExecuteAlways]
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(BoxCollider2D))]
    public class BoxCollider2DSizeFitter : SizeFitter
    {
        private RectTransform _rect;

        private BoxCollider2D _collider;
        
        private void Awake()
        {
            _rect = GetComponent<RectTransform>();
            _collider = GetComponent<BoxCollider2D>();
        }
        
        protected override void InitComponent()
        {
            _rect = GetComponent<RectTransform>();
            _collider = GetComponent<BoxCollider2D>();
        }

        protected override void UpdateSize()
        {
            if (_rect is null) return;
            var rect = _rect.rect;
            _collider.offset = rect.center;
            _collider.size = rect.size;
        }
    }
}
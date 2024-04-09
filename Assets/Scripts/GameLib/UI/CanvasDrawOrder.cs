using UnityEngine;

namespace GameLib.UI
{
    /// <summary>
    /// 使用Canvas作为渲染排序。
    /// </summary>
    [RequireComponent(typeof(Canvas))]
    public class CanvasDrawOrder : MonoBehaviour, IDrawOrder
    {
        private Canvas _canvas;
        private void Awake()
        {
            _canvas = GetComponent<Canvas>();
        }

        public int Order
        {
            get => _canvas.sortingOrder;
            set => _canvas.sortingOrder=value;
        }
    }
    
    /// <summary>
    /// 支持图像排序的对象。
    /// </summary>
    public interface IDrawOrder
    {
        /// <summary>
        /// 设置渲染顺序，越高越后渲染。
        /// </summary>
        public int Order { set; get; }
    }
}
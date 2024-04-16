// ReSharper disable once CheckNamespace
namespace GameLib.UI
{
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
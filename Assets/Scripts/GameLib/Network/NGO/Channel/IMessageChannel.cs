using System;
using GameLib.Common;

namespace GameLib.Network.NGO.Channel
{
    /// <summary>
    /// 发布者接口，实现此接口具有发布消息功能
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IPublisher<in T> : IGameService
    {
        /// <summary>
        /// 发布消息
        /// </summary>
        /// <param name="message"></param>
        public void Publish(T message);
    }

    /// <summary>
    /// 订阅者接口，实现此接口具有订阅功能
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ISubscriber<out T> : IGameService
    {
        /// <summary>
        /// 使用处理函数订阅此接口
        /// </summary>
        /// <param name="handler">处理函数</param>
        /// <returns></returns>
        public IDisposable Subscribe(Action<T> handler);

        /// <summary>
        /// 取消订阅
        /// </summary>
        /// <param name="handler">处理函数</param>
        public void Unsubscribe(Action<T> handler);
    }
    
    /// <summary>
    /// 信息通道，同时支持订阅和发布功能。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IMessageChannel<T> : IPublisher<T>, ISubscriber<T>, IDisposable
    {
        /// <summary>
        /// 是否已经释放完毕
        /// </summary>
        public bool IsDisposed { get; }
    }
    
    /// <summary>
    /// 支持缓存功能的信息通道。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IBufferedMessageChannel<T> : IMessageChannel<T>
    {
        /// <summary>
        /// 是否存在缓存的消息。
        /// </summary>
        bool HasBufferedMessage { get; }
        
        /// <summary>
        /// 被缓存的消息。
        /// </summary>
        T BufferedMessage { get; }
    }
}
using System;

namespace GameLib.Network.NGO.Channel
{
    /// <summary>
    /// 缓存上次发布的消息，有新的订阅时会自动向新订阅发送之前的消息。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BufferedMessageChannel<T> : MessageChannel<T>, IBufferedMessageChannel<T>
    {
        public override void Publish(T message)
        {
            HasBufferedMessage = true;
            BufferedMessage = message;
            base.Publish(message);
        }

        public override IDisposable Subscribe(Action<T> handler)
        {
            var subscription = base.Subscribe(handler);

            if (HasBufferedMessage)
            {
                handler?.Invoke(BufferedMessage);
            }

            return subscription;
        }

        public bool HasBufferedMessage { get; private set; } = false;

        public T BufferedMessage { get; private set; }
    }
}
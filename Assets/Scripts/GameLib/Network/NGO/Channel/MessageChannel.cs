using System;
using System.Collections.Generic;
using UnityEngine.Assertions;

namespace GameLib.Network.NGO.Channel
{
    /// <summary>
    /// 基础版本的信道
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MessageChannel<T> : IMessageChannel<T>
    {
        private readonly List<Action<T>> _messageHandlers = new();

        private readonly Dictionary<Action<T>, bool> _pendingHandlers = new();
        
        public bool IsDisposed { get; private set; } = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool isDisposing)
        {
            if (IsDisposed) return;

            IsDisposed = true;
            _messageHandlers.Clear();
            _pendingHandlers.Clear();
        }

        ~MessageChannel()
        {
            Dispose(false);
        }

        public virtual void Publish(T message)
        {
            ClearPendingHandlers();
            PublishMessage(message);
        }

        private void ClearPendingHandlers()
        {
            foreach (var handler in _pendingHandlers.Keys)
            {
                var shouldBeAdded = _pendingHandlers[handler];
                if (shouldBeAdded)
                {
                    _messageHandlers.Add(handler);
                }
                else
                {
                    _messageHandlers.Remove(handler);
                }
            }
            _pendingHandlers.Clear();
        }

        private void PublishMessage(T message)
        {
            foreach (var handler in _messageHandlers)
            {
                handler?.Invoke(message);
            }
        }

        public virtual IDisposable Subscribe(Action<T> handler)
        {
            Assert.IsTrue(!IsSubscribed(handler), "尝试添加重复的处理器");

            if (_pendingHandlers.ContainsKey(handler))
            {
                var shouldBeRemove = !_pendingHandlers[handler];
                if (shouldBeRemove)
                {
                    _pendingHandlers.Remove(handler);
                }
            }
            else
            {
                _pendingHandlers[handler] = true;
            }

            return new DisposableSubscription<T>(this, handler);
        }

        private bool IsSubscribed(Action<T> handler)
        {
            var isPendingRemoval = _pendingHandlers.ContainsKey(handler) && !_pendingHandlers[handler];
            var isPendingAdding = _pendingHandlers.ContainsKey(handler) && _pendingHandlers[handler];
            return (_messageHandlers.Contains(handler) && !isPendingRemoval) || isPendingAdding;
        }

        public void Unsubscribe(Action<T> handler)
        {
            if (!IsSubscribed(handler)) return;

            if (_pendingHandlers.ContainsKey(handler))
            {
                var shouldBeAdded = _pendingHandlers[handler];
                if (shouldBeAdded)
                {
                    _pendingHandlers.Remove(handler);
                }
            }
            else
            {
                _pendingHandlers[handler] = false;
            }
        }
    }
    
    /// <summary>
    /// 处理激活的信道订阅和取消订阅相关问题
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class DisposableSubscription<T> : IDisposable
    {
        private Action<T> _handler;
        private bool _isDisposed;
        private IMessageChannel<T> _channel;

        public DisposableSubscription(IMessageChannel<T> messageChannel, Action<T> handler)
        {
            _channel = messageChannel;
            _handler = handler;
        }

        ~DisposableSubscription()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool isDisposing)
        {
            if (_isDisposed) return;
            
            _isDisposed = true;
            if (_channel.IsDisposed)
            {
                _channel.Unsubscribe(_handler);
            }
            _handler = null;
            _channel = null;
        }
    }
}
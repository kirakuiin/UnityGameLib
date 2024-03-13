using System;
using System.Collections.Generic;

namespace GameLib.Common
{
    /// <summary>
    /// 管理多个可处理对象的组，对象终结时会负责内部内容的释放。
    /// </summary>
    public class DisposableGroup : IDisposable
    {
        private bool _isDisposed = false;

        private readonly List<IDisposable> _container = new();

        /// <summary>
        /// 添加一个可处理对象。
        /// </summary>
        /// <param name="disposable"></param>
        public void Add(IDisposable disposable)
        {
            _container.Add(disposable);
        }
        
        ~DisposableGroup()
        {
            Dispose(false);
            GC.SuppressFinalize(this);
        }
        
        public void Dispose()
        {
            Dispose(true);
        }
        
        protected virtual void Dispose(bool isDisposing)
        {
            if (_isDisposed) return;

            foreach (var element in _container)
            {
                element.Dispose();
            }
            
            _container.Clear();
            _isDisposed = true;
        }
    }
}
using System.Collections.Generic;
using UnityEngine;

namespace GameLib.Common
{
    /// <summary>
    /// 需要提供游戏服务的类型需要实现此接口。
    /// </summary>
    public interface IGameService
    {
    }
    
    /// <summary>
    /// 为 <see cref="IGameService"/> 类型对象提供定位器服务。
    /// </summary>
    public class ServiceLocator : Singleton<ServiceLocator>
    {
        private readonly Dictionary<string, IGameService> _services = new();
        
        /// <summary>
        /// 获得注册过的某个服务。
        /// </summary>
        /// <typeparam name="T">实现了<c>IGameService</c>的类型</typeparam>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public T Get<T>() where T : IGameService
        {
            var key = GetTypeName<T>();
            if (_services.TryGetValue(key, out var service)) return (T)service;
            return GetAtAll<T>();
        }
        
        private T GetAtAll<T>()
        {
            var key = GetTypeName<T>();
            foreach (var service in _services.Values)
            {
                if (service is T gameService)
                {
                    return gameService;
                }
            }
            throw new InvalidOperationException(key);
        }

        private string GetTypeName<T>()
        {
            return typeof(T).Name;
        }

        /// <summary>
        /// 注册一个服务到定位器。
        /// </summary>
        /// <param name="service">服务对象</param>
        /// <typeparam name="T">实现了<c>IGameService</c>的类型</typeparam>
        public void Register<T>(T service) where T : IGameService
        {
            var key = GetTypeName<T>();
            if (!_services.TryAdd(key, service))
            {
                Debug.LogError($"服务{key}已经存在。");
            }
        }

        /// <summary>
        /// 从定位器中取消某个对象的注册。
        /// </summary>
        /// <typeparam name="T">实现了<c>IGameService</c>的类型</typeparam>
        public void UnRegister<T>() where T : IGameService
        {
            var key = GetTypeName<T>();
            if (!_services.ContainsKey(key))
            {
                Debug.LogError($"定位器中不存在{key}类型的服务。");
                return;
            }

            _services.Remove(key);
        }
    }

    public class InvalidOperationException : LibException
    {
        public InvalidOperationException(string typeName)
            : base($"{typeName} 没有在{nameof(ServiceLocator)}中注册。", false)
        {
        }
    }
}
using UnityEngine;

namespace GameLib.Common
{
    /// <summary>
    /// 实现单例模式必须实现的接口
    /// </summary>
    public interface ISingleton
    {
        /// <summary>
        /// 初始化单例对象
        /// </summary>
        public void Initialize();

        /// <summary>
        /// 清除单例对象
        /// </summary>
        public void Clear();
    }
    
    /// <summary>
    /// 代表单例对象的初始化状态
    /// </summary>
    public enum SingletonInitializationStatus
    {
        /// <summary>
        /// 未初始化
        /// </summary>
        Uninitialize,
        /// <summary>
        /// 初始化中
        /// </summary>
        Initializing,
        /// <summary>
        /// 已经初始化
        /// </summary>
        Initialized,
    }

    /// <summary>
    /// 为一般c#类型使用的单例模式
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Singleton<T> : ISingleton where T : Singleton<T>, new()
    {
        private static T _instance;

        private SingletonInitializationStatus _status;

        private static readonly object _lockObj = new object();

        /// <summary>
        /// 返回单例模式实例
        /// </summary>
        /// <value><c>T</c>.</value>
        public static T Instance
        {
            get
            {
                if (_instance != null) return _instance;
                
                lock (_lockObj)
                {
                    _instance = new T();
                    _instance.Initialize();
                }
                return _instance;
            }
        }

        /// <summary>
        /// 判断单例是否已经初始化
        /// </summary>
        /// <returns>如果初始化完毕的话返回<c>true</c></returns>
        public virtual bool IsInitialized() => _status == SingletonInitializationStatus.Initialized;

        public virtual void Initialize()
        {
            if (_status != SingletonInitializationStatus.Uninitialize) return;

            _status = SingletonInitializationStatus.Initializing;
            OnInitializing();
            _status = SingletonInitializationStatus.Initialized;
            OnInitialized();
        }

        /// <summary>
        /// 初始化过程中调用
        /// </summary>
        protected virtual void OnInitializing()
        {
        }
        
        /// <summary>
        /// 初始化完毕调用
        /// </summary>
        protected virtual void OnInitialized()
        {
        }

        /// <summary>
        /// 创建一个新的单例
        /// </summary>
        public static void Create()
        {
            Destroy();
            _instance = Instance;
        }

        /// <summary>
        /// 摧毁已经存在的单例
        /// </summary>
        public static void Destroy()
        {
            if (_instance == null) return;

            _instance.Clear();
            _instance = default(T);
        }

        public virtual void Clear()
        {
        }
    }
    
    /// <summary>
    /// 基于MonoBehaviour的单例模式，场景变换时会被摧毁。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class MonoSingleton<T> : MonoBehaviour, ISingleton where T : MonoSingleton<T>
    {
        private static T _instance;

        private SingletonInitializationStatus _status = SingletonInitializationStatus.Uninitialize;

        /// <summary>
        /// 返回单例实例
        /// </summary>
        /// <value><c>T</c>.</value>
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<T>();
                    if (_instance == null)
                    {
                        var obj = new GameObject();
                        obj.name = typeof(T).Name;
                        _instance = obj.AddComponent<T>();
                        _instance.OnMonoSingletonCreated();
                    }
                }
                return _instance;
            }
        }

        /// <summary>
        /// 当mono对象被创建时调用
        /// </summary>
        protected virtual void OnMonoSingletonCreated()
        {
        }

        /// <summary>
        /// 判断单例是否已经初始化
        /// </summary>
        /// <returns>如果初始化完毕的话返回<c>true</c></returns>
        public virtual bool IsInitialized => _status == SingletonInitializationStatus.Initialized;

        protected virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = this as T;
                Initialize();
            }
            else
            {
                if (Application.isPlaying)
                {
                    Destroy(gameObject);
                }
                else
                {
                    DestroyImmediate(gameObject);
                }
            }
        }
        
        public virtual void Initialize()
        {
            if (_status != SingletonInitializationStatus.Uninitialize) return;

            _status = SingletonInitializationStatus.Initializing;
            OnInitializing();
            _status = SingletonInitializationStatus.Initialized;
            OnInitialized();
        }

        /// <summary>
        /// 初始化过程中调用
        /// </summary>
        protected virtual void OnInitializing()
        {
        }
        
        /// <summary>
        /// 初始化完毕调用
        /// </summary>
        protected virtual void OnInitialized()
        {
        }
        
        /// <summary>
        /// 创建一个新的单例
        /// </summary>
        public static void Create()
        {
            Destroy();
            _instance = Instance;
        }

        /// <summary>
        /// 摧毁已经存在的单例
        /// </summary>
        public static void Destroy()
        {
            if (_instance == null) return;

            _instance.Clear();
            _instance = default(T);
        }

        public virtual void Clear()
        {
        }
    }

    /// <summary>
    /// 基于MonoBehaviour的单例模式，场景变换时不会被摧毁。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class PersistentMonoSingleton<T> : MonoSingleton<T> where T : MonoSingleton<T>
    {
        protected override void OnInitializing()
        {
            base.OnInitializing();
            if (Application.isPlaying)
            {
                DontDestroyOnLoad(gameObject);
            }
        }
    }
}
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace GameLib.Common
{
    /// <summary>
    /// 游戏对象池，将不用的游戏对象缓存起来。
    /// </summary>
    public class GameObjectPool: MonoSingleton<GameObjectPool>
    {
        private readonly Dictionary<GameObject, ObjectPool<GameObject>> _pooledObjects = new();

        /// <summary>
        /// 获得一个指定预制体的实例。
        /// </summary>
        /// <param name="prefab">预制体</param>
        /// <returns><see cref="GameObject"/></returns>
        public GameObject Get(GameObject prefab)
        {
            if (!_pooledObjects.ContainsKey(prefab))
            {
                RegisterPrefab(prefab);
            }
            return _pooledObjects[prefab].Get();
        }
        
        private void RegisterPrefab(GameObject prefab)
        {
            CreateObjectPool(prefab);
        }

        private void CreateObjectPool(GameObject prefab)
        {
            GameObject CreateFunc()
            {
                return Instantiate(prefab);
            }

            void ActionOnGet(GameObject obj)
            {
                obj.SetActive(true);
            }

            void ActionOnRelease(GameObject obj)
            {
                obj.SetActive(false);
            }

            void ActionOnDestroy(GameObject obj)
            {
                Destroy(obj);
            }

            _pooledObjects[prefab] = new ObjectPool<GameObject>(
                CreateFunc, ActionOnGet, ActionOnRelease, ActionOnDestroy);
        }

        protected override void OnDestroy()
        {
            foreach (var prefab in _pooledObjects.Keys)
            {
                _pooledObjects[prefab].Clear();
            }
            _pooledObjects.Clear();
            base.OnDestroy();
        }


        /// <summary>
        /// 将对象返还给对象池。
        /// </summary>
        /// <param name="obj">被返回的对象</param>
        /// <param name="prefab">对象的预制体</param>
        public void Return(GameObject obj, GameObject prefab)
        {
            _pooledObjects[prefab].Release(obj);
        }

        /// <summary>
        /// 归还对象，将对象的父节点重置为对象池。
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="prefab"></param>
        public void ReturnWithReParent(GameObject obj, GameObject prefab)
        {
            Return(obj, prefab);
            obj.transform.SetParent(transform);
        }
    }
}
using System;
using UnityEngine;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine.Pool;
using System.Linq;
using UnityEngine.Assertions;

namespace GameLib.Network.NGO
{
    /// <summary>
    /// 存储对象池每个预制体配置信息的结构体。
    /// </summary>
    [Serializable]
    public struct PoolConfigObject
    {
        /// <summary>
        /// 生成对象的预制体。
        /// </summary>
        public GameObject prefab;
        
        /// <summary>
        /// 预热对象的数量
        /// </summary>
        /// <remarks>预热数量是指在真正使用对象之前提前创建出来的对象的数量。</remarks>
        public int preheatCount;
    }
    
    /// <summary>
    /// 用于维护<c>NetworkObject</c>的对象池，通过对象池可以让每次创建对象时不再重新分配内存，
    /// 而是复用对象池内的对象。一般用于生命周期短，数量较多的对象。
    /// </summary>
    public class NetworkObjectPool : NetworkSingleton<NetworkObjectPool>
    {
        [SerializeField]
        private List<PoolConfigObject> pooledPrefabsList;

        private HashSet<GameObject> _prefabs = new();

        private Dictionary<GameObject, ObjectPool<NetworkObject>> _pooledObjects = new();
        
        public override void OnNetworkSpawn()
        {
            foreach (var configObject in pooledPrefabsList)
            {
                RegisterPrefab(configObject);
            }
        }
        
        private void RegisterPrefab(PoolConfigObject configObject)
        {
            CreateObjectPool(configObject);
            PopulatePool(configObject);
            RegisterHandler(configObject.prefab);
        }

        private void CreateObjectPool(PoolConfigObject configObject)
        {
            NetworkObject CreateFunc()
            {
                return Instantiate(configObject.prefab).GetComponent<NetworkObject>();
            }

            void ActionOnGet(NetworkObject networkObject)
            {
                networkObject.gameObject.SetActive(true);
            }

            void ActionOnRelease(NetworkObject networkObject)
            {
                networkObject.gameObject.SetActive(false);
            }

            void ActionOnDestroy(NetworkObject networkObject)
            {
                Destroy(networkObject.gameObject);
            }

            _prefabs.Add(configObject.prefab);
            _pooledObjects[configObject.prefab] = new ObjectPool<NetworkObject>(
                CreateFunc, ActionOnGet, ActionOnRelease, ActionOnDestroy,
                defaultCapacity: configObject.preheatCount);
        }

        private void PopulatePool(PoolConfigObject configObject)
        {
            var preheatObjects = from _ in Enumerable.Range(0, configObject.preheatCount)
                select _pooledObjects[configObject.prefab].Get();

            foreach (var networkObject in preheatObjects)
            {
                _pooledObjects[configObject.prefab].Release(networkObject);
            }
        }

        private void RegisterHandler(GameObject prefab)
        {
            NetworkManager.Singleton.PrefabHandler.AddHandler(prefab, new PooledPrefabInstanceHandler(prefab, this));
        }

        public override void OnNetworkDespawn()
        {
            Destroy();
        }

        public void OnValidate()
        {
            for (var i = 0; i < pooledPrefabsList.Count; ++i)
            {
                var prefab = pooledPrefabsList[i].prefab;
                if (prefab != null)
                {
                    Assert.IsNotNull(prefab.GetComponent<NetworkObject>(),
                        $"{nameof(NetworkObjectPool)}: 位于索引{i}的预制体[{nameof(prefab.name)}]没有{nameof(NetworkObject)}组件。");
                }
            }
        }

        /// <summary>
        /// 获得一个指定预制体的实例，这个预制体必须在对象池里注册过。
        /// </summary>
        /// <remarks>这个方法只能在服务端调用，客户端会通过NGO自动进行处理</remarks>
        /// <param name="prefab">预制体</param>
        /// <param name="position">生成对象的位置</param>
        /// <param name="rotation">生成对象的角度</param>
        /// <returns></returns>
        public NetworkObject GetNetworkObject(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            var networkObject = _pooledObjects[prefab].Get();

            var networkTransform = networkObject.transform;
            networkTransform.position = position;
            networkTransform.rotation = rotation;

            return networkObject;
        }

        /// <summary>
        /// 将对象返还给对象池。
        /// </summary>
        /// <param name="networkObject">被返回的对象</param>
        /// <param name="prefab">对象的预制体</param>
        public void ReturnNetworkObject(NetworkObject networkObject, GameObject prefab)
        {
            _pooledObjects[prefab].Release(networkObject);
        }

        public override void Clear()
        {
            foreach (var prefab in _prefabs)
            {
                NetworkManager.Singleton.PrefabHandler.RemoveHandler(prefab);
                _pooledObjects[prefab].Clear();
            }
            _pooledObjects.Clear();
            _prefabs.Clear();
        }
    }

    /// <summary>
    /// 实现<c>INetworkPrefabInstanceHandler</c>接口，
    /// 当在<c>NetworkManager</c>注册预制体后，后续此预制体的创建和销毁将会通过此类进行。
    /// </summary>
    public class PooledPrefabInstanceHandler : INetworkPrefabInstanceHandler
    {
        private readonly GameObject _prefab;

        private readonly NetworkObjectPool _pool;

        public PooledPrefabInstanceHandler(GameObject prefab, NetworkObjectPool pool)
        {
            _prefab = prefab;
            _pool = pool;
        }

        public NetworkObject Instantiate(ulong ownerClientID, Vector3 position, Quaternion rotation)
        {
            return _pool.GetNetworkObject(_prefab, position, rotation);
        }

        public void Destroy(NetworkObject networkObject)
        {
            _pool.ReturnNetworkObject(networkObject, _prefab);
        }
    }
}
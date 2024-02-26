using GameLib.Network.NGO;
using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;
using GameLib.Common.Extension;


namespace UnitTest.Scene 
{
    public class PoolTest : MonoBehaviour
    {
        [SerializeField] private GameObject cubePrefab;

        private List<int> _range = new(){-4, -3, -2, -1, 0, 1, 2, 3, 4};

        private Queue<NetworkObject> _queue = new();
        
        void OnGUI()
        {
            GUILayout.BeginArea(new Rect(10, 10, 300, 300));
            if (NetworkManager.Singleton.IsServer)
            {
                SpawnCube();
                DespawnCube();
            }
            GUILayout.EndArea();
        }

        void SpawnCube()
        {
            if (GUILayout.Button("创建Cube"))
            {
                var networkObject = NetworkObjectPool.Instance.GetNetworkObject(cubePrefab, GetPosition(), 
                    Quaternion.identity);
                networkObject.Spawn();
                _queue.Enqueue(networkObject);
            }
        }

        private Vector3 GetPosition()
        {
            return new Vector3(_range.Choice(), _range.Choice());
        }
        
        private void DespawnCube()
        {
            if (GUILayout.Button("删除Cube"))
            {
                if (_queue.Count > 0)
                {
                    var networkObject = _queue.Dequeue();
                    networkObject.Despawn();
                }
            }
        }
    }
}

using System.Collections.Generic;
using GameLib.Common;
using GameLib.Common.Extension;
using UnityEngine;
using Random = System.Random;

namespace Tests.Scene
{
    public class GameObjectPoolTest : MonoBehaviour
    {
        [SerializeField] private GameObject cubePrefab;
        
        private List<int> _range = new(){-4, -3, -2, -1, 0, 1, 2, 3, 4};

        private Queue<GameObject> _queue = new();

        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(10, 10, 300, 300));
            if (GUILayout.Button("创建"))
            {
                Create();
            }
            if (GUILayout.Button("回收"))
            {
                Release();
            }
            GUILayout.EndArea();
        }
        
        void Create()
        {
            var obj = GameObjectPool.Instance.Get(cubePrefab);
            var rand = new Random();
            obj.transform.position = new Vector3(rand.Choice(_range), rand.Choice(_range));
            _queue.Enqueue(obj);
        }

        void Release()
        {
            if (_queue.Count > 0)
            {
                GameObjectPool.Instance.Return(_queue.Dequeue(), cubePrefab);
            }
        }
    }
}
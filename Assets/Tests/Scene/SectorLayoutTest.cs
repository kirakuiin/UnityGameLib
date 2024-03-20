using System.Collections.Generic;
using GameLib.UI.SectorLayout;
using UnityEngine;

namespace Tests.Scene
{
    public class SectorLayoutTest : MonoBehaviour
    {
        [SerializeField] private SectorLayout layout;
        [SerializeField] private GameObject prefab;

        private readonly Queue<GameObject> _objects = new();
    
        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(10, 10, 300, 300));
            if (GUILayout.Button("添加卡牌"))
            {
                var obj = Instantiate(prefab, Vector3.zero, Quaternion.identity);
                layout.Add(obj);
                _objects.Enqueue(obj);
            }

            if (GUILayout.Button("删除卡牌"))
            {
                if (_objects.Count > 0)
                {
                    var obj = _objects.Dequeue();
                    layout.Remove(obj);
                    Destroy(obj);
                }
            }
            GUILayout.EndArea();
        }
    }
}

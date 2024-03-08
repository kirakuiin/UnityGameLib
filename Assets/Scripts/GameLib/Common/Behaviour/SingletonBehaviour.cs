using System;
using UnityEngine;

namespace GameLib.Common.Behaviour
{
    /// <summary>
    /// 此游戏对象仅能存在一个，当创建新的对象时将自动删除。
    /// </summary>
    public class SingletonBehaviour : MonoBehaviour
    {
        private static SingletonBehaviour _instance;
        
        private void Awake()
        {
            if (_instance is null)
            {
                _instance = this;
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
    }
}
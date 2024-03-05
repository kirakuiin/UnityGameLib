using UnityEngine;

namespace GameLib.Common.Behaviour
{
    /// <summary>
    /// 场景切换时不会摧毁的<see cref="MonoBehaviour"/>。
    /// </summary>
    public class PersistBehaviour: MonoBehaviour
    {
        protected void Awake()
        {
            if (Application.isPlaying)
            {
                DontDestroyOnLoad(gameObject);
            }
        }
    }
}
using UnityEngine;

namespace GameLib.Common.Behaviour
{
    /// <summary>
    /// 经过一定事件后会自动销毁的<see cref="MonoBehaviour"/>
    /// </summary>
    public class SelfDestructBehaviour : MonoBehaviour
    {
        [Tooltip("存活时间")]
        [SerializeField]
        private float lifeSpanSeconds;

        protected void Start()
        {
            Destroy(gameObject, lifeSpanSeconds);
        }
    }
}
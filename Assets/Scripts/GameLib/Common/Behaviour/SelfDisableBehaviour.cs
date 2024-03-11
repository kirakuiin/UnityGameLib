using UnityEngine;

namespace GameLib.Common.Behaviour
{
    /// <summary>
    /// 每次启动后，经过一定时间后会自动禁用的<see cref="MonoBehaviour"/>
    /// </summary>
    public class SelfDisableBehaviour : MonoBehaviour
    {
        [Tooltip("自动关闭延迟")]
        [SerializeField]
        private float disableDelay;

        private float _disableTimestamp;

        protected virtual void Update()
        {
            if (Time.time >= _disableTimestamp)
            {
                gameObject.SetActive(false);
            }
        }

        protected void OnEnable()
        {
            _disableTimestamp = Time.time + disableDelay;
        }
    }
}
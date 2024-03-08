using System;
using Unity.Netcode;

// ReSharper disable once CheckNamespace
namespace GameLib.Network.NGO
{
    /// <summary>
    /// 有些类本身不能继承<see cref="NetworkBehaviour"/>，但必须和其相关联。
    /// 可以通过两个事件来监听网络对象的状态变化。
    /// </summary>
    public class NetworkHooks : NetworkBehaviour
    {
        /// <summary>
        /// 网络对象创建完毕时触发j。
        /// </summary>
        public event Action OnNetworkSpawnHook;

        /// <summary>
        /// 网络对象销毁完毕时触发。
        /// </summary>
        public event Action OnNetworkDespawnHook;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            OnNetworkSpawnHook?.Invoke();
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            OnNetworkDespawnHook?.Invoke();
        }
    }
}
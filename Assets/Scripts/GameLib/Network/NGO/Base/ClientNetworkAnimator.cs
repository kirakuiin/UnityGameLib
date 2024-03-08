using Unity.Netcode.Components;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace GameLib.Network.NGO
{
    /// <summary>
    /// 采用客户端权威模式的网络动画组件。也就是说允许客户端修改动画播放。
    /// </summary>
    [DisallowMultipleComponent]
    public class ClientNetworkAnimator: NetworkAnimator
    {
        protected override bool OnIsServerAuthoritative()
        {
            return false;
        }
    }
}
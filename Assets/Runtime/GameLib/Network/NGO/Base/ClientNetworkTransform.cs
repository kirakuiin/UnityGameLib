using Unity.Netcode.Components;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace GameLib.Network.NGO
{
    /// <summary>
    /// 采用客户端权威模式的网络变换组件。也就是说允许客户端修改对象位置。
    /// </summary>
    [DisallowMultipleComponent]
    public class ClientNetworkTransform : NetworkTransform
    {
        protected override bool OnIsServerAuthoritative()
        {
            return false;
        }
    }
}
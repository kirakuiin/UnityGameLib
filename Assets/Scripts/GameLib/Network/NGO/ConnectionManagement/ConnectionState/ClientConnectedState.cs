using UnityEngine;

// ReSharper disable once CheckNamespace
namespace GameLib.Network.NGO.ConnectionManagement
{
    /// <summary>
    /// 代表一个已连接的客户端，当断开连接时，如果没有给出断开原因则会转为
    /// <see cref="ClientReconnectingState"/>。否则转为<see cref="OfflineState"/>。
    /// ClientReconnecting state if no reason is given, or to the Offline state.
    /// </summary>
    public class ClientConnectedState : OnlineState
    {
        public override void Enter()
        {
        }

        public override void Exit()
        {
        }

        public override void OnClientDisconnected(ulong clientID)
        {
            var disconnectReason = NetManager.DisconnectReason;
            if (string.IsNullOrEmpty(disconnectReason))
            {
                Publisher.Publish(ConnectInfo.Create(ConnectStatus.Reconnecting));
                ConnManager.ChangeState<ClientReconnectingState>();
            }
            else
            {
                var connectInfo = JsonUtility.FromJson<ConnectInfo>(disconnectReason);
                Publisher.Publish(connectInfo);
                ConnManager.ChangeState<OfflineState>();
            }
        }
    }
}
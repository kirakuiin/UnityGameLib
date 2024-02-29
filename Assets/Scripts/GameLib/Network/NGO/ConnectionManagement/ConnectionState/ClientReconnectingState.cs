using GameLib.Network.NGO.Channel;

// ReSharper disable once CheckNamespace
namespace GameLib.Network.NGO.ConnectionManagement
{
    /// <summary>
    /// 表示客户端断开后尝试重新连接服务器。
    /// 有指定的尝试次数和超时时间。如果在满足条件的情况下重新连接上，
    /// 则进入<see cref="ClientConnectedState"/>；否则进入<see cref="OfflineState"/>。
    /// <remarks>如果断开给出了某些具体原因，可能不会触发重连，直接进入<see cref="OfflineState"/></remarks>
    /// </summary>
    public class ClientReconnectingState : ClientConnectingState
    {
        public ClientReconnectingState(ConnectionManager manager, IPublisher<ConnectStatus> publisher) : base(manager, publisher)
        {
        }
        
        public override void Enter()
        {
            throw new System.NotImplementedException();
        }

        public override void Exit()
        {
            throw new System.NotImplementedException();
        }

        public override string GetStateType()
        {
            return nameof(ClientReconnectingState);
        }
    }
}
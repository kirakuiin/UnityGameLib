using GameLib.Network.NGO.Channel;

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
        public ClientConnectedState(ConnectionManager manager, IPublisher<ConnectStatus> publisher) : base(manager, publisher)
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
            return nameof(ClientConnectedState);
        }
    }
}
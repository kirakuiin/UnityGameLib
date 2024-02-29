using GameLib.Network.NGO.Channel;

// ReSharper disable once CheckNamespace
namespace GameLib.Network.NGO.ConnectionManagement
{
    /// <summary>
    /// 表示客户端正尝试连接服务器，进入此状态则启动客户端。
    /// 如果连接成功则进入<see cref="ClientConnectedState"/>；否则进入<see cref="OfflineState"/>。
    /// </summary>
    public class ClientConnectingState : OnlineState
    {
        public ClientConnectingState(ConnectionManager manager, IPublisher<ConnectStatus> publisher) : base(manager, publisher)
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
            return nameof(ClientConnectingState);
        }
    }
}
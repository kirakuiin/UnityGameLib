using GameLib.Network.NGO.Channel;

// ReSharper disable once CheckNamespace
namespace GameLib.Network.NGO.ConnectionManagement
{
    /// <summary>
    /// 此状态代表一个监听中的主机，处理客户端连接。
    /// 当主机关闭时，转换到<see cref="OfflineState"/>状态。
    /// </summary>
    public class HostingState : OnlineState
    {
        public HostingState(ConnectionManager manager, IPublisher<ConnectStatus> publisher) : base(manager, publisher)
        {
        }

        public override void Enter()
        {
        }

        public override void Exit()
        {
        }

        public override string GetStateType()
        {
            return nameof(HostingState);
        }
    }
}
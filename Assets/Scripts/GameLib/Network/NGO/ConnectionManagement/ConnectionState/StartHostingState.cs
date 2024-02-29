using GameLib.Network.NGO.Channel;

// ReSharper disable once CheckNamespace
namespace GameLib.Network.NGO.ConnectionManagement
{
    /// <summary>
    /// 代表的是刚启动主机的时候。如果主机成功启动则进入<see cref="HostingState"/>状态；
    /// 否则进入<see cref="OfflineState"/>状态。
    /// </summary>
    public class StartHostingState : OnlineState
    {
        public StartHostingState(ConnectionManager manager, IPublisher<ConnectStatus> publisher) : base(manager, publisher)
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
            return nameof(StartHostingState);
        }
    }
}
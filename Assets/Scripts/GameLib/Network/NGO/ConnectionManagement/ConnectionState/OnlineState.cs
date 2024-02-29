using GameLib.Network.NGO.Channel;

// ReSharper disable once CheckNamespace
namespace GameLib.Network.NGO.ConnectionManagement
{
    /// <summary>
    /// 表达在线状态的抽象类。
    /// </summary>
    public abstract class OnlineState : ConnectionState
    {
        protected readonly ConnectionManager Manager;

        protected readonly IPublisher<ConnectStatus> Publisher;
        
        protected OnlineState(ConnectionManager manager, IPublisher<ConnectStatus> publisher)
        {
            Manager = manager;
            Publisher = publisher;
        }

        public override void OnUserRequestShutdown()
        {
            Publisher.Publish(ConnectStatus.UserRequestedDisconnect);
            Manager.ChangeState<OfflineState>();
        }

        public override void OnTransportFailure()
        {
            Manager.ChangeState<OfflineState>();
        }
    }
}
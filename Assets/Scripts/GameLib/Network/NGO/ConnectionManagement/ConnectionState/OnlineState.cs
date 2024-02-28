using GameLib.Network.NGO.Channel;

// ReSharper disable once CheckNamespace
namespace GameLib.Network.NGO.ConnectionManagement
{
    public class OnlineState : ConnectionState
    {
        public OnlineState(ConnectionManager manager, IPublisher<ConnectStatus> publisher)
            : base(manager, publisher)
        {
        }

        public override void Enter()
        {
        }

        public override void Exit()
        {
        }
    }
}
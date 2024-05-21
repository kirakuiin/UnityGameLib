// ReSharper disable once CheckNamespace
namespace GameLib.Network.NGO.ConnectionManagement
{
    /// <summary>
    /// 表达在线状态的抽象类。
    /// </summary>
    public abstract class OnlineState : ConnectionState
    {
        public override void OnUserRequestShutdown()
        {
            Publisher.Publish(ConnectInfo.Create(ConnectStatus.UserRequestedDisconnect));
            ConnManager.ChangeState<OfflineState>();
        }

        public override void OnTransportFailure()
        {
            ConnManager.ChangeState<OfflineState>();
        }
    }
}
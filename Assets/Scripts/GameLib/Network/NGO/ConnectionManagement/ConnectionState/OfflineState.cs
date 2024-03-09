using System.Net;
using Unity.Netcode;

// ReSharper disable once CheckNamespace
namespace GameLib.Network.NGO.ConnectionManagement
{
    /// <summary>
    /// 表达离线状态的类。
    /// </summary>
    public class OfflineState : ConnectionState
    {
        public override void Enter()
        {
            NetworkManager.Singleton.Shutdown();
        }

        public override void Exit()
        {
        }

        public override void StartClient()
        {
            ConnManager.ChangeState<ClientConnectingState>();
        }

        public override void StartHost()
        {
            ConnManager.ChangeState<StartHostingState>();
        }
    }
}
using Unity.Netcode;
using System;

// ReSharper disable once CheckNamespace
namespace GameLib.Network.NGO.ConnectionManagement
{
    /// <summary>
    /// 代表的是刚启动主机的时候。如果主机成功启动则进入<see cref="HostingState"/>状态；
    /// 否则进入<see cref="OfflineState"/>状态。
    /// </summary>
    public class StartHostingState : OnlineState
    {
        private readonly ConnectionMethod _connectionMethod;

        public StartHostingState(ConnectionMethod method)
        {
            _connectionMethod = method;
        }
        
        public override void Enter()
        {
            Start();
        }

        private async void Start()
        {
            try
            {
                await _connectionMethod.SetupHostConnectionAsync();

                if (!NetManager.StartHost())
                {
                    StartFailed();
                }
            }
            catch (Exception e)
            {
                StartFailed();
                throw new CommonConnectionException(e);
            }
        }

        private void StartFailed()
        {
            Publisher.Publish(ConnectStatus.StartHostFailed);
            ConnManager.ChangeState<OfflineState>();
        }

        public override void OnServerStarted()
        {
            Publisher.Publish(ConnectStatus.Success);
            ConnManager.ChangeState<HostingState>();
        }

        public override void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
        {
            var clientID = request.ClientNetworkId;

            // 这个调用发生在启动主机的时候，因此验证主机自身即可。
            if (clientID == NetManager.LocalClientId)
            {
                SetResponse(request, response);
            }
        }

        /// <summary>
        /// 设置连接认证的回复数据。
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        protected void SetResponse(NetworkManager.ConnectionApprovalRequest request,
            NetworkManager.ConnectionApprovalResponse response)
        {
                response.Approved = true;
                response.CreatePlayerObject = false;
        }

        public override void OnServerStopped()
        {
            StartFailed();
        }

        public override void Exit()
        {
        }

        public override string GetStateType()
        {
            return nameof(StartHostingState);
        }
    }
}
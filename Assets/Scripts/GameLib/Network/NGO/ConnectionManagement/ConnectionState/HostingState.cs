using Unity.Netcode;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace GameLib.Network.NGO.ConnectionManagement
{
    /// <summary>
    /// 此状态代表一个监听中的主机，处理客户端连接。
    /// 当主机关闭时，转换到<see cref="OfflineState"/>状态。
    /// </summary>
    public class HostingState : OnlineState
    {
        private const int MaxPayloadLength = 1024;
        
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

        public override void OnUserRequestShutdown()
        {
            var reason = JsonUtility.ToJson(ConnectStatus.HostEndSession);
            foreach (var clientID in NetManager.ConnectedClientsIds)
            {
                if (clientID != NetManager.LocalClientId)
                {
                    NetManager.DisconnectClient(clientID, reason);
                }
            }
            base.OnUserRequestShutdown();
        }

        public override void OnServerStopped()
        {
            Publisher.Publish(ConnectStatus.GenericDisconnect);
            ConnManager.ChangeState<OfflineState>();
        }

        public override void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
        {
            if (IsDosAttack(request)) return;
            SetResponse(request, response);
        }

        private bool IsDosAttack(NetworkManager.ConnectionApprovalRequest request)
        {
            return request.Payload.Length > MaxPayloadLength;
        }

        /// <summary>
        /// 设置连接认证的回复数据。
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        protected void SetResponse(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
        {
            var status = GetConnectStatus(request);
            if (status == ConnectStatus.Success)
            {
                response.Approved = true;
                response.CreatePlayerObject = false;
            }
            else
            {
                response.Approved = false;
                response.Reason = JsonUtility.ToJson(status);
            }
        }

        /// <summary>
        /// 获得当前的连接状态。
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        protected ConnectStatus GetConnectStatus(NetworkManager.ConnectionApprovalRequest request)
        {
            var payload = ConnectionMethod.DumpPayload<ConnectionPayload>(request.Payload);
            if (NetManager.ConnectedClientsIds.Count >= ConnManager.config.maxConnectedPlayerNum)
            {
                return ConnectStatus.ServerFull;
            }
            if (payload.isDebug != Debug.isDebugBuild)
            {
                return ConnectStatus.IncompatibleBuildType;
            }
            
            return ConnectStatus.Success;
        }
    }
}
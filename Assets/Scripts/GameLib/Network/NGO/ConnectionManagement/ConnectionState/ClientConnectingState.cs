using System;
using System.Threading.Tasks;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace GameLib.Network.NGO.ConnectionManagement
{
    /// <summary>
    /// 表示客户端正尝试连接服务器，进入此状态则启动客户端。
    /// 如果连接成功则进入<see cref="ClientConnectedState"/>；否则进入<see cref="OfflineState"/>。
    /// </summary>
    public class ClientConnectingState : OnlineState
    {
        protected readonly ConnectionMethod ConnMethod;

        public ClientConnectingState(ConnectionMethod method)
        {
            ConnMethod = method;
        }
        
        public override void Enter()
        {
#pragma warning disable CS4014 // 由于此调用不会等待，因此在调用完成前将继续执行当前方法
            ConnectAsync();
#pragma warning restore CS4014 // 由于此调用不会等待，因此在调用完成前将继续执行当前方法
        }

        protected async Task ConnectAsync()
        {
            try
            {
                await ConnMethod.SetupClientConnectionAsync();

                if (!NetManager.StartClient())
                {
                    throw new Exception("启动客户端失败！");
                }
            }
            catch (Exception e)
            {
                ConnectFailed();
                throw new CommonConnectionException(e);
            }
        }

        private void ConnectFailed()
        {
            var disconnectReason = NetManager.DisconnectReason;
            if (string.IsNullOrEmpty(disconnectReason))
            {
                Publisher.Publish(ConnectStatus.StartClientFailed);
            }
            else
            {
                var connectStatus = JsonUtility.FromJson<ConnectStatus>(disconnectReason);
                Publisher.Publish(connectStatus);
            }
            ConnManager.ChangeState<OfflineState>();
        }

        public override void OnClientConnected(ulong clientID)
        {
            Publisher.Publish(ConnectStatus.Success);
            ConnManager.ChangeState<ClientConnectedState>();
        }

        public override void OnClientDisconnected(ulong clientID)
        {
            ConnectFailed();
        }
        
        public override void Exit()
        {
        }
    }
}
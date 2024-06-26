﻿using Unity.Netcode;
using UnityEngine;
using System.Linq;
using GameLib.Common.Utility;

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

        public override void OnUserRequestShutdown()
        {
            var idsNeedToDisconnect = (from clientID in NetManager.ConnectedClientsIds
                where clientID != NetManager.LocalClientId
                select clientID).ToList();
            
            var reason = JsonUtility.ToJson(ConnectInfo.Create(ConnectStatus.HostEndSession));
            foreach (var clientID in idsNeedToDisconnect)
            {
                NetManager.DisconnectClient(clientID, reason);
            }
            base.OnUserRequestShutdown();
        }

        public override void OnServerStopped()
        {
            Publisher.Publish(ConnectInfo.Create(ConnectStatus.GenericDisconnect));
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
        protected virtual void SetResponse(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
        {
            var info = GetConnectInfo(request);
            if (info.Status == ConnectStatus.Success)
            {
                response.Approved = true;
                response.CreatePlayerObject = true;
            }
            else
            {
                response.Approved = false;
                response.Reason = JsonUtility.ToJson(info);
            }
        }

        /// <summary>
        /// 获得当前的连接状态。
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        protected virtual ConnectInfo GetConnectInfo(NetworkManager.ConnectionApprovalRequest request)
        {
            var payload = SerializeTool.Deserialize<ConnectionPayload>(request.Payload);
            if (NetManager.ConnectedClientsIds.Count >= ConnManager.config.maxConnectedPlayerNum)
            {
                return ConnectInfo.Create(ConnectStatus.ServerFull);
            }
            if (payload.isDebug != Debug.isDebugBuild)
            {
                return ConnectInfo.Create(ConnectStatus.IncompatibleBuildType);
            }
            
            return ConnectInfo.Create(ConnectStatus.Success);
        }
    }
}
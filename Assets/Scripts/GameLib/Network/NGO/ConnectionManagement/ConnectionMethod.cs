using System;
using System.Net;
using System.Threading.Tasks;
using GameLib.Common;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

namespace GameLib.Network.NGO.ConnectionManagement
{
    /// <summary>
    /// 连接时客户端携带的数据。
    /// </summary>
    [Serializable]
    public struct ConnectionPayload
    {
        /// <summary>
        /// 玩家的唯一ID
        /// </summary>
        public string playerID;

        /// <summary>
        /// 是否为测试版本。
        /// </summary>
        public bool isDebug;
    }

    /// <summary>
    /// 重连结果。
    /// </summary>
    public struct ReconnectResult
    {
        /// <summary>
        /// 重连是否成功。
        /// </summary>
        public bool IsSuccess;

        /// <summary>
        /// 是否继续重连。
        /// </summary>
        public bool ShouldTryAgain;
    }
    
    /// <summary>
    /// 代表连接方式的抽象类。内部包含了NGO所需要的全部连接设置。
    /// </summary>
    public abstract class ConnectionMethod
    {
        protected readonly string PlayerID;
        
        protected ConnectionMethod(string playerID)
        {
            PlayerID = playerID;
        }
        
        /// <summary>
        /// 异步地设置主机端的连接。
        /// </summary>
        /// <remarks>必须要在<c>NetworkManager</c>启动前进行设置。</remarks>
        /// <returns><c>Task</c></returns>
        public abstract Task SetupHostConnectionAsync();

        /// <summary>
        /// 异步地设置客户端端的连接。
        /// </summary>
        /// <remarks>必须要在<c>NetworkManager</c>启动前进行设置。</remarks>
        /// <returns></returns>
        public abstract Task SetupClientConnectionAsync();

        /// <summary>
        /// 异步地设置客户端重连。
        /// </summary>
        /// <returns></returns>
        public abstract Task<ReconnectResult> SetupClientReconnectAsync();
        
        /// <summary>
        /// 设置连接时携带的数据。
        /// </summary>
        /// <param name="playerID">玩家ID</param>
        protected virtual void SetConnectionPayload()
        {
            var payload = new ConnectionPayload()
            {
                playerID = PlayerID,
                isDebug = Debug.isDebugBuild,
            };
            NetworkManager.Singleton.NetworkConfig.ConnectionData = CreatePayload(payload);
        }
        
        private byte[] CreatePayload<T>(T obj) where T : struct
        {
            var payloadStr = JsonUtility.ToJson(obj);
            return System.Text.Encoding.UTF8.GetBytes(payloadStr);
        }

        /// <summary>
        /// 将结构体从字节流中解压出来。
        /// </summary>
        /// <param name="payload">字节流</param>
        /// <typeparam name="T"></typeparam>
        /// <returns>结构体对象</returns>
        public static T DumpPayload<T>(byte[] payload) where T : struct
        {
            var payloadStr = System.Text.Encoding.UTF8.GetString(payload);
            return JsonUtility.FromJson<T>(payloadStr);
        }
        
    }
    
    /// <summary>
    /// 通过IP和端口直接连接的连接方式。
    /// </summary>
    public class DirectIPConnectionMethod : ConnectionMethod
    {
        private readonly IPEndPoint _endPoint;

        public DirectIPConnectionMethod(IPEndPoint endPoint)
        : base(PlayerGuid.GetGuidByMachine())
        {
            _endPoint = endPoint;
        }
        
        public override async Task SetupHostConnectionAsync()
        {
            CommonSetup();
        }

        private void CommonSetup()
        {
            SetConnectionPayload();
            var utp = (UnityTransport)NetworkManager.Singleton.NetworkConfig.NetworkTransport;
            utp.SetConnectionData(_endPoint.Address.ToString(), (ushort)_endPoint.Port);
        }

        public override async Task SetupClientConnectionAsync()
        {
            CommonSetup();
        }

        public override async Task<ReconnectResult> SetupClientReconnectAsync()
        {
            return new ReconnectResult() { IsSuccess = true, ShouldTryAgain = true };
        }
    }
}
using System.Net;
using Unity.Netcode;

// ReSharper disable once CheckNamespace
namespace GameLib.Network.NGO.ConnectionManagement
{
    /// <summary>
    /// 代表连接状态的抽象类
    /// </summary>
    public abstract class ConnectionState
    {
        /// <summary>
        /// 处理进入状态前的准备工作。
        /// </summary>
        public abstract void Enter();

        /// <summary>
        /// 处理离开状态前的清理操作。
        /// </summary>
        public abstract void Exit();

        /// <summary>
        /// 获得状态的名称。
        /// </summary>
        /// <remarks>如果想要替换某个非抽象类的功能，那么不应该重写此函数，要保持同继承树内名称相同。</remarks>
        /// <returns>字符串</returns>
        public abstract string GetStateType();
        
        /// <summary>
        /// 处理客户端连接时的工作。
        /// </summary>
        /// <param name="clientID">客户端ID</param>
        public virtual void OnClientConnected(ulong clientID) {}
        
        /// <summary>
        /// 处理客户端断开连接时的工作。
        /// </summary>
        /// <param name="clientID">客户端ID</param>
        public virtual void OnClientDisconnected(ulong clientID) {}
        
        /// <summary>
        /// 处理服务端启动后的工作。
        /// </summary>
        public virtual void OnServerStarted() {}
        
        /// <summary>
        /// 处理服务端关闭后的工作。
        /// </summary>
        public virtual void OnServerStopped() {}
        
        /// <summary>
        /// 启动客户端。
        /// </summary>
        /// <param name="ipAddress">服务端的IP地址</param>
        /// <param name="port">服务端端口号</param>
        public virtual void StartClient(IPAddress ipAddress, int port) {}

        /// <summary>
        /// 启动主机。
        /// </summary>
        public virtual void StartHost() {}

        /// <summary>
        /// 处理玩家主动要求关闭连接。
        /// </summary>
        public virtual void OnUserRequestShutdown() {}
        
        /// <summary>
        /// 处理传输数据失败。
        /// </summary>
        public virtual void OnTransportFailure() {}
        
        /// <summary>
        /// 处理客户端的连接验证工作。
        /// </summary>
        /// <param name="request">客户端请求数据</param>
        /// <param name="response">服务端回复数据</param>
        public virtual void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request,
            NetworkManager.ConnectionApprovalResponse response) {}
    }
}
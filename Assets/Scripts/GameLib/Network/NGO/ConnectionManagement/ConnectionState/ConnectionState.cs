using GameLib.Common;
using GameLib.Network.NGO.Channel;
using Unity.Netcode;

// ReSharper disable once CheckNamespace
namespace GameLib.Network.NGO.ConnectionManagement
{
    /// <summary>
    /// 可以重新设置连接方法的类型。
    /// </summary>
    public interface IConnectionResettable
    {
        /// <summary>
        /// 重新设置连接方法。
        /// </summary>
        /// <param name="method">连接方法</param>
        public void SetConnectionMethod(ConnectionMethod method);
    }
    
    /// <summary>
    /// 代表连接状态的抽象类
    /// </summary>
    public abstract class ConnectionState
    {
        protected readonly IPublisher<ConnectInfo> Publisher = ServiceLocator.Instance.Get<IPublisher<ConnectInfo>>();

        protected readonly ConnectionManager ConnManager = ConnectionManager.Instance;

        protected readonly NetworkManager NetManager = NetworkManager.Singleton;

        /// <summary>
        /// 处理进入状态前的准备工作。
        /// </summary>
        public abstract void Enter();

        /// <summary>
        /// 处理离开状态前的清理操作。
        /// </summary>
        public abstract void Exit();
        
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
        public virtual void StartClient() {}

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
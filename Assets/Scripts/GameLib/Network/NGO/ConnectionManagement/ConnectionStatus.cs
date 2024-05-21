using Unity.Collections;
using Unity.Netcode;

namespace GameLib.Network.NGO.ConnectionManagement
{
    /// <summary>
    /// 网络连接状态
    /// </summary>
    public enum ConnectStatus
    {
        Undefined,
        
        /// <summary>
        /// 客户端连接成功或者重连成功。
        /// </summary>
        Success,
        
        /// <summary>
        /// 服务端容量已满，拒绝加入。
        /// </summary>
        ServerFull,
        
        /// <summary>
        /// 另一个客户端登录，导致当前客户端被踢出。
        /// </summary>
        LoggedInAgain,
        
        /// <summary>
        /// 由客户端主动触发的断开连接。
        /// </summary>
        UserRequestedDisconnect,
        
        /// <summary>
        /// 服务端断开连接，无原因。
        /// </summary>
        GenericDisconnect,
        
        /// <summary>
        /// 客户端断开连接后尝试重连。
        /// </summary>
        Reconnecting,
        
        /// <summary>
        /// 服务端和客户端的构建版本不一致。
        /// </summary>
        IncompatibleBuildType,
        
        /// <summary>
        /// 主机端主动终止连接。
        /// </summary>
        HostEndSession,
        
        /// <summary>
        /// 主机端启动失败。
        /// </summary>
        StartHostFailed,
        
        /// <summary>
        /// 客户端启动失败，可能是连接服务器失败或是提供的ip信息错误。
        /// </summary>
        StartClientFailed,
        
        /// <summary>
        /// 客户端认证失败，比如说密码错误。
        /// </summary>
        ApprovalFailed,
        
        /// <summary>
        /// 自定义错误。
        /// </summary>
        UserDefined,
    }

    /// <summary>
    /// 连接信息。
    /// </summary>
    public struct ConnectInfo : INetworkSerializeByMemcpy
    {
        /// <summary>
        /// 创建一个连接信息对象。
        /// </summary>
        /// <param name="status"></param>
        /// <param name="desc"></param>
        /// <returns></returns>
        public static ConnectInfo Create(ConnectStatus status, string desc = "")
        {
            return new ConnectInfo { Status = status, Desc = desc};
        }
        
        /// <summary>
        /// 连接状态。
        /// </summary>
        public ConnectStatus Status;

        /// <summary>
        /// 具体描述信息。
        /// </summary>
        public FixedString32Bytes Desc;

        public override string ToString()
        {
            return Desc == "" ? $"{Status}" : $"{Status}[{Desc}]";
        }
    }
}
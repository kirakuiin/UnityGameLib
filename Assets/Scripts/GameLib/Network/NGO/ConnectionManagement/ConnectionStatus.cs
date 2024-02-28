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
        /// 服务端和客户端的版本不一致。
        /// </summary>
        IncompatibleVersionType,
        
        /// <summary>
        /// 服务端主动终止连接。
        /// </summary>
        ServerEndSession,
        
        /// <summary>
        /// 服务端启动失败。
        /// </summary>
        StartServerFailed,
        
        /// <summary>
        /// 客户端启动失败，可能是连接服务器失败或是提供的ip信息错误。
        /// </summary>
        StartClientFailed,
    }
}
using System;
using GameLib.Common;
namespace GameLib.Network.NGO.ConnectionManagement
{
    /// <summary>
    /// 表示某个连接状态不存在的异常。
    /// </summary>
    internal class NotExistConnectionStateException : LibException
    {
        public NotExistConnectionStateException(string stateType) : base($"State: {stateType} not exist!")
        {
        }
    }

    /// <summary>
    /// 通用连接异常。
    /// </summary>
    internal class CommonConnectionException : LibException
    {
        public CommonConnectionException(Exception e) : base(e.ToString(), e)
        {
        }
    }
}
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
}
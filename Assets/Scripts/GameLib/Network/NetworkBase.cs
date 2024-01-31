using System.Net;

namespace GameLib.Network
{

    /// <summary>
    /// 提供若干和网络地址相关函数的静态类。
    /// </summary>
    public static class Address
    {
        /// <summary>
        /// 返回默认的终结点对象。
        /// </summary>
        public static readonly IPEndPoint DefaultIPEndPoint = new IPEndPoint(IPAddress.Any, 0);

        /// <summary>
        /// 得到本机的IP地址。
        /// </summary>
        /// <returns><c>IPAddress</c>代表本机的IP地址</returns>
        public static IPAddress GetLocalIPAddress()
        {
            var ipAddressList = Dns.GetHostEntry(Dns.GetHostName()).AddressList;
            return ipAddressList[^1];
        }

        /// <summary>
        /// 获得广播的终结点对象。
        /// </summary>
        /// <param name="port">广播使用的端口号</param>
        /// <returns><c>IPEndPoint</c>代表一个网络上的IP,端口对</returns>
        public static IPEndPoint GetBroadcastIPEndPoint(int port)
        {
            return new IPEndPoint(IPAddress.Broadcast, port);
        }
    }
}

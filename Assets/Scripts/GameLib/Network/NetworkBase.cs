using System.Net;
using GameLib.Network.NGO;

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

        /// <summary>
        /// 根据输入的ip和端口获得<see cref="IPEndPoint"/>对象。
        /// </summary>
        /// <param name="ip">ip地址</param>
        /// <param name="port">端口号</param>
        /// <returns>IPEndPoint对象</returns>
        /// <exception cref="IPParseException"></exception>
        public static IPEndPoint GetIPEndPoint(string ip, ushort port)
        {
            if (IPAddress.TryParse(ip, out var ipAddress))
            {
                return new IPEndPoint(ipAddress, port);
            }
            throw new IPParseException(ip);
        }
    }
}

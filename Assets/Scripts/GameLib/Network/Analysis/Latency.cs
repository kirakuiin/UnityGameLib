using System.Net;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using GameLib.Common;

namespace GameLib.Network.Analysis
{
    /// <summary>
    /// 获得到目标主机的延迟信息
    /// </summary>
    public class Latency
    {
        private readonly Ping _sender = new ();

        private readonly int _timeout;

        /// <value>
        /// 获得目标主机IP地址
        /// </value>
        private readonly string _targetAddress;

        /// <summary>
        /// 默认探测超时时间(s)
        /// </summary>
        private const int DefaultTimeout = 10;

        /// <summary>
        /// 以IP/host地址字符串和超时时间构造对象。
        /// </summary>
        /// <param name="targetIP"></param>
        /// <param name="timeout"></param>
        public Latency(string targetIP, int timeout=DefaultTimeout)
        {
            _targetAddress = targetIP;
            this._timeout = timeout;
        }

        /// <summary>
        /// 异步探测网络延迟信息。
        /// </summary>
        /// <returns><c>Task&lt;NetworkStatus&gt;</c></returns>
        public async Task<NetworkStatus> GetLatencyAsync()
        {
            var ipList = await Dns.GetHostAddressesAsync(_targetAddress);
            if (ipList.Length == 0) return NetworkStatus.CreateUnreachableStatus(_targetAddress);
            var reply = await _sender.SendPingAsync(ipList[0], TimeScalar.ConvertSecondToMs(_timeout));
            return new NetworkStatus(reply);
        }

        /// <summary>
        /// 网络状况结果
        /// </summary>
        public readonly struct NetworkStatus
        {
            private const int UnreachableTime = 9999;
            
            /// <summary>
            /// 创建不可达状态。
            /// </summary>
            /// <returns></returns>
            public static NetworkStatus CreateUnreachableStatus(string ip)
            {
                return new NetworkStatus(ip);
            }

            private NetworkStatus(string ip)
            {
                TargetIP = IPAddress.TryParse(ip, out var ipAddress) ? ipAddress : IPAddress.None;
                Status = IPStatus.DestinationUnreachable;
                Latency = UnreachableTime;
            }
            
            public NetworkStatus(PingReply reply)
            {
                TargetIP = reply.Address;
                Status = reply.Status;
                Latency = reply.RoundtripTime;
            }
            /// <summary>
            /// 目标主机IP
            /// </summary>
            public readonly IPAddress TargetIP;
            /// <summary>
            /// 网络状态
            /// </summary>
            public readonly IPStatus Status;
            /// <summary>
            /// 延迟(ms)
            /// </summary>
            public readonly long Latency;

            public override string ToString()
            {
                return $"To {TargetIP}, Status is {Status}, Latency is {Latency}ms";
            }

            /// <summary>
            /// 目标地址是否可达
            /// </summary>
            public bool IsReachable => Status == IPStatus.Success;
        }
    }
}
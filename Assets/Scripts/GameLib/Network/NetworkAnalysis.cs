using System.Net;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using GameLib.Network.Base;

namespace GameLib.Network.Analysis
{
    /// <summary>
    /// 获得到目标主机的延迟信息
    /// </summary>
    public class Latency
    {

        private Ping _sender = new Ping();

        private int _timeout;

        /// <value>
        /// 获得目标主机IP地址
        /// </value>
        public string _targetAddress = Address.GetLocalIPAddress().ToString();

        /// <summary>
        /// 默认探测超时时间(s)
        /// </summary>
        public const int DEFAULT_TIMEOUT = 10;

        /// <summary>
        /// 以IP/host地址字符串和超时时间构造对象。
        /// </summary>
        /// <param name="targetIP"></param>
        /// <param name="timeout"></param>
        public Latency(string targetIP, int timeout=DEFAULT_TIMEOUT)
        {
            _targetAddress = targetIP;
            _timeout = timeout;
        }

        /// <summary>
        /// 异步探测网络延迟信息。
        /// </summary>
        /// <returns><c>Task&lt;NetworkStatus&gt;</c></returns>
        public async Task<NetworkStatus> GetLatencyAsync()
        {
           var reply = await _sender.SendPingAsync(_targetAddress, TimeScalar.SecondToMs(_timeout));
           return new NetworkStatus(reply);
        }

        /// <summary>
        /// 网络状况结果
        /// </summary>
        public struct NetworkStatus
        {
            public NetworkStatus(PingReply reply)
            {
                TargetIP = reply.Address;
                Status = reply.Status;
                Latency = reply.RoundtripTime;
            }
            /// <summary>
            /// 目标主机IP
            /// </summary>
            public IPAddress TargetIP;
            /// <summary>
            /// 网络状态
            /// </summary>
            public IPStatus Status;
            /// <summary>
            /// 延迟(ms)
            /// </summary>
            public long Latency;

            public override string ToString()
            {
                return $"To {TargetIP}, Status is {Status}, Latency is {Latency}ms";
            }

            /// <summary>
            /// 目标地址是否可达
            /// </summary>
            /// <returns>如果可达返回<c>true</c></returns>
            public bool IsReachable() => Status == IPStatus.Success;
        }
    }
}
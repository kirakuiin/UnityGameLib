using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using GameLib.Common;

namespace GameLib.Network
{
    /// <summary>
    /// 提供广播发送功能
    /// </summary>
    public class BroadcastSender
    {
        private readonly UdpClient _udpSender = new(Address.DefaultIPEndPoint);

        private IPEndPoint _endPoint = Address.GetBroadcastIPEndPoint(DefaultBroadcastPort);

        /// <summary>
        /// 默认的广播端口。
        /// </summary>
        public const int DefaultBroadcastPort = 3344;

        /// <value>
        /// 广播默认发送的消息。
        /// <para>
        /// 默认值为空
        /// </para>
        /// </value>
        public string SentMessage {get; set;} = "";

        /// <value>
        /// 广播的端口号
        /// <para>
        /// 默认值为<c>DefaultBroadcastPort</c>
        /// </para>
        /// </value>
        public int BroadcastPort
        {
            set => _endPoint = Address.GetBroadcastIPEndPoint(value);
            get => _endPoint.Port;
        }

        /// <summary>
        /// 广播<c>SentMessage</c>内的消息。
        /// </summary>
        /// <returns>发送的字节数。</returns>
        public int Broadcast()
        {
            return Broadcast(SentMessage);
        }

        /// <summary>
        /// 广播指定的字符串消息。
        /// </summary>
        /// <param name="message">待广播字符串</param>
        /// <returns>发送的字节数。</returns>
        public int Broadcast(string message)
        {
            var data = Encoding.UTF8.GetBytes(message);
            return _udpSender.Send(data, data.Length, _endPoint);
        }
    }

    /// <summary>
    /// 提供接受Udp广播功能。
    /// </summary>
    public class BroadcastReceiver
    {
        private readonly UdpClient _udpReceiver;

        /// <summary>
        /// 以广播端口构建一个接受对象
        /// </summary>
        /// <param name="broadcastPort"></param>
        public BroadcastReceiver(int broadcastPort=BroadcastSender.DefaultBroadcastPort)
        {
            _udpReceiver = new UdpClient(new IPEndPoint(IPAddress.Any, broadcastPort));
        }

        /// <summary>
        /// 异步的接受一条广播。
        /// </summary>
        /// <returns><c>Task&lt;UdpReceiveResult&gt;</c></returns>
        /// <exception cref="SocketException"></exception>
        public async Task<UdpReceiveResult> ReceiveAsync()
        {
            return await _udpReceiver.ReceiveAsync();
        }

    }

    /// <summary>
    /// 提供定时发出广播的功能
    /// </summary>
    public class TimedBroadcaster
    {
        private readonly BroadcastSender _sender;

        private const float DefaultBroadcastInterval = 1.0f;

        public bool IsSending {private set; get;} = false;

        /// <summary>
        /// 以广播端口为参数构造对象
        /// </summary>
        /// <param name="broadcastPort"></param>
        public TimedBroadcaster(int broadcastPort=BroadcastSender.DefaultBroadcastPort)
        {
            _sender = new(){BroadcastPort=broadcastPort};
        }

        /// <value>
        /// 广播发送间隔(s)
        /// <para>默认值为<c>DefaultBroadcastInterval</c></para>
        /// </value>
        public float BroadcastInterval {set; get;} = DefaultBroadcastInterval;

        /// <summary>
        /// 开启广播，发送指定消息
        /// </summary>
        /// <param name="message">消息字符串</param>
        public async void StartBroadcast(string message)
        {
            IsSending = true;
            _sender.SentMessage = message;
            while (IsSending)
            {
                _sender.Broadcast();
                await Task.Delay((int)(BroadcastInterval*(int)TimeScalar.MillisecondsPerSecond));
            }
        }

        /// <summary>
        /// 停止发送广播
        /// </summary>
        public void StopBroadcast()
        {
            IsSending = false;
        }
    }

    /// <summary>
    /// 提供不断接受广播的功能
    /// </summary>
    public class BroadcastListener
    {
        private readonly BroadcastReceiver _receiver;

        public BroadcastListener(int broadcastPort=BroadcastSender.DefaultBroadcastPort)
        {
            _receiver = new(broadcastPort);
        }

        /// <value>
        /// 是否正在监听
        /// </value>
        public bool IsListening {private set; get;} = false;

        /// <summary>
        /// 开始监听广播
        /// </summary>
        public async void StartListen()
        {
            IsListening = true;
            while (IsListening)
            {
                var package = await _receiver.ReceiveAsync();
                InvokeEvent(package);
            }
        }

        private void InvokeEvent(UdpReceiveResult package)
        {
            var message = Encoding.UTF8.GetString(package.Buffer);
            OnReceivedBroadcast?.Invoke(package.RemoteEndPoint.Address, message);
        }

        /// <summary>
        /// 停止监听广播
        /// </summary>
        public void StopListen()
        {
            IsListening = false;
        }

        /// <summary>
        /// 当收到广播时触发事件。
        /// <para>
        /// <c>IPAddress</c>: 发送者的IP信息
        /// <c>string</c>: 广播携带的消息
        /// </para>
        /// </summary>
        public event Action<IPAddress, string> OnReceivedBroadcast;
    }
}
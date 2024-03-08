using GameLib.Common;

// ReSharper disable once CheckNamespace
namespace GameLib.Network.NGO
{
    /// <summary>
    /// IP地址解析异常。
    /// </summary>
    public class IPParseException : LibException
    {
        public IPParseException(string ip) : base($"解析IP地址{ip}失败。")
        {
        }
    }
}
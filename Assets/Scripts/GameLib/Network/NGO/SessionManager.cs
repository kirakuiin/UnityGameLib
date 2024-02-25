using System.Collections.Generic;
using GameLib.Common;
using UnityEngine;
using System.Linq;

namespace GameLib.Network.NGO
{
    /// <summary>
    /// 用来存储玩家在游戏中使用的数据
    /// </summary>
    public interface ISessionPlayerData
    {
        /// <summary>
        /// 玩家是否连接
        /// </summary>
        bool IsConnected {set; get;}

        /// <summary>
        /// 玩家ID
        /// </summary>
        ulong ClientID {set; get;}

        /// <summary>
        /// 重新初始化玩家数据
        /// </summary>
        public void Reinitialize();
    }

    /// <summary>
    /// 用来管理运行时玩家使用的数据，每当玩家连接时会使用一个ID来关联玩家使用的数据。
    /// 当玩家断开重连时，可以使用保留的数据来恢复场景。
    /// </summary>
    /// <remarks>
    /// 使用客户端生成的ID可能存在安全性的问题。
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    public class SessionManager<T> : Singleton<SessionManager<T>> where T : struct, ISessionPlayerData 
    {
        // 玩家ID到数据的映射
        private readonly Dictionary<string, T> _clientData = new();

        // 客户端ID到玩家ID的映射
        private readonly Dictionary<ulong, string> _clientIDToPlayerID = new();

        private bool _isSessionStarted;

        /// <summary>
        /// 初始化玩家会话数据，或者重新设置其数据
        /// </summary>
        /// <param name="clientID">由NGO分配给每个客户端的ID，每次连接其数值不一定相等</param>
        /// <param name="playerID">玩家独特的ID，相互之间不会重复</param>
        /// <param name="data">玩家的会话数据</param>
        public void SetupPlayerData(ulong clientID, string playerID, T data)
        {
            if (IsDuplicateConnection(playerID))
            {
                Debug.LogError($"玩家{playerID}的数据已存在，拒绝设置数据!");
                return;
            }

            if (IsReconnecting(playerID))
            {
                data = _clientData[playerID];
                data.ClientID = clientID;
                data.IsConnected = true;
            }

            _clientData[playerID] = data;
            _clientIDToPlayerID[clientID] = playerID;
        }

        /// <summary>
        /// 判断是否为重复连接
        /// </summary>
        /// <param name="playerID">玩家独有ID</param>
        /// <returns>当确实为重复连接时返回真</returns>
        public bool IsDuplicateConnection(string playerID)
        {
            return _clientData.ContainsKey(playerID) && _clientData[playerID].IsConnected;
        }

        private bool IsReconnecting(string playerID)
        {
            return _clientData.ContainsKey(playerID) && !_clientData[playerID].IsConnected;
        }

        /// <summary>
        /// 处理客户端断开连接。
        /// </summary>
        /// <param name="clientID">NGO分配的客户端ID</param>
        public void DisconnectClient(ulong clientID)
        {
            if (_isSessionStarted)
            {
                KeepPlayerData(clientID);
            }
            else
            {
                DiscardPlayerData(clientID);
            }
        }

        private void KeepPlayerData(ulong clientID)
        {
            var data = GetPlayerData(clientID);
            if (data is null) return;

            var playerID = GetPlayerID(clientID);
            var clientData = _clientData[playerID];
            clientData.IsConnected = false;
            UpdatePlayerData(clientID, clientData);
        }

        private void DiscardPlayerData(ulong clientID)
        {
            var playerID = GetPlayerID(clientID);
            if (playerID is null) return;

            _clientData.Remove(playerID);
            _clientIDToPlayerID.Remove(clientID);
        }

        /// <summary>
        /// 更新会话数据。
        /// </summary>
        /// <param name="clientID">NGO分配的客户端ID</param>
        /// <param name="data">最新数据</param>
        public void UpdatePlayerData(ulong clientID, T data)
        {
            var playerID = GetPlayerID(clientID);
            if (playerID != null)
            {
                _clientData[playerID] = data;
            }
            else
            {
                Debug.LogError($"更新{clientID}数据失败！");
            }
        }

        /// <summary>
        /// 根据客户端ID获得玩家ID
        /// </summary>
        /// <param name="clientID">NGO分配的客户端ID</param>
        /// <returns>玩家ID</returns>
        public string GetPlayerID(ulong clientID)
        {
            if (_clientIDToPlayerID.TryGetValue(clientID, out string playerID))
            {
                return playerID;
            }

            Debug.Log($"未保存{clientID}到玩家名的映射.");
            return null;
        }

        /// <summary>
        /// 根据客户端ID获取会话数据。
        /// </summary>
        /// <param name="clientID">NGO分配的客户端ID</param>
        /// <returns>会话数据</returns>
        public T? GetPlayerData(ulong clientID)
        {
            var playerID = GetPlayerID(clientID);
            if (playerID != null)
            {
                return _clientData[playerID];
            }

            Debug.Log($"未找到{clientID}对应的数据.");
            return null;
        }

        /// <summary>
        /// 启动会话。
        /// </summary>
        public void StartSession()
        {
            _isSessionStarted = true;
        }

        /// <summary>
        /// 清理全部数据。
        /// </summary>
        public void ClearAllData()
        {
            _clientData.Clear();
            _clientIDToPlayerID.Clear();
            _isSessionStarted = false;
        }

        /// <summary>
        /// 停止会话。
        /// </summary>
        /// <remarks>
        /// 停止会话会清理全部的断开连接玩家的数据，并且将已连接玩家的数据重新初始化。
        /// </remarks>
        public void StopSession()
        {
            ClearDisconnectedPlayersData();
            ReinitializePlayersData();
            _isSessionStarted = false;
        }

        private void ClearDisconnectedPlayersData()
        {
            var clientIDNeedToBeCleared = from clientID in _clientIDToPlayerID.Keys
                                          where GetPlayerData(clientID) is {IsConnected : false}
                                          select clientID;
                                        
            foreach (var clientID in clientIDNeedToBeCleared)
            {
                DiscardPlayerData(clientID);
            }
        }

        private void ReinitializePlayersData()
        {
            foreach (var clientID in _clientIDToPlayerID.Keys)
            {
                var playerID = _clientIDToPlayerID[clientID];
                var data = _clientData[playerID];
                data.Reinitialize();
                UpdatePlayerData(clientID, data);
            }
        }
    }
}
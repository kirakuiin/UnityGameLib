using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace GameLib.Network.NGO.ConnectionManagement
{
    /// <summary>
    /// 表示客户端断开后尝试重新连接服务器。
    /// 有指定的尝试次数和超时时间。如果在满足条件的情况下重新连接上，
    /// 则进入<see cref="ClientConnectedState"/>；否则进入<see cref="OfflineState"/>。
    /// <remarks>如果断开给出了某些具体原因，可能不会触发重连，直接进入<see cref="OfflineState"/></remarks>
    /// </summary>
    public class ClientReconnectingState : ClientConnectingState
    {
        private Coroutine _reconnectCoroutine;

        private int _currentAttemptNum;

        private const int TimeBeforeFirstAttempt = 1;

        private const int TimeBetweenAttempts = 5;

        private readonly List<ConnectStatus> _offlineStateList = new()
        {
            ConnectStatus.ServerFull,
            ConnectStatus.HostEndSession,
            ConnectStatus.IncompatibleBuildType,
            ConnectStatus.UserRequestedDisconnect,
        };
        
        public ClientReconnectingState(ConnectionMethod method) : base(method)
        {
        }
        
        public override void Enter()
        {
            _currentAttemptNum = 0;
            _reconnectCoroutine = ConnManager.StartCoroutine(ReconnectCoroutine());
        }

        private IEnumerator ReconnectCoroutine()
        {
            if (_currentAttemptNum > 0) yield return WaitForBetween();
            
            yield return WaitForShutdown();

            yield return WaitForReconnect();
        }

        private IEnumerator WaitForBetween()
        {
            yield return new WaitForSeconds(TimeBetweenAttempts);
        }

        private IEnumerator WaitForShutdown()
        {
            Debug.Log($"丢失和主机端的链接，开始进行重连...");
            NetManager.Shutdown();
            yield return new WaitWhile(() => NetManager.ShutdownInProgress);
        }

        private IEnumerator WaitForReconnect()
        {
            // 第一次断开重连时，留出足够的时间给服务更新状态。
            if (_currentAttemptNum == 0) yield return new WaitForSeconds(TimeBeforeFirstAttempt);
            
            _currentAttemptNum++;
            var reconnectSetupTask = ConnMethod.SetupClientReconnectAsync();
            yield return new WaitUntil(() => reconnectSetupTask.IsCompleted);

            Debug.Log($"重连 {_currentAttemptNum}/{ConnManager.config.reconnectAttemptNum}");
            yield return StartReconnect(reconnectSetupTask);
        }

        private IEnumerator StartReconnect(Task<ReconnectResult> reconnectTask)
        {
            if (!reconnectTask.IsFaulted && reconnectTask.Result.IsSuccess)
            {
                var connectingTask = ConnectAsync();
                yield return new WaitUntil(() => connectingTask.IsCompleted);
            }
            else
            {
                if (!reconnectTask.Result.ShouldTryAgain)
                {
                    _currentAttemptNum = ConnManager.config.maxConnectedPlayerNum;
                }
                OnClientDisconnected(0);
            }
        }

        public override void OnClientDisconnected(ulong clientID)
        {
            if (_currentAttemptNum < ConnManager.config.reconnectAttemptNum)
            {
                ContinueReconnect();
            }
            else
            {
                HandleByDisconnectReason(
                    nullAction: () => Publisher.Publish(ConnectInfo.Create(ConnectStatus.GenericDisconnect)),
                    statusAction: info => Publisher.Publish(info)
                );
                ConnManager.ChangeState<OfflineState>();
            }
        }

        private void ContinueReconnect()
        {
            HandleByDisconnectReason(
                nullAction: () => _reconnectCoroutine = ConnManager.StartCoroutine(ReconnectCoroutine()),
                statusAction: ChangeStateByStatus
            );
        }

        private void HandleByDisconnectReason(Action nullAction, Action<ConnectInfo> statusAction)
        {
            var disconnectReason = NetManager.DisconnectReason;
            if (string.IsNullOrEmpty(disconnectReason))
            {
                nullAction();
            }
            else
            {
                statusAction(GetConnInfoFromManager());
            }
        }

        private ConnectInfo GetConnInfoFromManager()
        {
            return JsonUtility.FromJson<ConnectInfo>(NetManager.DisconnectReason);
        }

        private void ChangeStateByStatus(ConnectInfo info)
        {
            Publisher.Publish(info);
            if (_offlineStateList.Contains(info.Status))
            {
                ConnManager.ChangeState<OfflineState>();
            }
            else
            {
                _reconnectCoroutine = ConnManager.StartCoroutine(ReconnectCoroutine());
            }
        }

        public override void Exit()
        {
            if (_reconnectCoroutine != null)
            {
                ConnManager.StopCoroutine(_reconnectCoroutine);
                _reconnectCoroutine = null;
            }
        }
    }
}
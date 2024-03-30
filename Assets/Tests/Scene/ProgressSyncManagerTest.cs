using GameLib.Network.NGO;
using Unity.Netcode;
using UnityEngine;

namespace Tests.Scene
{
    /// <summary>
    /// 测试<c>ProgressSync</c>
    /// </summary>
    public class ProgressSyncManagerTest: MonoBehaviour
    {
        void OnGUI()
        {
            GUILayout.BeginArea(new Rect(10, 10, 300, 300));
            if (NetworkManager.Singleton.IsListening)
            {
                if (NetworkManager.Singleton.IsServer)
                {
                    ServerSetting();
                }
                else
                {
                    ClientSetting();
                }
            }
            GUILayout.EndArea();
        }

        void ServerSetting()
        {
            if (GUILayout.Button("发送事件1"))
            {
                ProgressSyncManager.Instance.AddSyncEvent(SyncEvent.Create(EventType.Event1),
                    (e) => Debug.Log($"{e}完毕"));
            }
            if (GUILayout.Button("发送事件2"))
            {
                ProgressSyncManager.Instance.AddSyncEvent(SyncEvent.Create(EventType.Event2),
                    (e) => Debug.Log($"{e}完毕"));
            }
        }

        void ClientSetting()
        {
            if (GUILayout.Button("事件1完毕"))
            {
                ProgressSyncManager.Instance.ClientSyncDone(SyncEvent.Create(EventType.Event1));
            }
            if (GUILayout.Button("事件2完毕"))
            {
                ProgressSyncManager.Instance.ClientSyncDone(SyncEvent.Create(EventType.Event2));
            }
        }

    }

    public enum EventType
    {
        Event1,
        Event2
    }
}
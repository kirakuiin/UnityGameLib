using GameLib.Network.NGO;
using Unity.Netcode;
using UnityEngine;

namespace Tests.Scene
{
    /// <summary>
    /// 测试<c>ProgressSync</c>
    /// </summary>
    public class NetworkSyncManagerTest: MonoBehaviour
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
                NetworkSyncManager.Instance.AddSyncEvent(EventType.Event1);
            }
            if (GUILayout.Button("发送事件2"))
            {
                NetworkSyncManager.Instance.AddSyncEvent(EventType.Event2, () => Debug.Log("事件2结束"));
            }
            if (GUILayout.Button("重置全部"))
            {
                NetworkSyncManager.Instance.ResetAll();
            }
        }

        void ClientSetting()
        {
            if (GUILayout.Button("事件1完毕"))
            {
                NetworkSyncManager.Instance.AddSyncEvent(EventType.Event1, () => Debug.Log("事件1结束"));
            }
            if (GUILayout.Button("事件2完毕"))
            {
                NetworkSyncManager.Instance.AddSyncEvent(EventType.Event2);
            }
        }

    }

    public enum EventType
    {
        Event1,
        Event2
    }
}
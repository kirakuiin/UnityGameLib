using System;
using GameLib.Network.NGO;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Tests.Scene
{
    public class SceneSyncTest : MonoBehaviour
    {
        [SerializeField]
        private Text loadInfo;

        [SerializeField]
        private Text eventInfo;
        
        private void Start()
        {
            SceneLoader.Instance.OnSceneEvent += OnSceneEvent;
            SceneLoader.Instance.OnSceneLoaded += OnSceneLoaded;
        }

        private void OnDestroy()
        {
            SceneLoader.Instance.OnSceneEvent -= OnSceneEvent;
            SceneLoader.Instance.OnSceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, LoadSceneMode mode)
        {
            loadInfo.text = $"{scene.name} 以 {mode} 载入.";
        }

        private void OnSceneEvent(SceneEvent sceneEvent)
        {
            eventInfo.text = $"触发事件: {sceneEvent.SceneEventType}";
        }

        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(10, 10, 300, 300));
            if (!NetworkManager.Singleton.IsListening)
            {
                ShowSelection();
            }
            else
            {
                if (NetworkManager.Singleton.IsServer)
                {
                    ServerScene();
                }
                else
                {
                    ClientScene();
                }
            }
            GUILayout.EndArea();
        }

        void ShowSelection()
        {
            if (GUILayout.Button("创建主机"))
            {
                NetworkManager.Singleton.StartHost();
            }
            if (GUILayout.Button("加入服务端"))
            {
                NetworkManager.Singleton.StartClient();
            }
        }

        void ServerScene()
        {
            if (GUILayout.Button("追加场景1"))
            {
                SceneLoader.Instance.LoadSceneByNet("SceneTest01", LoadSceneMode.Additive);
            }

            if (GUILayout.Button("追加场景2"))
            {
                SceneLoader.Instance.LoadSceneByNet("SceneTest02", LoadSceneMode.Additive);
            }

            if (GUILayout.Button("切换场景1"))
            {
                SceneLoader.Instance.LoadSceneByNet("SceneTest01");
            }
            
            if (GUILayout.Button("切换场景2"))
            {
                SceneLoader.Instance.LoadSceneByNet("SceneTest02");
            }
            
            if (GUILayout.Button("追加场景1(本地)"))
            {
                SceneLoader.Instance.LoadScene("SceneTest01", LoadSceneMode.Additive);
            }

            if (GUILayout.Button("切换场景2(本地)"))
            {
                SceneLoader.Instance.LoadScene("SceneTest02");
            }

            if (GUILayout.Button("返回初始界面"))
            {
                SceneLoader.Instance.LoadSceneByNet("SceneSyncTest");
            }
        }

        void ClientScene()
        {
            if (GUILayout.Button("追加场景1"))
            {
                SceneLoader.Instance.LoadScene("SceneTest01", LoadSceneMode.Additive);
            }

            if (GUILayout.Button("追加场景2"))
            {
                SceneLoader.Instance.LoadScene("SceneTest02", LoadSceneMode.Additive);
            }

            if (GUILayout.Button("切换场景1"))
            {
                SceneLoader.Instance.LoadScene("SceneTest01");
            }
            
            if (GUILayout.Button("切换场景2"))
            {
                SceneLoader.Instance.LoadScene("SceneTest02");
            }
        }
    }
}
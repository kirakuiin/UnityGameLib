using UnityEngine;

namespace GameLib.UI.Fitter
{
    [ExecuteAlways]
    public abstract class SizeFitter : MonoBehaviour
    {
        /// <summary>
        /// 更新大小。
        /// </summary>
        protected abstract void UpdateSize();

        /// <summary>
        /// 初始化更新大小所需要的组件。
        /// </summary>
        protected abstract void InitComponent();

        private void Awake()
        {
            InitComponent();
        }

        private void Start()
        {
            UpdateSize();
        }

        private void OnRectTransformDimensionsChange()
        {
            UpdateSize();
        }

        private void OnValidate()
        {
            InitComponent();
            UpdateSize();
        }
        
#if UNITY_EDITOR
        private void Update()
        {
            if (Application.isPlaying) return;
            UpdateSize();
        }
#endif
    }
}
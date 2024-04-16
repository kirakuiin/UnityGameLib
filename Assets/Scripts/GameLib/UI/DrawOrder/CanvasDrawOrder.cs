using UnityEngine;

// ReSharper disable once CheckNamespace
namespace GameLib.UI
{
    [RequireComponent(typeof(Canvas))]
    public class CanvasDrawOrder : MonoBehaviour, IDrawOrder
    {
        [SerializeField] private bool isOverride = true;
        
        private void Start()
        {
            GetComponent<Canvas>().overrideSorting = isOverride;
        }

        public int Order
        {
            get => GetComponent<Canvas>().sortingOrder;
            set => GetComponent<Canvas>().sortingOrder = value;
        }
    }
}
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

namespace GameLib.UI.Card
{
    /// <summary>
    /// 支持卡牌被选中时的一系列特效。
    /// </summary>
    [RequireComponent(typeof(Canvas))]
    public class CardSelection : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField]
        private float scaleFactor = 1.0f;

        [SerializeField]
        private Vector3 moveOffset = Vector3.zero;

        [SerializeField]
        private bool isEnableRotation = false;

        [SerializeField]
        private Vector3 rotation = Vector3.zero;
        
        private const int MaxOrder = 9999;
        
        protected Canvas Sorter;

        private Transform _transform;

        private int _originOrder;

        private Vector3 _originScale;

        private Quaternion _originRotation;

        private void Awake()
        {
            Sorter = GetComponent<Canvas>();
            _transform = transform;
        }

        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            SaveOriginInfo();
            Sorter.sortingOrder = MaxOrder;
            _transform.localScale *= scaleFactor;
            _transform.position += moveOffset;
            if (isEnableRotation)
            {
                transform.rotation = Quaternion.Euler(rotation);
            }
        }

        protected virtual void SaveOriginInfo()
        {
            _originOrder = Sorter.sortingOrder;
            _originScale = _transform.localScale;
            _originRotation = _transform.rotation;
        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
            Sorter.sortingOrder = _originOrder;
            _transform.localScale = _originScale;
            _transform.position -= moveOffset;
            if (isEnableRotation)
            {
                _transform.rotation = _originRotation;
            }
        }
    }
}
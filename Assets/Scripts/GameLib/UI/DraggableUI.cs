using UnityEngine;
using UnityEngine.EventSystems;

namespace GameLib.UI
{
    /// <summary>
    /// 可拖拽UI对象。
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public class DraggableUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        protected Vector3 Offset;
        
        public virtual void OnBeginDrag(PointerEventData eventData)
        {
            RectTransformUtility.ScreenPointToWorldPointInRectangle(
                GetComponent<RectTransform>(),
                eventData.position,
                eventData.enterEventCamera,
                out var v3);
            Offset = transform.position - v3;
        }

        public virtual void OnDrag(PointerEventData eventData)
        {
            transform.position = Input.mousePosition + Offset;
        }

        public virtual void OnEndDrag(PointerEventData eventData)
        {
        }
    }
}
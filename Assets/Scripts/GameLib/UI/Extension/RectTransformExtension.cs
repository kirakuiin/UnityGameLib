using UnityEngine;

namespace GameLib.UI.Extension
{
    public static class RectTransformExtension
    {
        /// <summary>
        /// 设置矩形变换的大小。
        /// </summary>
        /// <param name="rectTransform"></param>
        /// <param name="size"></param>
        public static void SetRectSize(this RectTransform rectTransform, Vector2 size)
        {
            var oldSize = rectTransform.rect.size;
            var deltaSize = size - oldSize;
            var pivot = rectTransform.pivot;
            rectTransform.offsetMin -= new Vector2(deltaSize.x * pivot.x, deltaSize.y * pivot.y);
            rectTransform.offsetMax += new Vector2(deltaSize.x * (1f - rectTransform.pivot.x), deltaSize.y * (1f - pivot.y));
        }
    }
}
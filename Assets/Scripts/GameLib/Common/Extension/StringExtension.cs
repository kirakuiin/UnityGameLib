using UnityEngine;

namespace GameLib.Common.Extension
{
    public static class StringExtension
    {
        /// <summary>
        /// 将字符串加上颜色的富文本标记。
        /// </summary>
        /// <param name="text"></param>
        /// <param name="color">颜色</param>
        /// <returns>富文本</returns>
        public static string ToRichText(this string text, Color color)
        {
            var colorTag = ColorUtility.ToHtmlStringRGB(color);
            return $"<color=#{colorTag}>{text}</color>";
        }
        
        public static string ToBoldRichText(this string text)
        {
            return $"<b>{text}</b>";
        }
        
        public static string ToItalicRichText(this string text)
        {
            return $"<i>{text}</i>";
        }
    }
}
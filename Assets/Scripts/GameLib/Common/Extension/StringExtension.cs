
using System;

namespace GameLib.Common.Extension
{
    public static class StringExtension
    {
        /// <summary>
        /// 在字符串中随机挑选一个字符并返回
        /// </summary>
        /// <returns>被选中的子字符串</returns>
        public static string Choice(this string str)
        {
            var random = new Random();
            var index = random.Next(0, str.Length);
            return str[index].ToString();
        }
    }
}
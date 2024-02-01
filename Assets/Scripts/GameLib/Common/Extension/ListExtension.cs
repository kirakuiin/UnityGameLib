using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace GameLib.Common.Extension
{
    public static class ListExtension
    {
        /// <summary>
        /// 交换列表中两个元素的位置
        /// </summary>
        /// <param name="list">列表对象</param>
        /// <param name="i">第一个下标</param>
        /// <param name="j">第二个下标</param>
        /// <typeparam name="T"></typeparam>
        public static void Swap<T>(this List<T> list, int i, int j)
        {
            (list[i], list[j]) = (list[j], list[i]);
        }
        
        /// <summary>
        /// 在列表中随机挑选一个元素并返回
        /// </summary>
        /// <returns>被选中的元素</returns>
        public static T Choice<T>(this List<T> list)
        {
            var random = new Random();
            var index = random.Next(0, list.Count);
            return list[index];
        }
    }
}
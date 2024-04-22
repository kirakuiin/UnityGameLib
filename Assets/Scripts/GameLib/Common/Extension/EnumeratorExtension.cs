using System;
using System.Collections.Generic;

namespace GameLib.Common.Extension
{
    public static class EnumeratorExtension
    {
        /// <summary>
        /// 在原始序列的每个对象上执行操作。
        /// </summary>
        /// <param name="enumerable"></param>
        /// <param name="action"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static void Apply<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (var elem in enumerable)
            {
                action(elem);
            }
        }
    }
}
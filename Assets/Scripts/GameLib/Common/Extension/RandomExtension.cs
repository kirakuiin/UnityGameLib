using System;
using System.Collections.Generic;

namespace GameLib.Common.Extension
{
    public static class RandomExtension
    {
        /// <summary>
        /// 在列表中随机挑选一个元素并返回
        /// </summary>
        /// <returns>被选中的元素</returns>
        public static T Choice<T>(this Random random, IList<T> list)
        {
            var index = random.Next(0, list.Count);
            return list[index];
        }

        /// <summary>
        /// 打乱列表中元素顺序。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void Shuffle<T>(this Random random, IList<T> list)
        {
            for (var i = list.Count - 1; i >= 0; --i)
            {
                var randIndex = random.Next(i+1);
                list.Swap(randIndex, i);
            }
        }
    }
}
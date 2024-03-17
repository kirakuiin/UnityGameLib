using System;
using Unity.Netcode;

namespace GameLib.Network.NGO.Extension
{
    /// <summary>
    /// 针对<see cref="NetworkList{T}"/>的扩展。
    /// </summary>
    public static class NetworkListExtension
    {
        /// <summary>
        /// 根据指定的谓词函数替换掉一个元素。
        /// </summary>
        /// <remarks>如果没有找到的话则无事发生。</remarks>
        /// <param name="listObj"></param>
        /// <param name="findFunc">谓词函数</param>
        /// <param name="newElem">新元素</param>
        /// <typeparam name="T"></typeparam>
        public static void Replace<T>(this NetworkList<T> listObj, Predicate<T> findFunc, T newElem)
            where T : unmanaged, IEquatable<T>
        {
            for (var i = 0; i < listObj.Count; ++i)
            {
                if (!findFunc(listObj[i])) continue;
                listObj.Insert(i, newElem);
                listObj.RemoveAt(i+1);
                break;
            }
        }

        /// <summary>
        /// 寻找列表中满足条件的第一个元素。未能找到则返回<c>null</c>。
        /// </summary>
        /// <param name="listObj"></param>
        /// <param name="findFunc">谓词函数</param>
        public static T? Find<T>(this NetworkList<T> listObj, Predicate<T> findFunc)
            where T : unmanaged, IEquatable<T>
        {
            foreach (var elem in listObj)
            {
                if (findFunc(elem))
                {
                    return elem;
                }
            }
            return null;
        }
    }
}
using System;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GameLib.Common.Extension
{
    public static class TransformExtension
    {
        /// <summary>
        /// 对所有的子节点执行同一操作。
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="func"></param>
        public static void DoSomethingToAllChildren(this Transform trans, Action<GameObject> func)
        {
            var allChildren = (from idx in Enumerable.Range(0, trans.childCount)
                select trans.GetChild(idx).gameObject).ToList();
            allChildren.Apply(func);
        }

        /// <summary>
        /// 销毁全部子节点。
        /// </summary>
        /// <param name="trans"></param>
        public static void DestroyAllChildren(this Transform trans)
        {
            trans.DoSomethingToAllChildren(Object.Destroy);
        }
    }
}
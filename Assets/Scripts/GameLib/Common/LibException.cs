using System;
using UnityEngine;

namespace GameLib.Common
{
    /// <summary>
    /// 游戏库的异常基类
    /// </summary>
    public class LibException : Exception
    {
        protected LibException(string msg, bool isLogInUnity = true) : base(msg)
        {
            if (isLogInUnity)
            {
                Debug.LogError(msg);
            }
        }
    }
}
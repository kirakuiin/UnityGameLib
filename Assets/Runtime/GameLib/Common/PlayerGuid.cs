using UnityEngine;

namespace GameLib.Common
{
    /// <summary>
    /// 生成和获得玩家的GUID
    /// </summary>
    public static class PlayerGuid
    {
        private const string GuidKey = "PlayerGuid";
        
        /// <summary>
        /// 获得当前主机内存储的Guid
        /// </summary>
        /// <remarks>如果是第一次获取，则会生成新的，否则返回之前存储的。</remarks>
        /// <param name="domain">指明Guid的作用域，不同的作用域有不同的Guid</param>
        /// <returns>Guid字符串</returns>
        public static string GetGuidByMachine(string domain="")
        {
            var key = $"{domain}{GuidKey}";
            if (!PlayerPrefs.HasKey(key))
            {
                CreateGuid(key);
            }
            return PlayerPrefs.GetString(key);
        }

        private static void CreateGuid(string key)
        {
            var guid = System.Guid.NewGuid();
            PlayerPrefs.SetString(key, guid.ToString());
        }
    }
}
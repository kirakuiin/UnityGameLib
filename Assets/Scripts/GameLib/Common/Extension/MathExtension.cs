using UnityEngine;

namespace GameLib.Common.Extension
{
    public struct MathExtension
    {
        /// <summary>
        /// 默认精度值
        /// </summary>
        private const float DefaultFloatPrecision = 1E-06f;
        
        public static bool Approximately(float a, float b, float precision = DefaultFloatPrecision)
        {
            return Mathf.Abs(b - a) < precision;
        }
        
        /// <summary>
        /// 判断V3是否近似。
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="precision"></param>
        /// <returns></returns>
        public static bool Approximately(Vector3 a, Vector3 b, float precision=DefaultFloatPrecision)
        {
            return Approximately(a.x, b.x, precision) && Approximately(a.y, b.y, precision) && Approximately(a.z, b.z, precision);
        }

        /// <summary>
        /// 判断V2的近似性。
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="precision"></param>
        /// <returns></returns>
        public static bool Approximately(Vector2 a, Vector2 b, float precision=DefaultFloatPrecision)
        {
            return Approximately(a.x, b.x, precision) && Approximately(a.y, b.y, precision);
        }

        /// <summary>
        /// 判断两个四元量的近似性。
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="precision"></param>
        /// <returns></returns>
        public static bool Approximately(Quaternion a, Quaternion b, float precision=DefaultFloatPrecision)
        {
            return Approximately(a.eulerAngles, b.eulerAngles, precision);
        }
    }
}
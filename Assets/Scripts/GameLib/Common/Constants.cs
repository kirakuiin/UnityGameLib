namespace GameLib
{
    public static class TimeScalar
    {
        /// <summary>
        /// 每秒包含多少毫秒
        /// </summary>
        public const int MillsecondsPerSecond = 1000;

        /// <summary>
        /// 将秒转为毫秒
        /// </summary>
        /// <param name="seconds"></param>
        /// <returns>毫秒</returns>
        public static int SecondToMs(float seconds) => (int)(seconds*MillsecondsPerSecond);

        /// <summary>
        /// 将秒转为毫秒
        /// </summary>
        /// <param name="seconds"></param>
        /// <returns>毫秒</returns>
        public static int SecondToMs(int seconds) => seconds*MillsecondsPerSecond;
    }
}
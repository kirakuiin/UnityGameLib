namespace GameLib.Common.Extension
{
    public static class TimeExtension
    {
        /// <summary>
        /// 每秒包含多少毫秒
        /// </summary>
        public const int MillisecondsPerSecond = 1000;

        /// <summary>
        /// 将秒转为毫秒
        /// </summary>
        /// <param name="seconds"></param>
        /// <returns>毫秒</returns>
        public static int ConvertSecondToMs(float seconds) => (int)(seconds*MillisecondsPerSecond);
        
        /// <summary>
        /// 将秒转为毫秒
        /// </summary>
        /// <param name="seconds"></param>
        /// <returns>毫秒</returns>
        public static int ConvertSecondToMs(int seconds) => seconds*MillisecondsPerSecond;
    }
}
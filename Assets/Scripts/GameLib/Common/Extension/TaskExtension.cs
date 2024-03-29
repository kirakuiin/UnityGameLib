using System;
using System.Collections;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

namespace GameLib.Common.Extension
{
    public static class TaskExtension
    {
        private const int DefaultInterval = 17;
        
        /// <summary>
        /// 在UnityTest替代await关键字来执行异步操作。
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        public static IEnumerator AsEnumeratorReturnNull(this Task task)
        {
            while (!task.IsCompleted)
            {
                yield return null;
            }
            if (task.IsFaulted && task.Exception != null)
            {
                ExceptionDispatchInfo.Capture(task.Exception).Throw();
            }
            yield return null;
        }
        
        /// <summary>
        /// 在UnityTest替代await关键字来执行异步操作。
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        public static IEnumerator AsEnumeratorReturnNull<T>(this Task<T> task)
        {
            while (!task.IsCompleted)
            {
                yield return null;
            }
            if (task.IsFaulted && task.Exception != null)
            {
                ExceptionDispatchInfo.Capture(task.Exception).Throw();
            }
            yield return null;
        }

        /// <summary>
        /// 等待直到某个断言为真。
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="interval"></param>
        public static async Task Wait(Func<bool> predicate, int interval=17)
        {
            while (!predicate())
            {
                await Task.Delay(interval);
            }
        }
    }
}
using System;
using System.Linq;
using System.Threading;

namespace Thriot.Framework.Batching
{
    public class MultiWaitHandler
    {
        private const int Limit = 64;

        public static void WaitAll(WaitHandle[] waitHandles, TimeSpan waitTime)
        {
            WaitAllHandler(waitHandles, waitTime);
        }

        private static void WaitAllHandler(WaitHandle[] waitHandles, TimeSpan waitTime)
        {
            if (waitHandles.Length <= Limit)
            {
                WaitHandle.WaitAll(waitHandles, waitTime);
                return;
            }

            var waitEnd = DateTime.UtcNow + waitTime;
            foreach(var items in waitHandles.Buffer(Limit))
            {
                var timeLeft = waitEnd - DateTime.UtcNow;
                var timeToWait = timeLeft.TotalMilliseconds > 0 ? timeLeft : TimeSpan.FromMilliseconds(1);
                WaitAllHandler(items.ToArray(), timeToWait);
            };
        }
    }
}
using System;
using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling;

namespace IoT.Framework.Azure.TableOperations
{
    public static class TransientErrorHandling
    {
        public static void Run(Action action)
        {
            var retryPolicy =
                new RetryPolicy<StorageTransientErrorDetectionStrategyWithConcurrency>(
                    new ExponentialBackoff(10, TimeSpan.FromMilliseconds(10),
                        TimeSpan.FromMilliseconds(10000), TimeSpan.FromMilliseconds(200)));
            
            retryPolicy.ExecuteAction(action);
        }
    }
}

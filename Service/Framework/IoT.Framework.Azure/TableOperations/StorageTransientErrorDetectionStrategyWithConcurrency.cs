using System;
using System.Net;
using IoT.Framework.DataAccess;
using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling;

namespace IoT.Framework.Azure.TableOperations
{
    public class StorageTransientErrorDetectionStrategyWithConcurrency :
        StorageTransientErrorDetectionStrategy
    {
        protected override bool CheckIsTransient(Exception ex)
        {
            if (base.CheckIsTransient(ex))
                return true;

            if (ex is StorageAccessException &&
                (ex as StorageAccessException).HttpStatusCode == HttpStatusCode.PreconditionFailed)
                return true;

            if (ex is OptimisticConcurrencyException)
                return true;

            return false;
        }
    }
}
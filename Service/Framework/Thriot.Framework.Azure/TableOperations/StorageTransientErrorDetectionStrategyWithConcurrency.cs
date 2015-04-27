using System;
using System.Net;
using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling;
using Thriot.Framework.DataAccess;

namespace Thriot.Framework.Azure.TableOperations
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
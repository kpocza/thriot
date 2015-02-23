using System;
using System.Collections.Generic;

namespace IoT.Framework.Batching
{
    public interface IBatchWorker<TParameter, TResult>
    {
        IDictionary<Guid, TResult> Process(IEnumerable<BatchItem<TParameter>> parameters);

        bool IsItemThrottled(IEnumerable<WorkItem<TParameter, TResult>> existingItems, TParameter newItem);

        void Throttle(WorkItem<TParameter, TResult> newItem);

        void ReportCancellation(WorkItem<TParameter, TResult> newItem);
    }
}

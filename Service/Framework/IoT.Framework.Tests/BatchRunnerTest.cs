using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using IoT.Framework.Batching;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IoT.Framework.Tests
{
    [TestClass]
    public class BatchRunnerTest
    {
        [TestMethod]
        public void StartStopTest()
        {
            var scheduledRunner = new BatchRunner<Param, bool>(TimeSpan.FromMilliseconds(100), 10, 1024, TimeSpan.FromMilliseconds(1000), TimeSpan.FromMilliseconds(1000), new BatchWorker());

            scheduledRunner.Start();

            Thread.Sleep(100);

            scheduledRunner.Stop();
        }

        [TestMethod]
        public void EnqueueWaitTest()
        {
            var scheduledRunner = new BatchRunner<Param, bool>(TimeSpan.FromMilliseconds(100), 10, 1024, TimeSpan.FromMilliseconds(1000), TimeSpan.FromMilliseconds(1000), new BatchWorker());

            scheduledRunner.Start();

            var @event = new ManualResetEvent(false);
            var workItem = new WorkItem<Param, bool>(@event, new Param());

            scheduledRunner.EnqueueItem(workItem);

            @event.WaitOne(scheduledRunner.WaitTime);

            scheduledRunner.Stop();

            Assert.IsTrue(workItem.Result);
        }

        [TestMethod]
        public void EnqueueAndWaitTest()
        {
            var scheduledRunner = new BatchRunner<Param, bool>(TimeSpan.FromMilliseconds(100), 10, 1024, TimeSpan.FromMilliseconds(1000), TimeSpan.FromMilliseconds(1000), new BatchWorker());

            scheduledRunner.Start();

            var result = scheduledRunner.EnqueueAndWaitItem(new Param());

            scheduledRunner.Stop();

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void StoppingEnqueueWaitTest()
        {
            var scheduledRunner = new BatchRunner<Param, bool>(TimeSpan.FromMilliseconds(100), 10, 1024, TimeSpan.FromMilliseconds(1000), TimeSpan.FromMilliseconds(1000), new BatchWorker());

            scheduledRunner.Start();

            var @event = new ManualResetEvent(false);
            var workItem = new WorkItem<Param, bool>(@event, new Param());

            scheduledRunner.Stop();

            scheduledRunner.EnqueueItem(workItem);

            @event.WaitOne(scheduledRunner.WaitTime);

            Assert.IsFalse(workItem.Result);
        }

        [TestMethod]
        public void EnqueueWaitFailTooTest()
        {
            var scheduledRunner = new BatchRunner<Param, bool>(TimeSpan.FromMilliseconds(100), 10, 1024, TimeSpan.FromMilliseconds(1000), TimeSpan.FromMilliseconds(1000), new BatchWorker());

            scheduledRunner.Start();

            var event1 = new ManualResetEvent(false);
            var workItem1 = new WorkItem<Param, bool>(event1, new Param());
            scheduledRunner.EnqueueItem(workItem1);

            var event2 = new ManualResetEvent(false);
            var workItem2 = new WorkItem<Param, bool>(event2, new Param());
            scheduledRunner.EnqueueItem(workItem2);

            event1.WaitOne(scheduledRunner.WaitTime);
            event2.WaitOne(scheduledRunner.WaitTime);

            scheduledRunner.Stop();

            Assert.IsTrue(workItem1.Result);
            Assert.IsFalse(workItem2.Result);
        }

        [TestMethod]
        public void EnqueueAndWaitThrottledTest()
        {
            var scheduledRunner = new BatchRunner<Param, bool>(TimeSpan.FromMilliseconds(100), 10, 1024, TimeSpan.FromMilliseconds(1000), TimeSpan.FromMilliseconds(1000), new BatchWorkerThrottle());

            scheduledRunner.Start();

            var result = scheduledRunner.EnqueueAndWaitItem(new Param());

            scheduledRunner.Stop();

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void EnqueueAndWaitManyItemsBelow64Test()
        {
            bool wasOk = false;
            var thread = new Thread(() =>
            {
                var scheduledRunner = new BatchRunner<Param, bool>(TimeSpan.FromMilliseconds(100), 10, 1024, TimeSpan.FromMilliseconds(1000), TimeSpan.FromMilliseconds(1000), new BatchWorkerFullOk());

                scheduledRunner.Start();

                var result = scheduledRunner.EnqueueAndWaitManyItems(Enumerable.Range(0, 30).Select(p => new Param()));

                scheduledRunner.Stop();

                wasOk = result.All(t => t.Result);
            });
            thread.SetApartmentState(ApartmentState.MTA);
            thread.Start();
            thread.Join();

            Assert.IsTrue(wasOk);
        }

        [TestMethod]
        public void EnqueueAndWaitManyItemsAbove64Test()
        {
            bool wasOk = false;
            var thread = new Thread(() =>
            {
                var scheduledRunner = new BatchRunner<Param, bool>(TimeSpan.FromMilliseconds(100), 10, 1024, TimeSpan.FromMilliseconds(1000), TimeSpan.FromMilliseconds(1000), new BatchWorkerFullOk());

                scheduledRunner.Start();

                var result = scheduledRunner.EnqueueAndWaitManyItems(Enumerable.Range(0, 1000).Select(p => new Param()));

                scheduledRunner.Stop();

                wasOk = result.All(t => t.Result);
            });
            thread.SetApartmentState(ApartmentState.MTA);
            thread.Start();
            thread.Join();

            Assert.IsTrue(wasOk);
        }

        class Param
        {
            
        }

        class BatchWorker : IBatchWorker<Param, bool>
        {
            public virtual IDictionary<Guid, bool> Process(IEnumerable<BatchItem<Param>> parameters)
            {
                int idx = 0;
                var result = new Dictionary<Guid, bool>();
                foreach (var p in parameters)
                {
                    result.Add(p.Id, idx%2 == 0);
                    idx++;
                }

                return result;
            }

            public virtual bool IsItemThrottled(IEnumerable<WorkItem<Param, bool>> existingItems, Param newItem)
            {
                return false;
            }

            public void Throttle(WorkItem<Param, bool> newItem)
            {
                newItem.Result = false;
            }

            public void ReportCancellation(WorkItem<Param, bool> newItem)
            {
                newItem.Result = false;
            }
        }

        class BatchWorkerFullOk : IBatchWorker<Param, bool>
        {
            public virtual IDictionary<Guid, bool> Process(IEnumerable<BatchItem<Param>> parameters)
            {
                int idx = 0;
                var result = new Dictionary<Guid, bool>();
                foreach (var p in parameters)
                {
                    result.Add(p.Id, true);
                    idx++;
                }

                return result;
            }

            public virtual bool IsItemThrottled(IEnumerable<WorkItem<Param, bool>> existingItems, Param newItem)
            {
                return false;
            }

            public void Throttle(WorkItem<Param, bool> newItem)
            {
                newItem.Result = false;
            }

            public void ReportCancellation(WorkItem<Param, bool> newItem)
            {
                newItem.Result = false;
            }
        }

        class BatchWorkerThrottle : BatchWorker
        {
            public override bool IsItemThrottled(IEnumerable<WorkItem<Param, bool>> existingItems, Param newItem)
            {
                return true;
            }
        }

    }
}

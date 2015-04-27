using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Thriot.Framework.Batching;

namespace Thriot.Framework.Tests
{
    [TestClass]
    public class MultiWaitHandlerTests
    {
        [TestMethod]
        public void SingleTest()
        {
            var autoresetEvent = new AutoResetEvent(false);

            new Thread(() => autoresetEvent.Set()).Start();

            MultiWaitHandler.WaitAll(new WaitHandle[] { autoresetEvent }, TimeSpan.FromMilliseconds(100));
        }

        [TestMethod]
        public void MultiTest()
        {
            bool wasOk = false;
            var thread = new Thread(() =>
            {
                var autoresetEvents =
                    Enumerable.Range(0, 30).Select(are => (WaitHandle) new AutoResetEvent(false)).ToList();

                new Thread(() => autoresetEvents.ForEach(are => ((AutoResetEvent) are).Set())).Start();

                var stopWatch = Stopwatch.StartNew();
                MultiWaitHandler.WaitAll(autoresetEvents.ToArray(), TimeSpan.FromMilliseconds(100));
                wasOk = stopWatch.ElapsedMilliseconds < 200;
            });
            thread.Start();
            thread.Join();

            Assert.IsTrue(wasOk);
        }

        [TestMethod]
        public void ManyMultiTest()
        {
            bool wasOk = false;
            var thread = new Thread(() =>
            {
                var autoresetEvents =
                    Enumerable.Range(0, 3000).Select(are => (WaitHandle)new AutoResetEvent(false)).ToList();

                new Thread(() => autoresetEvents.ForEach(are => ((AutoResetEvent)are).Set())).Start();

                var stopWatch = Stopwatch.StartNew();
                MultiWaitHandler.WaitAll(autoresetEvents.ToArray(), TimeSpan.FromMilliseconds(100));
                wasOk = stopWatch.ElapsedMilliseconds < 200;
            });
            thread.Start();
            thread.Join();

            Assert.IsTrue(wasOk);
        }
    }
}

using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Thriot.Platform.Services.Telemetry.Recording;
using Thriot.Plugins.Core;

namespace Thriot.Platform.Services.Telemetry.Tests
{
    [TestClass]
    public class QueueProcessorTest
    {
        [TestMethod]
        public void StartStopTest()
        {
            var queueReceiverAdapter = Substitute.For<IQueueReceiveAdapter>();

            var queueProcessor = new QueueProcessor(queueReceiverAdapter, null);

            queueProcessor.Start();

            queueReceiverAdapter.ReceivedWithAnyArgs().StartReceiver(null);
            queueReceiverAdapter.DidNotReceive().StopReceiver();

            queueProcessor.Stop();

            queueReceiverAdapter.Received().StopReceiver();
        }

        [TestMethod]
        public void ProcessElementTest()
        {
            var queueReceiverAdapter = new QueueReceiveAdapter();
            var directTelemetryDataService = Substitute.For<IDirectTelemetryDataService>();

            var queueProcessor = new QueueProcessor(queueReceiverAdapter, directTelemetryDataService);

            queueProcessor.Start();

            queueReceiverAdapter.SetTelemetryData(new TelemetryData("1", "{}", DateTime.UtcNow));

            TelemetryData telemetryData = null;
            directTelemetryDataService.When(d => d.RecordTelemetryData(Arg.Any<TelemetryData>())).Do(call => telemetryData = (TelemetryData)call.Args()[0]);

            var ok = false;
            for (int i = 0; i < 30; i++)
            {
                if (telemetryData?.Payload == "{}")
                {
                    ok = true;
                    break;
                }
                Thread.Sleep(100);
            }

            queueProcessor.Stop();

            Assert.IsTrue(ok);
        }

        class QueueReceiveAdapter : IQueueReceiveAdapter
        {
            private readonly Thread _thread;
            private readonly CancellationTokenSource _cts;
            private Action<TelemetryData> _receivedAction;
            private TelemetryData _telemetryData;

            public QueueReceiveAdapter()
            {
                _cts = new CancellationTokenSource();
                _thread = new Thread(Body);
                _telemetryData = null;
            }

            public void SetTelemetryData(TelemetryData telemetryData)
            {
                _telemetryData = telemetryData;
            }

            public void StartReceiver(Action<TelemetryData> receivedAction)
            {
                _receivedAction = receivedAction;
                _thread.Start();
            }

            public void StopReceiver()
            {
                _cts.Cancel();
                _thread.Join();
            }

            private void Body()
            {
                while (!_cts.IsCancellationRequested)
                {
                    if (_telemetryData != null)
                    {
                        _receivedAction(_telemetryData);
                        _telemetryData = null;
                    }
                    Thread.Sleep(10);
                }
            }
        }
    }
}

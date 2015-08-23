using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Thriot.Platform.Services.Telemetry.Recording;
using Thriot.Plugins.Core;
using Thriot.TestHelpers;

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

            queueReceiverAdapter.ReceivedWithAnyArgs().Start(null);
            queueReceiverAdapter.DidNotReceive().Stop();

            queueProcessor.Stop();

            queueReceiverAdapter.Received().Stop();
        }

        [TestMethod]
        public void ProcessElementTest()
        {
            var environmentFactory = EnvironmentFactoryFactory.Create();

            var queueSendAdapter = environmentFactory.QueueSendAdapter;
            queueSendAdapter.Clear();

            var queueReceiverAdapter = environmentFactory.QueueReceiveAdapter;
            var directTelemetryDataService = Substitute.For<IDirectTelemetryDataService>();

            int COUNT = 50;

            var outputList = new List<TelemetryData>(COUNT);
            directTelemetryDataService.When(d => d.RecordTelemetryData(Arg.Any<TelemetryData>())).Do(call => outputList.Add((TelemetryData)call.Args()[0]));

            var queueProcessor = new QueueProcessor(queueReceiverAdapter, directTelemetryDataService);
            queueProcessor.Start();

            var inputList = new List<TelemetryData>(COUNT);

            for (var i = 0; i < COUNT; i++)
            {
                var inputTd = new TelemetryData((i%5).ToString(), $"{{\"p\": {i}}}", DateTime.UtcNow);
                inputList.Add(inputTd);
                queueSendAdapter.Send(inputTd);
            }

            var ok = false;
            for (var r = 0; r < 30; r++)
            {
                if (outputList.Count == COUNT)
                {
                    bool isOk = true;
                    for (var i = 0; i < COUNT; i++)
                    {
                        var input = inputList[i];
                        var output = outputList[i];

                        if (input.DeviceId != output.DeviceId.Trim() || input.Payload != output.Payload)
                        {
                            isOk = false;
                            break;
                        }
                    }
                    if (isOk)
                    {
                        ok = true;
                        break;
                    }
                }
                Thread.Sleep(100);
            }

            queueProcessor.Stop();

            Assert.IsTrue(ok);
        }
    }
}

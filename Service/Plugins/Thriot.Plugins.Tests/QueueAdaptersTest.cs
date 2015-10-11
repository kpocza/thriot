using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Thriot.Plugins.Core;
using Thriot.TestHelpers;

namespace Thriot.Plugins.Tests
{
    [TestClass]
    public class QueueAdaptersTest
    {
        [TestMethod]
        public void QueueSendReceiveTest()
        {
            var environmentFactory = EnvironmentFactoryFactory.Create();

            var queueSendAdapter = environmentFactory.QueueEnvironment.QueueSendAdapter;
            queueSendAdapter.Clear();

            var queueReceiverAdapter = environmentFactory.QueueEnvironment.QueueReceiveAdapter;

            int COUNT = 50;

            var outputList = new List<TelemetryData>(COUNT);
            queueReceiverAdapter.Start(td => outputList.Add(td));

            var inputList = new List<TelemetryData>(COUNT);

            for (var i = 0; i < COUNT; i++)
            {
                var inputTd = new TelemetryData((i % 5).ToString(), $"{{\"p\": {i}}}", DateTime.UtcNow);
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

            queueReceiverAdapter.Stop();

            Assert.IsTrue(ok);
        }
    }
}

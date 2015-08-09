using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Thriot.Framework;
using Thriot.Messaging.Services.Dto;

namespace Thriot.Messaging.Services.Tests
{
    [TestClass]
    public class StressLikeTest : TestBase
    {
        [TestInitialize]
        public void Init()
        {
            Initialize();
        }

        #region Simple tests

        [TestMethod]
        public void EnqueueDequeueTest()
        {
            var deviceId = MessagingService.Initialize(Identity.Next());
            int enqueueCounter = 0;
            int dequeueCounter = 0;
            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;

            var dequeueThread = new Thread(() =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var result = MessagingService.Dequeue(new DeviceListDto() { DeviceIds = new List<long>() { deviceId } });
                    Interlocked.Add(ref dequeueCounter, result.Messages.Count);
                }
            });

            var enqueueThread = new Thread(() =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    MessagingService.Enqueue(new EnqueueMessagesDto
                    {
                        Messages = new List<EnqueueMessageDto>
                        {
                            new EnqueueMessageDto
                            {
                                DeviceId = deviceId,
                                Payload = Encoding.UTF8.GetBytes("Stress test message no. " + enqueueCounter),
                                TimeStamp = DateTime.UtcNow,
                                SenderDeviceId = Identity.Next()
                            }
                        }
                    });
                    Interlocked.Increment(ref enqueueCounter);
                }
            });

            dequeueThread.Start();
            enqueueThread.Start();

            WaitUntil(500, 1000, () => enqueueCounter);
            cancellationTokenSource.Cancel();
            enqueueThread.Join();
            dequeueThread.Join();

            Assert.IsTrue(dequeueCounter > 0);
            Assert.IsTrue(enqueueCounter > 0);
            Assert.IsTrue(enqueueCounter >= dequeueCounter);
        }

        [TestMethod]
        public void EnqueuePeekCommitTest()
        {
            var deviceId = MessagingService.Initialize(Identity.Next());
            int enqueueCounter = 0;
            int peekcommitCounter = 0;
            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;

            var peekcommitThread = new Thread(() =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var resultPeek = MessagingService.Peek(new DeviceListDto() { DeviceIds = new List<long>() { deviceId } });
                    if (resultPeek.Messages.Count == 1)
                    {
                        var resultCommit = MessagingService.Commit(new DeviceListDto() { DeviceIds = new List<long>() { deviceId } });
                        Interlocked.Add(ref peekcommitCounter, resultCommit.DeviceIds.Count);
                    }
                }
            });

            var enqueueThread = new Thread(() =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    MessagingService.Enqueue(new EnqueueMessagesDto
                    {
                        Messages = new List<EnqueueMessageDto>
                        {
                            new EnqueueMessageDto
                            {
                                DeviceId = deviceId,
                                Payload = Encoding.UTF8.GetBytes("Stress test message no. " + enqueueCounter),
                                TimeStamp = DateTime.UtcNow,
                                SenderDeviceId = Identity.Next()
                            }
                        }
                    });
                    Interlocked.Increment(ref enqueueCounter);
                }
            });

            peekcommitThread.Start();
            enqueueThread.Start();

            WaitUntil(500, 1000, () => enqueueCounter);
            cancellationTokenSource.Cancel();
            enqueueThread.Join();
            peekcommitThread.Join();

            Assert.IsTrue(peekcommitCounter > 0);
            Assert.IsTrue(enqueueCounter > 0);
            Assert.IsTrue(enqueueCounter >= peekcommitCounter);
        }

        #endregion

        #region Slowdowned enqueue

        [TestMethod]
        public void SlowEnqueueNormalDequeueTest()
        {
            var deviceId = MessagingService.Initialize(Identity.Next());
            int enqueueCounter = 0;
            int dequeueCounter = 0;
            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;

            var dequeueThread = new Thread(() =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var result = MessagingService.Dequeue(new DeviceListDto() { DeviceIds = new List<long>() { deviceId } });
                    Interlocked.Add(ref dequeueCounter, result.Messages.Count);
                }
            });

            var enqueueThread = new Thread(() =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    MessagingService.Enqueue(new EnqueueMessagesDto
                    {
                        Messages = new List<EnqueueMessageDto>
                        {
                            new EnqueueMessageDto
                            {
                                DeviceId = deviceId,
                                Payload = Encoding.UTF8.GetBytes("Stress test message no. " + enqueueCounter),
                                TimeStamp = DateTime.UtcNow,
                                SenderDeviceId = Identity.Next()
                            }
                        }
                    });
                    Interlocked.Increment(ref enqueueCounter);
                    Thread.Sleep(1);
                }
            });

            dequeueThread.Start();
            enqueueThread.Start();

            WaitUntil(500, 50, () => enqueueCounter);
            cancellationTokenSource.Cancel();
            enqueueThread.Join();
            dequeueThread.Join();

            Assert.IsTrue(dequeueCounter > 0);
            Assert.IsTrue(enqueueCounter > 0);
            Assert.IsTrue(enqueueCounter >= dequeueCounter);
        }

        [TestMethod]
        public void SlowEnqueueNormalPeekCommitTest()
        {
            var deviceId = MessagingService.Initialize(Identity.Next());
            int enqueueCounter = 0;
            int peekcommitCounter = 0;
            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;

            var peekcommitThread = new Thread(() =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var resultPeek = MessagingService.Peek(new DeviceListDto() { DeviceIds = new List<long>() { deviceId } });
                    if (resultPeek.Messages.Count == 1)
                    {
                        var resultCommit = MessagingService.Commit(new DeviceListDto() { DeviceIds = new List<long>() { deviceId } });
                        Interlocked.Add(ref peekcommitCounter, resultCommit.DeviceIds.Count);
                    }
                }
            });

            var enqueueThread = new Thread(() =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    MessagingService.Enqueue(new EnqueueMessagesDto
                    {
                        Messages = new List<EnqueueMessageDto>
                        {
                            new EnqueueMessageDto
                            {
                                DeviceId = deviceId,
                                Payload = Encoding.UTF8.GetBytes("Stress test message no. " + enqueueCounter),
                                TimeStamp = DateTime.UtcNow,
                                SenderDeviceId = Identity.Next()
                            }
                        }
                    });
                    Interlocked.Increment(ref enqueueCounter);
                    Thread.Sleep(1);
                }
            });

            peekcommitThread.Start();
            enqueueThread.Start();

            WaitUntil(500, 50, () => enqueueCounter);
            cancellationTokenSource.Cancel();
            enqueueThread.Join();
            peekcommitThread.Join();

            Assert.IsTrue(peekcommitCounter > 0);
            Assert.IsTrue(enqueueCounter > 0);
            Assert.IsTrue(enqueueCounter >= peekcommitCounter);
        }

        #endregion

        #region Many devices tests

        [TestMethod]
        public void EnqueueDequeueManyDevicesTest()
        {
            var dequeueDictionary = new Dictionary<long, List<string>>();
            var enqueueDictionary = new Dictionary<long, List<string>>();
            const int deviceCount = 100;
            const int dequeueRandomDeviceCount = 30;
            const int enqueueRandomDeviceCount = 5;

            for (int i = 0; i < deviceCount; i++)
            {
                var deviceId = MessagingService.Initialize(Identity.Next());
                dequeueDictionary.Add(deviceId, new List<string>());
                enqueueDictionary.Add(deviceId, new List<string>());
            }

            var deviceIds = dequeueDictionary.Keys.ToList();
            
            int enqueueCounter = 0;
            int dequeueCounter = 0;
            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;

            var dequeueThread = new Thread(() =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var result =
                        MessagingService.Dequeue(new DeviceListDto()
                        {
                            DeviceIds = GetRandomDevices(deviceIds, dequeueRandomDeviceCount)
                        });
                    Interlocked.Add(ref dequeueCounter, result.Messages.Count);
                    foreach (var msg in result.Messages)
                    {
                        dequeueDictionary[msg.DeviceId].Add(Encoding.UTF8.GetString(msg.Payload));
                    }
                }
            });

            var enqueueThread = new Thread(() =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var randomDeviceIds = GetRandomDevices(deviceIds, enqueueRandomDeviceCount);
                    var messagesToEnqueue = new EnqueueMessagesDto {Messages = new List<EnqueueMessageDto>()};
                    foreach (var deviceId in randomDeviceIds)
                    {
                        var nextMsgId = enqueueDictionary[deviceId].Count + 1;
                        var nextMsg = "Stress test message no. " + nextMsgId;
                        enqueueDictionary[deviceId].Add(nextMsg);
                        var msgToEnqueue = new EnqueueMessageDto
                        {
                            DeviceId = deviceId,
                            Payload = Encoding.UTF8.GetBytes(nextMsg),
                            TimeStamp = DateTime.UtcNow,
                            SenderDeviceId = Identity.Next()
                        };
                        messagesToEnqueue.Messages.Add(msgToEnqueue);
                    }

                    var result = MessagingService.Enqueue(messagesToEnqueue);
                    Interlocked.Add(ref enqueueCounter, result.DeviceIds.Count);
                }
            });

            dequeueThread.Start();
            enqueueThread.Start();

            WaitUntil(500, 10000, () => enqueueCounter);
            cancellationTokenSource.Cancel();
            enqueueThread.Join();
            dequeueThread.Join();

            Assert.IsTrue(dequeueCounter > 0);
            Assert.IsTrue(enqueueCounter > 0);
            Assert.IsTrue(enqueueCounter >= dequeueCounter);

            foreach (var deviceId in deviceIds)
            {
                var enqueuedItems = enqueueDictionary[deviceId];
                var dequeuedItems = dequeueDictionary[deviceId];

                Assert.IsTrue(dequeuedItems.Count <= enqueuedItems.Count);
                Assert.IsTrue(dequeuedItems.All(enqueuedItems.Contains));
                var messageNumbers = dequeuedItems.Select(d => int.Parse(d.Substring(d.LastIndexOf(' ')))).ToList();
                Assert.IsTrue(messageNumbers.SequenceEqual(messageNumbers.OrderBy(d => d)));
            }
        }

        [TestMethod]
        public void EnqueuePeekCommitManyDevicesTest()
        {
            var dequeueDictionary = new Dictionary<long, List<string>>();
            var enqueueDictionary = new Dictionary<long, List<string>>();
            const int deviceCount = 100;
            const int dequeueRandomDeviceCount = 30;
            const int enqueueRandomDeviceCount = 5;

            for (int i = 0; i < deviceCount; i++)
            {
                var deviceId = MessagingService.Initialize(Identity.Next());
                dequeueDictionary.Add(deviceId, new List<string>());
                enqueueDictionary.Add(deviceId, new List<string>());
            }

            var deviceIds = dequeueDictionary.Keys.ToList();

            int enqueueCounter = 0;
            int dequeueCounter = 0;
            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;

            var dequeueThread = new Thread(() =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    DequeueMessagesDto result = MessagingService.Peek(new DeviceListDto()
                    {
                        DeviceIds = GetRandomDevices(deviceIds, dequeueRandomDeviceCount)
                    });
                    MessagingService.Commit(new DeviceListDto
                    {
                        DeviceIds = result.Messages.Select(m => m.DeviceId).ToList()
                    });

                    Interlocked.Add(ref dequeueCounter, result.Messages.Count);
                    foreach (var msg in result.Messages)
                    {
                        dequeueDictionary[msg.DeviceId].Add(Encoding.UTF8.GetString(msg.Payload));
                    }
                }
            });

            var enqueueThread = new Thread(() =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var randomDeviceIds = GetRandomDevices(deviceIds, enqueueRandomDeviceCount);
                    var messagesToEnqueue = new EnqueueMessagesDto {Messages = new List<EnqueueMessageDto>()};
                    foreach (var deviceId in randomDeviceIds)
                    {
                        var nextMsgId = enqueueDictionary[deviceId].Count + 1;
                        var nextMsg = "Stress test message no. " + nextMsgId;
                        enqueueDictionary[deviceId].Add(nextMsg);
                        var msgToEnqueue = new EnqueueMessageDto
                        {
                            DeviceId = deviceId,
                            Payload = Encoding.UTF8.GetBytes(nextMsg),
                            TimeStamp = DateTime.UtcNow,
                            SenderDeviceId = Identity.Next()
                        };
                        messagesToEnqueue.Messages.Add(msgToEnqueue);
                    }

                    var result = MessagingService.Enqueue(messagesToEnqueue);
                    Interlocked.Add(ref enqueueCounter, result.DeviceIds.Count);
                }
            });

            dequeueThread.Start();
            enqueueThread.Start();

            WaitUntil(500, 10000, () => enqueueCounter);
            cancellationTokenSource.Cancel();
            enqueueThread.Join();
            dequeueThread.Join();

            Assert.IsTrue(dequeueCounter > 0);
            Assert.IsTrue(enqueueCounter > 0);
            Assert.IsTrue(enqueueCounter >= dequeueCounter);

            foreach (var deviceId in deviceIds)
            {
                var enqueuedItems = enqueueDictionary[deviceId];
                var dequeuedItems = dequeueDictionary[deviceId];

                Assert.IsTrue(dequeuedItems.Count <= enqueuedItems.Count);
                Assert.IsTrue(dequeuedItems.All(enqueuedItems.Contains));
                var messageNumbers = dequeuedItems.Select(d => int.Parse(d.Substring(d.LastIndexOf(' ')))).ToList();
                Assert.IsTrue(messageNumbers.SequenceEqual(messageNumbers.OrderBy(d => d)));
                Assert.AreEqual(messageNumbers.Count, messageNumbers.Distinct().Count());
            }
        }

        #endregion

        private void WaitUntil(int timeout, int limit, Func<int> currentValue)
        {
            var watch = Stopwatch.StartNew();
            while (watch.ElapsedMilliseconds < timeout && currentValue() < limit)
            {
                Thread.Sleep(1);
            }
        }

        private List<long> GetRandomDevices(IReadOnlyList<long> devices, int randomCount)
        {
            var rnd = new Random();
            var hashSet = new HashSet<long>();
            while (hashSet.Count < randomCount)
            {
                hashSet.Add(devices[rnd.Next(devices.Count)]);
            }

            return hashSet.ToList();
        }
    }
}

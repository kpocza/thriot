using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IoT.Framework;
using IoT.Messaging.Dto;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IoT.Messaging.Services.Tests
{
    [TestClass]
    public class PeekTest : TestBase
    {
        [TestInitialize]
        public void Init()
        {
            Initialize();
        }

        [TestMethod]
        public void PeekNothingTest()
        {
            var deviceId = MessagingService.Initialize(Identity.Next());

            var result = MessagingService.Peek(new DeviceListDto {DeviceIds = new List<long> {deviceId}});

            Assert.AreEqual(0, result.Messages.Count);
        }

        [TestMethod]
        public void PeekOneTest()
        {
            var deviceId = MessagingService.Initialize(Identity.Next());

            MessagingService.Enqueue(new EnqueueMessagesDto
            {
                Messages =
                    new List<EnqueueMessageDto>
                    {
                        new EnqueueMessageDto
                        {
                            DeviceId = deviceId,
                            Payload = Encoding.UTF8.GetBytes("Message no. 1"),
                            TimeStamp = DateTime.UtcNow
                        }
                    }
            });

            var result = MessagingService.Peek(new DeviceListDto { DeviceIds = new List<long> { deviceId } });

            Assert.AreEqual(1, result.Messages.Count);
            Assert.AreEqual(deviceId, result.Messages[0].DeviceId);
            Assert.IsTrue(Encoding.UTF8.GetBytes("Message no. 1").SequenceEqual(result.Messages[0].Payload));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TryPeekSameDeviceTest()
        {
            var deviceId = MessagingService.Initialize(Identity.Next());

            MessagingService.Peek(new DeviceListDto { DeviceIds = new List<long> { deviceId, deviceId } });
        }

        [TestMethod]
        public void PeekKickCacheTest()
        {
            var deviceId = MessagingService.Initialize(Identity.Next());

            MessagingService.Enqueue(new EnqueueMessagesDto
            {
                Messages =
                    new List<EnqueueMessageDto>
                    {
                        new EnqueueMessageDto
                        {
                            DeviceId = deviceId,
                            Payload = Encoding.UTF8.GetBytes("Message no. 1"),
                            TimeStamp = DateTime.UtcNow
                        }
                    }
            });

            RemoveCacheItem(deviceId, 0);

            var result = MessagingService.Peek(new DeviceListDto { DeviceIds = new List<long> { deviceId } });

            Assert.AreEqual(1, result.Messages.Count);
            Assert.AreEqual(deviceId, result.Messages[0].DeviceId);
            Assert.IsTrue(Encoding.UTF8.GetBytes("Message no. 1").SequenceEqual(result.Messages[0].Payload));
        }

        [TestMethod]
        public void PeekTwiceTheSameTest()
        {
            var deviceId = MessagingService.Initialize(Identity.Next());

            MessagingService.Enqueue(new EnqueueMessagesDto
            {
                Messages =
                    new List<EnqueueMessageDto>
                    {
                        new EnqueueMessageDto
                        {
                            DeviceId = deviceId,
                            Payload = Encoding.UTF8.GetBytes("Message no. 1"),
                            TimeStamp = DateTime.UtcNow
                        }
                    }
            });

            var result = MessagingService.Peek(new DeviceListDto { DeviceIds = new List<long> { deviceId } });

            Assert.AreEqual(1, result.Messages.Count);
            Assert.AreEqual(deviceId, result.Messages[0].DeviceId);
            Assert.IsTrue(Encoding.UTF8.GetBytes("Message no. 1").SequenceEqual(result.Messages[0].Payload));

            result = MessagingService.Peek(new DeviceListDto { DeviceIds = new List<long> { deviceId } });

            Assert.AreEqual(1, result.Messages.Count);
            Assert.AreEqual(deviceId, result.Messages[0].DeviceId);
            Assert.IsTrue(Encoding.UTF8.GetBytes("Message no. 1").SequenceEqual(result.Messages[0].Payload));
        }

        [TestMethod]
        public void PeekOneNoInMemoryTest()
        {
            var deviceId = MessagingService.Initialize(Identity.Next());

            MessagingService.Enqueue(new EnqueueMessagesDto
            {
                Messages =
                    new List<EnqueueMessageDto>
                    {
                        new EnqueueMessageDto
                        {
                            DeviceId = deviceId,
                            Payload = Encoding.UTF8.GetBytes("Message no. 1"),
                            TimeStamp = DateTime.UtcNow
                        }
                    }
            });

            DeviceEntryRegistry.Instance.Remove(deviceId);

            var result = MessagingService.Peek(new DeviceListDto { DeviceIds = new List<long> { deviceId } });

            Assert.AreEqual(1, result.Messages.Count);
            Assert.AreEqual(deviceId, result.Messages[0].DeviceId);
            Assert.IsTrue(Encoding.UTF8.GetBytes("Message no. 1").SequenceEqual(result.Messages[0].Payload));
        }

        [TestMethod]
        public void EnqueuePeekEnqueueManyPeekTest()
        {
            var deviceId = MessagingService.Initialize(Identity.Next());

            MessagingService.Enqueue(new EnqueueMessagesDto
            {
                Messages =
                    new List<EnqueueMessageDto>
                    {
                        new EnqueueMessageDto
                        {
                            DeviceId = deviceId,
                            Payload = Encoding.UTF8.GetBytes("Message no. pre"),
                            TimeStamp = DateTime.UtcNow
                        }
                    }
            });

            var result = MessagingService.Peek(new DeviceListDto { DeviceIds = new List<long> { deviceId } });

            Assert.AreEqual(1, result.Messages.Count);
            Assert.AreEqual(deviceId, result.Messages[0].DeviceId);
            Assert.IsTrue(Encoding.UTF8.GetBytes("Message no. pre").SequenceEqual(result.Messages[0].Payload));

            for (int i = 0; i < 98; i++)
            {
                MessagingService.Enqueue(new EnqueueMessagesDto
                {
                    Messages =
                        new List<EnqueueMessageDto>
                        {
                            new EnqueueMessageDto
                            {
                                DeviceId = deviceId,
                                Payload = Encoding.UTF8.GetBytes("Message no. " + i),
                                TimeStamp = DateTime.UtcNow
                            }
                        }
                });
            }

            result = MessagingService.Peek(new DeviceListDto { DeviceIds = new List<long> { deviceId } });

            Assert.AreEqual(1, result.Messages.Count);
            Assert.AreEqual(deviceId, result.Messages[0].DeviceId);
            Assert.IsTrue(Encoding.UTF8.GetBytes("Message no. pre").SequenceEqual(result.Messages[0].Payload));

            MessagingService.Enqueue(new EnqueueMessagesDto
            {
                Messages =
                    new List<EnqueueMessageDto>
                        {
                            new EnqueueMessageDto
                            {
                                DeviceId = deviceId,
                                Payload = Encoding.UTF8.GetBytes("Message no. post"),
                                TimeStamp = DateTime.UtcNow
                            }
                        }
            });

            result = MessagingService.Peek(new DeviceListDto { DeviceIds = new List<long> { deviceId } });

            Assert.AreEqual(1, result.Messages.Count);
            Assert.AreEqual(deviceId, result.Messages[0].DeviceId);
            Assert.IsTrue(Encoding.UTF8.GetBytes("Message no. 0").SequenceEqual(result.Messages[0].Payload));
        }
    }
}

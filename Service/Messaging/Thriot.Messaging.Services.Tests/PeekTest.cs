using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Thriot.Framework;
using Thriot.Messaging.Dto;

namespace Thriot.Messaging.Services.Tests
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

            string senderDeviceId = Identity.Next();
            MessagingService.Enqueue(new EnqueueMessagesDto
            {
                Messages =
                    new List<EnqueueMessageDto>
                    {
                        new EnqueueMessageDto
                        {
                            DeviceId = deviceId,
                            Payload = Encoding.UTF8.GetBytes("Message no. 1"),
                            TimeStamp = DateTime.UtcNow,
                            SenderDeviceId = senderDeviceId
                        }
                    }
            });

            var result = MessagingService.Peek(new DeviceListDto { DeviceIds = new List<long> { deviceId } });

            Assert.AreEqual(1, result.Messages.Count);
            Assert.AreEqual(deviceId, result.Messages[0].DeviceId);
            Assert.IsTrue(Encoding.UTF8.GetBytes("Message no. 1").SequenceEqual(result.Messages[0].Payload));
            Assert.AreEqual(senderDeviceId, result.Messages[0].SenderDeviceId);
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

            string senderDeviceId = Identity.Next();
            MessagingService.Enqueue(new EnqueueMessagesDto
            {
                Messages =
                    new List<EnqueueMessageDto>
                    {
                        new EnqueueMessageDto
                        {
                            DeviceId = deviceId,
                            Payload = Encoding.UTF8.GetBytes("Message no. 1"),
                            TimeStamp = DateTime.UtcNow,
                            SenderDeviceId = senderDeviceId
                        }
                    }
            });

            RemoveCacheItem(deviceId, 0);

            var result = MessagingService.Peek(new DeviceListDto { DeviceIds = new List<long> { deviceId } });

            Assert.AreEqual(1, result.Messages.Count);
            Assert.AreEqual(deviceId, result.Messages[0].DeviceId);
            Assert.IsTrue(Encoding.UTF8.GetBytes("Message no. 1").SequenceEqual(result.Messages[0].Payload));
            Assert.AreEqual(senderDeviceId, result.Messages[0].SenderDeviceId);
        }

        [TestMethod]
        public void PeekTwiceTheSameTest()
        {
            var deviceId = MessagingService.Initialize(Identity.Next());

            string senderDeviceId = Identity.Next();
            MessagingService.Enqueue(new EnqueueMessagesDto
            {
                Messages =
                    new List<EnqueueMessageDto>
                    {
                        new EnqueueMessageDto
                        {
                            DeviceId = deviceId,
                            Payload = Encoding.UTF8.GetBytes("Message no. 1"),
                            TimeStamp = DateTime.UtcNow,
                            SenderDeviceId = senderDeviceId
                        }
                    }
            });

            var result = MessagingService.Peek(new DeviceListDto { DeviceIds = new List<long> { deviceId } });

            Assert.AreEqual(1, result.Messages.Count);
            Assert.AreEqual(deviceId, result.Messages[0].DeviceId);
            Assert.IsTrue(Encoding.UTF8.GetBytes("Message no. 1").SequenceEqual(result.Messages[0].Payload));
            Assert.AreEqual(senderDeviceId, result.Messages[0].SenderDeviceId);

            result = MessagingService.Peek(new DeviceListDto { DeviceIds = new List<long> { deviceId } });

            Assert.AreEqual(1, result.Messages.Count);
            Assert.AreEqual(deviceId, result.Messages[0].DeviceId);
            Assert.IsTrue(Encoding.UTF8.GetBytes("Message no. 1").SequenceEqual(result.Messages[0].Payload));
            Assert.AreEqual(senderDeviceId, result.Messages[0].SenderDeviceId);
        }

        [TestMethod]
        public void PeekOneNoInMemoryTest()
        {
            var deviceId = MessagingService.Initialize(Identity.Next());

            string senderDeviceId = Identity.Next();
            MessagingService.Enqueue(new EnqueueMessagesDto
            {
                Messages =
                    new List<EnqueueMessageDto>
                    {
                        new EnqueueMessageDto
                        {
                            DeviceId = deviceId,
                            Payload = Encoding.UTF8.GetBytes("Message no. 1"),
                            TimeStamp = DateTime.UtcNow,
                            SenderDeviceId = senderDeviceId
                        }
                    }
            });

            DeviceEntryRegistry.Instance.Remove(deviceId);

            var result = MessagingService.Peek(new DeviceListDto { DeviceIds = new List<long> { deviceId } });

            Assert.AreEqual(1, result.Messages.Count);
            Assert.AreEqual(deviceId, result.Messages[0].DeviceId);
            Assert.IsTrue(Encoding.UTF8.GetBytes("Message no. 1").SequenceEqual(result.Messages[0].Payload));
            Assert.AreEqual(senderDeviceId, result.Messages[0].SenderDeviceId);
        }

        [TestMethod]
        public void EnqueuePeekEnqueueManyPeekTest()
        {
            var deviceId = MessagingService.Initialize(Identity.Next());

            string senderDeviceId = Identity.Next();
            MessagingService.Enqueue(new EnqueueMessagesDto
            {
                Messages =
                    new List<EnqueueMessageDto>
                    {
                        new EnqueueMessageDto
                        {
                            DeviceId = deviceId,
                            Payload = Encoding.UTF8.GetBytes("Message no. pre"),
                            TimeStamp = DateTime.UtcNow,
                            SenderDeviceId = senderDeviceId
                        }
                    }
            });

            var result = MessagingService.Peek(new DeviceListDto { DeviceIds = new List<long> { deviceId } });

            Assert.AreEqual(1, result.Messages.Count);
            Assert.AreEqual(deviceId, result.Messages[0].DeviceId);
            Assert.IsTrue(Encoding.UTF8.GetBytes("Message no. pre").SequenceEqual(result.Messages[0].Payload));
            Assert.AreEqual(senderDeviceId, result.Messages[0].SenderDeviceId);

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
                                TimeStamp = DateTime.UtcNow,
                                SenderDeviceId = Identity.Next()
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
                                TimeStamp = DateTime.UtcNow,
                                SenderDeviceId = Identity.Next()
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

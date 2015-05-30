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
    public class DequeueTest : TestBase
    {
        [TestInitialize]
        public void Init()
        {
            Initialize();
        }

        [TestMethod]
        public void DequeueNothingTest()
        {
            var deviceId = MessagingService.Initialize(Identity.Next());

            var result = MessagingService.Dequeue(new DeviceListDto {DeviceIds = new List<long> {deviceId}});

            Assert.AreEqual(0, result.Messages.Count);
        }

        [TestMethod]
        public void DequeueOneTest()
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

            var result = MessagingService.Dequeue(new DeviceListDto { DeviceIds = new List<long> { deviceId } });

            Assert.AreEqual(1, result.Messages.Count);
            Assert.AreEqual(deviceId, result.Messages[0].DeviceId);
            Assert.IsTrue(Encoding.UTF8.GetBytes("Message no. 1").SequenceEqual(result.Messages[0].Payload));
            Assert.AreEqual(senderDeviceId, result.Messages[0].SenderDeviceId);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TryDequeueSameDeviceTest()
        {
            var deviceId = MessagingService.Initialize(Identity.Next());

            MessagingService.Dequeue(new DeviceListDto { DeviceIds = new List<long> { deviceId, deviceId } });
        }

        [TestMethod]
        public void DequeueKickCacheTest()
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

            var result = MessagingService.Dequeue(new DeviceListDto { DeviceIds = new List<long> { deviceId } });

            Assert.AreEqual(1, result.Messages.Count);
            Assert.AreEqual(deviceId, result.Messages[0].DeviceId);
            Assert.IsTrue(Encoding.UTF8.GetBytes("Message no. 1").SequenceEqual(result.Messages[0].Payload));
            Assert.AreEqual(senderDeviceId, result.Messages[0].SenderDeviceId);
        }

        [TestMethod]
        public void DequeueOneNothingTest()
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

            var result = MessagingService.Dequeue(new DeviceListDto { DeviceIds = new List<long> { deviceId } });

            Assert.AreEqual(1, result.Messages.Count);
            Assert.AreEqual(deviceId, result.Messages[0].DeviceId);
            Assert.IsTrue(Encoding.UTF8.GetBytes("Message no. 1").SequenceEqual(result.Messages[0].Payload));
            Assert.AreEqual(senderDeviceId, result.Messages[0].SenderDeviceId);

            result = MessagingService.Dequeue(new DeviceListDto { DeviceIds = new List<long> { deviceId } });

            Assert.AreEqual(0, result.Messages.Count);
        }

        [TestMethod]
        public void DequeueOneManyTimesTest()
        {
            var deviceId = MessagingService.Initialize(Identity.Next());

            for (int i = 0; i < 120; i++)
            {
                string senderDeviceId = Identity.Next();
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
                                SenderDeviceId = senderDeviceId
                            }
                        }
                });

                var result = MessagingService.Dequeue(new DeviceListDto {DeviceIds = new List<long> {deviceId}});

                Assert.AreEqual(1, result.Messages.Count);
                Assert.AreEqual(deviceId, result.Messages[0].DeviceId);
                Assert.IsTrue(Encoding.UTF8.GetBytes("Message no. " + i).SequenceEqual(result.Messages[0].Payload));
                Assert.AreEqual(senderDeviceId, result.Messages[0].SenderDeviceId);
            }
        }

        [TestMethod]
        public void Enqueue110TimesDequeue100TimesTest()
        {
            var deviceId = MessagingService.Initialize(Identity.Next());

            for (int i = 0; i < 110; i++)
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

            for (int i = 11; i < 110; i++)
            {
                var result = MessagingService.Dequeue(new DeviceListDto {DeviceIds = new List<long> {deviceId}});

                Assert.AreEqual(1, result.Messages.Count);
                Assert.AreEqual(deviceId, result.Messages[0].DeviceId);
                Assert.IsTrue(Encoding.UTF8.GetBytes("Message no. " + i).SequenceEqual(result.Messages[0].Payload));
                Assert.AreEqual(32, result.Messages[0].SenderDeviceId.Length);
            }

            var noresult = MessagingService.Dequeue(new DeviceListDto { DeviceIds = new List<long> { deviceId } });
            
            Assert.AreEqual(0, noresult.Messages.Count);
        }


        [TestMethod]
        public void DequeueOneNoInMemoryTest()
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

            var result = MessagingService.Dequeue(new DeviceListDto { DeviceIds = new List<long> { deviceId } });

            Assert.AreEqual(1, result.Messages.Count);
            Assert.AreEqual(deviceId, result.Messages[0].DeviceId);
            Assert.IsTrue(Encoding.UTF8.GetBytes("Message no. 1").SequenceEqual(result.Messages[0].Payload));
            Assert.AreEqual(senderDeviceId, result.Messages[0].SenderDeviceId);
        }
    }
}

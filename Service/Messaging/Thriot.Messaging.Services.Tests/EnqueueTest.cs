using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Thriot.Framework;
using Thriot.Messaging.Services.Dto;

namespace Thriot.Messaging.Services.Tests
{
    [TestClass]
    public class EnqueueTest : TestBase
    {
        [TestInitialize]
        public void Init()
        {
            Initialize();
        }

        [TestMethod]
        public void EnqueueOneTest()
        {
            var deviceId = MessagingService.Initialize(Identity.Next());

            var result = MessagingService.Enqueue(new EnqueueMessagesDto
            {
                Messages =
                    new List<EnqueueMessageDto>
                    {
                        new EnqueueMessageDto
                        {
                            DeviceId = deviceId,
                            Payload = Encoding.UTF8.GetBytes("Message no. 1"),
                            TimeStamp = DateTime.UtcNow,
                            SenderDeviceId = Identity.Next()
                        }
                    }
            });

            Assert.AreEqual(1, result.DeviceIds.Count);
            Assert.AreEqual(deviceId, result.DeviceIds[0]);

            var deviceEntry = DeviceEntryRegistry.Instance.Get(deviceId);
            Assert.AreEqual(deviceId, deviceEntry.Id);
            Assert.AreEqual(0, deviceEntry.DequeueIndex);
            Assert.AreEqual(1, deviceEntry.EnqueueIndex);
            Assert.AreEqual(1, deviceEntry.Version);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TryEnqueueSameDeviceTest()
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
                            TimeStamp = DateTime.UtcNow,
                            SenderDeviceId = Identity.Next()
                        },
                        new EnqueueMessageDto
                        {
                            DeviceId = deviceId,
                            Payload = Encoding.UTF8.GetBytes("Message no. 2"),
                            TimeStamp = DateTime.UtcNow,
                            SenderDeviceId = Identity.Next()
                        }
                    }
            });
        }

        [TestMethod]
        public void EnqueueTenTimesTest()
        {
            var deviceId = MessagingService.Initialize(Identity.Next());

            for (int i = 1; i <= 10; i++)
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

            var deviceEntry = DeviceEntryRegistry.Instance.Get(deviceId);
            Assert.AreEqual(0, deviceEntry.DequeueIndex);
            Assert.AreEqual(10, deviceEntry.EnqueueIndex);
            Assert.AreEqual(10, deviceEntry.Version);
        }

        [TestMethod]
        public void Enqueue99Plus1Plus1TimesTest()
        {
            var deviceId = MessagingService.Initialize(Identity.Next());

            for (int i = 1; i <= 99; i++)
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

            var deviceEntry = DeviceEntryRegistry.Instance.Get(deviceId);
            Assert.AreEqual(0, deviceEntry.DequeueIndex);
            Assert.AreEqual(99, deviceEntry.EnqueueIndex);
            Assert.AreEqual(99, deviceEntry.Version);

            MessagingService.Enqueue(new EnqueueMessagesDto
            {
                Messages =
                    new List<EnqueueMessageDto>
                        {
                            new EnqueueMessageDto
                            {
                                DeviceId = deviceId,
                                Payload = Encoding.UTF8.GetBytes("Message no. 100"),
                                TimeStamp = DateTime.UtcNow,
                                SenderDeviceId = Identity.Next()
                            }
                        }
            });

            deviceEntry = DeviceEntryRegistry.Instance.Get(deviceId);
            Assert.AreEqual(1, deviceEntry.DequeueIndex);
            Assert.AreEqual(100, deviceEntry.EnqueueIndex);
            Assert.AreEqual(100, deviceEntry.Version);

            MessagingService.Enqueue(new EnqueueMessagesDto
            {
                Messages =
                    new List<EnqueueMessageDto>
                        {
                            new EnqueueMessageDto
                            {
                                DeviceId = deviceId,
                                Payload = Encoding.UTF8.GetBytes("Message no. 100"),
                                TimeStamp = DateTime.UtcNow,
                                SenderDeviceId = Identity.Next()
                            }
                        }
            });

            deviceEntry = DeviceEntryRegistry.Instance.Get(deviceId);
            Assert.AreEqual(2, deviceEntry.DequeueIndex);
            Assert.AreEqual(101, deviceEntry.EnqueueIndex);
            Assert.AreEqual(101, deviceEntry.Version);
        }
    }
}

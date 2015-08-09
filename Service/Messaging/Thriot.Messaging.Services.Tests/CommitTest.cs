using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Thriot.Framework;
using Thriot.Messaging.Services.Dto;

namespace Thriot.Messaging.Services.Tests
{
    [TestClass]
    public class CommitTest : TestBase
    {
        [TestInitialize]
        public void Init()
        {
            Initialize();
        }

        [TestMethod]
        public void CommitNothingTest()
        {
            var deviceId = MessagingService.Initialize(Identity.Next());

            var result = MessagingService.Commit(new DeviceListDto {DeviceIds = new List<long> {deviceId}});

            Assert.AreEqual(0, result.DeviceIds.Count);
        }

        [TestMethod]
        public void CommitOneTest()
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
                        }
                    }
            });

            MessagingService.Peek(new DeviceListDto { DeviceIds = new List<long> { deviceId } });

            var result = MessagingService.Commit(new DeviceListDto { DeviceIds = new List<long> { deviceId } });

            Assert.AreEqual(1, result.DeviceIds.Count);
            Assert.AreEqual(deviceId, result.DeviceIds[0]);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TryCommitSameDeviceTest()
        {
            var deviceId = MessagingService.Initialize(Identity.Next());

            MessagingService.Commit(new DeviceListDto { DeviceIds = new List<long> { deviceId, deviceId } });
        }

        [TestMethod]
        public void TryCommitTwiceTest()
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
                        }
                    }
            });

            MessagingService.Peek(new DeviceListDto { DeviceIds = new List<long> { deviceId } });

            MessagingService.Commit(new DeviceListDto { DeviceIds = new List<long> { deviceId } });
            MessagingService.Commit(new DeviceListDto { DeviceIds = new List<long> { deviceId } });

            var result = MessagingService.Peek(new DeviceListDto { DeviceIds = new List<long> { deviceId } });

            Assert.AreEqual(0, result.Messages.Count);
        }

        [TestMethod]
        public void TryCommitNoInMemoryTest()
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
                        }
                    }
            });

            MessagingService.Peek(new DeviceListDto { DeviceIds = new List<long> { deviceId } });

            DeviceEntryRegistry.Instance.Remove(deviceId);

            var result = MessagingService.Commit(new DeviceListDto { DeviceIds = new List<long> { deviceId } });

            Assert.AreEqual(1, result.DeviceIds.Count);
        }
    }
}

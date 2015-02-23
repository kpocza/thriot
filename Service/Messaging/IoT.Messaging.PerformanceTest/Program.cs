using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IoT.Framework;
using IoT.ServiceClient.Messaging;

namespace IoT.Messaging.PerformanceTest
{
    class Program
    {
        private static int _enc;
        private static int _dec;
        private static int _encI;
        private static int _decI;
        private static int _decIAll;
        private static List<long> _deviceIds;
        private const int _stepSize = 100;

        private const int QueueSize = 10000;
        private const int EnqueueBatch = 100;
        private const int DequeueBatch = 1000;

        private static IMessagingService _messagingService;

        public static void Main(string[] args)
        {
            //_messagingService = new PureDatabaseCalls();
            //_messagingService = new InprocMessagingService();
            _messagingService = new WebApiMessagingService();
            _messagingService.Setup("http://localhost/msvc/v1/messaging", "bb9lWD60BTCBWJO0FlOwtVZxLn0lrC3/");

            _deviceIds = new List<long>();

            for (int i = 0; i < QueueSize; i++)
            {
                _deviceIds.Add(_messagingService.Initialize(Identity.Next()));
                if (i % _stepSize == 0)
                {
                    Console.WriteLine("Idx: " + i);
                }
            }

            _enc = 0;
            _dec = 0;
            _decI = 0;
            _decIAll = 0;

            for (int inst = 0; inst < 10; inst++)
            {
                Task.Factory.StartNew(Enqueue);

                Task.Factory.StartNew(DequeueMany);
            }

            Console.WriteLine("Press enter to exit");
            Console.ReadLine();
        }

        private static void Enqueue()
        {
            var rndE = new Random();
            var prevDate = DateTime.UtcNow;
            var prevEnci = 0;

            while (true)
            {
                var deviceIdSet = new HashSet<long>();
                for (int i = 0; i < EnqueueBatch; i++)
                {
                    deviceIdSet.Add(_deviceIds[rndE.Next(QueueSize)]);
                }

                var msgs = new EnqueueMessagesDto
                {
                    Messages = deviceIdSet.Select(d => new EnqueueMessageDto
                    {
                        DeviceId = d,
                        Payload = Enumerable.Range(1, 200).Select(i => (byte)i).ToArray(),
                        TimeStamp = DateTime.UtcNow
                    }).ToList()
                };

                _messagingService.Enqueue(msgs);

                Interlocked.Increment(ref _enc);
                Interlocked.Add(ref _encI, msgs.Messages.Count);

                if (_enc % _stepSize == 0)
                {
                    var now = DateTime.UtcNow;

                    Console.WriteLine("Enqueue: " + EnqueueBatch * _enc + " perf: " + (_encI - prevEnci) * TimeSpan.FromSeconds(1).Ticks / (now - prevDate).Ticks + " msg/s");

                    prevDate = now;
                    prevEnci = _encI;
                }
                Thread.Sleep(200);
            }
        }

        private static void DequeueMany()
        {
            var rndD = new Random();
            var prevDate = DateTime.UtcNow;
            int prevDeci = 0;
            int prevDecIAll = 0;

            while (true)
            {
                var deviceIdSet = new HashSet<long>();
                for (int i = 0; i < DequeueBatch; i++)
                {
                    deviceIdSet.Add(_deviceIds[rndD.Next(QueueSize)]);
                }

                var result = _messagingService.Dequeue(new DeviceListDto { DeviceIds = deviceIdSet.ToList()});

                Interlocked.Increment(ref _dec);
                Interlocked.Add(ref _decI, result.Messages.Count);
                Interlocked.Add(ref _decIAll, deviceIdSet.Count);

                if (_dec % _stepSize == 0)
                {
                    var now = DateTime.UtcNow;

                    Console.WriteLine("Dequeue: " + _dec + " items " + _decI + " perf: " + (_decI - prevDeci) * TimeSpan.FromSeconds(1).Ticks / (now - prevDate).Ticks + " msg/s" + " perf all: " + (_decIAll - prevDecIAll) * TimeSpan.FromSeconds(1).Ticks / (now - prevDate).Ticks + " msg/s");

                    prevDate = now;
                    prevDeci = _decI;
                    prevDecIAll = _decIAll;
                    Thread.Sleep(100);
                }
            }
        }

        private static void PeekCommitMany()
        {
            var rndD = new Random();
            var prevDate = DateTime.UtcNow;
            int prevDeci = 0;
            int prevDecIAll = 0;

            while (true)
            {
                var deviceIdSet = new HashSet<long>();
                for (int i = 0; i < DequeueBatch; i++)
                {
                    deviceIdSet.Add(_deviceIds[rndD.Next(QueueSize)]);
                }

                var result = _messagingService.Peek(new DeviceListDto { DeviceIds = deviceIdSet.ToList() });
                _messagingService.Commit(new DeviceListDto
                {
                    DeviceIds = result.Messages.Select(m => m.DeviceId).ToList()
                });

                Interlocked.Increment(ref _dec);
                Interlocked.Add(ref _decI, result.Messages.Count);
                Interlocked.Add(ref _decIAll, deviceIdSet.Count);

                if (_dec % _stepSize == 0)
                {
                    var now = DateTime.UtcNow;

                    Console.WriteLine("Dequeue: " + _dec + " items " + _decI + " perf: " + (_decI - prevDeci) * TimeSpan.FromSeconds(1).Ticks / (now - prevDate).Ticks + " msg/s" + " perf all: " + (_decIAll - prevDecIAll) * TimeSpan.FromSeconds(1).Ticks / (now - prevDate).Ticks + " msg/s");

                    prevDate = now;
                    prevDeci = _decI;
                    prevDecIAll = _decIAll;
                    Thread.Sleep(100);
                }
            }
        }
    }
}

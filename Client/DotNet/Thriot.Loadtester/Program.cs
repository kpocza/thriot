using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
using Thriot.Client.DotNet.Management;
using Thriot.Client.DotNet.Platform;

namespace Thriot.Loadtester
{
    class Program
    {
        private static Config Config { get; set; }

        static void Main(string[] args)
        {
            ServicePointManager.DefaultConnectionLimit = 200;
            var jsonSerializer = new DataContractJsonSerializer(typeof(Config));

            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(File.ReadAllText(args[0]))))
            {
                ms.Position = 0;
                Config = (Config)jsonSerializer.ReadObject(ms);
            }

            string operation = args[1];
            if (operation == "/generate")
            {
                RegisterDevice(args[2], int.Parse(args[3]));
                return;
            }

            if (operation == "/generateforuser")
            {
                RegisterDevice(args[2], int.Parse(args[3]), args[4], args[5]);
                return;
            }

            var lines = File.ReadAllLines(args[2]);
            var deviceOrders =
                args[3].Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();
            var sleep = int.Parse(args[4]);

            if (operation == "/ocrecord")
            {
                StartMulti(deviceOrders, deviceOrder => OcassionalRecord(lines, deviceOrder, sleep));
            }
            if (operation == "/ocsendto")
            {
                StartMulti(deviceOrders, deviceOrder => OcassionalSend(lines, deviceOrder, sleep, int.Parse(args[5])));
            }
            if (operation == "/ocrecvforget")
            {
                StartMulti(deviceOrders, deviceOrder => OcassionalRecvForget(lines, deviceOrder, sleep));
            }
            if (operation == "/ocrecvcommit")
            {
                StartMulti(deviceOrders, deviceOrder => OcassionalRecvCommit(lines, deviceOrder, sleep));
            }

            if (operation == "/precord")
            {
                StartMulti(deviceOrders, deviceOrder => PersistentRecord(lines, deviceOrder, sleep));
            }
            if (operation == "/psendto")
            {
                StartMulti(deviceOrders, deviceOrder => PersistentSend(lines, deviceOrder, sleep, int.Parse(args[5])));
            }
            if (operation == "/precvforget")
            {
                StartMulti(deviceOrders, deviceOrder => PersistentRecvForget(lines, deviceOrder, sleep));
            }
            if (operation == "/precvcommit")
            {
                StartMulti(deviceOrders, deviceOrder => PersistentRecvCommit(lines, deviceOrder, sleep));
            }
            Console.ReadLine();
        }

        private static void StartMulti(IEnumerable<int> deviceOrders, Action<int> operation)
        {
            foreach (var deviceOrder in deviceOrders)
            {
                var thread = new Thread(() => operation(deviceOrder));
                thread.Start();
            }
        }

        private static void RegisterDevice(string devicesFile, int count, string email = null, string password = null)
        {
            var lines = new List<string>();

            var managementClient = new ManagementClient(Config.ManagementApi);

            if (email == null && password == null)
            {
                email = Guid.NewGuid() + "@test.hu";
                managementClient.User.Register(new Register
                {
                    Email = email,
                    Name = "test user",
                    Password = "p@ssw0rd"
                });
            }
            else
            {
                managementClient.User.Login(new Login {Email = email, Password = password});
            }

            Console.WriteLine("Username: {0}, Password: {1}", email, password);

            var companyId = managementClient.Company.Create(new Company { Name = "company" + DateTime.UtcNow.Ticks });
            var serviceId =
                managementClient.Service.Create(new Service { CompanyId = companyId, Name = "service" });
            var networkId =
                managementClient.Network.Create(new Network
                {
                    CompanyId = companyId,
                    ServiceId = serviceId,
                    Name = "árvíztűrő tükörfúrógép"
                });

            var messageSinkParameters = new List<TelemetryDataSinkParameters>
                {
                    new TelemetryDataSinkParameters
                    {
                        SinkName = Config.SinkData,
                        Parameters = new Dictionary<string, string>()
                    },
                    new TelemetryDataSinkParameters
                    {
                        SinkName = Config.SinkTimeSeries,
                        Parameters = new Dictionary<string, string>()
                    }
                };

            managementClient.Service.UpdateIncomingTelemetryDataSinks(serviceId, messageSinkParameters);
            var apiKey = managementClient.Service.Get(serviceId).ApiKey;

            for (int i = 0; i < count; i++)
            {
                try
                {
                    var deviceId =
                        managementClient.Device.Create(new Device
                        {
                            CompanyId = companyId,
                            ServiceId = serviceId,
                            NetworkId = networkId,
                            Name = "árvíztűrő tükörfúrógép" + i
                        });

                    var line = string.Format("{0} {1}", deviceId, apiKey);
                    lines.Add(line);
                    Log(i + ": " + line);
                }
                catch (Exception ex)
                {
                    Log(ex);
                    i--;
                }
            }

            File.WriteAllLines(devicesFile, lines);
        }

        private static void OcassionalRecord(string[] lines, int deviceOrder, int sleep)
        {
            var line = lines[deviceOrder];

            var parts = line.Split(' ');

            var deviceId = parts[0];
            var apiKey = parts[1];

            Log(deviceId);

            var ocassionalConnectionClient = new OccasionallyConnectionClient(Config.PlatformApi, deviceId, apiKey);
            var rnd = new Random();
            int cnt = 0;
            while (true)
            {
                try
                {
                    var doit = rnd.Next(100);
                    var a = doit%10 == 0 ? string.Format(", \"A\": {0}", rnd.Next(10)) : "";
                    var b = doit%15 == 0 ? string.Format(", \"B\": {0}", rnd.Next(20)) : "";

                    ocassionalConnectionClient.RecordTelemetryData(
                        string.Format("{{\"Temperature\": {0}, \"Humidity\": {1}, \"Time\": {2}{3}}}", rnd.Next(10) + 18, rnd.Next(40) + 40, DateTime.UtcNow.Second, a + b));
                    cnt++;
                }
                catch (Exception ex)
                {
                    Log(ex);
                }
                Thread.Sleep(sleep);
                Log(cnt.ToString());
            }
        }

        private static void OcassionalSend(string[] lines, int deviceOrder, int sleep, int maxSend)
        {
            var line = lines[deviceOrder];

            var parts = line.Split(' ');

            var deviceId = parts[0];
            var apiKey = parts[1];

            Log(deviceId);

            var ocassionalConnectionClient = new OccasionallyConnectionClient(Config.PlatformApi, deviceId, apiKey);
            var rnd = new Random();

            int cnt = 0;
            while (true)
            {
                var target = lines[rnd.Next(maxSend)];
                var targetDeviceId = target.Split(' ')[0];

                try
                {
                    ocassionalConnectionClient.SendMessageTo(targetDeviceId, "{\"Temperature:\": 24, \"Humidity\": 60, \"Time\":" + DateTime.UtcNow.Ticks + "}");
                    cnt++;
                }
                catch (WebException)
                {
                    Log("sendthrottled");
                }

                Thread.Sleep(sleep);
                Log(cnt.ToString());
            }
        }

        private static void OcassionalRecvForget(string[] lines, int deviceOrder, int sleep)
        {
            var line = lines[deviceOrder];

            var parts = line.Split(' ');

            var deviceId = parts[0];
            var apiKey = parts[1];

            Log(deviceId);

            var ocassionalConnectionClient = new OccasionallyConnectionClient(Config.PlatformApi, deviceId, apiKey);
            int cnt = 0;
            while (true)
            {
                PushedMessage pushedMsg = null;
                try
                {
                    pushedMsg = ocassionalConnectionClient.ReceiveAndForgetMessage();
                    cnt++;
                }
                catch (WebException)
                {
                    Log("throttled");
                }
                Thread.Sleep(sleep);
                Log(cnt.ToString() + ((pushedMsg != null) ? "PM" : ""));
            }
        }

        private static void OcassionalRecvCommit(string[] lines, int deviceOrder, int sleep)
        {
            var line = lines[deviceOrder];

            var parts = line.Split(' ');

            var deviceId = parts[0];
            var apiKey = parts[1];

            Log(deviceId);

            var ocassionalConnectionClient = new OccasionallyConnectionClient(Config.PlatformApi, deviceId, apiKey);

            int cnt = 0;
            while (true)
            {
                PushedMessage pushedMsg = null;
                try
                {
                    pushedMsg = ocassionalConnectionClient.PeekMessage();
                    cnt++;
                }
                catch (WebException)
                {
                    Log("peekthrottled");
                }
                try
                {
                    if (pushedMsg != null)
                    {
                        ocassionalConnectionClient.CommitMessage();
                    }
                    cnt++;
                }
                catch (WebException)
                {
                    Log("commiythrottled");
                }
                Thread.Sleep(sleep);
                Log((cnt/2).ToString() + ((pushedMsg != null) ? "PM" : ""));
            }
        }

        private static void PersistentRecord(string[] lines, int deviceOrder, int sleep)
        {
            var line = lines[deviceOrder];

            var parts = line.Split(' ');

            var deviceId = parts[0];
            var apiKey = parts[1];

            var rnd = new Random();
            int cnt = 0;
            Log(deviceId);

            var persistentConnectionClient = new PersistentConnectionClient(Config.PlatformApiWS);
            while (true)
            {
                RetryableLogin(persistentConnectionClient, deviceId, apiKey);

                while (true)
                {
                    try
                    {
                        var doit = rnd.Next(100);
                        var a = doit % 10 == 0 ? string.Format(", \"A\": {0}", rnd.Next(10)) : "";
                        var b = doit % 15 == 0 ? string.Format(", \"B\": {0}", rnd.Next(20)) : "";
                        
                        persistentConnectionClient.RecordTelemetryData(string.Format("{{\"Temperature\": {0}, \"Humidity\": {1}, \"Time\": {2}{3}}}", rnd.Next(10) + 18, rnd.Next(40) + 40, DateTime.UtcNow.Second, a + b));
                        cnt++;
                    }
                    catch (Exception ex)
                    {
                        Log(ex);
                    }
                    persistentConnectionClient.Spin();
                    Thread.Sleep(sleep);
                    Log(cnt.ToString());
                }
            }
        }

        private static void PersistentSend(string[] lines, int deviceOrder, int sleep, int maxSend)
        {
            var line = lines[deviceOrder];

            var parts = line.Split(' ');

            var deviceId = parts[0];
            var apiKey = parts[1];

            Log(deviceId);


            int cnt = 0;
            var rnd = new Random();

            var persistentConnectionClient = new PersistentConnectionClient(Config.PlatformApiWS);

            while (true)
            {
                RetryableLogin(persistentConnectionClient, deviceId, apiKey);

                while (true)
                {
                    var target = lines[rnd.Next(maxSend)];
                    var targetDeviceId = target.Split(' ')[0];
                    try
                    {
                        persistentConnectionClient.SendMessageTo(targetDeviceId,
                            "{\"Temperature:\": 24, \"Humidity\": 60, \"Time\":" + DateTime.UtcNow.Ticks + "}");
                        cnt++;
                    }
                    catch (Exception ex)
                    {
                        Log(ex);
                    }
                    persistentConnectionClient.Spin();
                    Thread.Sleep(sleep);
                    Log("To " +  targetDeviceId + " Cnt: " + cnt);
                }
            }
        }

        private static int allCounter = 0;

        private static void PersistentRecvForget(string[] lines, int deviceOrder, int sleep)
        {
            var line = lines[deviceOrder];

            var parts = line.Split(' ');

            var deviceId = parts[0];
            var apiKey = parts[1];

            Log(deviceId);

            int cnt = 0;
            var persistentConnectionClient = new PersistentConnectionClient(Config.PlatformApiWS);
            RetryableLogin(persistentConnectionClient, deviceId, apiKey);
            RetryableSubscribe(persistentConnectionClient, SubscriptionType.ReceiveAndForget, message =>
            {
                if (message != null)
                {
                    cnt = message.MessageId;
                    Interlocked.Increment(ref allCounter);
                    Log(cnt.ToString());
                    Log("Allcounter: " + allCounter);
                }
            });

            while (true)
            {
                persistentConnectionClient.Spin();
                Thread.Sleep(sleep);
            }
        }

        private static void PersistentRecvCommit(string[] lines, int deviceOrder, int sleep)
        {
            var line = lines[deviceOrder];

            var parts = line.Split(' ');

            var deviceId = parts[0];
            var apiKey = parts[1];

            Log(deviceId);


            PushedMessage pushedMessage = null;
            int cnt = 0;
            var persistentConnectionClient = new PersistentConnectionClient(Config.PlatformApiWS);
            RetryableLogin(persistentConnectionClient, deviceId, apiKey);
            RetryableSubscribe(persistentConnectionClient, SubscriptionType.PeekAndCommit, message =>
            {
                if (message != null)
                {
                    pushedMessage = message;
                    cnt = message.MessageId;
                    Interlocked.Increment(ref allCounter);
                    Log(cnt.ToString());
                    Log("Allcounter: " + allCounter);
                }
            });

            while (true)
            {
                persistentConnectionClient.Spin();
                Thread.Sleep(sleep);
            }
        }

        private static void RetryableLogin(PersistentConnectionClient persistentConnectionClient, string deviceId,
            string apiKey)
        {
            try
            {
                persistentConnectionClient.Login(deviceId, apiKey);
            }
            catch (Exception ex)
            {
                Log(ex);
            }
        }

        private static void RetryableSubscribe(PersistentConnectionClient persistentConnectionClient, SubscriptionType subscriptionType, Action<PushedMessage> action)
        {
            try
            {
                persistentConnectionClient.Subscribe(subscriptionType, action);
            }
            catch (Exception ex)
            {
                Log(ex);
            }
        }

        //private static readonly object _lock = new object();
        //private static readonly int _pid = Process.GetCurrentProcess().Id;
        //private static readonly List<string> _logLines = new List<string>();

        private static void Log(Exception ex)
        {
            try
            {
                if (ex is WebException)
                {
                    using (var stream = (ex as WebException).Response.GetResponseStream())
                    {
                        using (var reader = new StreamReader(stream))
                        {
                            Log(ex.ToString());
                            Log(reader.ReadToEnd());
                        }
                    }
                }
                else
                {
                    Log(ex.ToString());
                }
            }
            catch (Exception lex)
            {
                Console.WriteLine("Error logging: " + lex.ToString());
            }
        }

        private static void Log(string str)
        {
            var totalSeconds = (int)(DateTime.UtcNow - DateTime.UtcNow.Date).TotalSeconds;
            var id = Thread.CurrentThread.ManagedThreadId + " " + totalSeconds + ": ";

//            var logFile = string.Format("c:\\download\\loadtestlog{0}.txt", _pid);

            Console.WriteLine(id + str);

            //lock (_lock)
            //{
            //    _logLines.Add(id + str);
            //    try
            //    {
            //        if (_logLines.Count > 10)
            //        {
            //            File.AppendAllLines(logFile, _logLines);
            //            _logLines.Clear();
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        Console.WriteLine("Log error: " + ex);
            //    }
            //}
        }
    }

    public class Config
    {
        public string ManagementApi { get; set; }
        public string PlatformApi { get; set; }
        public string PlatformApiWS { get; set; }
        public string SinkData { get; set; }
        public string SinkTimeSeries { get; set; }
    }
}

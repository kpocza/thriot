using System.Collections.Generic;
using Thriot.Messaging.Services.Storage;

namespace Thriot.Messaging.Services
{
    public class DeviceEntryRegistry
    {
        private readonly Dictionary<long, DeviceEntry> _deviceEntries;
        private readonly object _lock;

        public static readonly DeviceEntryRegistry Instance = new DeviceEntryRegistry();

        private DeviceEntryRegistry()
        {
            _deviceEntries = new Dictionary<long, DeviceEntry>();
            _lock = new object();
        }

        public void Merge(IReadOnlyCollection<DeviceEntry> newEntries)
        {
            lock (_lock)
            {
                foreach (var newEntry in newEntries)
                {
                    DeviceEntry currentEntry;
                    if (!_deviceEntries.TryGetValue(newEntry.Id, out currentEntry))
                    {
                        _deviceEntries.Add(newEntry.Id, newEntry);
                    }
                    else
                    {
                        if (newEntry.Version > currentEntry.Version)
                        {
                            currentEntry.DequeueIndex = newEntry.DequeueIndex;
                            currentEntry.EnqueueIndex = newEntry.EnqueueIndex;
                            currentEntry.Peek = newEntry.Peek;
                            currentEntry.Version = newEntry.Version;
                        }
                    }
                }
            }
        }

        public DequeueHint GetDequeueHint(IEnumerable<long> deviceIds)
        {
            var dequeueHint = new DequeueHint();

            lock (_lock)
            {
                foreach (var deviceId in deviceIds)
                {
                    DeviceEntry currentEntry;
                    if (_deviceEntries.TryGetValue(deviceId, out currentEntry))
                    {
                        if (currentEntry.DequeueIndex < currentEntry.EnqueueIndex)
                        {
                            dequeueHint.NewMessages.Add(currentEntry.Clone());
                        }
                    }
                    else
                    {
                        dequeueHint.UnknownDevices.Add(deviceId);
                    }
                }
            }

            return dequeueHint;
        }

        public IReadOnlyCollection<long> GetCommitHint(List<long> deviceIds)
        {
            var commitHint = new List<long>();

            lock (_lock)
            {
                foreach (var deviceId in deviceIds)
                {
                    DeviceEntry currentEntry;
                    if (_deviceEntries.TryGetValue(deviceId, out currentEntry))
                    {
                        if (currentEntry.DequeueIndex < currentEntry.EnqueueIndex && currentEntry.Peek)
                        {
                            commitHint.Add(deviceId);
                        }
                    }
                    else
                    {
                        commitHint.Add(deviceId);
                    }
                }
            }

            return commitHint;
        }

        public DeviceEntry Get(long deviceId)
        {
            return _deviceEntries[deviceId].Clone();
        }

        public void Remove(long deviceId)
        {
            _deviceEntries.Remove(deviceId);
        }
    }
}
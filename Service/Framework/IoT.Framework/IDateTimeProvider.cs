using System;

namespace IoT.Framework
{
    public interface IDateTimeProvider
    {
        DateTime UtcNow { get; }
    }
}

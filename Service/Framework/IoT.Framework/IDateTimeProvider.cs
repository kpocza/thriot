using System;

namespace Thriot.Framework
{
    public interface IDateTimeProvider
    {
        DateTime UtcNow { get; }
    }
}

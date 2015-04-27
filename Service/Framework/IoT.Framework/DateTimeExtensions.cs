using System;

namespace Thriot.Framework
{
    public static class DateTimeExtensions
    {
        public static long ToUnixTime(this DateTime dateTime)
        {
            return (long) (dateTime - new DateTime(1970, 1, 1)).TotalSeconds;
        }

        public static DateTime FromUnixTime(long seconds)
        {
            return new DateTime(1970, 1, 1).AddSeconds(seconds);
        }
    }
}

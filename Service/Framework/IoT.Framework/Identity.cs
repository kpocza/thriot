using System;

namespace Thriot.Framework
{
    public static class Identity
    {
        public static string Next()
        {
            return Guid.NewGuid().ToString().ToLowerInvariant().Replace("-", "");
        }

        public static string NextIncremental()
        {
            var next = Next();

            var seconds = (long)((DateTime.UtcNow - new DateTime(2014, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds);

            if(seconds < 0)
                throw new Exception("Bad system time");

            var secondsString = seconds.ToString();

            if (secondsString.Length > 10)
            {
                secondsString = secondsString.Substring(0, 10);
            }
            while (secondsString.Length < 10)
            {
                secondsString = "0" + secondsString;
            }

            return secondsString + next.Substring(0, 32 - secondsString.Length);
        }
    }
}

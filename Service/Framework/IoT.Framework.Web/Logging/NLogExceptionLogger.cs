using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.ExceptionHandling;
using IoT.Framework.Logging;

namespace IoT.Framework.Web.Logging
{
    public class NLogExceptionLogger : IExceptionLogger
    {
        public static readonly ILogger Logger = LoggerFactory.GetCurrentClassLogger();

        public Task LogAsync(ExceptionLoggerContext context, CancellationToken cancellationToken)
        {
            Logger.Error(context.Exception.ToString());

            return Task.FromResult(0);
        }
    }
}

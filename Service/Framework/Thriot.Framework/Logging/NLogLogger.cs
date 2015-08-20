using System;
using NLog;

namespace Thriot.Framework.Logging
{
    public class NLogLogger : ILogger
    {
        private readonly Logger _logger;

        public NLogLogger(Logger logger)
        {
            _logger = logger;
        }

        public void Debug(string message)
        {
            _logger.Debug(message);
        }

        public void Debug(string message, params object[] values)
        {
            _logger.Debug(message, values);
        }

        public void Fatal(string message)
        {
            _logger.Fatal(message);
        }

        public void Fatal(string message, params object[] values)
        {
            _logger.Fatal(message, values);
        }

        public void Error(string message)
        {
            _logger.Error(message);
        }

        public void Error(string message, params object[] values)
        {
            _logger.Error(message, values);
        }

        public void Info(string message)
        {
            _logger.Info(message);
        }

        public void Info(string message, params object[] values)
        {
            _logger.Info(message, values);
        }

        public void Trace(string message)
        {
            _logger.Trace(message);
        }

        public void Trace(string message, params object[] values)
        {
            _logger.Trace(message, values);
        }

        public void Warning(string message)
        {
            _logger.Warn(message);
        }

        public void Warning(string message, params object[] values)
        {
            _logger.Warn(message, values);
        }

        public void Exception(Exception exception)
        {
            Error(exception.ToString());
        }
    }
}

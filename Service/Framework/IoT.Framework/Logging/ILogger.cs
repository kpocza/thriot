namespace IoT.Framework.Logging
{
    public interface ILogger
    {
        void Debug(string message);
        void Debug(string message, params object[] values);

        void Fatal(string message);
        void Fatal(string message, params object[] values);

        void Error(string message);
        void Error(string message, params object[] values);

        void Info(string message);
        void Info(string message, params object[] values);

        void Trace(string message);
        void Trace(string message, params object[] values);

        void Warning(string message);
        void Warning(string message, params object[] values);
    }
}

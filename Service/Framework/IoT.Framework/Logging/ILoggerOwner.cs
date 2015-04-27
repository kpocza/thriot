namespace Thriot.Framework.Logging
{
    public interface ILoggerOwner
    {
        ILogger Logger { get; }

        string UserDefinedLogValue { get; }
    }
}

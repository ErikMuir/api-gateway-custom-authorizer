using System;
using System.Text.Json;
using Amazon.Lambda.Core;
using Microsoft.Extensions.Logging;

namespace ApiGatewayCustomAuthorizer
{
    public interface ILoggerLambda
    {
        void Log(LogLevel logLevel, string message, dynamic logData, Exception e = null);
        void LogCritital(string message, dynamic logData = null);
        void LogDebug(string message, dynamic logData = null);
        void LogError(string message, dynamic logData = null);
        void LogError(Exception ex, string message, dynamic logData = null);
        void LogInformation(string message, dynamic logData = null);
        void LogTrace(string message, dynamic logData = null);
        void LogWarning(string message, dynamic logData = null);
    }

    public class Logger : ILoggerLambda
    {
        private readonly string _name;
        private readonly IEnvironmentWrapper _environmentWrapper;

        public static Logger Create<T>() => new Logger(typeof(T).Name);

        public static Logger Create(string name) => new Logger(name);

        private Logger(string name)
        {
            _environmentWrapper = EnvironmentWrapper.Instance;
            _name = name;
        }

        public void Log(LogLevel logLevel, string message, dynamic logData, Exception e = null)
        {
            if (CurrentLogLevel.Value > logLevel)
            {
                return;
            }
            var logDynamic = new
            {
                Time = DateTime.UtcNow.ToString("o"),
                Level = $"{logLevel}",
                Logger = _name,
                Message = message,
                Exception = e?.ToString(),
                Data = logData,
                _environmentWrapper.Version,
                _environmentWrapper.Context?.AwsRequestId,
                _environmentWrapper.Context?.FunctionName,
                _environmentWrapper.Context?.RemainingTime,
                _environmentWrapper.Request?.MethodArn,
            };

            var log = JsonSerializer.Serialize(logDynamic, JsonConfig.SerializerOptions);

            LambdaLogger.Log(log);
        }

        public void LogTrace(string message, dynamic logData = null)
            => Log(LogLevel.Trace, message, logData);

        public void LogDebug(string message, dynamic logData = null)
            => Log(LogLevel.Debug, message, logData);

        public void LogInformation(string message, dynamic logData = null)
            => Log(LogLevel.Information, message, logData);

        public void LogWarning(string message, dynamic logData = null)
            => Log(LogLevel.Warning, message, logData);

        public void LogError(string message, dynamic logData = null)
            => Log(LogLevel.Error, message, logData);

        public void LogError(Exception ex, string message, dynamic logData = null)
            => Log(LogLevel.Error, message, logData, ex);

        public void LogCritital(string message, dynamic logData = null)
            => Log(LogLevel.Critical, message, logData);

        public static Lazy<LogLevel> CurrentLogLevel => new Lazy<LogLevel>(() =>
        {
            Enum.TryParse(EnvironmentWrapper.Instance.LogLevel, out LogLevel currentLogLevel);
            return currentLogLevel;
        });
    }
}

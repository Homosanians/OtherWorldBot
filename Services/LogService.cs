using DSharpPlus;
using System;

namespace OtherWorldBot.Services
{
    public class LogService
    {
        public enum Application
        {
            Other,
            Bot,
            DiscordAPI,
            Database,
            Service
        }

        private readonly DiscordClient _client;

        public LogService(DiscordClient client)
        {
            _client = client;
        }

        public void Debug(Application application, string message)
        {
            Log(LogLevel.Debug, application, message);
        }

        public void Info(Application application, string message)
        {
            Log(LogLevel.Info, application, message);
        }

        public void Warning(Application application, string message)
        {
            Log(LogLevel.Warning, application, message);
        }

        public void Error(Application application, string message)
        {
            Log(LogLevel.Error, application, message);
        }

        public void Critical(Application application, string message)
        {
            Log(LogLevel.Critical, application, message);
        }

        private void Log(LogLevel logLevel, Application application, string message)
        {
            _client.DebugLogger.LogMessage(logLevel, application.ToString(), message, DateTime.Now);
        }
    }
}
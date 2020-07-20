using DSharpPlus;
using System;

namespace OtherWorldBot.Services
{
    public class LogService
    {
        private readonly DiscordClient _client;

        public LogService(DiscordClient client)
        {
            _client = client;
        }

        public void Log(LogLevel level, string application, string message)
        {
            _client.DebugLogger.LogMessage(level, application, message, DateTime.Now);
        }
    }
}
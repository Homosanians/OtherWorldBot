using DSharpPlus;
using Newtonsoft.Json;
using System.IO;

namespace DisgraceDiscordBot.Entities
{
    public class BotConfig
    {
        [JsonProperty("Token")]
        public string Token { get; private set; } = "";

        [JsonProperty("CommandPrefix")]
        public string CommandPrefix { get; private set; } = "!";

        [JsonProperty("LogLevel")]
        public LogLevel LogLevel { get; set; } = LogLevel.Debug;
        // можно просто стринг
        [JsonProperty("GoodColor")]
        public int GoodColor { get; set; } = 0x00FF00;

        [JsonProperty("BadColor")]
        public int BadColor { get; set; } = 0xFF0000;

        [JsonProperty("TimeoutColor")]
        public int TimeoutColor { get; set; } = 0xFFFF00;

        [JsonProperty("CommonColor")]
        public int CommonColor { get; set; } = 0x404040;
    }
}
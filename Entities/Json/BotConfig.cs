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

        [JsonProperty("UpdateDecreaseValue")]
        public int UpdateDecreaseValue { get; set; } = 2;

        [JsonProperty("UpdateRateInMinutes")]
        public int UpdateRateInMinutes { get; set; } = 10;

        [JsonProperty("GoodColor")]
        public string GoodColor { get; set; } = "00FF00";

        [JsonProperty("BadColor")]
        public string BadColor { get; set; } = "FF0000";

        [JsonProperty("WarningColor")]
        public string WarningColor { get; set; } = "FFFF00";

        [JsonProperty("CommonColor")]
        public string CommonColor { get; set; } = "404040";
    }
}
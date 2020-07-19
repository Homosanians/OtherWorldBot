using DSharpPlus;
using Newtonsoft.Json;
using System.IO;

namespace OtherWorldBot.Entities
{
    public class BotConfig
    {
        [JsonProperty("Token")]
        public string Token { get; private set; } = "";

        [JsonProperty("CommandPrefix")]
        public string CommandPrefix { get; private set; } = "+";

        [JsonProperty("PlayingGameString")]
        public string PlayingGameString { get; private set; } = "+show";

        [JsonProperty("LogLevel")]
        public LogLevel LogLevel { get; private set; } = LogLevel.Info;

        [JsonProperty("UpdateDecreaseValue")]
        public int UpdateDecreaseValue { get; private set; } = 2;

        [JsonProperty("UpdateRateInMinutes")]
        public int UpdateRateInMinutes { get; private set; } = 10;

        [JsonProperty("GoodColor")]
        public string GoodColor { get; private set; } = "00FF00";

        [JsonProperty("BadColor")]
        public string BadColor { get; private set; } = "FF0000";

        [JsonProperty("WarningColor")]
        public string WarningColor { get; private set; } = "FFFF00";

        [JsonProperty("CommonColor")]
        public string CommonColor { get; private set; } = "404040";
    }
}
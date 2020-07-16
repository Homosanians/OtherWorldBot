using DSharpPlus;
using Newtonsoft.Json;
using System.IO;

namespace DisgraceDiscordBot.Entities
{
    public class BotConfig
    {
        [JsonProperty("token")]
        public string Token { get; private set; } = "";

        [JsonProperty("prefix")]
        public string CommandPrefix { get; private set; } = "!";

        [JsonProperty("log_level")]
        public LogLevel LogLevel { get; set; } = LogLevel.Debug;

        [JsonProperty("good_color")]
        public int GoodColor { get; set; } = 0x00FF00;

        [JsonProperty("bad_color")]
        public int BadColor { get; set; } = 0xFF0000;

        [JsonProperty("timeout_color")]
        public int TimeoutColor { get; set; } = 0xFFFF00;

        [JsonProperty("common_color")]
        public int CommonColor { get; set; } = 0x404040;

        /// <summary>
        /// Loads config from a JSON file.
        /// </summary>
        /// <param name="path">Path to your config file.</param>
        /// <returns></returns>
        public static BotConfig LoadFromFile(string path)
        {
            using (var sr = new StreamReader(path))
            {
                return JsonConvert.DeserializeObject<BotConfig>(sr.ReadToEnd());
            }
        }

        /// <summary>
        /// Saves config to a JSON file.
        /// </summary>
        /// <param name="path"></param>
        public void SaveToFile(string path)
        {
            using (var sw = new StreamWriter(path))
            {
                sw.Write(JsonConvert.SerializeObject(this));
            }
        }
    }
}
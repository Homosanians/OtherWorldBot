using DisgraceDiscordBot.Entities;
using Newtonsoft.Json;
using System.IO;

namespace DisgraceDiscordBot.Services
{
    public class ConfigService
    {
        public BotConfig BotConfig
        {
            get
            {
                if (configCache == null)
                {
                    configCache = GetConfig();
                }
                
                return configCache;
            }
            set
            {
                configCache = value;
                SetConfig(value);
            }
        }
        private BotConfig configCache;

        public ConfigService()
        {
            if (!File.Exists("config.json"))
            {
                System.Console.WriteLine("config.json was not found. Creating new one.");
                SetConfig(new BotConfig());
            }
        }

        public BotConfig GetConfig()
        {
            using (var sr = new StreamReader("config.json"))
            {
                return JsonConvert.DeserializeObject<BotConfig>(sr.ReadToEnd());
            }
        }

        public void SetConfig(BotConfig config)
        {
            using (var sw = new StreamWriter("config.json"))
            {
                sw.Write(JsonConvert.SerializeObject(config, Formatting.Indented));
            }
        }
    }
}
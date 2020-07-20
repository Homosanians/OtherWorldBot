using OtherWorldBot.Entities;
using Newtonsoft.Json;
using System;
using System.IO;

namespace OtherWorldBot.Services
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
                Console.WriteLine("config.json was not found. A new one is beeing created.");
                SetConfig(new BotConfig());
                Environment.Exit(0);
            }
        }

        public static BotConfig GetConfig()
        {
            using (var sr = new StreamReader("config.json"))
            {
                return JsonConvert.DeserializeObject<BotConfig>(sr.ReadToEnd());
            }
        }

        public static void SetConfig(BotConfig config)
        {
            using (var sw = new StreamWriter("config.json"))
            {
                sw.Write(JsonConvert.SerializeObject(config, Formatting.Indented));
            }
        }
    }
}
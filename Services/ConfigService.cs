using DisgraceDiscordBot.Entities;
using Newtonsoft.Json;
using System.IO;
using System.Text;
using System.Threading.Tasks;

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

        public async Task<BotConfig> GetConfigAsync()
        {
            var json = "";
            using (var fs = File.OpenRead("config.json"))
            using (var sr = new StreamReader(fs, new UTF8Encoding(true)))
                json = await sr.ReadToEndAsync();

            return JsonConvert.DeserializeObject<BotConfig>(json);
        }

        public async Task SetConfigAsync(BotConfig config)
        {
            var cfgjson = JsonConvert.SerializeObject(config, Formatting.Indented);

            using (var fs = File.OpenWrite("config.json"))
            using (var sw = new StreamWriter(fs, new UTF8Encoding(true)))
                await sw.WriteAsync(cfgjson);
        }

        public BotConfig GetConfig()
        {
            var json = "";
            using (var fs = File.OpenRead("config.json"))
            using (var sr = new StreamReader(fs, new UTF8Encoding(true)))
                json = sr.ReadToEnd();
            
            return JsonConvert.DeserializeObject<BotConfig>(json);
        }

        public void SetConfig(BotConfig config)
        {
            var cfgjson = JsonConvert.SerializeObject(config, Formatting.Indented);

            using (var fs = File.OpenWrite("config.json"))
            using (var sw = new StreamWriter(fs, new UTF8Encoding(true)))
                sw.Write(cfgjson);
        }
    }
}
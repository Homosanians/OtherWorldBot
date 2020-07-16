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

        private LogService _logService;

        public ConfigService()
        {
            if (!File.Exists("config.json"))
            {
                SetConfig(new BotConfig());
            }
        }

        public ConfigService (LogService logService)
        {
            _logService = logService;

            if (!File.Exists("config.json"))
            {
                logService.Log(DSharpPlus.LogLevel.Info, "ConfigService", "Config not exists");
                SetConfig(new BotConfig());
            }
        }

        public async Task<BotConfig> GetConfigAsync()
        {
            var json = "";
            using (var fs = File.OpenRead("config.json"))
            using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                json = await sr.ReadToEndAsync();

            return JsonConvert.DeserializeObject<BotConfig>(json);
        }

        public async Task SetConfigAsync(BotConfig config)
        {
            if (_logService != null)
                _logService.Log(DSharpPlus.LogLevel.Debug, "ConfigService", "Writing config file async");

            var cfgjson = JsonConvert.SerializeObject(config);

            await File.WriteAllTextAsync("config.json", cfgjson);
        }

        public BotConfig GetConfig()
        {
            var json = "";
            using (var fs = File.OpenRead("config.json"))
            using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                json = sr.ReadToEnd();

            return JsonConvert.DeserializeObject<BotConfig>(json);
        }

        public void SetConfig(BotConfig config)
        {
            if (_logService != null)
                _logService.Log(DSharpPlus.LogLevel.Debug, "ConfigService", "Writing config file");

            var cfgjson = JsonConvert.SerializeObject(config);

            File.WriteAllText("config.json", cfgjson);
        }
    }
}
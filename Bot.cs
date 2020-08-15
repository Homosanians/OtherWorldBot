using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Interactivity;
using OtherWorldBot.Services;
using OtherWorldBot.Handlers;
using Microsoft.Extensions.DependencyInjection;
using OtherWorldBot.Entities;
using System.Linq;
using DSharpPlus.Interactivity.Enums;

namespace OtherWorldBot
{
    public class Bot
    {
        public DiscordClient Client { get; set; }
        public InteractivityExtension Interactivity { get; set; }
        public CommandsNextExtension Commands { get; set; }
        public LogService LogService { get; set; }
        public ConfigService ConfigService { get; set; }
        public DatabaseService DatabaseService { get; set; }
        public ScheduleUpdateService ScheduleUpdateService { get; set; }

        public async Task InitAsync()
        {
            ConfigService = new ConfigService();
            
            var cfg = new DiscordConfiguration
            {
                Token = ConfigService.BotConfig.Token,
                TokenType = TokenType.Bot,
                
                AutoReconnect = true,
                LogLevel = ConfigService.BotConfig.LogLevel,
                UseInternalLogHandler = true
            };

            Client = new DiscordClient(cfg);
            LogService = new LogService(Client);
            DatabaseService = new DatabaseService();
            ScheduleUpdateService = new ScheduleUpdateService(LogService, ConfigService, DatabaseService);
           
            var deps = new ServiceCollection()
                .AddSingleton(ConfigService)
                .AddSingleton(LogService)
                .AddSingleton(DatabaseService)
                .AddSingleton(ScheduleUpdateService)
                .AddSingleton(new EventsHandler(Client, ConfigService))
                .BuildServiceProvider();
            
            Client.UseInteractivity(new InteractivityConfiguration
            {
                PaginationBehaviour = PaginationBehaviour.Ignore,
                Timeout = TimeSpan.FromMinutes(2)
            });

            Client.UseCommandsNext(new CommandsNextConfiguration
            {
                StringPrefixes = new[] { ConfigService.BotConfig.CommandPrefix },
                EnableDms = true,
                Services = deps,
                EnableMentionPrefix = true
            });

            new CommandHandler(Commands, ConfigService, LogService);

            await Client.ConnectAsync();

            await Task.Delay(-1).ConfigureAwait(false);
        }
    }
}

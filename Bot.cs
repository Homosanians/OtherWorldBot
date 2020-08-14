using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Interactivity;
using OtherWorldBot.Services;
using OtherWorldBot.Handlers;

namespace OtherWorldBot
{
    public class Bot
    {
        public DiscordClient Client { get; set; }
        public InteractivityModule Interactivity { get; set; }
        public CommandsNextModule Commands { get; set; }
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
           
            var deps = new DependencyCollectionBuilder()
                .AddInstance(ConfigService)
                .AddInstance(LogService)
                .AddInstance(DatabaseService)
                .AddInstance(ScheduleUpdateService)
                .AddInstance(new EventsHandler(Client, ConfigService))
                .Build();
            
            Client.UseInteractivity(new InteractivityConfiguration
            {
                PaginationBehaviour = TimeoutBehaviour.Ignore,
                PaginationTimeout = TimeSpan.FromMinutes(5),
                Timeout = TimeSpan.FromMinutes(2)
            });
            
            var commandsConfig = new CommandsNextConfiguration
            {
                StringPrefix = ConfigService.BotConfig.CommandPrefix,
                EnableDms = true,
                Dependencies = deps,                
                EnableMentionPrefix = true
            };

            Commands = this.Client.UseCommandsNext(commandsConfig);
            new CommandHandler(Commands, ConfigService, LogService);

            await Client.ConnectAsync();

            await Task.Delay(-1).ConfigureAwait(false);
        }
    }
}

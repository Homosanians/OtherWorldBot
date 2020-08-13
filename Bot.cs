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
            this.ConfigService = new ConfigService();
            
            var cfg = new DiscordConfiguration
            {
                Token = ConfigService.BotConfig.Token,
                TokenType = TokenType.Bot,
                
                AutoReconnect = true,
                LogLevel = ConfigService.BotConfig.LogLevel,
                UseInternalLogHandler = true
            };

            this.Client = new DiscordClient(cfg);
            this.LogService = new LogService(Client);
            this.DatabaseService = new DatabaseService();
            this.ScheduleUpdateService = new ScheduleUpdateService(LogService, ConfigService, DatabaseService);
           
            var deps = new DependencyCollectionBuilder()
                .AddInstance(ConfigService)
                .AddInstance(LogService)
                .AddInstance(DatabaseService)
                .AddInstance(ScheduleUpdateService)
                .AddInstance(new EventsHandler(Client, ConfigService))
                .AddInstance(new CommandHandler(Commands, ConfigService))
                .Build();
            
            this.Client.UseInteractivity(new InteractivityConfiguration
            {
                // Default pagination behaviour to just ignore the reactions
                PaginationBehaviour = TimeoutBehaviour.Ignore,

                // Default pagination timeout to 5 minutes
                PaginationTimeout = TimeSpan.FromMinutes(5),
                
                // Default timeout for other actions to 2 minutes
                Timeout = TimeSpan.FromMinutes(2)
            });
            
            var ccfg = new CommandsNextConfiguration
            {
                // Use the string prefix defined in config.json
                StringPrefix = ConfigService.BotConfig.CommandPrefix,
                
                // Disable responding in direct messages
                EnableDms = false,
                
                // Pass built DI collection
                Dependencies = deps,
                
                // Enable mentioning the bot as a command prefix
                EnableMentionPrefix = true
            };

            this.Commands = this.Client.UseCommandsNext(ccfg);

            // Connect and log in
            await this.Client.ConnectAsync();

            // Prevent premature quitting
            await Task.Delay(-1).ConfigureAwait(false);
        }
    }
}

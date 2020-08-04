using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Exceptions;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using OtherWorldBot.Commands;
using Newtonsoft.Json;
using OtherWorldBot.Services;

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
                .Build();

            this.Client.Ready += this.Client_Ready;
            this.Client.GuildAvailable += this.Client_GuildAvailable;
            this.Client.ClientErrored += this.Client_ClientError;
            this.Client.GuildMemberAdded += Client_GuildMemberAdded;
            
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

            this.Commands.CommandExecuted += this.Commands_CommandExecuted;
            this.Commands.CommandErrored += this.Commands_CommandErrored;

            // Register the commands
            this.Commands.RegisterCommands<CommonCommands>();
            this.Commands.RegisterCommands<AdminCommands>();

            // Connect and log in
            await this.Client.ConnectAsync();

            // Prevent premature quitting
            await Task.Delay(-1).ConfigureAwait(false);
        }

        private Task Client_GuildMemberAdded(GuildMemberAddEventArgs e)
        {
            if (!e.Member.IsBot)
            {
                var embed = new DiscordEmbedBuilder
                {
                    Color = new DiscordColor(ConfigService.BotConfig.GoodColor),
                    Title = "Добро пожаловать",
                    Description = $"Хей, {e.Member.Username}, приветствуем тебя на нашем сервере!\n" +
                    $"\n" +
                    $"Чтобы подать заявку на проходку, " +
                    $"просто пропиши команду .request в абсолютно любой чат на сервере. Дальше следуй инструкциям, которые тебе отпишут в ЛС.\n" +
                    $"\n" +
                    $"Обрати внимание, что после отправки анкеты, проходка может достаться тебе как бесплатно, так и с оплатой. " +
                    $"Постарайся заполнить всё более детально, укажи что у тебя большой опыт, покажи что ты настоящий майнкрафтер и мы примим тебя бесплатно!\n" +
                    $"\n" +
                    $"Остальную информацию ты сможешь найти в канале #информация\n" +
                    $"\n" +
                    $"До скорых встреч! :wave:",
                    Footer = new DiscordEmbedBuilder.EmbedFooter { Text = "Other World" }
                };

                e.Member.SendMessageAsync(embed: embed);
            }

            return Task.CompletedTask;
        }

        private Task Client_Ready(ReadyEventArgs e)
        {
            e.Client.DebugLogger.LogMessage(LogLevel.Info, "OtherWorld", "Client is ready to process events.", DateTime.Now);

            // Let's set a help command info
            this.Client.UpdateStatusAsync(new DiscordGame($"{this.ConfigService.BotConfig.CommandPrefix}show"));

            // Since this method is not async, let's return 
            // a completed task, so that no additional work is done
            return Task.CompletedTask;
        }

        private Task Client_GuildAvailable(GuildCreateEventArgs e)
        {
            // Log the name of the guild that was just sent to our client
            e.Client.DebugLogger.LogMessage(LogLevel.Info, "OtherWorld", $"Guild available: {e.Guild.Name}", DateTime.Now);

            return Task.CompletedTask;
        }

        private Task Client_ClientError(ClientErrorEventArgs e)
        {
            // Log the details of the error that just occured in our client
            e.Client.DebugLogger.LogMessage(LogLevel.Error, "OtherWorld", $"Exception occured: {e.Exception.GetType()}: {e.Exception.Message}\n{e.Exception.StackTrace}", DateTime.Now);

            return Task.CompletedTask;
        }

        private Task Commands_CommandExecuted(CommandExecutionEventArgs e)
        {
            // Log the name of the command and user
            e.Context.Client.DebugLogger.LogMessage(LogLevel.Info, "OtherWorld", $"{e.Context.User.Username} successfully executed '{e.Command.QualifiedName}'", DateTime.Now);
            
            return Task.CompletedTask;
        }

        private async Task Commands_CommandErrored(CommandErrorEventArgs e)
        {
            // Log the error details
            e.Context.Client.DebugLogger.LogMessage(LogLevel.Error, "OtherWorld", $"{e.Context.User.Username} tried executing '{e.Command?.QualifiedName ?? "<unknown command>"}' but it errored: {e.Exception.GetType()}: {e.Exception.Message ?? "<no message>"}\n{e.Exception.StackTrace}", DateTime.Now);

            // Check if the error is a result of lack of required permissions
            if (e.Exception is ChecksFailedException)
            {
                var emoji = DiscordEmoji.FromName(e.Context.Client, ":no_entry:");

                var embed = new DiscordEmbedBuilder
                {
                    Title = "Доступ запрещен",
                    Description = $"{emoji} У вас нет привилегий для выполнения этой команды.",
                    Color = new DiscordColor(ConfigService.BotConfig.BadColor)
                };
                await e.Context.RespondAsync(embed: embed);
            }
        }
    }
}

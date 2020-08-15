using DSharpPlus;
using DSharpPlus.CommandsNext;
using OtherWorldBot.Modules;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using DSharpPlus.Entities;
using DSharpPlus.CommandsNext.Exceptions;
using OtherWorldBot.Services;
using System.Reflection;
using System.Linq;
using DSharpPlus.CommandsNext.Attributes;
using System.Security.Cryptography.X509Certificates;

namespace OtherWorldBot.Handlers
{
    public class CommandHandler
    {
        private readonly ConfigService configService;
        private readonly LogService logService;

        public CommandHandler(CommandsNextExtension commands, ConfigService configService, LogService logService)
        {
            this.configService = configService;
            this.logService = logService;

            commands.CommandExecuted += Commands_CommandExecuted;
            commands.CommandErrored += Commands_CommandErrored;

            // Register the commands
            // commands.RegisterCommands(Assembly.GetExecutingAssembly());
            commands.RegisterCommands<CommonCommandsModule>();
            commands.RegisterCommands<AdminCommandsModule>();
            commands.RegisterCommands<OthergramModule>();
        }

        private async Task Commands_CommandErrored(CommandErrorEventArgs e)
        {
            // Check if the error is a result of unknown command
            if (e.Exception is CommandNotFoundException)
            {
                return;
            }
            
            // Check if the error is a result of failed checks
            if (e.Exception is ChecksFailedException)
            {
                // Check if the error is a result of message in DM when only guild is allowed
                if (e.Command.ExecutionChecks.Any(x => x.GetType() == typeof(RequireGuildAttribute)))
                {
                    var emoji = DiscordEmoji.FromName(e.Context.Client, ":no_entry:");

                    await e.Context.RespondAsync(embed: new DiscordEmbedBuilder
                    {
                        Title = "Выполнение остановлено",
                        Description = $"{emoji} Команда не может быть выполнена в личных сообщениях. Используйте на сервере.",
                        Color = new DiscordColor(configService.BotConfig.BadColor)
                    });
                }

                // Check if the error is a result of message in guild when only DM is allowed
                else if (e.Command.ExecutionChecks.Any(x => x.GetType() == typeof(RequireDirectMessageAttribute)))
                {
                    var emoji = DiscordEmoji.FromName(e.Context.Client, ":no_entry:");

                    await e.Context.RespondAsync(embed: new DiscordEmbedBuilder
                    {
                        Title = "Выполнение остановлено",
                        Description = $"{emoji} Команда не может быть выполнена на сервере. Используйте в личных сообщениях.",
                        Color = new DiscordColor(configService.BotConfig.BadColor)
                    });
                }
                // The error is a result of lack of required permissions
                else
                {
                    var emoji = DiscordEmoji.FromName(e.Context.Client, ":no_entry:");

                    await e.Context.RespondAsync(embed: new DiscordEmbedBuilder
                    {
                        Title = "Доступ запрещен",
                        Description = $"{emoji} У вас нет привилегий для выполнения этой команды.",
                        Color = new DiscordColor(configService.BotConfig.BadColor)
                    });
                }

                return;
            }

            logService.Error(LogService.Application.Bot, $"{e.Context.User.Username} tried executing '{e.Command?.QualifiedName ?? "<unknown command>"}' but it errored: {e.Exception.GetType()}: {e.Exception.Message ?? "<no message>"}\n{e.Exception.StackTrace}");
        }

        private Task Commands_CommandExecuted(CommandExecutionEventArgs e)
        {
            logService.Debug(LogService.Application.Bot, $"{e.Context.User.Username} successfully executed '{e.Command.QualifiedName}'");
            
            return Task.CompletedTask;
        }
    }
}

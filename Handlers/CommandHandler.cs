using DSharpPlus;
using DSharpPlus.CommandsNext;
using OtherWorldBot.Commands;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using DSharpPlus.Entities;
using DSharpPlus.CommandsNext.Exceptions;
using OtherWorldBot.Services;

namespace OtherWorldBot.Handlers
{
    public class CommandHandler
    {
        private readonly ConfigService configService;

        public CommandHandler(CommandsNextModule commands, ConfigService configService)
        {
            this.configService = configService;

            commands.CommandExecuted += Commands_CommandExecuted;
            commands.CommandErrored += Commands_CommandErrored;

            // Register the commands
            commands.RegisterCommands<CommonCommands>();
            commands.RegisterCommands<AdminCommands>();
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
                    Color = new DiscordColor(configService.BotConfig.BadColor)
                };
                await e.Context.RespondAsync(embed: embed);
            }
        }

        private Task Commands_CommandExecuted(CommandExecutionEventArgs e)
        {
            // Log the name of the command and user
            e.Context.Client.DebugLogger.LogMessage(LogLevel.Info, "OtherWorld", $"{e.Context.User.Username} successfully executed '{e.Command.QualifiedName}'", DateTime.Now);

            return Task.CompletedTask;
        }
    }
}

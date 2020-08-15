using DSharpPlus;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using OtherWorldBot.Services;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using System.Linq;

namespace OtherWorldBot.Handlers
{
    public class EventsHandler
    {
        private readonly DiscordClient client;
        private readonly ConfigService configService;

        public EventsHandler(DiscordClient client, ConfigService configService)
        {
            this.client = client;
            this.configService = configService;

            this.client.Ready += Client_Ready;
            this.client.GuildAvailable += Client_GuildAvailable;
            this.client.ClientErrored += Client_ClientErrored;
            this.client.GuildMemberAdded += Client_GuildMemberAdded;
        }

        private Task Client_GuildMemberAdded(GuildMemberAddEventArgs e)
        {
            if (!e.Member.IsBot)
            {
                var embed = new DiscordEmbedBuilder
                {
                    Color = new DiscordColor(configService.BotConfig.GoodColor),
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

                var role = e.Guild.Roles.FirstOrDefault(x => x.Value.Name == configService.BotConfig.GuestRoleName).Value;
                e.Member.GrantRoleAsync(role);
            }

            return Task.CompletedTask;
        }

        private Task Client_ClientErrored(ClientErrorEventArgs e)
        {
            // Log the details of the error that just occured in our client
            e.Client.DebugLogger.LogMessage(LogLevel.Error, "OtherWorld", $"Exception occured: {e.Exception.GetType()}: {e.Exception.Message}\n{e.Exception.StackTrace}", DateTime.Now);

            return Task.CompletedTask;
        }

        private Task Client_GuildAvailable(GuildCreateEventArgs e)
        {
            // Log the name of the guild that was just sent to our client
            e.Client.DebugLogger.LogMessage(LogLevel.Info, "OtherWorld", $"Guild available: {e.Guild.Name}", DateTime.Now);

            return Task.CompletedTask;
        }

        private Task Client_Ready(ReadyEventArgs e)
        {
            e.Client.DebugLogger.LogMessage(LogLevel.Info, "OtherWorld", "Client is ready to process events.", DateTime.Now);

            // Let's set a help command info
            client.UpdateStatusAsync(new DiscordActivity($"{configService.BotConfig.CommandPrefix}show", ActivityType.ListeningTo));

            return Task.CompletedTask;
        }
    }
}

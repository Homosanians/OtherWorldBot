using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DisgraceDiscordBot.Services;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;

namespace DisgraceDiscordBot.Commands
{
    [Description("Команды пользователя.")]
    public class CommonCommands
    {
        private ConfigService configSrv;
        private DatabaseService databaseSrv;

        public CommonCommands(ConfigService configService, DatabaseService databaseService)
        {
            configSrv = configService;
            databaseSrv = databaseService;
        }

        [Command("ping")]
        [Description("Показывает пинг клиента")]
        [Aliases("pong")]
        public async Task Ping(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();

            var emoji = DiscordEmoji.FromName(ctx.Client, ":ping_pong:");

            await ctx.RespondAsync($"{emoji} Pong! Ping: {ctx.Client.Ping}ms");
        }

        [Command("show")]
        [Description("Показывает все страны")]
        [Aliases("показать", "страны", "countries", "list", "список")]
        public async Task ShowCountries(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();

            var countries = databaseSrv.GetAllCountries();

            if (countries.Length == 0)
            {
                var embed = new Discord​Embed​Builder()
                {
                    Title = "Страны",
                    Description = "Ни одна страна не была найдена.",
                    Footer = new DiscordEmbedBuilder.EmbedFooter() { Text = "Cyka" },
                    Color = new DiscordColor(configSrv.BotConfig.TimeoutColor)
                };

                await ctx.RespondAsync(null, false, embed.Build());
            }
            else
            {
                var embed = new Discord​Embed​Builder()
                {
                    Title = "Страны",
                    Footer = new DiscordEmbedBuilder.EmbedFooter() { Text = "Cyka" },
                    Color = new DiscordColor(configSrv.BotConfig.GoodColor)
                };

                foreach (var entry in countries)
                {
                    embed.AddField(entry.Name, $"Id: {entry.Id} Очки: {entry.DisgracePoints}", true);
                }

                await ctx.RespondAsync(null, false, embed.Build());
            }
        }
    }
}
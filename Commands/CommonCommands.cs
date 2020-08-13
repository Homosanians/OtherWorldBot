using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OtherWorldBot.Services;
using OtherWorldBot.Utils;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using System.Reflection;
using System.Diagnostics;

namespace OtherWorldBot.Commands
{
    [Description("Команды пользователя.")]
    public class CommonCommands
    {
        private readonly ConfigService configSrv;
        private readonly DatabaseService databaseSrv;
        private readonly ScheduleUpdateService scheduleUpdateSrv;

        public CommonCommands(ConfigService configService, DatabaseService databaseService, ScheduleUpdateService scheduleUpdateService)
        {
            configSrv = configService;
            databaseSrv = databaseService;
            scheduleUpdateSrv = scheduleUpdateService;
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

            var countries = await databaseSrv.GetAllCountriesAsync();

            if (countries.Length == 0)
            {
                var embed = new Discord​Embed​Builder()
                {
                    Title = "Страны",
                    Description = "Ни одна страна не была найдена.",
                    Footer = new DiscordEmbedBuilder.EmbedFooter { Text = "Other World" },
                    Color = new DiscordColor(configSrv.BotConfig.WarningColor)
                };

                await ctx.RespondAsync(embed: embed);
            }
            else
            {
                var embed = new Discord​Embed​Builder()
                {
                    Title = "Страны",
                    Footer = new DiscordEmbedBuilder.EmbedFooter { Text = "Other World" },
                    Color = new DiscordColor(configSrv.BotConfig.CommonColor)
                };

                foreach (var entry in countries)
                {
                    embed.AddField(entry.Name, $"Бесчестие: {entry.DisgracePoints}", false);
                }

                await ctx.RespondAsync(embed: embed);
            }
        }

        [Command("time")]
        [Description("Показывается время до обновления")]
        [Aliases("время")]
        public async Task ShowTimeTillUpdate(CommandContext ctx)
        {
            string description = $"Очки стран будут обновлены через ";
            description += string.Format(new TimeWordFormatter(), "{0:W}", scheduleUpdateSrv.GetTimeTillUpdate());
            description += ".";

            var embed = new Discord​Embed​Builder
            {
                Color = new DiscordColor(configSrv.BotConfig.CommonColor),
                Title = "Время до обновления",
                Description = description,
                Footer = new DiscordEmbedBuilder.EmbedFooter { Text = "Other World" }
            };

            await ctx.RespondAsync(embed: embed);
        }

        [Command("version")]
        [Description("Отображает версию текущей сборки")]
        public async Task SendCurrentVersion(CommandContext ctx)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);

            var embed = new Discord​Embed​Builder
            {
                Color = new DiscordColor(configSrv.BotConfig.CommonColor),
                Title = "Версия",
                Description = $"Сборка {fvi.ProductName}\n{fvi.LegalCopyright}",
                Footer = new DiscordEmbedBuilder.EmbedFooter { Text = "Other World" }
            };

            await ctx.RespondAsync(embed: embed);
        }
    }
}

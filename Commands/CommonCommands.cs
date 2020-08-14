﻿using System;
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
            await ctx.TriggerTypingAsync();

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
        [Aliases("v")]
        public async Task ShowCurrentVersion(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();

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

        [Command("roles")]
        [Description("Отображает список ролей, отсортированных по количеству носителей")]
        public async Task ShowRoles(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();

            if (ctx.Guild == null)
            {
                await ctx.RespondAsync(embed: new Discord​Embed​Builder
                {
                    Color = new DiscordColor(configSrv.BotConfig.CommonColor),
                    Title = "Статистика",
                    Description = $"Данная команда не может быть выполнена в личных сообщениях",
                    Footer = new DiscordEmbedBuilder.EmbedFooter { Text = "Other World" }
                });
                return;
            }

            string description = "Участников — Роль\n";

            Dictionary<int, string> roleNamePairs = new Dictionary<int, string>();

            foreach (var el in ctx.Guild.Roles)
            {
                int memberCount = ctx.Guild.Members.Where(x => x.Roles.Contains(el)).Count();
                if (memberCount > 1)
                {
                    roleNamePairs.Add(memberCount, el.Name);
                }
            }

            roleNamePairs.OrderByDescending(x => x.Key);

            foreach (var el in roleNamePairs)
            {
                description += $"\n{el.Key} — {el.Value}";
            }

            var embed = new Discord​Embed​Builder
            {
                Color = new DiscordColor(configSrv.BotConfig.CommonColor),
                Title = "Список ролей",
                Description = description,
                Footer = new DiscordEmbedBuilder.EmbedFooter { Text = "Other World" }
            };

            await ctx.RespondAsync(embed: embed);
        }

        [Command("stats")]
        [Description("Отображает статистику по серверу")]
        public async Task ShowServerStats(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();

            if (ctx.Guild == null)
            {
                await ctx.RespondAsync(embed: new Discord​Embed​Builder
                {
                    Color = new DiscordColor(configSrv.BotConfig.CommonColor),
                    Title = "Статистика",
                    Description = $"Данная команда не может быть выполнена в личных сообщениях",
                    Footer = new DiscordEmbedBuilder.EmbedFooter { Text = "Other World" }
                });
                return;
            }

            var members = ctx.Guild.Members;
            
            // Людей всего
            int membersCount = members.Where(x => x.IsBot == false).Count();
            // Людей онлайн (не оффлайн)
            int membersOnlineCount = members.Where(x => x.IsBot == false).Where(x => x.Presence != null).Count();
            // Людей в голосовых чатах сейчас
            int membersInVoiceChatsCount = members.Where(x => x.IsBot == false).Where(x => x.VoiceState != null).Where(x => x.VoiceState.Channel != null).Count();
            // Людей играет
            int membersPlaying = members.Where(x => x.IsBot == false).Where(x => x.Presence != null).Where(x => x.Presence.Game != null).Count();
            // Ботов всего
            int botsCount = members.Where(x => x.IsBot == true).Count();
            // Ролей всего
            int rolesCount = ctx.Guild.Roles.Count;
            // За 24 часа присоединилось
            int recentMembersCount = members.Where(x => x.JoinedAt.DateTime.CompareTo(DateTime.Now.AddDays(-1)) == 1).Count();

            var embed = new Discord​Embed​Builder
            {
                Color = new DiscordColor(configSrv.BotConfig.CommonColor),
                Title = "Статистика",
                Description = $"Людей всего {membersCount}\n" +
                $"Людей онлайн {membersOnlineCount}\n" +
                $"Людей в голосовых каналах {membersInVoiceChatsCount}\n" +
                $"Людей играет {membersPlaying}\n" +
                $"Ботов всего {botsCount}\n" +
                $"Ролей всего {rolesCount}\n" +
                $"За последние 24 часа присоединились {recentMembersCount}",
                Footer = new DiscordEmbedBuilder.EmbedFooter { Text = "Other World" }
            };

            await ctx.RespondAsync(embed: embed);
        }
    }
}

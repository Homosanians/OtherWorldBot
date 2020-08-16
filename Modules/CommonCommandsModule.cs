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

namespace OtherWorldBot.Modules
{
    [ModuleLifespan(ModuleLifespan.Singleton)]
    [Description("Команды пользователя.")]
    public class CommonCommandsModule : BaseCommandModule
    {
        private readonly ConfigService configSrv;
        private readonly DatabaseService databaseSrv;
        private readonly ScheduleUpdateService scheduleUpdateSrv;

        public CommonCommandsModule(ConfigService configService, DatabaseService databaseService, ScheduleUpdateService scheduleUpdateService)
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

        [RequireGuild]
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

        [RequireGuild]
        [Command("time")]
        [Description("Показывается время до обновления")]
        [Aliases("время")]
        public async Task ShowTimeTillUpdate(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();

            string description = $"Очки стран будут обновлены через ";
            description += string.Format(new TimeWordFormatter(), "{0:W}.", scheduleUpdateSrv.GetTimeTillUpdate());

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

        [RequireGuild]
        [Command("roles")]
        [Description("Отображает список ролей, отсортированных по количеству носителей")]
        public async Task ShowRoles(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();

            string description = "Участников — Роль\n";

            List<Tuple<int, string>> rolePairs = new List<Tuple<int, string>>();

            foreach (var el in ctx.Guild.Roles)
            {
                int memberCount = ctx.Guild.Members.Where(x => x.Value.Roles.Contains(el.Value)).Count();
                if (memberCount > 1)
                {
                    rolePairs.Add(new Tuple<int, string>(memberCount, el.Value.Name));
                }
            }

            rolePairs.OrderByDescending(x => x.Item1);

            foreach (var el in rolePairs)
            {
                description += $"\n{el.Item1} — {el.Item2}";
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

        [RequireGuild]
        [Command("stats")]
        [Description("Отображает статистику по серверу")]
        public async Task ShowServerStats(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();

            var members = ctx.Guild.Members;
            
            // Людей всего
            int membersCount = members.Where(x => x.Value.IsBot == false).Count();
            // Людей онлайн (не оффлайн)
            int membersOnlineCount = members.Where(x => x.Value.IsBot == false).Where(x => x.Value.Presence != null).Count();
            // Людей в голосовых чатах сейчас
            int membersInVoiceChatsCount = members.Where(x => x.Value.IsBot == false).Where(x => x.Value.VoiceState != null).Where(x => x.Value.VoiceState.Channel != null).Count();
            // Людей играет
            int membersPlaying = members.Where(x => x.Value.IsBot == false).Where(x => x.Value.Presence != null).Where(x => x.Value.Presence.Activity.ActivityType == ActivityType.Playing).Count();
            // Ботов всего
            int botsCount = members.Where(x => x.Value.IsBot == true).Count();
            // Ролей всего
            int rolesCount = ctx.Guild.Roles.Count;
            // За 24 часа присоединилось
            int recentMembersCount = members.Where(x => x.Value.JoinedAt.DateTime.CompareTo(DateTime.Now.AddDays(-1)) == 1).Count();

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

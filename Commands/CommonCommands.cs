using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;

namespace NationDiscordBot.Commands
{
    [Description("Команды пользователя.")]
    public class CommonCommands
    {
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
        [Aliases("показать", "страны", "countries")]
        public async Task ShowCountries(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();

            var embed = new Discord​Embed​Builder()
            {
                Title = "Страны",
                Description = "Суки, выкусите",
                Footer = new DiscordEmbedBuilder.EmbedFooter() { Text = "Cyka" },
                Color = DiscordColor.DarkGreen
            };

            embed.AddField("Дания", "13", true);
            embed.AddField("Хуйхуй", "231", true);
            embed.AddField("Дадая", "0", true);

            await ctx.RespondAsync(null, false, embed.Build());
        }
    }
}
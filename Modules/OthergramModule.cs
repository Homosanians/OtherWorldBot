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
    [ModuleLifespan(ModuleLifespan.Transient)]
    [Description("Публикация фотографий.")]
    public class OthergramModule : BaseCommandModule
    {
        private readonly ConfigService configSrv;

        public OthergramModule(ConfigService configService)
        {
            configSrv = configService;
        }

        [RequireDirectMessage]
        [Command("othergram")]
        [Description("Публикация новой фотографии.")]
        [Aliases("og")]
        public async Task AddImagePost(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();

            var emoji = DiscordEmoji.FromName(ctx.Client, ":heart");

            // await ctx.RespondAsync($"{emoji} Pong! Ping: {ctx.Client.Ping}ms");

            await ctx.RespondAsync(embed: new Discord​Embed​Builder
            {
                Color = new DiscordColor(configSrv.BotConfig.CommonColor),
                Title = "Публикация фотографии",
                Description = $"Отправьте фотографию, которую вы хотите опубликовать.",
                Footer = new DiscordEmbedBuilder.EmbedFooter { Text = "Other World" }
            });
            return;
        }
    }
}

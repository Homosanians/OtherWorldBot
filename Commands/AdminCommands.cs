using System;
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
    [Description("Административные команды.")]
    [RequirePermissions(Permissions.ManageGuild)]
    public class AdminCommands
    {
        private ConfigService configSrv;

        public AdminCommands(ConfigService configService)
        {
            configSrv = configService;
        }

        [Command("add"), Description("Добавляет страну.")]
        public async Task Add(CommandContext ctx, [Description("Название страны.")] string country)
        {
            await ctx.TriggerTypingAsync();

            var embed = new DiscordEmbedBuilder
            {
                Color = DiscordColor.Green,
                Title = "Создание страны",
                Description = $"Вы успешно создали страну {country}.",
                Footer = new DiscordEmbedBuilder.EmbedFooter() { Text = "Cyka" }
            };

            // DATABASE ACTIONS HERE

            await ctx.RespondAsync(embed: embed);
        }

        [Command("remove"), Description("Удаляет страну.")]
        public async Task Remove(CommandContext ctx, [Description("Название страны.")] string country)
        {
            await ctx.TriggerTypingAsync();

            var interactivity = ctx.Client.GetInteractivityModule();

            var emojiConfirm = DiscordEmoji.FromName(ctx.Client, ":white_check_mark:");
            var emojiCancel = DiscordEmoji.FromName(ctx.Client, ":x:");

            var embed = new Discord​Embed​Builder()
            {
                Color = new DiscordColor(configSrv.BotConfig.CommonColor),
                Title = "Удаление страны",
                Description = $"Вы уверены, что хотите удалить страну {country}?",
                Footer = new DiscordEmbedBuilder.EmbedFooter() { Text = "Cyka" }
            };

            // Sent confirmation message.
            var sentMessage = await ctx.RespondAsync(embed: embed.Build());

            // Add choice reactions to the sent message.
            await sentMessage.CreateReactionAsync(emojiConfirm);
            await sentMessage.CreateReactionAsync(emojiCancel);

            // Wait for reactions
            var reactionCtx = await interactivity.WaitForReactionAsync(discordEmoji => discordEmoji == emojiConfirm || discordEmoji == emojiCancel, ctx.User, TimeSpan.FromSeconds(60));
            if (reactionCtx == null)
            {
                var embedCountryWasNotCreated = new DiscordEmbedBuilder
                {
                    Color = new DiscordColor(configSrv.BotConfig.TimeoutColor),
                    Title = "Удаление страны",
                    Description = $"Время истекло. Страна {country} не была удалена.",
                    Footer = new DiscordEmbedBuilder.EmbedFooter() { Text = "Cyka" }
                };
                await sentMessage.ModifyAsync(embed: embedCountryWasNotCreated);
            }
            else if (reactionCtx.Emoji == emojiConfirm)
            {
                var embedConfirm = new DiscordEmbedBuilder
                {
                    Color = new DiscordColor(configSrv.BotConfig.GoodColor),
                    Title = "Удаление страны",
                    Description = $"Вы подтвердили действие. Страна {country} была успешно удалена.",
                    Footer = new DiscordEmbedBuilder.EmbedFooter() { Text = "Cyka" }
                };

                // DATABASE ACTIONS HERE

                await sentMessage.ModifyAsync(embed: embedConfirm);
            }
            else if (reactionCtx.Emoji == emojiCancel)
            {
                var embedCancel = new DiscordEmbedBuilder
                {
                    Color = new DiscordColor(configSrv.BotConfig.BadColor),
                    Title = "Удаление страны",
                    Description = $"Вы отменили действие. Страна {country} не была удалена.",
                    Footer = new DiscordEmbedBuilder.EmbedFooter() { Text = "Cyka" }
                };
                await sentMessage.ModifyAsync(embed: embedCancel);
            }
        }

        [Command("charge"), Description("Добавляет очки бесчестия стране.")]
        public async Task Charge(CommandContext ctx, [Description("Название страны.")] string country, [Description("Количество очков бесчестия для добавления.")] int amount)
        {
            await ctx.TriggerTypingAsync();

            var embed = new Discord​Embed​Builder()
            {
                Color = new DiscordColor(configSrv.BotConfig.GoodColor),
                Title = "Начисление очков бесчестия",
                Description = $"Вы начислили {amount} очков бесчестия стране {country}",
                Footer = new DiscordEmbedBuilder.EmbedFooter() { Text = "Cyka" }
            };

            // DATABASE ACTIONS HERE

            await ctx.RespondAsync(null, false, embed.Build());
        }
    }
}
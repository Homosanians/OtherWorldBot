using System;
using System.Linq;
using System.Threading.Tasks;
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
        [Command("add"), Description("Добавляет страну."), RequireOwner]
        public async Task Add(CommandContext ctx, [Description("Название страны.")] string country)
        {
            await ctx.TriggerTypingAsync();

            var interactivity = ctx.Client.GetInteractivityModule();

            var emojiConfirm = DiscordEmoji.FromName(ctx.Client, ":white_check_mark:");
            var emojiCancel = DiscordEmoji.FromName(ctx.Client, ":x:");

            var embed = new Discord​Embed​Builder()
            {
                Color = DiscordColor.DarkGray,
                Title = "Создание страны",
                Description = $"Вы уверены, что хотите создать страну {country}?",
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
                    Color = DiscordColor.Yellow,
                    Title = "Создание страны",
                    Description = $"Время истекло. Страна {country} не была создана.",
                    Footer = new DiscordEmbedBuilder.EmbedFooter() { Text = "Cyka" }
                };
                await sentMessage.ModifyAsync(embed: embedCountryWasNotCreated);
            }
            else if (reactionCtx.Emoji == emojiConfirm)
            {
                var embedConfirm = new DiscordEmbedBuilder
                {
                    Color = DiscordColor.Green,
                    Title = "Создание страны",
                    Description = $"Вы подтвердили действие. Страна {country} была успешно создана.",
                    Footer = new DiscordEmbedBuilder.EmbedFooter() { Text = "Cyka" }
                };
                await sentMessage.ModifyAsync(embed: embedConfirm);
            }
            else if (reactionCtx.Emoji == emojiCancel)
            {
                var embedCancel = new DiscordEmbedBuilder
                {
                    Color = DiscordColor.Red,
                    Title = "Создание страны",
                    Description = $"Вы отменили действие. Страна {country} не была создана.",
                    Footer = new DiscordEmbedBuilder.EmbedFooter() { Text = "Cyka" }
                };
                await sentMessage.ModifyAsync(embed: embedCancel);
            }
        }

        [Command("remove"), Description("Удаляет страну."), RequireOwner]
        public async Task Remove(CommandContext ctx, [Description("Название страны.")] string country)
        {
            await ctx.TriggerTypingAsync();

            var embed = new Discord​Embed​Builder()
            {
                Title = "Подтвердите действие!",
                Description = $"Вы уверены, что хотите удалить страну {null}",
                Footer = new DiscordEmbedBuilder.EmbedFooter() { Text = "Cyka" },
                Color = DiscordColor.DarkGreen
            };

            await ctx.RespondAsync(null, false, embed.Build());
        }

        [Command("charge"), Description("Добавляет очки бесчестия стране."), RequireOwner]
        public async Task Charge(CommandContext ctx, [Description("Название страны.")] string country, [Description("Количество очков бесчестия для добавления.")] int amount)
        {
            await ctx.TriggerTypingAsync();

            var embed = new Discord​Embed​Builder()
            {
                Title = "Уведомление о действие!",
                Description = $"Вы начислили {null} очков бесчестия стране {null}",
                Footer = new DiscordEmbedBuilder.EmbedFooter() { Text = "Cyka" },
                Color = DiscordColor.DarkGreen
            };

            await ctx.RespondAsync(null, false, embed.Build());
        }
    }
}
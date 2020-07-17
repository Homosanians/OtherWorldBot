using System;
using System.Linq;
using System.Threading.Tasks;
using DisgraceDiscordBot.Entities;
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
        private DatabaseService databaseSrv;

        public AdminCommands(ConfigService configService, DatabaseService databaseService)
        {
            configSrv = configService;
            databaseSrv = databaseService;
        }

        [Command("add"), Description("Добавляет страну.")]
        public async Task Add(CommandContext ctx, [Description("Название страны.")] string countryName)
        {
            await ctx.TriggerTypingAsync();

            // DATABASE ACTIONS HERE
            var entry = new Country();
            entry.Name = countryName;
            entry.DisgracePoints = 0;
            entry.LastUpdateTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            var entryAdded = databaseSrv.SetCountry(entry);

            if (entryAdded)
            {
                var embedSuccess = new DiscordEmbedBuilder
                {
                    Color = new DiscordColor(configSrv.BotConfig.GoodColor),
                    Title = "Создание страны",
                    Description = $"Вы успешно создали страну {countryName}.",
                    Footer = new DiscordEmbedBuilder.EmbedFooter() { Text = "Cyka" }
                };

                await ctx.RespondAsync(embed: embedSuccess);
            }
            else
            {
                var embedError = new DiscordEmbedBuilder
                {
                    Color = new DiscordColor(configSrv.BotConfig.BadColor),
                    Title = "Создание страны",
                    Description = $"Произошла внутренняя ошибка. Страна {countryName} не была создана.",
                    Footer = new DiscordEmbedBuilder.EmbedFooter() { Text = "Cyka" }
                };

                await ctx.RespondAsync(embed: embedError);
            }
        }

        // сначало чек на существование, а потом всё остальное. если нет, то красная ошибка. если есть, то везде пишем название, даже если получили страну из ИД
        [Command("remove"), Description("Удаляет страну.")]
        public async Task Remove(CommandContext ctx, [Description("Название страны.")] string country)
        {
            await ctx.TriggerTypingAsync();

            Country foundCountry = null;

            try
            {
                Country countryById = databaseSrv.GetCountryById(int.Parse(country));
                if (countryById != null)
                    foundCountry = countryById;
            }
            catch (Exception) { }

            try
            {
                Country countryByName = databaseSrv.GetCountryByName(country);
                if (countryByName != null)
                    foundCountry = countryByName;
            }
            catch (Exception) { }

            if (foundCountry == null)
            {
                // Error occured
                var embedError = new DiscordEmbedBuilder
                {
                    Color = new DiscordColor(configSrv.BotConfig.BadColor),
                    Title = "Удаление страны",
                    Description = $"Страна {country} не была найдена.",
                    Footer = new DiscordEmbedBuilder.EmbedFooter() { Text = "Cyka" }
                };

                await ctx.RespondAsync(embed: embedError);
            }
            else
            {
                var interactivity = ctx.Client.GetInteractivityModule();

                var emojiConfirm = DiscordEmoji.FromName(ctx.Client, ":white_check_mark:");
                var emojiCancel = DiscordEmoji.FromName(ctx.Client, ":x:");

                var embed = new Discord​Embed​Builder()
                {
                    Color = new DiscordColor(configSrv.BotConfig.CommonColor),
                    Title = "Удаление страны",
                    Description = $"Вы уверены, что хотите удалить страну {foundCountry.Name}?",
                    Footer = new DiscordEmbedBuilder.EmbedFooter() { Text = "Cyka" }
                };

                // Sent confirmation message.
                var sentMessage = await ctx.RespondAsync(embed: embed);

                // Add choice reactions to the sent message.
                await sentMessage.CreateReactionAsync(emojiConfirm);
                await sentMessage.CreateReactionAsync(emojiCancel);

                // Wait for reactions
                var reactionCtx = await interactivity.WaitForReactionAsync(discordEmoji => discordEmoji == emojiConfirm || discordEmoji == emojiCancel, ctx.User, TimeSpan.FromSeconds(60));
                if (reactionCtx == null)
                {
                    var embedCountryWasNotDeleted = new DiscordEmbedBuilder
                    {
                        Color = new DiscordColor(configSrv.BotConfig.TimeoutColor),
                        Title = "Удаление страны",
                        Description = $"Время истекло. Страна {foundCountry.Name} не была удалена.",
                        Footer = new DiscordEmbedBuilder.EmbedFooter() { Text = "Cyka" }
                    };
                    await sentMessage.ModifyAsync(embed: embedCountryWasNotDeleted);
                }
                else if (reactionCtx.Emoji == emojiConfirm)
                {
                    // DATABASE ACTIONS HERE
                    bool deleted = databaseSrv.RemoveCountry(foundCountry);
                    if (deleted)
                    {
                        var embedSuccess = new DiscordEmbedBuilder
                        {
                            Color = new DiscordColor(configSrv.BotConfig.GoodColor),
                            Title = "Удаление страны",
                            Description = $"Вы подтвердили действие. Страна {foundCountry.Name} была успешно удалена.",
                            Footer = new DiscordEmbedBuilder.EmbedFooter() { Text = "Cyka" }
                        };

                        await sentMessage.ModifyAsync(embed: embedSuccess);
                    }
                    else
                    {
                        var embedError = new DiscordEmbedBuilder
                        {
                            Color = new DiscordColor(configSrv.BotConfig.BadColor),
                            Title = "Удаление страны",
                            Description = $"Произошла внутренняя ошибка. Страна {foundCountry.Name} не была удалена.",
                            Footer = new DiscordEmbedBuilder.EmbedFooter() { Text = "Cyka" }
                        };

                        await sentMessage.ModifyAsync(embed: embedError);
                    }
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
        }

        [Command("charge"), Description("Начисляет очки бесчестия стране.")]
        public async Task Charge(CommandContext ctx, [Description("Название страны.")] string country, [Description("Количество очков бесчестия для добавления.")] int amount)
        {
            await ctx.TriggerTypingAsync();

            if (amount > 0)
            {
                // Начисляет

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
            else if (amount < 0)
            {
                // Списывает

                var embed = new Discord​Embed​Builder()
                {
                    Color = new DiscordColor(configSrv.BotConfig.GoodColor),
                    Title = "Списание очков бесчестия",
                    Description = $"Вы списали {Math.Abs(amount)} очков бесчестия стране {country}",
                    Footer = new DiscordEmbedBuilder.EmbedFooter() { Text = "Cyka" }
                };


                // DATABASE ACTIONS HERE

                await ctx.RespondAsync(null, false, embed.Build());
            }
            else
            {
                // Попытка списание/начисления 0 очков или исключение

                var embed = new Discord​Embed​Builder()
                {
                    Color = new DiscordColor(configSrv.BotConfig.BadColor),
                    Title = "Произошла ошибка",
                    Description = $"Убедитесь, что вы используете команду правильно.",
                    Footer = new DiscordEmbedBuilder.EmbedFooter() { Text = "Cyka" }
                };

                await ctx.RespondAsync(null, false, embed.Build());
            }
        }
    }
}
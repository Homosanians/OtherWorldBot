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

            if (databaseSrv.IsCountryExist(countryName))
            {
                var embedError = new DiscordEmbedBuilder
                {
                    Color = new DiscordColor(configSrv.BotConfig.BadColor),
                    Title = "Создание страны",
                    Description = $"Страна {countryName} не была создана, поскольку она уже существует.",
                    Footer = new DiscordEmbedBuilder.EmbedFooter() { Text = "Cyka" }
                };

                await ctx.RespondAsync(embed: embedError);
            }
            else
            {
                // DATABASE ACTIONS HERE
                var entry = new Country();
                entry.Name = countryName;
                entry.DisgracePoints = 0;

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
        }

        // сначало чек на существование, а потом всё остальное. если нет, то красная ошибка. если есть, то везде пишем название, даже если получили страну из ИД
        [Command("remove"), Description("Удаляет страну.")]
        public async Task Remove(CommandContext ctx, [Description("Название страны.")] string countryName)
        {
            await ctx.TriggerTypingAsync();

            Country foundCountry = null;

            try
            {
                foundCountry = databaseSrv.GetCountryByName(countryName);
            }
            catch (Exception) { }

            if (foundCountry == null)
            {
                // Error occured
                var embedError = new DiscordEmbedBuilder
                {
                    Color = new DiscordColor(configSrv.BotConfig.TimeoutColor),
                    Title = "Удаление страны",
                    Description = $"Страна {countryName} не была найдена.",
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
                    Description = $"Вы уверены, что хотите удалить страну {countryName}?",
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
                        Description = $"Время истекло. Страна {countryName} не была удалена.",
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
                            Description = $"Вы подтвердили действие. Страна {countryName} была успешно удалена.",
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
                            Description = $"Произошла внутренняя ошибка. Страна {countryName} не была удалена.",
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
                        Description = $"Вы отменили действие. Страна {countryName} не была удалена.",
                        Footer = new DiscordEmbedBuilder.EmbedFooter() { Text = "Cyka" }
                    };
                    await sentMessage.ModifyAsync(embed: embedCancel);
                }
            }
        }

        [Command("charge"), Description("Начисляет очки бесчестия стране.")]
        public async Task Charge(CommandContext ctx, [Description("Название страны.")] string countryName, [Description("Количество очков бесчестия для добавления.")] int amount)
        {
            await ctx.TriggerTypingAsync();

            Country foundCountry = null;

            try
            {
                foundCountry = databaseSrv.GetCountryByName(countryName);
            }
            catch (Exception) { }

            if (foundCountry == null)
            {
                var embedError = new DiscordEmbedBuilder
                {
                    Color = new DiscordColor(configSrv.BotConfig.TimeoutColor),
                    Title = "Начисление очков бесчестия",
                    Description = $"Страна {countryName} не была найдена.",
                    Footer = new DiscordEmbedBuilder.EmbedFooter() { Text = "Cyka" }
                };

                await ctx.RespondAsync(embed: embedError);
            }
            else if (amount == -527694)
            {
                var embedError = new DiscordEmbedBuilder
                {
                    Color = new DiscordColor(configSrv.BotConfig.BadColor),
                    Title = "Начисление очков бесчестия",
                    Description = $"Введены символы. Невереное использование команды.",
                    Footer = new DiscordEmbedBuilder.EmbedFooter() { Text = "Cyka" }
                };

                await ctx.RespondAsync(embed: embedError);
            }
            else
            {
                if (amount > 0)
                {
                    // Начисляет

                    // DATABASE ACTIONS HERE
                    foundCountry.DisgracePoints += amount;
                    bool successed = databaseSrv.UpdateCountry(foundCountry);

                    if (successed)
                    {
                        var embed = new Discord​Embed​Builder()
                        {
                            Color = new DiscordColor(configSrv.BotConfig.GoodColor),
                            Title = "Начисление очков бесчестия",
                            Description = $"Вы начислили {amount} очков бесчестия стране {countryName}",
                            Footer = new DiscordEmbedBuilder.EmbedFooter() { Text = "Cyka" }
                        };

                        await ctx.RespondAsync(embed: embed);
                    }
                    else
                    {
                        var embed = new Discord​Embed​Builder()
                        {
                            Color = new DiscordColor(configSrv.BotConfig.BadColor),
                            Title = "Начисление очков бесчестия",
                            Description = $"Произошла внутренняя ошибка. Страна {foundCountry.Name} по прежднему имеет {foundCountry.DisgracePoints} очков бесчестия.",
                            Footer = new DiscordEmbedBuilder.EmbedFooter() { Text = "Cyka" }
                        };

                        await ctx.RespondAsync(embed: embed);
                    }
                }
                else if (amount < 0)
                {
                    // Списывает

                    // Проверяет если после списания будет меньше 0 очков
                    if (foundCountry.DisgracePoints + amount < 0)
                    {
                        int previousValue = foundCountry.DisgracePoints;

                        // DATABASE ACTIONS HERE
                        foundCountry.DisgracePoints = 0;
                        bool successed = databaseSrv.UpdateCountry(foundCountry);

                        if (successed)
                        {
                            var embed = new Discord​Embed​Builder()
                            {
                                Color = new DiscordColor(configSrv.BotConfig.GoodColor),
                                Title = "Списание очков бесчестия",
                                Description = $"Вы попытались списать {Math.Abs(amount)} очков бесчестия, но только {previousValue} было списано. Теперь страна {countryName} имеет {foundCountry.DisgracePoints} очков бесчестия.",
                                Footer = new DiscordEmbedBuilder.EmbedFooter() { Text = "Cyka" }
                            };

                            await ctx.RespondAsync(embed: embed);
                        }
                        else
                        {
                            var embed = new Discord​Embed​Builder()
                            {
                                Color = new DiscordColor(configSrv.BotConfig.BadColor),
                                Title = "Начисление очков бесчестия",
                                Description = $"Произошла внутренняя ошибка. Страна {foundCountry.Name} по прежднему имеет {foundCountry.DisgracePoints} очков бесчестия.",
                                Footer = new DiscordEmbedBuilder.EmbedFooter() { Text = "Cyka" }
                            };

                            await ctx.RespondAsync(embed: embed);
                        }
                    }
                    else
                    {
                        // DATABASE ACTIONS HERE
                        foundCountry.DisgracePoints += amount;
                        bool successed = databaseSrv.UpdateCountry(foundCountry);

                        if (successed)
                        {
                            var embed = new Discord​Embed​Builder()
                            {
                                Color = new DiscordColor(configSrv.BotConfig.GoodColor),
                                Title = "Списание очков бесчестия",
                                Description = $"Вы списали {Math.Abs(amount)} очков бесчестия. Теперь страна {countryName} имеет {foundCountry.DisgracePoints} очков бесчестия.",
                                Footer = new DiscordEmbedBuilder.EmbedFooter() { Text = "Cyka" }
                            };

                            await ctx.RespondAsync(embed: embed);
                        }
                        else
                        {
                            var embed = new Discord​Embed​Builder()
                            {
                                Color = new DiscordColor(configSrv.BotConfig.BadColor),
                                Title = "Начисление очков бесчестия",
                                Description = $"Произошла внутренняя ошибка. Страна {foundCountry.Name} по прежднему имеет {foundCountry.DisgracePoints} очков бесчестия.",
                                Footer = new DiscordEmbedBuilder.EmbedFooter() { Text = "Cyka" }
                            };

                            await ctx.RespondAsync(embed: embed);
                        }
                    }
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
}
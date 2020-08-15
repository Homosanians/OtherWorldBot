using System;
using System.Linq;
using System.Threading.Tasks;
using OtherWorldBot.Entities;
using OtherWorldBot.Services;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;

namespace OtherWorldBot.Modules
{
    [ModuleLifespan(ModuleLifespan.Singleton)]
    [Description("Административные команды.")]
    [RequirePermissions(Permissions.Administrator)]
    public class AdminCommandsModule : BaseCommandModule
    {
        private readonly ConfigService configSrv;
        private readonly DatabaseService databaseSrv;

        public AdminCommandsModule(ConfigService configService, DatabaseService databaseService)
        {
            configSrv = configService;
            databaseSrv = databaseService;
        }

        [Command("add"), Description("Добавляет страну.")]
        public async Task Add(CommandContext ctx, [RemainingText, Description("Название страны.")] string countryName)
        {
            if (!string.IsNullOrWhiteSpace(countryName))
            {
                await ctx.TriggerTypingAsync();

                if (await databaseSrv.IsCountryExistAsync(countryName))
                {
                    var embedError = new DiscordEmbedBuilder
                    {
                        Color = new DiscordColor(configSrv.BotConfig.WarningColor),
                        Title = "Создание страны",
                        Description = $"Страна {countryName} не была создана, поскольку она уже существует.",
                        Footer = new DiscordEmbedBuilder.EmbedFooter { Text = "Other World" }
                    };

                    await ctx.RespondAsync(embed: embedError);
                }
                else
                {
                    var entry = new Country();
                    entry.Name = countryName;
                    entry.DisgracePoints = 0;

                    var entryAdded = databaseSrv.SetCountryAsync(entry);

                    if (await entryAdded)
                    {
                        var embedSuccess = new DiscordEmbedBuilder
                        {
                            Color = new DiscordColor(configSrv.BotConfig.GoodColor),
                            Title = "Создание страны",
                            Description = $"Вы успешно создали страну {countryName}.",
                            Footer = new DiscordEmbedBuilder.EmbedFooter { Text = "Other World" }
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
                            Footer = new DiscordEmbedBuilder.EmbedFooter { Text = "Other World" }
                        };

                        await ctx.RespondAsync(embed: embedError);
                    }
                }
            }
        }

        [Command("remove"), Aliases("del", "delete"), Description("Удаляет страну.")]
        public async Task Remove(CommandContext ctx, [RemainingText, Description("Название страны.")] string countryName)
        {
            await ctx.TriggerTypingAsync();

            Country foundCountry = await databaseSrv.GetCountryByNameAsync(countryName);

            if (foundCountry == null)
            {
                // Error occured
                var embedError = new DiscordEmbedBuilder
                {
                    Color = new DiscordColor(configSrv.BotConfig.WarningColor),
                    Title = "Удаление страны",
                    Description = $"Страна {countryName} не была найдена.",
                    Footer = new DiscordEmbedBuilder.EmbedFooter { Text = "Other World" }
                };

                await ctx.RespondAsync(embed: embedError);
            }
            else
            {
                var interactivity = ctx.Client.GetInteractivity();

                var emojiConfirm = DiscordEmoji.FromName(ctx.Client, ":white_check_mark:");
                var emojiCancel = DiscordEmoji.FromName(ctx.Client, ":x:");

                var embed = new Discord​Embed​Builder
                {
                    Color = new DiscordColor(configSrv.BotConfig.CommonColor),
                    Title = "Удаление страны",
                    Description = $"Вы уверены, что хотите удалить страну {countryName}?",
                    Footer = new DiscordEmbedBuilder.EmbedFooter { Text = "Other World" }
                };

                // Sent confirmation message.
                var sentMessage = await ctx.RespondAsync(embed: embed);

                // Add choice reactions to the sent message.
                await sentMessage.CreateReactionAsync(emojiConfirm);
                await sentMessage.CreateReactionAsync(emojiCancel);

                // Wait for reactions
                var reactionCtx = await interactivity.WaitForReactionAsync(x => x.Emoji == emojiConfirm || x.Emoji == emojiCancel, ctx.User, TimeSpan.FromSeconds(60));
                if (reactionCtx.TimedOut)
                {
                    var embedCountryWasNotDeleted = new DiscordEmbedBuilder
                    {
                        Color = new DiscordColor(configSrv.BotConfig.WarningColor),
                        Title = "Удаление страны",
                        Description = $"Время истекло. Страна {countryName} не была удалена.",
                        Footer = new DiscordEmbedBuilder.EmbedFooter { Text = "Other World" }
                    };
                    await sentMessage.ModifyAsync(embed: embedCountryWasNotDeleted.Build());
                }
                else if (reactionCtx.Result.Emoji == emojiConfirm)
                {
                    bool deleted = await databaseSrv.RemoveCountryAsync(foundCountry);
                    if (deleted)
                    {
                        var embedSuccess = new DiscordEmbedBuilder
                        {
                            Color = new DiscordColor(configSrv.BotConfig.GoodColor),
                            Title = "Удаление страны",
                            Description = $"Вы подтвердили действие. Страна {countryName} была успешно удалена.",
                            Footer = new DiscordEmbedBuilder.EmbedFooter { Text = "Other World" }
                        };

                        await sentMessage.ModifyAsync(embed: embedSuccess.Build());
                    }
                    else
                    {
                        var embedError = new DiscordEmbedBuilder
                        {
                            Color = new DiscordColor(configSrv.BotConfig.BadColor),
                            Title = "Удаление страны",
                            Description = $"Произошла внутренняя ошибка. Страна {countryName} не была удалена.",
                            Footer = new DiscordEmbedBuilder.EmbedFooter { Text = "Other World" }
                        };

                        await sentMessage.ModifyAsync(embed: embedError.Build());
                    }
                }
                else if (reactionCtx.Result.Emoji == emojiCancel)
                {
                    var embedCancel = new DiscordEmbedBuilder
                    {
                        Color = new DiscordColor(configSrv.BotConfig.BadColor),
                        Title = "Удаление страны",
                        Description = $"Вы отменили действие. Страна {countryName} не была удалена.",
                        Footer = new DiscordEmbedBuilder.EmbedFooter { Text = "Other World" }
                    };
                    await sentMessage.ModifyAsync(embed: embedCancel.Build());
                }
            }
        }

        [Command("charge"), Description("Начисляет очки бесчестия стране.")]
        public async Task Charge(CommandContext ctx, [Description("Количество очков бесчестия для добавления.")] int amount, [RemainingText, Description("Название страны.")] string countryName)
        {
            await ctx.TriggerTypingAsync();

            Country foundCountry = await databaseSrv.GetCountryByNameAsync(countryName);

            if (foundCountry == null)
            {
                var embedError = new DiscordEmbedBuilder
                {
                    Color = new DiscordColor(configSrv.BotConfig.WarningColor),
                    Title = "Начисление очков бесчестия",
                    Description = $"Страна {countryName} не была найдена.",
                    Footer = new DiscordEmbedBuilder.EmbedFooter { Text = "Other World" }
                };

                await ctx.RespondAsync(embed: embedError);
            }
            else
            {
                if (amount > 0)
                {
                    // Начисляет

                    foundCountry.DisgracePoints += amount;
                    bool successed = await databaseSrv.UpdateCountryAsync(foundCountry);

                    if (successed)
                    {
                        var embed = new Discord​Embed​Builder()
                        {
                            Color = new DiscordColor(configSrv.BotConfig.GoodColor),
                            Title = "Начисление очков бесчестия",
                            Description = $"Вы начислили {amount} очков бесчестия стране {countryName}",
                            Footer = new DiscordEmbedBuilder.EmbedFooter { Text = "Other World" }
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
                            Footer = new DiscordEmbedBuilder.EmbedFooter { Text = "Other World" }
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

                        foundCountry.DisgracePoints = 0;
                        bool successed = await databaseSrv.UpdateCountryAsync(foundCountry);

                        if (successed)
                        {
                            var embed = new Discord​Embed​Builder()
                            {
                                Color = new DiscordColor(configSrv.BotConfig.GoodColor),
                                Title = "Списание очков бесчестия",
                                Description = $"Вы попытались списать {Math.Abs(amount)} очков бесчестия, но только {previousValue} было списано. Теперь страна {countryName} имеет {foundCountry.DisgracePoints} очков бесчестия.",
                                Footer = new DiscordEmbedBuilder.EmbedFooter { Text = "Other World" }
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
                                Footer = new DiscordEmbedBuilder.EmbedFooter { Text = "Other World" }
                            };

                            await ctx.RespondAsync(embed: embed);
                        }
                    }
                    else
                    {
                        foundCountry.DisgracePoints += amount;
                        bool successed = await databaseSrv.UpdateCountryAsync(foundCountry);

                        if (successed)
                        {
                            var embed = new Discord​Embed​Builder()
                            {
                                Color = new DiscordColor(configSrv.BotConfig.GoodColor),
                                Title = "Списание очков бесчестия",
                                Description = $"Вы списали {Math.Abs(amount)} очков бесчестия. Теперь страна {countryName} имеет {foundCountry.DisgracePoints} очков бесчестия.",
                                Footer = new DiscordEmbedBuilder.EmbedFooter { Text = "Other World" }
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
                                Footer = new DiscordEmbedBuilder.EmbedFooter { Text = "Other World" }
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
                        Footer = new DiscordEmbedBuilder.EmbedFooter { Text = "Other World" }
                    };

                    await ctx.RespondAsync(null, false, embed.Build());
                }
            }
        }
    }
}
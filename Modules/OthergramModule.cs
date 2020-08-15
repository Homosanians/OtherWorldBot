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
        [Aliases("og", "post")]
        public async Task AddImagePost(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();

            var interactivity = ctx.Client.GetInteractivity();

            var sentMessage = await ctx.RespondAsync(embed: new Discord​Embed​Builder
            {
                Color = new DiscordColor(configSrv.BotConfig.CommonColor),
                Title = "Публикация фотографии",
                Description = $"Введите описание и прикрепите саму фотографию, которую вы хотите опубликовать.",
                Footer = new DiscordEmbedBuilder.EmbedFooter { Text = "Other World" }
            });

            var messageCtx = await interactivity.WaitForMessageAsync(x => x.Attachments.Count > 0, TimeSpan.FromSeconds(120));

            if (messageCtx.TimedOut)
            {
                await sentMessage.ModifyAsync(embed: new Discord​Embed​Builder
                {
                    Color = new DiscordColor(configSrv.BotConfig.WarningColor),
                    Title = "Публикация фотографии",
                    Description = $"Время истекло. Фотография не была опубликована.",
                    Footer = new DiscordEmbedBuilder.EmbedFooter { Text = "Other World" }
                }.Build());
            }
            else
            {
                var postsChannel = await ctx.Client.GetChannelAsync(configSrv.BotConfig.OthergramConfig.PostsChannelId);

                if (postsChannel != null)
                {
                    var postMessage = await ctx.Client.SendMessageAsync(postsChannel, embed: new Discord​Embed​Builder
                    {
                        Color = new DiscordColor(configSrv.BotConfig.OthergramConfig.PostsColor),
                        Author = new DiscordEmbedBuilder.EmbedAuthor { Name = messageCtx.Result.Author.Username, IconUrl = messageCtx.Result.Author.AvatarUrl },
                        Title = "Новая фотография!",
                        ImageUrl = messageCtx.Result.Attachments[0].Url,
                        Description = messageCtx.Result.Content,
                        Footer = new DiscordEmbedBuilder.EmbedFooter { Text = $"Для публикации напишите {configSrv.BotConfig.CommandPrefix}othergram © Other World" }
                    });

                    var emoji = DiscordEmoji.FromName(ctx.Client, configSrv.BotConfig.OthergramConfig.PostsEmoji);

                    await postMessage.CreateReactionAsync(emoji);

                    await sentMessage.ModifyAsync(embed: new Discord​Embed​Builder
                    {
                        Color = new DiscordColor(configSrv.BotConfig.GoodColor),
                        Title = "Публикация фотографии",
                        Description = $"Фотография была успешно опубликована!",
                        Footer = new DiscordEmbedBuilder.EmbedFooter { Text = "Other World" }
                    }.Build());
                }
                else
                {
                    await sentMessage.ModifyAsync(embed: new Discord​Embed​Builder
                    {
                        Color = new DiscordColor(configSrv.BotConfig.BadColor),
                        Title = "Публикация фотографии",
                        Description = $"Произошла внутренняя ошибка. Похоже, канал для публикаций удалён.",
                        Footer = new DiscordEmbedBuilder.EmbedFooter { Text = "Other World" }
                    }.Build());
                }
            }
        }
    }
}

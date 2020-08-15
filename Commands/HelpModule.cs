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
using DSharpPlus.CommandsNext.Exceptions;
using System.Collections.Generic;

namespace OtherWorldBot.Commands
{
    [ModuleLifespan(ModuleLifespan.Transient)]
    public class HelpModule : BaseCommandModule
    {
        [Command("help"), Description("Показывает помощь по командам.")]
        public async Task HelpAsync(CommandContext ctx, [Description("Расскажет о других командах.")] params string[] command)
        {
            var topLevel = ctx.CommandsNext.TopLevelCommands.Values.Distinct();
            var helpBuilder = ctx.CommandsNext.HelpFormatter.Create(ctx);

            if (command != null && command.Any())
            {
                Command cmd = null;
                var searchIn = topLevel;
                foreach (var c in command)
                {
                    if (searchIn == null)
                    {
                        cmd = null;
                        break;
                    }

                    if (ctx.Config.CaseSensitive)
                        cmd = searchIn.FirstOrDefault(xc => xc.Name == c || (xc.Aliases != null && xc.Aliases.Contains(c)));
                    else
                        cmd = searchIn.FirstOrDefault(xc => xc.Name.ToLowerInvariant() == c.ToLowerInvariant() || (xc.Aliases != null && xc.Aliases.Select(xs => xs.ToLowerInvariant()).Contains(c.ToLowerInvariant())));

                    if (cmd == null)
                        break;

                    var failedChecks = await cmd.RunChecksAsync(ctx, true).ConfigureAwait(false);
                    if (failedChecks.Any())
                        throw new ChecksFailedException(cmd, ctx, failedChecks);

                    if (cmd is CommandGroup)
                        searchIn = (cmd as CommandGroup).Children;
                    else
                        searchIn = null;
                }

                if (cmd == null)
                    throw new CommandNotFoundException(string.Join(" ", command));

                helpBuilder.WithCommand(cmd);

                if (cmd is CommandGroup group)
                {
                    var commandsToSearch = group.Children.Where(xc => !xc.IsHidden);
                    var eligibleCommands = new List<Command>();
                    foreach (var candidateCommand in commandsToSearch)
                    {
                        if (candidateCommand.ExecutionChecks == null || !candidateCommand.ExecutionChecks.Any())
                        {
                            eligibleCommands.Add(candidateCommand);
                            continue;
                        }

                        var candidateFailedChecks = await candidateCommand.RunChecksAsync(ctx, true).ConfigureAwait(false);
                        if (!candidateFailedChecks.Any())
                            eligibleCommands.Add(candidateCommand);
                    }

                    if (eligibleCommands.Any())
                        helpBuilder.WithSubcommands(eligibleCommands.OrderBy(xc => xc.Name));
                }
            }
            else
            {
                var commandsToSearch = topLevel.Where(xc => !xc.IsHidden);
                var eligibleCommands = new List<Command>();
                foreach (var sc in commandsToSearch)
                {
                    if (sc.ExecutionChecks == null || !sc.ExecutionChecks.Any())
                    {
                        eligibleCommands.Add(sc);
                        continue;
                    }

                    var candidateFailedChecks = await sc.RunChecksAsync(ctx, true).ConfigureAwait(false);
                    if (!candidateFailedChecks.Any())
                        eligibleCommands.Add(sc);
                }

                if (eligibleCommands.Any())
                    helpBuilder.WithSubcommands(eligibleCommands.OrderBy(xc => xc.Name));
            }

            var helpMessage = helpBuilder.Build();

            if (!ctx.Config.DmHelp || ctx.Channel is DiscordDmChannel || ctx.Guild == null)
                await ctx.RespondAsync(helpMessage.Content, embed: helpMessage.Embed).ConfigureAwait(false);
            else
                await ctx.Member.SendMessageAsync(helpMessage.Content, embed: helpMessage.Embed).ConfigureAwait(false);
        }
    }
}

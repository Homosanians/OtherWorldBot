using System;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

namespace OtherWorldBot.Attributes
{
    /// <summary>
    /// Defines that a command is not usable within a direct message channel.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class RestrictDirectMessageAttribute : CheckBaseAttribute
    {
        /// <summary>
        /// Defines that this command is not usable within a direct message channel.
        /// </summary>
        public RestrictDirectMessageAttribute()
        { }

        public override Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help)
            => Task.FromResult(!(ctx.Channel is DiscordDmChannel));
    }
}

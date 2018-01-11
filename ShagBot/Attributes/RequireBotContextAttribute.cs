using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShagBot.Attributes
{

    public class RequireBotContextAttribute : PreconditionAttribute
    {
        private CmdChannelType _channelType;

        public RequireBotContextAttribute(CmdChannelType channelType)
        {
            _channelType = channelType;
        }

        public async override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            var guild = await context.Client.GetGuildAsync(GuildContext.GuildId);
            var user = await guild?.GetUserAsync(context.User.Id);

            if (_channelType == CmdChannelType.BotChannel)
            {
                if (context.Channel.Id != GuildContext.CmdChannelId)
                {
                    return PreconditionResult.FromError(CommandHandler.IgnoreErrorOutput);
                }
            }
            else if (_channelType == CmdChannelType.GuildChannel)
            {
                if (context.Guild?.Id != GuildContext.GuildId)
                {
                    return PreconditionResult.FromError(CommandHandler.IgnoreErrorOutput);
                }
            }
            else if (_channelType == CmdChannelType.DM)
            {
                if ((context as SocketCommandContext)?.IsPrivate != true)
                {
                    return PreconditionResult.FromError(CommandHandler.IgnoreErrorOutput);
                }
            }
            // else (channelType == Any), do nothing

            if (user?.RoleIds.Intersect(GuildContext.CmdRoleIds).Any() != true)
            {
                return PreconditionResult.FromError("Only Crew members can use ShagBot");
            }

            return PreconditionResult.FromSuccess();
        }
    }

    public enum CmdChannelType
    {
        GuildChannel = 1, 
        BotChannel = 2, 
        DM = 3
    }
}

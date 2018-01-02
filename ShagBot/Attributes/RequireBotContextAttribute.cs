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
        public async override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            if (context.Channel.Id != CommandHandler.CmdChannel)
            {
                return PreconditionResult.FromError(CommandHandler.IgnoreErrorOutput);
            }

            if ((context.User as IGuildUser)?.RoleIds?.Intersect(CommandHandler.CmdRoleIds).Any() != true)
            {
                return PreconditionResult.FromError("Only Crew members can use ShagBot");
            }

            return PreconditionResult.FromSuccess();
        }
    }
}

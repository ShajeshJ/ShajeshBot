using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShagBot.Attributes
{
    public class RequireDMContextAttribute : PreconditionAttribute
    {
        public async override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            var guild = await context.Client.GetGuildAsync(CommandHandler.GuildId);
            var user = await guild?.GetUserAsync(context.User.Id);

            if ((context as SocketCommandContext)?.IsPrivate != true)
            {
                return PreconditionResult.FromError(CommandHandler.IgnoreErrorOutput);
            }
            else if (user?.RoleIds.Intersect(CommandHandler.CmdRoleIds).Any() != true)
            {
                return PreconditionResult.FromError("Only Crew members can use ShagBot");
            }
            else
            {
                return PreconditionResult.FromSuccess();
            }
        }
    }
}

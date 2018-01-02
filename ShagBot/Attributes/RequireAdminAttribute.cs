using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShagBot.Attributes
{
    public class RequireAdminAttribute : PreconditionAttribute
    {
        public async override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            var guild = await context.Client.GetGuildAsync(CommandHandler.GuildId);
            var user = await guild.GetUserAsync(context.User.Id);

            if (user.RoleIds.Any(x => x == CommandHandler.AdminRoleId) == true)
            {
                return PreconditionResult.FromSuccess();
            }
            else
            {
                return PreconditionResult.FromError("This command can only be used by the admin group in Peanuts");
            }
        }
    }
}

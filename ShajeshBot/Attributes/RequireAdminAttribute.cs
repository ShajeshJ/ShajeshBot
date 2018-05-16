using Discord.Commands;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ShajeshBot.Attributes
{
    public class RequireAdminAttribute : PreconditionAttribute
    {
        public async override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            var guild = await context.Client.GetGuildAsync(GuildContext.GuildId);
            var user = await guild.GetUserAsync(context.User.Id);

            if (user.RoleIds.Any(x => x == GuildContext.AdminRoleId) == true)
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

using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShagBot.Attributes
{
    public class RequireDMAttribute : PreconditionAttribute
    {
        public async override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            if ((context as SocketCommandContext)?.IsPrivate == true)
            {
                return PreconditionResult.FromSuccess();
            }
            else
            {
                return PreconditionResult.FromError(CommandHandler.IgnoreErrorOutput);
            }
        }
    }
}

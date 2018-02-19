using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShagBot.Utilities
{
    public class DiscordUtilities
    {
        private SocketCommandContext _context;

        public DiscordUtilities(SocketCommandContext context)
        {
            _context = context;
        }

        public async Task MessageAdmins(string msg)
        {
            await MessageAdmins(new string[] { msg });
        }

        public async Task MessageAdmins(IEnumerable<string> msgs)
        {
            var adminRoleId = GuildContext.AdminRoleId;
            var adminUsers = _context.Guild.Users.Where(x => x.Roles.Any(y => y.Id == adminRoleId));

            foreach (var admin in adminUsers)
            {
                foreach (var msg in msgs)
                {
                    await admin.SendMessageAsync(msg);
                }
            }
        }
    }
}

using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShagBot.Extensions
{
    public static class SocketGuildExtensions
    {
        public static async Task MessageAdmins(this SocketGuild guild, string msg)
        {
            await guild.MessageAdmins(new string[] { msg });
        }

        public static async Task MessageAdmins(this SocketGuild guild, IEnumerable<string> msgs)
        {
            var adminRoleId = GuildContext.AdminRoleId;
            var adminUsers = guild.Users.Where(x => x.Roles.Any(y => y.Id == adminRoleId));

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

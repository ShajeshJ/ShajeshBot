using Discord;
using Discord.WebSocket;
using Discord.Commands;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace ShagBot.Modules
{
    public class RoleModule : ModuleBase<SocketCommandContext>
    {
        [Command("CreateGroup")]
        [Summary("Creates a new mentionable group with the name given by the roleName parameter.")]
        [Remarks("The command will fail if a role with the specified name already exists.")]
        public async Task CreateMentionGroup([Remainder]string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
            {
                await ReplyAsync("Cannot specify a role with an empty name");
                return;
            }

            if (Context.Guild.Roles.Any(x => x.Name.ToLower() == roleName.ToLower()))
            {
                await Context.Channel.SendMessageAsync($"Cannot create another group with the name \"{roleName}\"");
                return;
            }

            var role = await Context.Guild.CreateRoleAsync(roleName);
            await role.ModifyAsync(x =>
            {
                x.Mentionable = true;
            });

            var botUser = Context.Guild.GetUser(Context.Client.CurrentUser.Id);
            await botUser.AddRoleAsync(role);
            await Context.Channel.SendMessageAsync($"\"{roleName}\" created successfully.");
        }

        [Command("Join")]
        [Summary("Use to add yourself to an existing mentionable group.")]
        [Remarks("The command will fail if you try to join an unjoinable group.")]
        public async Task JoinMentionGroup([Remainder]IRole role)
        {
            var user = Context.User as IGuildUser;
            var botUser = Context.Guild.GetUser(Context.Client.CurrentUser.Id);
            
            if (GetJoinableGroupIds(botUser).Contains(role.Id))
            {
                await user.AddRoleAsync(role);
                await Context.Channel.SendMessageAsync($"Joined \"{role.Name}\" successfully");
            }
            else
            {
                await Context.Channel.SendMessageAsync($"\"{role.Name}\" is not a joinable group");
            }
        }

        [Command("Leave")]
        [Summary("Use to leave a mentionable group that you are currently part of.")]
        [Remarks("The command will fail if you are not part of the group, or if you do not have permission to leave the specified role.")]
        public async Task LeaveMentionGroup([Remainder]IRole role)
        {
            var user = Context.User as IGuildUser;
            var botUser = Context.Guild.GetUser(Context.Client.CurrentUser.Id);

            if (!user.RoleIds.Contains(role.Id))
            {
                await Context.Channel.SendMessageAsync($"You are not part of group \"{role.Name}\"");
            }
            else if (GetJoinableGroupIds(botUser).Contains(role.Id))
            {
                await user.RemoveRoleAsync(role);
                await Context.Channel.SendMessageAsync($"Left \"{role.Name}\" successfully");
            }
            else
            {
                await Context.Channel.SendMessageAsync($"Cannot leave group \"{role.Name}\"");
            }
        }

        [Command("ListGroups")]
        [Summary("Displays a list of all the available mentionable groups that you can join.")]
        public async Task ListMentionGroups()
        {
            var botUser = Context.Guild.GetUser(Context.Client.CurrentUser.Id);

            var roleList = new EmbedBuilder();

            roleList.WithTitle("Available Roles");

            var strList = "";

            foreach(var role in GetJoinableGroups(botUser))
            {
                strList += role.Name + Environment.NewLine;
            }

            roleList.WithDescription(strList);

            await Context.Channel.SendMessageAsync("", embed: roleList.Build());
        }

        private IEnumerable<ulong> GetJoinableGroupIds(SocketGuildUser bot)
        {
            return GetJoinableGroups(bot).Select(x => x.Id);
        }

        private IEnumerable<IRole> GetJoinableGroups(SocketGuildUser bot)
        {
            return bot.Roles.Where(x => !x.IsManaged && !x.IsEveryone);
        }
    }
}

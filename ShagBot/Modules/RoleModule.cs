using Discord;
using Discord.WebSocket;
using Discord.Commands;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using ShagBot.Attributes;

namespace ShagBot.Modules
{
    public class RoleModule : ModuleBase<SocketCommandContext>
    {
        [Command("creategroup")]
        [CmdSummary(nameof(Resource.CreateGroupSummary), typeof(Resource))]
        [CmdRemarks(nameof(Resource.CreateGroupRemarks), typeof(Resource))]
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

        [Command("joingroup")]
        [Alias("join")]
        [CmdSummary(nameof(Resource.JoinGroupSummary), typeof(Resource))]
        [CmdRemarks(nameof(Resource.JoinGroupRemarks), typeof(Resource))]
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

        [Command("leavegroup")]
        [Alias("leave")]
        [CmdSummary(nameof(Resource.JoinGroupSummary), typeof(Resource))]
        [CmdRemarks(nameof(Resource.LeaveGrroupRemarks), typeof(Resource))]
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

        [Command("listgroups")]
        [CmdSummary(nameof(Resource.ListGroupsSummary), typeof(Resource))]
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

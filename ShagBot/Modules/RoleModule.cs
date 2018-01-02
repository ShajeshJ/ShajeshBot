using Discord;
using Discord.WebSocket;
using Discord.Commands;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using ShagBot.Attributes;
using ShagBot.Extensions;

namespace ShagBot.Modules
{
    public class RoleModule : ModuleBase<SocketCommandContext>
    {
        [Command("creategroup")]
        [Alias("create")]
        [RequireBotContext]
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
        [RequireBotContext]
        [CmdSummary(nameof(Resource.JoinGroupSummary), typeof(Resource))]
        [CmdRemarks(nameof(Resource.JoinGroupRemarks), typeof(Resource))]
        public async Task JoinMentionGroup([Remainder]IRole role)
        {
            var user = Context.User as IGuildUser;
            
            if (GetJoinableGroupIds().Contains(role.Id))
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
        [RequireBotContext]
        [CmdSummary(nameof(Resource.JoinGroupSummary), typeof(Resource))]
        [CmdRemarks(nameof(Resource.LeaveGrroupRemarks), typeof(Resource))]
        public async Task LeaveMentionGroup([Remainder]IRole role)
        {
            var user = Context.User as IGuildUser;

            if (!user.RoleIds.Contains(role.Id))
            {
                await Context.Channel.SendMessageAsync($"You are not part of group \"{role.Name}\"");
            }
            else if (GetJoinableGroupIds().Contains(role.Id))
            {
                await user.RemoveRoleAsync(role);
                await Context.Channel.SendMessageAsync($"Left \"{role.Name}\" successfully");
            }
            else
            {
                await Context.Channel.SendMessageAsync($"Cannot leave group \"{role.Name}\"");
            }
        }

        [Command("deletegroup")]
        [Alias("delete")]
        [RequireBotContext]
        [CmdSummary(nameof(Resource.DeleteGroupSummary), typeof(Resource))]
        [CmdRemarks(nameof(Resource.DeleteGroupRemarks), typeof(Resource))]
        public async Task DeleteMentionGroup([Remainder]IRole role)
        {
            if (!GetJoinableGroupIds().Contains(role.Id))
            {
                await ReplyAsync($"Cannot delete group \"{role.Name}\" as it is not a custom mention group.");
            }
            else if (Context.Guild.Users.Any(u => !u.IsBot && u.Roles.Select(r => r.Id).Contains(role.Id)))
            {
                await ReplyAsync($"There are currently members part of the group \"{role.Name}\". Only mention groups without members can be deleted.");
            }
            else
            {
                await role.DeleteAsync();
                await ReplyAsync($"Mention group deleted successfully.");
            }
        }

        [Command("listmembers")]
        [Alias("listgroupmembers", "members")]
        [RequireBotContext]
        [CmdSummary(nameof(Resource.ListMembersSummary), typeof(Resource))]
        [CmdRemarks(nameof(Resource.ListMembersRemarks), typeof(Resource))]
        public async Task ListMentionGroupMembers([Remainder]IRole role)
        {
            if (GetJoinableGroupIds().Contains(role.Id))
            {
                var members = Context.Guild.Users.Where(u => !u.IsBot && u.Roles.Select(r => r.Id).Contains(role.Id));

                var memberList = new EmbedBuilder();
                memberList.WithTitle($"Members of {role.Name}: ");
                memberList.WithDescription(members.Count() > 0 ? 
                    string.Join(Environment.NewLine, members.Select(u => u.Nickname.IsNullOrWhitespace() ? u.Username : u.Nickname)) 
                    : "<No Members>");

                await ReplyAsync("", embed: memberList.Build());
            }
            else
            {
                await ReplyAsync($"Role \"{role.Name}\" is not a custom mention group.");
            }
        }

        [Command("listgroups")]
        [Alias("list")]
        [RequireBotContext]
        [CmdSummary(nameof(Resource.ListGroupsSummary), typeof(Resource))]
        public async Task ListMentionGroups()
        {
            var roleList = new EmbedBuilder();

            roleList.WithTitle("Available Roles");

            var strList = "";

            foreach(var role in GetJoinableGroups())
            {
                strList += role.Name + Environment.NewLine;
            }

            roleList.WithDescription(strList);

            await Context.Channel.SendMessageAsync("", embed: roleList.Build());
        }

        private IEnumerable<ulong> GetJoinableGroupIds()
        {
            return GetJoinableGroups().Select(x => x.Id);
        }

        private IEnumerable<IRole> GetJoinableGroups()
        {
            var bot = Context.Guild.GetUser(Context.Client.CurrentUser.Id);
            return bot.Roles.Where(x => !x.IsManaged && !x.IsEveryone);
        }
    }
}

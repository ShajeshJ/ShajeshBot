﻿using Discord;
using Discord.Commands;
using Discord.WebSocket;
using ShajeshBot.Attributes;
using ShajeshBot.Extensions;
using ShajeshBot.Models;
using System.Collections.Concurrent;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ShajeshBot.Modules
{
    public class ChannelModule : ModuleBase<SocketCommandContext>
    {
        private static ConcurrentDictionary<string, PendingChannelModel> _pendingChannels
        {
            get
            {
                return (ConcurrentDictionary<string, PendingChannelModel>)ShajeshBot.Memory["_pendingChannelDictionary"];
            }
            set
            {
                ShajeshBot.Memory.AddOrUpdate("_pendingChannelDictionary", value, (key, old) => value);
            }
        }

        static ChannelModule()
        {
            _pendingChannels = new ConcurrentDictionary<string, PendingChannelModel>();
        }

        [Command("requestchannel")]
        [RequireBotContext(CmdChannelType.BotChannel)]
        [CmdSummary(nameof(Resource.RequestChannelSummary), typeof(Resource))]
        [CmdRemarks(nameof(Resource.RequestChannelRemarks), typeof(Resource))]
        public async Task RequestChannel(string channelName, [Remainder]ICategoryChannel category = null)
        {
            if (channelName.IsNullOrWhitespace() || !IsValidChannelName(channelName))
            {
                await ReplyAsync("Channel name cannot be empty. Channel names can only contain alphanumeric, dash, and underscore characters.");
                return;
            }

            if (Context.Guild.Channels.Any(x => x.Name == channelName))
            {
                await ReplyAsync($"A channel with the specified name already exists.");
            }

            if (_pendingChannels.ContainsKey(channelName))
            {
                await ReplyAsync($"A pending channel request is already requesting the specified name.");
                return;
            }

            var channelRequest = new PendingChannelModel()
            {
                CategoryId = category?.Id, 
                ChannelName = channelName, 
                RequestUserId = Context.User.Id
            };

            _pendingChannels.AddOrUpdate(channelName, channelRequest, (key, value) => channelRequest);

            if (!_pendingChannels.ContainsKey(channelName))
            {
                await ReplyAsync($"An unexpected error occurred when attempting to create the channel request.");
                return;
            }

            var adminMsg = $"{Context.User.Username} requested a text channel with the name {channelName}";
            if (category != null)
            {
                adminMsg += $" in the {category.Name} category";
            }
            await Context.Guild.MessageAdmins(adminMsg);

            await ReplyAsync("Channel request has been created successfully.");
        }

        [Command("approvechannel")]
        [Alias("acceptchannel")]
        [RequireAdmin]
        [RequireBotContext(CmdChannelType.DM)]
        [CmdSummary(nameof(Resource.ApproveChannelSummary), typeof(Resource))]
        [CmdRemarks(nameof(Resource.ApproveChannelRemarks), typeof(Resource))]
        public async Task ApproveChannel(string channelName)
        {
            if (!_pendingChannels.ContainsKey(channelName))
            {
                await ReplyAsync($"{channelName} does not correspond to a currently active channel request.");
                return;
            }

            var success = _pendingChannels.TryRemove(channelName, out var channelRequest);

            if (!success)
            {
                await ReplyAsync($"An unexpected error occurred when attempting to retrieve the request for {channelName}.");
            }
            else
            {
                var guild = Context.Client.GetGuild(GuildContext.GuildId);
                var category = guild.CategoryChannels.Where(x => x.Id == channelRequest.CategoryId).FirstOrDefault();
                var channel = await guild.CreateTextChannelAsync(channelName);

                if (category != null)
                {
                    await channel.ModifyAsync(x => x.CategoryId = category.Id);

                    foreach (var permissionSet in category.PermissionOverwrites)
                    {
                        if (permissionSet.TargetType == PermissionTarget.Role)
                        {
                            var targetRole = guild.GetRole(permissionSet.TargetId);
                            await channel.AddPermissionOverwriteAsync(targetRole, permissionSet.Permissions);
                        }
                        else if (permissionSet.TargetType == PermissionTarget.User)
                        {
                            var targetUser = guild.GetUser(permissionSet.TargetId);
                            await channel.AddPermissionOverwriteAsync(targetUser, permissionSet.Permissions);
                        }
                    }
                }

                var botChannel = guild.GetChannel(GuildContext.CmdChannelId) as ISocketMessageChannel;
                await botChannel.SendMessageAsync($"The channel {channelRequest.ChannelName} has been approved and created successfully.");
            }
        }

        [Command("rejectchannel")]
        [Alias("denychannel")]
        [RequireAdmin]
        [RequireBotContext(CmdChannelType.DM)]
        [CmdSummary(nameof(Resource.RejectChannelSummary), typeof(Resource))]
        [CmdRemarks(nameof(Resource.RejectChannelRemarks), typeof(Resource))]
        public async Task RejectChannel(string channelName, [Remainder]string reason)
        {
            if (!_pendingChannels.ContainsKey(channelName))
            {
                await ReplyAsync($"{channelName} does not correspond to a currently active channel request.");
                return;
            }

            var success = _pendingChannels.TryRemove(channelName, out var channelRequest);

            if (!success)
            {
                await ReplyAsync($"An unexpected error occurred when attempting to retrieve the request for {channelName}.");
            }
            else
            {
                var user = Context.Client.GetUser(channelRequest.RequestUserId);

                if (user != null)
                {
                    var botChannel = Context.Client.GetChannel(GuildContext.CmdChannelId) as ISocketMessageChannel;

                    var embededMsg = new EmbedBuilder();
                    embededMsg.Title = "Reason";
                    embededMsg.Description = reason;

                    await botChannel.SendMessageAsync(
                        $"{user.Mention}'s request for the text channel {channelRequest.ChannelName} was rejected.", 
                        embed: embededMsg.Build());
                }
            }
        }

        [Command("listpendingchannels")]
        [Alias("listchannels")]
        [RequireAdmin]
        [RequireBotContext(CmdChannelType.DM)]
        [CmdSummary(nameof(Resource.ListPendingChannelsSummary), typeof(Resource))]
        [CmdRemarks(nameof(Resource.ListPendingChannelsRemarks), typeof(Resource))]
        public async Task ListPendingChannels()
        {
            if (_pendingChannels.Count == 0)
            {
                await ReplyAsync("There are no currently pending emoji requests.");
            }
            foreach (var request in _pendingChannels)
            {
                var requester = Context.Client.GetUser(request.Value.RequestUserId)?.Username;
                var category = Context.Client.GetGuild(GuildContext.GuildId).CategoryChannels.Where(x => x.Id == request.Value.CategoryId).FirstOrDefault();

                var msg = $"{requester} requested to add a text channel with the name {request.Value.ChannelName}";
                msg += category == null ? "." : $" in the {category.Name} category.";
                msg += $" Request id is '{request.Key}'";

                await ReplyAsync(msg);
            }
        }

        private bool IsValidChannelName(string name)
        {
            Regex channelPattern = new Regex(@"^[a-zA-Z0-9_\\-]*$");
            return channelPattern.IsMatch(name);
        }
    }
}

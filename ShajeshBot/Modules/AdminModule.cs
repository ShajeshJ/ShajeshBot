﻿using Discord.Commands;
using Discord.WebSocket;
using ShajeshBot.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShajeshBot.Modules
{
    [RequireAdmin]
    public class AdminModule : ModuleBase<SocketCommandContext>
    {
        [Command("say")]
        [RequireBotContext(CmdChannelType.DM)]
        [CmdSummary(nameof(Resource.SaySummary), typeof(Resource))]
        [CmdRemarks(nameof(Resource.SayRemarks), typeof(Resource))]
        public async Task MakeBotSay([Remainder] string message)
        {
            var channel = Context.Client.GetChannel(GuildContext.CmdChannelId) as ISocketMessageChannel;

            if (channel != null)
            {
                await channel.SendMessageAsync(message);
            }
            else
            {
                await ReplyAsync("Error retrieving bot channel information.");
            }
        }

        [Command("removemessages")]
        [Alias("rm")]
        [RequireBotContext(CmdChannelType.GuildChannel)]
        [CmdSummary(nameof(Resource.RemoveLastMessagesSummary), typeof(Resource))]
        [CmdRemarks(nameof(Resource.RemoveLastMessagesRemarks), typeof(Resource))]
        public async Task RemoveLastMessages(int numberOfMsgs, bool suppressMessage = false)
        {
            if (Context.IsPrivate || Context.Guild.Id != GuildContext.GuildId)
            {
                await ReplyAsync("The command can only be executed in a Peanuts guild channel");
            }
            else if (numberOfMsgs < 1 || numberOfMsgs > 99)
            {
                await ReplyAsync("Requested number of messages to delete must be between 1 to 99 inclusive.");
            }
            else
            {
                var messages = (await Context.Channel.GetMessagesAsync(numberOfMsgs + 1).ToList()).SelectMany(x => x);
                var tasklist = new List<Task>();

                foreach (var msg in messages)
                {
                    tasklist.Add(msg.DeleteAsync());
                }

                Task.WaitAll(tasklist.ToArray());

                if (!suppressMessage)
                {
                    await ReplyAsync($"{numberOfMsgs} Messages deleted successfully.");
                }
            }
        }
    }
}

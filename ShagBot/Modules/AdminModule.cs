using Discord;
using Discord.Commands;
using ShagBot.Attributes;
using ShagBot.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShagBot.Modules
{
    [RequireAdmin]
    public class AdminModule : ModuleBase<SocketCommandContext>
    {
        [Command("say")]
        [RequireBotContext(CmdChannelType.DM)]
        public async Task MakeBotSay([Remainder] string message)
        {
            var channel = Context.Client.GetChannel(CommandHandler.CmdChannel) as ITextChannel;

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
        [RequireBotContext(CmdChannelType.Any)]
        public async Task RemoveLastMessages(int numberOfMsgs)
        {
            if (Context.IsPrivate || Context.Guild.Id != CommandHandler.GuildId)
            {
                await ReplyAsync("The command can only be executed in a Peanuts guild channel");
            }
            else if (numberOfMsgs < 1 || numberOfMsgs > 10)
            {
                await ReplyAsync("Requested number of messages to delete must be between 1 to 10 inclusive.");
            }
            else
            {
                var messages = await Context.Channel.GetMessagesAsync(numberOfMsgs + 1).Flatten();

                foreach(var msg in messages)
                {
                    await msg.DeleteAsync();
                }

                await ReplyAsync($"{numberOfMsgs} Messages deleted successfully.");
            }
        }
    }
}

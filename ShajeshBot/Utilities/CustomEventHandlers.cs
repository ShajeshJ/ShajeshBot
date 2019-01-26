using Discord;
using Discord.Commands;
using Discord.WebSocket;
using ShajeshBot.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ShajeshBot.Utilities
{
    public static class CustomEventHandlers
    {
        public static string QUOTE_REGEX = "https?:\\/\\/discordapp\\.com\\/channels\\/([0-9]+)\\/([0-9]+)\\/([0-9]+)";

        public static async Task<bool> CreateQuoteMsg(string originalMsg, SocketCommandContext context)
        {
            var regMatches = Regex.Matches(originalMsg, QUOTE_REGEX);
            var processed = new HashSet<string>();
            var quoteCreated = false;
            IDisposable typingState = null;

            foreach (Match match in regMatches)
            {
                if (!processed.Add(match.Value))
                {
                    continue;
                }

                if (!UInt64.TryParse(match.Groups[1].Value, out var guildId)
                    || !UInt64.TryParse(match.Groups[2].Value, out var channelId)
                    || !UInt64.TryParse(match.Groups[3].Value, out var msgId))
                {
                    continue;
                }

                if (guildId != context.Guild.Id)
                {
                    continue;
                }

                var channel = context.Guild.GetChannel(channelId);
                if (channel == null)
                {
                    continue;
                }

                var quotedMsg = await ((ISocketMessageChannel)channel).GetMessageAsync(msgId);
                if (quotedMsg == null)
                {
                    continue;
                }

                if (typingState == null)
                {
                    typingState = context.Channel.EnterTypingState();
                }
                var embed = new EmbedBuilder();

                embed.Author = new EmbedAuthorBuilder()
                {
                    Name = quotedMsg.Author.Username,
                    IconUrl = quotedMsg.Author.GetAvatarUrl()
                };

                embed.Description = "";
                if (!quotedMsg.Content.IsNullOrWhitespace())
                {
                    embed.Description += quotedMsg.Content + "\n";
                }
                if (quotedMsg.Attachments.FirstOrDefault()?.Url != null)
                {
                    embed.Description += quotedMsg.Attachments.FirstOrDefault().Url + "\n";
                }

                if (embed.Description != "")
                {
                    embed.Description += @"\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_";
                }
                embed.Description += $"\n[**View Original**](https://discordapp.com/channels/{guildId}/{channelId}/{msgId})";

                embed.Footer = new EmbedFooterBuilder()
                {
                    Text = $"in #{channel.Name}"
                };
                embed.Timestamp = quotedMsg.EditedTimestamp ?? quotedMsg.Timestamp;
                
                await context.Channel.SendMessageAsync("", embed: embed.Build());

                quoteCreated = true;
                break; //Set it so that only attempts 1 embedded link at a time
            }

            if (typingState != null)
            {
                typingState.Dispose();
            }

            return quoteCreated;
        }
    }
}
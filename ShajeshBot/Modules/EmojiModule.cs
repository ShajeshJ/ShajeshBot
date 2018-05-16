using Discord;
using Discord.Commands;
using Discord.WebSocket;
using ShajeshBot.Attributes;
using ShajeshBot.Extensions;
using ShajeshBot.Models;
using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ShajeshBot.Modules
{
    public class EmojiModule : ModuleBase<SocketCommandContext>
    {
        private static ConcurrentDictionary<string, PendingEmojiModel> _pendingEmojis
        {
            get
            {
                return (ConcurrentDictionary<string, PendingEmojiModel>)ShajeshBot.Memory["_pendingEmojiDictionary"];
            }
            set
            {
                ShajeshBot.Memory.AddOrUpdate("_pendingEmojiDictionary", value, (key, old) => value);
            }
        }

        static EmojiModule()
        {
            _pendingEmojis = new ConcurrentDictionary<string, PendingEmojiModel>();
        }

        [Command("requestemoji")]
        [RequireBotContext(CmdChannelType.BotChannel)]
        [CmdSummary(nameof(Resource.RequestEmojiSummary), typeof(Resource))]
        [CmdRemarks(nameof(Resource.RequestEmojiRemarks), typeof(Resource))]
        public async Task RequestEmoji(string shortcut, string url = null)
        {
            if (string.IsNullOrWhiteSpace(shortcut) || !IsAlphaNumeric(shortcut))
            {
                await ReplyAsync("Emoji shortcut cannot be empty and can only contain alphanumeric and underscore characters");
                return;
            }

            if (Context.Guild.Emotes.Any(x => x.Name == shortcut))
            {
                await ReplyAsync("Emoji with the specified shortcut already exists.");
                return;
            }

            if (url != null && !Regex.Match(url, "https?://.*\\..*").Success)
            {
                await ReplyAsync("Ensure the requested shortcut does not contain white spaces, and that the provided url is valid");
                return;
            }

            if (_pendingEmojis.ContainsKey(shortcut) && _pendingEmojis[shortcut].RequestUserId != Context.User.Id)
            {
                await ReplyAsync("There is already a pending emoji request that is requesting this shortcut");
                return;
            }

            if (url != null && Context.Message.Attachments.Any() || Context.Message.Attachments.Count > 1)
            {
                await Context.Channel.SendMessageAsync("Only one emoji reqeuest can be sent at a time");
                return;
            }
            else
            {
                url = url ?? Context.Message.Attachments.FirstOrDefault().Url;
            }

            var emojiRequest = new PendingEmojiModel()
            {
                Shortcut = shortcut, 
                Url = url,
                RequestUserId = Context.User.Id
            };

            var isAddRequest = true;

            _pendingEmojis.AddOrUpdate(shortcut, emojiRequest, (key, value) =>
            {
                isAddRequest = false;
                return emojiRequest;
            });

            if (!_pendingEmojis.ContainsKey(shortcut))
            {
                await ReplyAsync($"An unexpected error occurred when attempting to create the emoji request.");
                return;
            }

            if (isAddRequest)
            {
                await Context.Guild.MessageAdmins($"{Context.User.Username} requested to add the emoji {url} with shortcut '{shortcut}'.");
                await ReplyAsync("Emoji request has been created successfully.");
            }
            else
            {
                await Context.Guild.MessageAdmins($"{Context.User.Username} requested {url} to be used as the emoji for emoji request with shortcut '{shortcut}'.");
                await ReplyAsync($"Emoji request with shortcut '{shortcut}' was successfully updated with the emoji {url}.");
            }
        }

        [Command("listpendingemojis")]
        [Alias("listemojis")]
        [RequireAdmin]
        [RequireBotContext(CmdChannelType.DM)]
        [CmdSummary(nameof(Resource.ListPendingEmojisSummary), typeof(Resource))]
        [CmdRemarks(nameof(Resource.ListPendingEmojisRemarks), typeof(Resource))]
        public async Task ListPendingEmojis()
        {
            if (_pendingEmojis.Count == 0)
            {
                await ReplyAsync("There are no currently pending emoji requests.");
            }
            foreach (var request in _pendingEmojis)
            {
                var requestUserName = Context.Client.GetUser(request.Value.RequestUserId)?.Username;
                await ReplyAsync($"{requestUserName} requested to add an emoji {request.Value.Url} with shortcut '{request.Value.Shortcut}'. Request id is '{request.Key}'");
            }
        }

        [Command("approveemoji")]
        [Alias("acceptemoji")]
        [RequireAdmin]
        [RequireBotContext(CmdChannelType.DM)]
        [CmdSummary(nameof(Resource.ApproveEmojiSummary), typeof(Resource))]
        [CmdRemarks(nameof(Resource.ApproveEmojiRemarks), typeof(Resource))]
        public async Task ApprovePendingEmoji(string requestId)
        {
            if (!_pendingEmojis.ContainsKey(requestId))
            {
                await ReplyAsync($"{requestId} does not correspond to an active emoji request.");
                return;
            }

            var success = _pendingEmojis.TryRemove(requestId, out var emojiRequest);

            if (!success)
            {
                await ReplyAsync($"An unexpected error occurred when attempting to approve emoji request {requestId}");
                return;
            }
            else
            {
                var client = new WebClient();

                using (var stream = client.OpenRead(emojiRequest.Url))
                {
                    var image = new Image(stream);

                    var emote = await Context.Client.GetGuild(GuildContext.GuildId).CreateEmoteAsync(emojiRequest.Shortcut, image);

                    var botChannel = Context.Client.GetChannel(GuildContext.CmdChannelId) as ISocketMessageChannel;

                    var msg = await botChannel.SendMessageAsync($"Emoji with shortcut '{emote.Name}' added successfully.");
                    await msg?.AddReactionAsync(emote);
                }
            }
        }

        [Command("rejectemoji")]
        [Alias("denyemoji")]
        [RequireAdmin]
        [RequireBotContext(CmdChannelType.DM)]
        [CmdSummary(nameof(Resource.RejectEmojiSummary), typeof(Resource))]
        [CmdRemarks(nameof(Resource.RejectEmojiRemarks), typeof(Resource))]
        public async Task RejectPendingEmoji(string requestId, [Remainder]string reason)
        {
            if (!_pendingEmojis.ContainsKey(requestId))
            {
                await ReplyAsync($"{requestId} does not correspond to an active emoji request number.");
                return;
            }

            var success = _pendingEmojis.TryRemove(requestId, out var emojiRequest);

            if (!success)
            {
                await ReplyAsync($"An unexpected error occurred when attempting to reject emoji request {requestId}");
                return;
            }
            else
            {
                var user = Context.Client.GetUser(emojiRequest.RequestUserId);

                if (user != null)
                {
                    var botChannel = Context.Client.GetChannel(GuildContext.CmdChannelId) as ISocketMessageChannel;

                    var embededMsg = new EmbedBuilder();
                    embededMsg.Title = "Reason";
                    embededMsg.Description = reason;

                    await botChannel.SendMessageAsync(
                        $"{user.Mention} your emoji '{emojiRequest.Url}' with shortcut '{emojiRequest.Shortcut}' was rejected.", 
                        embed: embededMsg.Build());
                }
            }
        }

        private bool IsAlphaNumeric(string str)
        {
            Regex alphaNumericPattern = new Regex(@"^[a-zA-Z0-9_]*$");
            return alphaNumericPattern.IsMatch(str);
        }
    }
}

using Discord;
using Discord.Commands;
using ShagBot.Attributes;
using ShagBot.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ShagBot.Modules
{
    public class EmojiModule : ModuleBase<SocketCommandContext>
    {
        private Random _rand;
        private static ConcurrentDictionary<string, PendingEmojiModel> _pendingEmojis
        {
            get
            {
                return (ConcurrentDictionary<string, PendingEmojiModel>)ShagBot.Memory["_pendingEmojiDictionary"];
            }
            set
            {
                ShagBot.Memory.AddOrUpdate("_pendingEmojiDictionary", value, (key, old) => value);
            }
        }

        static EmojiModule()
        {
            _pendingEmojis = new ConcurrentDictionary<string, PendingEmojiModel>();
        }

        public EmojiModule()
        {
            _rand = new Random();
        }

        [Command("requestemoji")]
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
                await ReplyAsync($"An error occurred when attempting to create the emoji request.");
                return;
            }

            if (isAddRequest)
            {
                await MessageAdmins($"{Context.User.Username} requested to add the emoji {url} with shortcut '{shortcut}'.");
                await ReplyAsync("Emoji request has been created successfully.");
            }
            else
            {
                await MessageAdmins($"{Context.User.Username} requested {url} to be used as the emoji for emoji request with shortcut '{shortcut}'.");
                await ReplyAsync($"Emoji request with shortcut '{shortcut}' was successfully updated with the emoji {url}.");
            }
        }

        [Command("listpendingemojis")]
        [Alias("listemojis")]
        [RequireAdmin]
        public async Task ListPendingEmojis()
        {
            if (_pendingEmojis.Count == 0)
            {
                await Context.User.SendMessageAsync("There are no currently pending emoji requests.");
            }
            foreach (var request in _pendingEmojis)
            {
                var requestUserName = Context.Guild.GetUser(request.Value.RequestUserId)?.Username;
                await Context.User.SendMessageAsync(
                    $"{requestUserName} requested to add an emoji {request.Value.Url} with shortcut '{request.Value.Shortcut}'. Request id is '{request.Key}'");
            }
        }

        [Command("approveemoji")]
        [Alias("acceptemoji")]
        [RequireAdmin]
        public async Task ApprovePendingEmoji(string requestId)
        {
            if (!_pendingEmojis.ContainsKey(requestId))
            {
                await ReplyAsync($"{requestId} does not correspond to an active emoji request number.");
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

                try
                {
                    using (var stream = client.OpenRead(emojiRequest.Url))
                    {
                        var image = new Image(stream);

                        var emote = await Context.Guild.CreateEmoteAsync(emojiRequest.Shortcut, image);

                        var msg = await ReplyAsync($"Emoji with shortcut '{emote.Name}' added successfully.");
                        await msg.AddReactionAsync(emote);
                    }
                }
                catch (Exception ex)
                {
                    await ReplyAsync($"An error occurred when trying to process the image from url '{emojiRequest.Url}'");
                    await MessageAdmins(new string[] {
                        $"Exception thrown by request with shortcut {emojiRequest.Shortcut} with url {emojiRequest.Url}.",
                        $"Exception: {ex.Message}"
                    });
                }
            }
        }

        [Command("rejectemoji")]
        [Alias("denyemoji")]
        [RequireAdmin]
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
                var user = Context.Guild.GetUser(emojiRequest.RequestUserId);

                if (user != null)
                {
                    await Context.Channel.SendMessageAsync(
                        $"{user.Mention} your emoji '{emojiRequest.Url}' with shortcut '{emojiRequest.Shortcut}' was rejected. Reason: {reason}");
                }
            }
        }

        private bool IsAlphaNumeric(string str)
        {
            Regex alphaNumericPattern = new Regex(@"^[a-zA-Z0-9_]*$");
            return alphaNumericPattern.IsMatch(str);
        }

        private async Task MessageAdmins(string msg)
        {
            await MessageAdmins(new string[] { msg });
        }

        private async Task MessageAdmins(IEnumerable<string> msgs)
        {
            var adminRoleId = CommandHandler.AdminRoleId;
            var adminUsers = Context.Guild.Users.Where(x => x.Roles.Any(y => y.Id == adminRoleId));

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

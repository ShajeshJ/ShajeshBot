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
        private ConcurrentDictionary<int, PendingEmojiModel> _pendingEmojis
        {
            get
            {
                return (ConcurrentDictionary<int, PendingEmojiModel>)ShagBot.Memory["_pendingEmojiDictionary"];
            }
            set
            {
                ShagBot.Memory.AddOrUpdate("_pendingEmojiDictionary", value, (key, old) => value);
            }
        }

        #region Summary/Remarks constants

        private const string _requestEmojiSummary = "Use this command to request an emoji to be added to the server. Emojis can either be provided by direct upload, or a direct url to an image.";
        private const string _requeustEmojiRemarks = "1) You can only use this command with either a url of the image, or a single image attachment (cannot use both).\r\n\r\n2) To use the command with an image upload, first upload the image as an attachment to the bot channel. When it asks you type in an optional message with the attachment, enter the command.\r\n\r\n3) To use the command with a url of the image, you must ensure the url is a direct url to the image.";

        #endregion

        public EmojiModule()
        {
            _pendingEmojis = new ConcurrentDictionary<int, PendingEmojiModel>();
            _rand = new Random();
        }

        [Command("RequestEmoji")]
        [Summary(_requestEmojiSummary)]
        [Remarks(_requeustEmojiRemarks)]
        public async Task RequestEmoji(string shortcut, string url = null)
        {
            if (string.IsNullOrWhiteSpace(shortcut) || shortcut.Contains(' '))
            {
                await ReplyAsync("Emoji shortcut cannot be empty or contain white spaces");
                return;
            }

            if (Context.Guild.Emotes.Any(x => x.Name == shortcut))
            {
                await ReplyAsync("Emoji with the specified shortcut already exists.");
                return;
            }

            if (url != null && !Regex.Match(url, "https?://.*\\..*").Success)
            {
                await Context.Channel.SendMessageAsync("Ensure the requested shortcut does not contain white spaces, and that the provided url is valid");
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

            int key = -1;
            var attempts = 0;

            do
            {
                key = _rand.Next(1, 1000000);
                attempts++;
            } while (attempts < 10 && !_pendingEmojis.TryAdd(key, emojiRequest));

            if(!_pendingEmojis.ContainsKey(key))
            {
                await ReplyAsync($"An unexpected error occurred when attempting to create your emoji request. Please try again.");
                return;
            }

            var adminRoleId = CommandHandler.AdminRoleId;
            var adminUsers = Context.Guild.Users.Where(x => x.Roles.Any(y => y.Id == adminRoleId));

            foreach (var admin in adminUsers)
            {
                await admin.SendMessageAsync(
                    $"{Context.User.Username} requested to add an emoji from the url '{url}' with shortcut '{shortcut}'. Request number is {key}");
            }

            await Context.Channel.SendMessageAsync("Emoji request has been created successfully.");
        }

        [Command("ListPendingEmojis")]
        [RequireAdmin]
        public async Task ListPendingEmojis()
        {
            foreach (var request in _pendingEmojis)
            {
                var requestUserName = Context.Guild.GetUser(request.Value.RequestUserId)?.Username;
                await Context.User.SendMessageAsync(
                    $"{requestUserName} requested to add an emoji from the url '{request.Value.Url}' with shortcut '{request.Value.Shortcut}'. Request number is {request.Key}");
            }
        }

        [Command("ApproveEmoji")]
        [RequireAdmin]
        public async Task ApprovePendingEmoji(int requestId)
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
                }
            }
        }

        [Command("RejectEmoji")]
        [RequireAdmin]
        public async Task RejectPendingEmoji(int requestId, [Remainder]string reason)
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
    }
}

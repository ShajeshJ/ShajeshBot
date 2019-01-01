using Discord.Commands;
using Discord.Net;
using ShajeshBot.Attributes;
using ShajeshBot.Extensions;
using System.Linq;
using System.Threading.Tasks;

namespace ShajeshBot.Modules
{
    public class UserModule : ModuleBase<SocketCommandContext>
    {
        [Command("changename")]
        [Alias("name", "nickname")]
        [RequireBotContext(CmdChannelType.BotChannel)]
        [CmdSummary(nameof(Resource.ChangeNameSummary), typeof(Resource))]
        [CmdRemarks(nameof(Resource.ChangeNameRemarks), typeof(Resource))]
        public async Task ChangeNickname([Remainder]string nickname = "")
        {
            var user = Context.Guild.Users.FirstOrDefault(x => x.Id == Context.User.Id);
            var oldNickname = user.Nickname;

            if (string.IsNullOrWhiteSpace(oldNickname))
            {
                oldNickname = user.Username;
            }

            if (string.IsNullOrWhiteSpace(nickname))
            {
                nickname = "";
            }
            else
            {
                nickname = nickname.Trim();

                if (nickname == user.Username)
                {
                    nickname = "";
                }
                else
                {
                    nickname += $" [{user.Username}]";
                }
            }

            if (nickname.Length > 32)
            {
                await Context.Channel.SendMessageAsync($"The modified name, '{nickname}' is too long. It must be less than or equal to 32 characters long.");
                return;
            }

            try
            {
                await user.ModifyAsync(x => x.Nickname = nickname);
            }
            catch (HttpException httpEx)
            {
                if (httpEx.HttpCode == System.Net.HttpStatusCode.Forbidden)
                {
                    await Context.Channel.SendMessageAsync("You have higher privileges than me. Please change your own nickname or contact an admin.");
                    return;
                }

                await Context.Channel.SendMessageAsync($"Had issues with trying to change your nickname to '{nickname}'. Please contact an admin.");
                await Context.Guild.MessageAdmins(new string[]
                {
                    "Change name command threw the following exception",
                    httpEx.Message,
                    httpEx.StackTrace
                });

                return;
            }

            if (nickname == "")
            {
                await Context.Channel.SendMessageAsync($"Name reset from {oldNickname} successfully.");
            }
            else
            {
                await Context.Channel.SendMessageAsync($"Name changed from {oldNickname} to {nickname} successfully.");
            }
        }
    }
}

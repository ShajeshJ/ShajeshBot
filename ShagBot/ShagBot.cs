using Discord;
using Discord.WebSocket;
using ShagBot.Extensions;
using System.Collections.Concurrent;
using System.Configuration;
using System.Threading.Tasks;

namespace ShagBot
{
    public class ShagBot
    {
        private DiscordSocketClient _client;
        private CommandHandler _handler;
        private string _patchnotes;

        public static ConcurrentDictionary<string, object> Memory;

        public ShagBot(string patchNotes = null)
        {
            _patchnotes = patchNotes;
            Memory = new ConcurrentDictionary<string, object>();
            _client = new DiscordSocketClient();
        }

        public async Task StartAsync()
        {
            _handler = new CommandHandler(_client);
            await _client.LoginAsync(TokenType.Bot, ConfigurationManager.AppSettings["Bot_Token"]);
            await _client.SetGameAsync(CommandHandler.CmdPrefix + "help");
            await _client.StartAsync();
            _client.GuildAvailable += async (guild) =>
            {
                if (_patchnotes != null)
                {
                    var botChannel = guild.GetChannel(ConfigurationManager.AppSettings["Bot_Channel"].ToUInt64()) as SocketTextChannel;
                    
                    if (botChannel == null)
                    {
                        return;
                    }

                    var embed = new EmbedBuilder();
                    embed.WithTitle("Patch Notes");
                    embed.WithDescription(_patchnotes);
                    var msg = await botChannel.SendMessageAsync(botChannel.Mention, embed: embed.Build());
                    await msg.PinAsync();
                }
            };

            await Task.Delay(-1);
        }
    }
}

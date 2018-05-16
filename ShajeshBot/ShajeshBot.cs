using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace ShajeshBot
{
    public class ShajeshBot
    {
        private DiscordSocketClient _client;
        private CommandHandler _handler;
        private string _patchnotes;

        public static ConcurrentDictionary<string, object> Memory;

        public ShajeshBot(string patchNotes = null)
        {
            _patchnotes = patchNotes;
            Memory = new ConcurrentDictionary<string, object>();
            _client = new DiscordSocketClient();
        }

        public async Task StartAsync()
        {
            _handler = new CommandHandler(_client);
            await _client.LoginAsync(TokenType.Bot, Environment.GetEnvironmentVariable("SHAJESHBOT_TOKEN"));
            await _client.SetGameAsync(CommandHandler.CmdPrefix + "help");
            await _client.StartAsync();
            _client.GuildAvailable += async (guild) =>
            {
                if (_patchnotes != null)
                {
                    var botChannel = guild.GetChannel(GuildContext.CmdChannelId) as SocketTextChannel;
                    
                    if (botChannel == null)
                    {
                        return;
                    }

                    var embed = new EmbedBuilder();
                    embed.WithTitle("Patch Notes");
                    embed.WithDescription(_patchnotes);
                    var msg = await botChannel.SendMessageAsync("", embed: embed.Build());
                    await msg.PinAsync();

                    _patchnotes = null;
                }
            };

            await Task.Delay(-1);
        }
    }
}

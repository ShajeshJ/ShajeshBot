using Discord;
using Discord.WebSocket;
using System.Collections.Concurrent;
using System.Configuration;
using System.Threading.Tasks;

namespace ShagBot
{
    public class ShagBot
    {
        private DiscordSocketClient _client;
        private CommandHandler _handler;

        public static ConcurrentDictionary<string, object> Memory;

        public ShagBot()
        {
            Memory = new ConcurrentDictionary<string, object>();
            _client = new DiscordSocketClient();
        }

        public async Task StartAsync()
        {
            _handler = new CommandHandler(_client);
            await _client.LoginAsync(TokenType.Bot, ConfigurationManager.AppSettings["Bot_Token"]);
            await _client.SetGameAsync(CommandHandler.CmdPrefix + "help");
            await _client.StartAsync();
            await Task.Delay(-1);
        }
    }
}

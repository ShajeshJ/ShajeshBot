using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ShagBot
{
    public class CommandHandler
    {
        private DiscordSocketClient _client;
        private CommandService _cmdService;
        public static readonly char CmdPrefix = '!';
        private readonly ulong _cmdChannel = Convert.ToUInt64(ConfigurationManager.AppSettings["Bot_Channel"]);
        private readonly ulong[] _cmdRoleIds = ConfigurationManager.AppSettings["Bot_AllowedRoles"]
                                                .Split('|').Select(x => Convert.ToUInt64(x.Trim(' '))).ToArray();

        public static IEnumerable<CommandInfo> Commands { get; private set; }
        public static readonly ulong AdminRoleId = Convert.ToUInt64(ConfigurationManager.AppSettings["Admin_Role"]);

        public CommandHandler(DiscordSocketClient client)
        {
            _client = client;
            _cmdService = new CommandService();
            _cmdService.AddModulesAsync(Assembly.GetEntryAssembly());
            Commands = _cmdService.Commands;

            _client.MessageReceived += HandleCommandAsync;
        }

        private async Task HandleCommandAsync(SocketMessage s)
        {
            var msg = s as SocketUserMessage;

            if (msg == null)
            {
                return;
            }

            var context = new SocketCommandContext(_client, msg);

            if (context.Channel.Id != _cmdChannel || context.User.IsBot)
            {
                return;
            }

            if ((context.User as IGuildUser)?.RoleIds?.Intersect(_cmdRoleIds).Any() != true)
            {
                await context.Channel.SendMessageAsync("Only Crew members can use ShagBot");
                return;
            }

            var argPos = 0;

            if (msg.HasCharPrefix(CmdPrefix, ref argPos))
            {
                var result = await _cmdService.ExecuteAsync(context, argPos);

                if (!result.IsSuccess && result.Error != CommandError.UnknownCommand)
                {
                    await context.Channel.SendMessageAsync(result.ErrorReason);
                }
            }
        }
    }
}

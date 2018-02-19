using Discord;
using Discord.Commands;
using Discord.WebSocket;
using ShagBot.Extensions;
using ShagBot.Utilities;
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
        public static readonly string IgnoreErrorOutput = "<Do Not Show Error Output>";
        public static IEnumerable<CommandInfo> Commands { get; private set; }

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

            if (context.User.IsBot)
            {
                return;
            }

            var argPos = 0;

            if (msg.HasCharPrefix(CmdPrefix, ref argPos))
            {
                var result = await _cmdService.ExecuteAsync(context, argPos);

                var noError = result.IsSuccess
                            || result.Error == CommandError.UnknownCommand
                            || (result.Error == CommandError.UnmetPrecondition && result.ErrorReason == IgnoreErrorOutput);

                //No error message to show in one of the three cases
                // 1) The command executed successfully
                // 2) The requested command is an unknown command (no need to output errors if users trigger command with the prefix)
                // 3) A precondition failed but indicates that no error should be shown using a specific error reason string

                if (!noError)
                {
                    if (result.Error == CommandError.Exception)
                    {
                        var utilities = new DiscordUtilities(context);
                        await utilities.MessageAdmins($"Error caused by command: '{msg.Content}'\r\nException: '{result.ErrorReason}'");
                        await context.Channel.SendMessageAsync("An unexpected error occurred.");
                    }
                    else
                    {
                        await context.Channel.SendMessageAsync(result.ErrorReason);
                    }
                }
            }
        }
    }
}

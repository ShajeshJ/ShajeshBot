using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Util.Store;
using Microsoft.Extensions.DependencyInjection;
using ShagBot.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace ShagBot
{
    public class CommandHandler
    {
        private DiscordSocketClient _client;
        private CommandService _cmdService;
        private IServiceProvider _services;

        public static readonly char CmdPrefix = '!';
        public static readonly string IgnoreErrorOutput = "<Do Not Show Error Output>";
        public static IEnumerable<CommandInfo> Commands { get; private set; }

        private static string[] _sheetScope = { SheetsService.Scope.Spreadsheets };
        private static string _sheetAppName = "ShajeshBot";

        public CommandHandler(DiscordSocketClient client)
        {
            _client = client;

            BuildServices();

            _cmdService = new CommandService();
            _cmdService.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
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
                var typingState = context.Channel.EnterTypingState();
                var result = await _cmdService.ExecuteAsync(context, argPos, _services);

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
                        await context.Guild.MessageAdmins($"Error caused by command: '{msg.Content}'\r\nException: '{result.ErrorReason}'");
                        await context.Channel.SendMessageAsync("An unexpected error occurred.");
                    }
                    else
                    {
                        await context.Channel.SendMessageAsync(result.ErrorReason);
                    }
                }
                typingState.Dispose();
            }
        }

        private void BuildServices()
        {
            UserCredential credentials;

            using (var clSecret = new FileStream("client_secret.json", FileMode.Open, FileAccess.Read))
            {
                var credPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                credPath = Path.Combine(credPath, ".google_sheets_creds/shajeshbot_creds.json");

                credentials = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(clSecret).Secrets,
                    _sheetScope,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
            }

            var sheetService = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credentials,
                ApplicationName = _sheetAppName,
            });

            _services = new ServiceCollection()
                            .AddSingleton(sheetService)
                            .BuildServiceProvider();
        }
    }
}

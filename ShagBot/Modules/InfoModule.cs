using Discord;
using Discord.Commands;
using ShagBot.Attributes;
using ShagBot.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShagBot.Modules
{
    public class InfoModule : ModuleBase<SocketCommandContext>
    {
        private IEnumerable<CommandInfo> _commands;

        public InfoModule()
        {
            _commands = CommandHandler.Commands;
        }

        [Command("help")]
        [RequireBotContext]
        [CmdSummary(nameof(Resource.HelpSummary), typeof(Resource))]
        public async Task GetHelp(string commandName = null)
        {
            if (commandName == null)
            {
                await DisplayGeneralHelp();
            }
            else
            {
                await DisplaySpecificHelp(commandName);
            }
        }

        private async Task DisplaySpecificHelp(string cmdName)
        {
            var cmd = _commands.FirstOrDefault(x => x.Aliases.Contains(cmdName));

            if (cmd == null)
            {
                await ReplyAsync($"!{cmdName} is not an existing command");
                return;
            }

            if (cmd.Preconditions.Any(x => x.GetType() == typeof(RequireAdminAttribute)))
            {
                await ReplyAsync($"!{cmdName} is an admin command. Help details cannot be requested for this command.");
                return;
            }

            var msg = new EmbedBuilder();

            msg.WithTitle(CommandHandler.CmdPrefix + cmd.Name
                            + " " +
                            cmd.Parameters.Aggregate("",
                                            (output, param) =>
                                            {
                                                if (param.IsOptional)
                                                    return output + "[" + param.Name + "] ";
                                                else
                                                    return output + "<" + param.Name + "> ";
                                            },
                                            output => output.TrimEnd(' ')));

            msg.WithDescription(string.IsNullOrWhiteSpace(cmd.Summary) ? "<No Description Available>" : cmd.Summary);

            if (cmd.Aliases.Count > 0)
            {
                msg.AddField("Aliases: ",
                string.Join("\r\n", cmd.Aliases.Select(x => CommandHandler.CmdPrefix + x)),
                true);
            }

            if (!cmd.Remarks.IsNullOrWhitespace())
            {
                msg.AddField("Notes: ", cmd.Remarks ?? "", true);
            }
            
            msg.WithFooter("Parameters surrounded by square brackets are optional.");

            await ReplyAsync("", false, msg.Build());
        }

        private async Task DisplayGeneralHelp()
        {
            var msg = new EmbedBuilder();

            var cmdList = "";

            foreach (var cmd in _commands)
            {
                if (!cmd.Preconditions.Any(x => x.GetType() == typeof(RequireAdminAttribute)))
                {
                    cmdList += CommandHandler.CmdPrefix + cmd.Name
                            + " " +
                            cmd.Parameters.Aggregate("",
                                            (output, param) =>
                                            {
                                                if (param.IsOptional)
                                                    return output + "[" + param.Name + "] ";
                                                else
                                                    return output + "<" + param.Name + "> ";
                                            },
                                            output => output.TrimEnd(' '))
                            + Environment.NewLine + Environment.NewLine;
                }
            }

            msg.AddField("Commands", cmdList, true);
            msg.WithFooter("Parameters surrounded by square brackets are optional.");

            await ReplyAsync("", false, msg.Build());
        }
    }
}

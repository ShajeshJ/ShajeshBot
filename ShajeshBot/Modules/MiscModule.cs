using Discord;
using Discord.Commands;
using Discord.Rest;
using ShajeshBot.Attributes;
using ShajeshBot.Enums;
using ShajeshBot.Extensions;
using ShajeshBot.Utilities;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ShajeshBot.Modules
{
    public class MiscModule : ModuleBase<SocketCommandContext>
    {
        private static SudokuMemory _sudoku
        {
            get
            {
                return (SudokuMemory)ShajeshBot.Memory["_sudokuRunnerMemory"];
            }
            set
            {
                ShajeshBot.Memory.AddOrUpdate("_sudokuRunnerMemory", value, (key, old) => value);
            }
        }

        static MiscModule()
        {
            _sudoku = new SudokuMemory()
            {
                Runner = new SudokuRunner(),
                Msg = null
            };
        }

        #region Sudoku Commands

        [Command("sudoku new")]
        [Alias("s new")]
        [RequireBotContext(CmdChannelType.BotChannel)]
        [CmdSummary(nameof(Resource.SudokuNewSummary), typeof(Resource))]
        [CmdRemarks(nameof(Resource.SudokuNewRemarks), typeof(Resource))]
        public async Task StartSudokuGame()
        {
            if (_sudoku.Msg != null)
            {
                await SendSudokuError(SudokuResult.GameExists);
                return;
            }

            _sudoku.Runner.Create();

            await Context.Message.DeleteAsync();
            await SendSudokuImg();
        }

        [Command("sudoku show")]
        [Alias("s show")]
        [RequireBotContext(CmdChannelType.BotChannel)]
        [CmdSummary(nameof(Resource.SudokuShowSummary), typeof(Resource))]
        [CmdRemarks(nameof(Resource.SudokuShowRemarks), typeof(Resource))]
        public async Task ShowSudokuPuzzle()
        {
            if (_sudoku.Msg == null)
            {
                await SendSudokuError(SudokuResult.GameNotExists);
                return;
            }

            await Context.Message.DeleteAsync();
            await SendSudokuImg();
        }

        [Command("sudoku end")]
        [Alias("s end")]
        [RequireBotContext(CmdChannelType.BotChannel)]
        [CmdSummary(nameof(Resource.SudokuEndSummary), typeof(Resource))]
        [CmdRemarks(nameof(Resource.SudokuEndRemarks), typeof(Resource))]
        public async Task EndSudokuGame()
        {
            if (_sudoku.Msg == null)
            {
                await SendSudokuError(SudokuResult.GameNotExists);
                return;
            }

            _sudoku.Msg = null;
            
            await ReplyAsync("Sudoku game ended successfully.");
        }

        [Command("sudoku reset")]
        [Alias("s reset")]
        [RequireBotContext(CmdChannelType.BotChannel)]
        [CmdSummary(nameof(Resource.SudokuResetSummary), typeof(Resource))]
        [CmdRemarks(nameof(Resource.SudokuResetRemarks), typeof(Resource))]
        public async Task ResetSudokuGame()
        {
            if (_sudoku.Msg == null)
            {
                await SendSudokuError(SudokuResult.GameNotExists);
                return;
            }

            _sudoku.Runner.Reset();
            
            await SendSudokuImg();
        }

        [Command("sudoku set")]
        [Alias("s set")]
        [RequireBotContext(CmdChannelType.BotChannel)]
        [CmdSummary(nameof(Resource.SudokuSetSummary), typeof(Resource))]
        [CmdRemarks(nameof(Resource.SudokuSetRemarks), typeof(Resource))]
        public async Task SetSudokuPiece(string coordinates, int value)
        {
            if (_sudoku.Msg == null)
            {
                await SendSudokuError(SudokuResult.GameNotExists);
                return;
            }

            if (coordinates.Length != 2)
            {
                await SendSudokuError(SudokuResult.BadCoords);
                return;
            }

            var row = coordinates.ToLower()[0] - 'a';

            if (!Int32.TryParse(coordinates[1].ToString(), out var col))
            {
                await SendSudokuError(SudokuResult.BadCoords);
                return;
            }

            col--;

            var result = _sudoku.Runner.InsertValue(row, col, value);

            if (result == SudokuResult.Success)
            {
                await SendSudokuImg();
            }
            else if (result == SudokuResult.Completed)
            {
                await Context.Message.DeleteAsync();
                await SendSudokuImg();
                _sudoku.Msg = null;
                await ReplyAsync($"You actually finished the sudoku {Context.User.Mention}. I guess miracles do happen.");
            }
            else
            {
                await SendSudokuError(result);
            }
        }

        [Command("sudoku del")]
        [Alias("s del")]
        [RequireBotContext(CmdChannelType.BotChannel)]
        [CmdSummary(nameof(Resource.SudokuDelSummary), typeof(Resource))]
        [CmdRemarks(nameof(Resource.SudokuDelRemarks), typeof(Resource))]
        public async Task DeleteSudokuPiece(string coordinates)
        {
            if (_sudoku.Msg == null)
            {
                await SendSudokuError(SudokuResult.GameNotExists);
                return;
            }

            if (coordinates.Length != 2)
            {
                await SendSudokuError(SudokuResult.BadCoords);
                return;
            }

            var row = coordinates.ToLower()[0] - 'a';

            if (!Int32.TryParse(coordinates[1].ToString(), out var col))
            {
                await SendSudokuError(SudokuResult.BadCoords);
                return;
            }

            col--;

            var result = _sudoku.Runner.RemoveValue(row, col);

            if (result == SudokuResult.Success)
            {
                await SendSudokuImg();
            }
            else
            {
                await SendSudokuError(result);
            }
        }

        private async Task SendSudokuImg()
        {
            var puzzleDrawing = _sudoku.Runner.DrawGame();

            Task delSudokuTask;
            Task<RestUserMessage> makeSudokuTask;

            if (_sudoku.Msg != null)
                delSudokuTask = _sudoku.Msg.DeleteAsync();
            else
                delSudokuTask = Task.FromResult(0);

            var imgIdx = 0;
            while (File.Exists($"sudoku{imgIdx}.png"))
            {
                imgIdx++;
            }
            puzzleDrawing.Save($"sudoku{imgIdx}.png");

            makeSudokuTask = Context.Channel.SendFileAsync($"sudoku{imgIdx}.png");

            await Task.WhenAll(delSudokuTask, makeSudokuTask);

            _sudoku.Msg = makeSudokuTask.Result;

            File.Delete($"sudoku{imgIdx}.png");
        }

        private async Task SendSudokuError(SudokuResult error)
        {
            if (error == SudokuResult.BadCoords)
            {
                await ReplyAsync("Invalid set of board coordinates. To specify a cell, enter the row letter followed by the column number. Ex: \"B7\"");
            }
            else if (error == SudokuResult.GameNotExists)
            {
                await ReplyAsync("There is no active sudoku game. Use \"!sudoku new\" to create a new sudoku game.");
            }
            else if (error == SudokuResult.GameExists)
            {
                await ReplyAsync("There is a currently active sudoku game. Use \"!sudoku show\" to re-display the board, or \"!sudoku end\" to end the game.");
            }
            else if (error == SudokuResult.BadVal)
            {
                await ReplyAsync("Cannot insert the specified value. Value must be in the range 1 to 9 inclusively.");
            }
            else if (error == SudokuResult.Conflict)
            {
                await ReplyAsync("Cannot insert value at the specified location as it conflicts with another cell in the same row, column, or box.");
            }
            else if (error == SudokuResult.Occupied)
            {
                await ReplyAsync("The specified location already has a value. You can remove the value using \"!sudoku del\"");
            }
            else if (error == SudokuResult.Unoccupied)
            {
                await ReplyAsync("The specified location does not have a value. You can add a value using \"!sudoku set\"");
            }
            else if (error == SudokuResult.Immutable)
            {
                await ReplyAsync("The specified location cannot be modified as it is part of the original puzzle.");
            }
        }

        #endregion

        [Command("dice")]
        [Alias("roll")]
        [RequireBotContext(CmdChannelType.BotChannel)]
        [CmdSummary(nameof(Resource.DiceSummary), typeof(Resource))]
        [CmdRemarks(nameof(Resource.DiceRemarks), typeof(Resource))]
        public async Task GenRandomNumber(int max, int min = 1, int number_of_roles = 1)
        {
            if (min > max)
            {
                await ReplyAsync("Min value must be less than or equal to max value.");
                return;
            }

            if (number_of_roles < 1)
            {
                await ReplyAsync("Cannot specify < 1 for the number of roles to make.");
                return;
            }

            var rand = new Random();
            var output = "";

            for (int i = 0; i < number_of_roles; i++)
            {
                var value = rand.Next(min, max + 1);
                output += $"Rolling ({min} - {max}): {value}\n";
            }

            await ReplyAsync($"```{output.TrimEnd('\n')}```");
        }

        [Command("dice")]
        [Alias("roll")]
        [RequireBotContext(CmdChannelType.BotChannel)]
        [CmdSummary(nameof(Resource.DiceSummary), typeof(Resource))]
        [CmdRemarks(nameof(Resource.DiceSummary), typeof(Resource))]
        public async Task GenRandomNumber([Remainder]string dice_roll_notation)
        {
            // Try to match "{num}d{num}+{num}", such as 3d10+2
            if (!Regex.IsMatch(dice_roll_notation, "^ *\\d* *d *\\d+ *((\\+|-) *\\d+ *)?$", RegexOptions.IgnoreCase))
            {
                await ReplyAsync($"'{dice_roll_notation}' does not follow valid dice notation. Dice notation takes a form such as `<# of dice>d<# of dice sides>(+/-)<flat value>`");
                return;
            }

            dice_roll_notation = dice_roll_notation.Replace(" ", "");

            var notationParts = dice_roll_notation.Split('d');
            var numOfDice = notationParts[0].TryParseInt(1);
            int diceSize;
            int flatIncr;

            if (notationParts[1].Contains("+"))
            {
                notationParts = notationParts[1].Split('+');
                diceSize = notationParts[0].TryParseInt();
                flatIncr = notationParts[1].TryParseInt();
            }
            else if (notationParts[1].Contains("-"))
            {
                notationParts = notationParts[1].Split('-');
                diceSize = notationParts[0].TryParseInt();
                flatIncr = ("-" + notationParts[1]).TryParseInt();
            }
            else
            {
                diceSize = notationParts[1].TryParseInt();
                flatIncr = 0;
            }

            if (numOfDice < 1)
            {
                await ReplyAsync($"Number of dice to roll cannot be less than 1.");
                return;
            }

            if (diceSize < 2)
            {
                await ReplyAsync($"The number of sides on the dice cannot be less than 2.");
            }

            var rand = new Random();
            string output = "";
            var minLen = 20 + diceSize.ToString().Length*2 + flatIncr.ToString().Length;

            for (int i = 0; i < numOfDice; i++)
            {
                var value = rand.Next(1, diceSize + 1);

                if (flatIncr == 0)
                {
                    output += $"Rolling (1 - {diceSize}): {value}\n";
                }
                else if (flatIncr > 0)
                {
                    var line = $"Rolling (1 - {diceSize}): {value} + {flatIncr} ";
                    while (line.Length < minLen)
                        line += ' ';

                    output += line + $"= {value + flatIncr}\n";
                }
                else
                {
                    var line = $"Rolling (1 - {diceSize}): {value} - {flatIncr * -1} ";
                    while (line.Length < minLen)
                        line += ' ';

                    output += line + $"= {value + flatIncr}\n";
                }
            }
            output = output.TrimEnd('\n');

            await ReplyAsync($"```{output}```");
        }
    }

    public class SudokuMemory
    {
        public SudokuRunner Runner;
        public IMessage Msg;
    }
}

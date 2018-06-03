using Discord;
using Discord.Commands;
using Discord.Rest;
using ShajeshBot.Attributes;
using ShajeshBot.Enums;
using ShajeshBot.Utilities;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ShajeshBot.Modules
{
    public class MiscModule : ModuleBase<SocketCommandContext>
    {
        private static SudokuRunner _sudoku;
        private static IMessage _sudokuMsg;

        static MiscModule()
        {
            _sudoku = new SudokuRunner();
            _sudokuMsg = null;
        }

        #region Sudoku Commands

        [Command("sudoku new")]
        [Alias("s new")]
        [RequireBotContext(CmdChannelType.BotChannel)]
        [CmdSummary(nameof(Resource.SudokuNewSummary), typeof(Resource))]
        [CmdRemarks(nameof(Resource.SudokuNewRemarks), typeof(Resource))]
        public async Task StartSudokuGame()
        {
            if (_sudokuMsg != null)
            {
                await SendSudokuError(SudokuResult.GameExists);
                return;
            }

            _sudoku.Create();

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
            if (_sudokuMsg == null)
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
            if (_sudokuMsg == null)
            {
                await SendSudokuError(SudokuResult.GameNotExists);
                return;
            }

            _sudokuMsg = null;
            
            await ReplyAsync("Sudoku game ended successfully.");
        }

        [Command("sudoku reset")]
        [Alias("s reset")]
        [RequireBotContext(CmdChannelType.BotChannel)]
        [CmdSummary(nameof(Resource.SudokuResetSummary), typeof(Resource))]
        [CmdRemarks(nameof(Resource.SudokuResetRemarks), typeof(Resource))]
        public async Task ResetSudokuGame()
        {
            if (_sudokuMsg == null)
            {
                await SendSudokuError(SudokuResult.GameNotExists);
                return;
            }

            _sudoku.Reset();
            
            await SendSudokuImg();
        }

        [Command("sudoku set")]
        [Alias("s set")]
        [RequireBotContext(CmdChannelType.BotChannel)]
        [CmdSummary(nameof(Resource.SudokuSetSummary), typeof(Resource))]
        [CmdRemarks(nameof(Resource.SudokuSetRemarks), typeof(Resource))]
        public async Task SetSudokuPiece(string coordinates, int value)
        {
            if (_sudokuMsg == null)
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

            var result = _sudoku.InsertValue(row, col, value);

            if (result == SudokuResult.Success)
            {
                await SendSudokuImg();
            }
            else if (result == SudokuResult.Completed)
            {
                await Context.Message.DeleteAsync();
                await SendSudokuImg();
                _sudokuMsg = null;
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
            if (_sudokuMsg == null)
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

            var result = _sudoku.RemoveValue(row, col);

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
            var puzzleDrawing = _sudoku.DrawGame();

            Task delSudokuTask;
            Task<RestUserMessage> makeSudokuTask;

            if (_sudokuMsg != null)
                delSudokuTask = _sudokuMsg.DeleteAsync();
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

            _sudokuMsg = makeSudokuTask.Result;

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
        public async Task GenRandomNumber(int min, int max)
        {
            if (min > max)
            {
                await ReplyAsync("Min value must be less than max value.");
                return;
            }

            var rand = new Random();
            var value = rand.Next(min, max + 1);

            await ReplyAsync($"Rolled: {value}");
        }
    }
}

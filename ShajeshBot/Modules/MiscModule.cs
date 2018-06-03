using Discord.Commands;
using ShajeshBot.Attributes;
using ShajeshBot.Utilities;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ShajeshBot.Modules
{
    public class MiscModule : ModuleBase<SocketCommandContext>
    {
        [Command("sudoku")]
        [RequireBotContext(CmdChannelType.BotChannel)]
        [CmdSummary(nameof(Resource.SudokuSummary), typeof(Resource))]
        public async Task GenerateSudokuPuzzle()
        {
            var sudokuGen = new SudokuRunner();

            sudokuGen.Create();
            var puzzleDrawing = sudokuGen.DrawPuzzle();

            var sudokuIdx = 0;

            while (File.Exists($"sudoku{sudokuIdx}.png"))
            {
                sudokuIdx++;
            }

            puzzleDrawing.Save($"sudoku{sudokuIdx}.png");

            await Context.Channel.SendFileAsync($"sudoku{sudokuIdx}.png");

            File.Delete($"sudoku{sudokuIdx}.png");
        }
        
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

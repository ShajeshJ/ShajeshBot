using Discord.Commands;
using ShagBot.Attributes;
using ShagBot.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShagBot.Modules
{
    public class MiscModule : ModuleBase<SocketCommandContext>
    {
        [Command("sudoku")]
        [RequireBotContext(CmdChannelType.BotChannel)]
        [CmdSummary(nameof(Resource.SudokuSummary), typeof(Resource))]
        public async Task GenerateSudokuPuzzle()
        {
            var sudokuGen = new SudokuGenerator();

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
    }
}

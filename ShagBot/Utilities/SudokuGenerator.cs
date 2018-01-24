using ShagBot.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShagBot.Utilities
{
    public class SudokuGenerator
    {
        private Random _rng;

        private int[] _solvedBoard;
        private int[] _puzzleBoard;

        private static Dictionary<int, IEnumerable<int>> _setLookup;

        public int[,] SolvedBoard
        {
            get
            {
                var board = new int[9, 9];

                for (int i = 0; i <= board.GetUpperBound(0); i++)
                {
                    for (int j = 0; j <= board.GetUpperBound(1); j++)
                    {
                        board[i, j] = _solvedBoard[i * 9 + j];
                    }
                }

                return board;
            }
        }

        public int[,] PuzzleBoard
        {
            get
            {
                var board = new int[9, 9];

                for (int i = 0; i <= board.GetUpperBound(0); i++)
                {
                    for (int j = 0; j <= board.GetUpperBound(1); j++)
                    {
                        board[i, j] = _puzzleBoard[i * 9 + j];
                    }
                }

                return board;
            }
        }

        static SudokuGenerator()
        {
            InitSetLookup();
        }

        public SudokuGenerator()
        {
            _rng = new Random();
            _solvedBoard = new int[81];
            _puzzleBoard = new int[81];
        }

        private static void InitSetLookup()
        {
            List<List<int>> allSets = new List<List<int>>
            {
                new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8 },            //Row 1
                new List<int> { 9, 10, 11, 12, 13, 14, 15, 16, 17 },    //Row 2
                new List<int> { 18, 19, 20, 21, 22, 23, 24, 25, 26 },   //Row 3
                new List<int> { 27, 28, 29, 30, 31, 32, 33, 34, 35 },   //Row 4
                new List<int> { 36, 37, 38, 39, 40, 41, 42, 43, 44 },   //Row 5
                new List<int> { 45, 46, 47, 48, 49, 50, 51, 52, 53 },   //Row 6
                new List<int> { 54, 55, 56, 57, 58, 59, 60, 61, 62 },   //Row 7
                new List<int> { 63, 64, 65, 66, 67, 68, 69, 70, 71 },   //Row 8
                new List<int> { 72, 73, 74, 75, 76, 77, 78, 79, 80 },   //Row 9

                new List<int> { 0, 9, 18, 27, 36, 45, 54, 63, 72 },     //Col 1
                new List<int> { 1, 10, 19, 28, 37, 46, 55, 64, 73 },    //Col 2
                new List<int> { 2, 11, 20, 29, 38, 47, 56, 65, 74 },    //Col 3
                new List<int> { 3, 12, 21, 30, 39, 48, 57, 66, 75 },    //Col 4
                new List<int> { 4, 13, 22, 31, 40, 49, 58, 67, 76 },    //Col 5
                new List<int> { 5, 14, 23, 32, 41, 50, 59, 68, 77 },    //Col 6
                new List<int> { 6, 15, 24, 33, 42, 51, 60, 69, 78 },    //Col 7
                new List<int> { 7, 16, 25, 34, 43, 52, 61, 70, 79 },    //Col 8
                new List<int> { 8, 17, 26, 35, 44, 53, 62, 71, 80 },    //Col 9

                new List<int> { 0, 1, 2, 9, 10, 11, 18, 19, 20 },       //Box 1
                new List<int> { 3, 4, 5, 12, 13, 14, 21, 22, 23 },      //Box 2
                new List<int> { 6, 7, 8, 15, 16, 17, 24, 25, 26 },      //Box 3
                new List<int> { 27, 28, 29, 36, 37, 38, 45, 46, 47 },   //Box 4
                new List<int> { 30, 31, 32, 39, 40, 41, 48, 49, 50 },   //Box 5
                new List<int> { 33, 34, 35, 42, 43, 44, 51, 52, 53 },   //Box 6
                new List<int> { 54, 55, 56, 63, 64, 65, 72, 73, 74 },   //Box 7
                new List<int> { 57, 58, 59, 66, 67, 68, 75, 76, 77 },   //Box 8
                new List<int> { 60, 61, 62, 69, 70, 71, 78, 79, 80 }    //Box 9
            };

            _setLookup = new Dictionary<int, IEnumerable<int>>();

            allSets.ForEach(x => x.ForEach(k =>
            {
                if (_setLookup.ContainsKey(k))
                    _setLookup[k] = _setLookup[k].Union(x.Where(v => v != k));
                else
                    _setLookup[k] = new List<int>(x.Where(v => v != k));
            }));
        }

        public void Create()
        {
            _solvedBoard = new int[81];

            _solvedBoard = FillBoard(0, _solvedBoard);
            _puzzleBoard = GeneratePuzzle(_solvedBoard);
        }

        private int[] FillBoard(int cell, int[] board)
        {
            var validValues = new List<int>(Enumerable.Range(1, 9));

            for (int i = 0; i < cell; i++)
            {
                if (_setLookup[cell].Contains(i))
                {
                    validValues.Remove(board[i]);
                }
            }

            while (true)
            {
                if (validValues.Count == 0)
                {
                    return null;
                }

                board[cell] = validValues[_rng.Next(0, validValues.Count)];
                validValues.Remove(board[cell]);

                if (cell == 80)
                {
                    return board;
                }

                var newBoard = FillBoard(cell + 1, board);

                if (newBoard != null)
                {
                    return newBoard;
                }
            }
        }

        private int[] GeneratePuzzle(int[] board)
        {
            int[] newboard = new int[board.Length];

            do
            {
                var randomizedCells = Enumerable.Range(0, 41).ToArray();
                randomizedCells.Shuffle(_rng);

                var cellsToFill = new List<int>();

                for (int i = 0; i < 18; i++)
                {
                    cellsToFill.Add(randomizedCells[i]);

                    //Add random cells symmetrically
                    if (80 - randomizedCells[i] != randomizedCells[i])
                    {
                        cellsToFill.Add(80 - randomizedCells[i]);
                    }
                }

                newboard = new int[board.Length];

                for (int i = 0; i < board.Length; i++)
                {
                    if (cellsToFill.Contains(i))
                    {
                        newboard[i] = board[i];
                    }
                    else
                    {
                        newboard[i] = 0;
                    }
                }
            } while (!IsUniquePuzzle(0, newboard, board));

            return newboard;
        }

        public Image DrawPuzzle()
        {
            return DrawBoard(PuzzleBoard);
        }

        public Image DrawSolution()
        {
            return DrawBoard(SolvedBoard);
        }

        private static Image DrawBoard(int[,] board)
        {
            var tmpImg = new Bitmap(1, 1);
            var tmpDrawing = Graphics.FromImage(tmpImg);

            var font = new Font(FontFamily.GenericMonospace, 24, FontStyle.Bold, GraphicsUnit.Pixel);
            var textBrush = new SolidBrush(Color.Black);

            var textSize = tmpDrawing.MeasureString("1", font);

            tmpImg.Dispose();
            tmpDrawing.Dispose();

            var imgs = new Bitmap[10];
            var drawings = new Graphics[10];

            imgs[0] = new Bitmap((int)textSize.Width, (int)textSize.Height);
            drawings[0] = Graphics.FromImage(imgs[0]);
            drawings[0].Clear(Color.White);
            drawings[0].Save();
            drawings[0].Dispose();

            for (int i = 1; i < imgs.Length; i++)
            {
                imgs[i] = new Bitmap((int)textSize.Width, (int)textSize.Height);
                drawings[i] = Graphics.FromImage(imgs[i]);
                drawings[i].Clear(Color.White);
                drawings[i].DrawString((i + 1).ToString(), font, textBrush, 0, 0);
                drawings[i].Save();
                drawings[i].Dispose();
            }

            var tileWidth = imgs[0].Width;
            var tileHeight = imgs[0].Height;

            var boardImg = new Bitmap(tileWidth * 9, tileHeight * 9);
            var boardDrawing = Graphics.FromImage(boardImg);

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (board[i, j] != -1)
                    {
                        boardDrawing.DrawImage(imgs[board[i, j]], j * tileWidth, i * tileHeight);
                    }
                    else
                    {
                        boardDrawing.DrawImage(imgs[0], j * tileWidth, i * tileHeight);
                    }
                }
            }

            var smallPen = new Pen(textBrush, 1);
            var bigPen = new Pen(textBrush, 3);

            for (int i = 1; i < 9; i++)
            {
                boardDrawing.DrawLine(smallPen, tileWidth * i, 0, tileWidth * i, tileHeight * 9);
                boardDrawing.DrawLine(smallPen, 0, tileHeight * i, tileWidth * 9, tileHeight * i);
            }

            for (int i = 0; i < 9; i += 3)
            {
                boardDrawing.DrawLine(bigPen, tileWidth * i, 0, tileWidth * i, tileHeight * 9);
                boardDrawing.DrawLine(bigPen, 0, tileHeight * i, tileWidth * 9, tileHeight * i);
            }
            boardDrawing.DrawLine(bigPen, tileWidth * 9 - 1, 0, tileWidth * 9 - 1, tileHeight * 9);
            boardDrawing.DrawLine(bigPen, 0, tileHeight * 9 - 1, tileWidth * 9, tileHeight * 9 - 1);

            boardDrawing.Save();

            return boardImg;
        }

        private bool IsUniquePuzzle(int cell, int[] board, int[] solution)
        {
            if (cell >= board.Length)
            {
                return CheckSolutionsMatch(board, solution);
            }
            else if (board[cell] > 0)
            {
                return IsUniquePuzzle(cell + 1, board, solution);
            }
            else
            {
                var validValues = new List<int>(Enumerable.Range(1, 9));

                for (int i = 0; i < board.Length; i++)
                {
                    if (_setLookup[cell].Contains(i))
                    {
                        validValues.Remove(board[i]);
                    }
                }

                while (true)
                {
                    if (validValues.Count == 0)
                    {
                        return true;
                    }

                    board[cell] = validValues[_rng.Next(0, validValues.Count)];
                    validValues.Remove(board[cell]);

                    var isUnique = IsUniquePuzzle(cell + 1, board, solution);

                    board[cell] = 0;

                    if (!isUnique)
                    {
                        return false;
                    }
                }
            }
        }

        private bool CheckSolutionsMatch(int[] board1, int[] board2)
        {
            if (board1.Length != board2.Length)
            {
                return false;
            }

            for (int i = 0; i < board1.Length; i++)
            {
                if (board1[i] != board2[i])
                {
                    return false;
                }
            }

            return true;
        }
    }
}

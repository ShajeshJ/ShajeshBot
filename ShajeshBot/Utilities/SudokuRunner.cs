using ShajeshBot.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace ShajeshBot.Utilities
{
    public class SudokuRunner
    {
        private Random _rng;

        private int[] _solvedBoard;
        private int[] _puzzleBoard;

        private static Bitmap _blankTile;
        private static Bitmap[] _numberTiles;
        private static Bitmap[] _letterTiles;
        private static Font _font;
        private static SolidBrush _textBrush;

        private static int _tileWidth;
        private static int _tileHeight;

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

        static SudokuRunner()
        {
            InitSetLookup();
            CreateArtifacts();
        }

        public SudokuRunner()
        {
            _rng = new Random();
            _solvedBoard = new int[81];
            _puzzleBoard = new int[81];
        }

        #region Setup Methods

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

        private static void CreateArtifacts()
        {
            var tmpImg = new Bitmap(1, 1);
            var tmpDrawing = Graphics.FromImage(tmpImg);

            _font = new Font(FontFamily.GenericMonospace, 24, FontStyle.Bold, GraphicsUnit.Pixel);
            _textBrush = new SolidBrush(Color.Black);

            var textSize = tmpDrawing.MeasureString("1", _font);

            tmpImg.Dispose();
            tmpDrawing.Dispose();
            
            _blankTile = new Bitmap((int)textSize.Width, (int)textSize.Height);
            var blankDrawing = Graphics.FromImage(_blankTile);
            blankDrawing.Clear(Color.White);
            blankDrawing.Save();
            blankDrawing.Dispose();

            _numberTiles = new Bitmap[9];
            var numDrawings = new Graphics[9];

            _letterTiles = new Bitmap[9];
            var letterDrawings = new Graphics[9];

            for (int i = 0; i < _numberTiles.Length; i++)
            {
                _numberTiles[i] = new Bitmap((int)textSize.Width, (int)textSize.Height);
                numDrawings[i] = Graphics.FromImage(_numberTiles[i]);
                numDrawings[i].Clear(Color.White);
                numDrawings[i].DrawString((i + 1).ToString(), _font, _textBrush, 0, 0);
                numDrawings[i].Save();
                numDrawings[i].Dispose();

                _letterTiles[i] = new Bitmap((int)textSize.Width, (int)textSize.Height);
                letterDrawings[i] = Graphics.FromImage(_letterTiles[i]);
                letterDrawings[i].Clear(Color.White);
                letterDrawings[i].DrawString(Convert.ToChar('A' + i).ToString(), _font, _textBrush, 0, 0);
                letterDrawings[i].Save();
                letterDrawings[i].Dispose();
            }

            _tileWidth = _blankTile.Width;
            _tileHeight = _blankTile.Height;
        }

        #endregion

        public void Create()
        {
            _solvedBoard = new int[81];

            _solvedBoard = FillBoard(0, _solvedBoard);
            _puzzleBoard = GeneratePuzzle(_solvedBoard);
        }

        #region Board Generation

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

        #endregion

        #region Puzzle Generation

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

        #endregion

        #region Board Drawing

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
            var boardImg = new Bitmap(_tileWidth * 10, _tileHeight * 10);
            var boardDrawing = Graphics.FromImage(boardImg);

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    if (j == 0)
                    {
                        boardDrawing.DrawImage(_letterTiles[i], j * _tileWidth, i * _tileHeight);
                    }
                    else if (board[i, j - 1] != 0)
                    {
                        boardDrawing.DrawImage(_numberTiles[board[i, j - 1] - 1], j * _tileWidth, i * _tileHeight);
                    }
                    else
                    {
                        boardDrawing.DrawImage(_blankTile, j * _tileWidth, i * _tileHeight);
                    }
                }
            }

            boardDrawing.DrawImage(_blankTile, 0, 9 * _tileHeight);

            for (int j = 1; j < 10; j++)
            {
                boardDrawing.DrawImage(_numberTiles[j - 1], j * _tileWidth, 9 * _tileHeight);
            }

            var smallPen = new Pen(_textBrush, 1);
            var bigPen = new Pen(_textBrush, 3);

            for (int i = 1; i < 10; i++)
            {
                boardDrawing.DrawLine(smallPen, _tileWidth * i, 0, _tileWidth * i, _tileHeight * 10); //horizontal lines
                boardDrawing.DrawLine(smallPen, 0, _tileHeight * i, _tileWidth * 10, _tileHeight * i); //vertical lines
            }

            for (int i = 0; i <= 9; i += 3)
            {
                boardDrawing.DrawLine(bigPen, _tileWidth * (i + 1), 0, _tileWidth * (i + 1), _tileHeight * 9); //horizontal lines
                boardDrawing.DrawLine(bigPen, _tileWidth, _tileHeight * i, _tileWidth * 10, _tileHeight * i); //vertical lines
            }

            boardDrawing.Save();

            return boardImg;
        }

        #endregion

        #region User Actions



        #endregion
    }
}

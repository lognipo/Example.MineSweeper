using System;
using System.Collections.Generic;
using System.Text;

namespace Example.MineSweeper
{
    public class Board
    {
        private Cell[,] Cells { get; }
        private Random Random { get; }
        private int MineThreshold { get; }

        public int Width => Cells.GetLength(0);
        public int Height => Cells.GetLength(1);

        public int MineCount { get; private set; }
        public int HiddenCount { get; private set; }

        public Cell this[int x, int y]
        {
            get
            {
                ValidateCoordinates(x, y);
                return Cells[x, y];
            }
        }

        public Board(int width, int height, float fill)
        {
            Cells = new Cell[width, height];
            Random = new Random();
            MineThreshold = (int)(fill * 100);
        }

        public void Reset()
        {
            MineCount = 0;
            HiddenCount = Cells.Length;

            // initialize all cells if necessary, or clear their mine counts
            for (int x = 0; x < Cells.GetLength(0); x++)
                for (int y = 0; y < Cells.GetLength(1); y++)
                {
                    if (Cells[x, y] is null)
                        Cells[x, y] = new Cell();
                    else
                        Cells[x, y].MineCount = 0;
                }

            // now ready the rest of the board
            for (int x = 0; x < Cells.GetLength(0); x++)
                for (int y = 0; y < Cells.GetLength(1); y++)
                {
                    ref var cell = ref Cells[x, y];

                    cell.IsRevealed = false;
                    cell.IsMarked = false;
                    cell.HasMine = Random.Next(0, 100) < MineThreshold;

                    if (cell.HasMine)
                    {
                        MineCount++;
                        IncrementMineCountsAround(x, y);
                    }
                }
        }

        public int ClampX(int x) => Clamp(x, 0);
        public int ClampY(int y) => Clamp(y, 1);

        public void Reveal(int x, int y)
        {
            ValidateCoordinates(x, y);
            var cell = Cells[x, y];

            // only reveal once
            if (cell.IsRevealed)
                return;

            
            if (cell.MineCount == 0 && !cell.HasMine)
                RevealZeroCounts(x, y); // reveal until we find counts
            else
                RevealCell(cell);       // reveal just this cell
        }

        public void RevealMines()
        {
            // show all mines
            for (int x = 0; x < Cells.GetLength(0); x++)
                for (int y = 0; y < Cells.GetLength(1); y++)
                    if (Cells[x, y].HasMine)
                        Cells[x, y].IsRevealed = true;
        }

        private void RevealCell(Cell cell)
        {
            if (cell.IsRevealed)
                return;

            cell.IsRevealed = true;
            HiddenCount--;
        }

        private void RevealZeroCounts(int x, int y)
        {
            var processing = new Queue<(int X, int Y)>();
            processing.Enqueue((x, y));

            while(processing.Count > 0)
            {
                var coord = processing.Dequeue();
                var cell = Cells[coord.X, coord.Y];

                if (cell.IsRevealed)
                    continue;

                // reveal the cell
                RevealCell(cell);

                // queue neighbors for processing
                int minX = ClampX(coord.X - 1), maxX = ClampX(coord.X + 1),
                    minY = ClampY(coord.Y - 1), maxY = ClampY(coord.Y + 1);

                // reveal no further if we are near mines
                if (cell.MineCount != 0)
                    continue;

                for(int ix = minX; ix <= maxX; ix++)
                    for(int iy = minY; iy <= maxY; iy++)
                    {
                        // skip the current cell
                        if (ix == coord.X & iy == coord.Y)
                            continue;

                        var other = Cells[ix, iy];

                        // skip cells that have already been revealed
                        if (other.IsRevealed)
                            continue;

                        // add to processing
                        processing.Enqueue((ix, iy));
                    }
            }
        }

        public void ToggleMark(int x, int y)
        {
            ValidateCoordinates(x, y);
            Cells[x, y].IsMarked = !Cells[x, y].IsMarked;
        }

        public string GetBoardString()
        {
            var builder = new StringBuilder();

            for (int y = 0; y < Cells.GetLength(1); y++)
            {
                for (int x = 0; x < Cells.GetLength(0); x++)
                {
                    builder.Append(GetCellCharacter(Cells[x, y]));
                }
                builder.Append("\r\n");
            }

            return builder.ToString();
        }

        private int Clamp(int num, int dim)
        {
            if (num < 0)
                return 0;

            var bound = Cells.GetUpperBound(dim);
            if (num > bound)
                return bound;

            return num;
        }

        private void IncrementMineCountsAround(int x, int y)
        {
            int minX = ClampX(x - 1), maxX = ClampX(x + 1),
                minY = ClampY(y - 1), maxY = ClampY(y + 1);

            for (int ix = minX; ix <= maxX; ix++)
                for (int iy = minY; iy <= maxY; iy++)
                {
                    if (ix == x & iy == y)
                        continue;

                    Cells[ix, iy].MineCount++;
                }
        }

        private void ValidateCoordinates(int x, int y)
        {
            if (x < 0 || x > Cells.GetUpperBound(0))
                throw new IndexOutOfRangeException($"X ({x}) must be between 0 and {Cells.GetUpperBound(0)}.");

            if (y < 0 || y > Cells.GetUpperBound(0))
                throw new IndexOutOfRangeException($"Y ({y}) must be between 0 and {Cells.GetUpperBound(1)}.");
        }



        private char GetCellCharacter(Cell cell)
        {
            if (cell.IsMarked)
                return 'X';

            if (!cell.IsRevealed)
                return '.';

            if (cell.HasMine)
                return '*';

            if (cell.MineCount == 0)
                return ' ';

            return cell.MineCount.ToString()[0];
        }

    }
}

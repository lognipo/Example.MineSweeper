using System;
using System.Collections.Generic;
using System.Text;

namespace Example.MineSweeper
{
    public class Board
    {
        private BoardCell[,] Cells { get; }
        private Random Random { get; }
        private double Fill { get; }

        public int Width => Cells.GetLength(0);
        public int Height => Cells.GetLength(1);

        public int MineCount { get; private set; }
        public int HiddenCount { get; private set; }

        public BoardCell this[int x, int y]
        {
            get
            {
                ValidateCoordinates(x, y);
                return Cells[x, y];
            }
        }

        public Board(int width, int height, double fill)
        {
            Cells = new BoardCell[width, height];
            Random = new Random();
            Fill = fill;
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
                        Cells[x, y] = new BoardCell();
                    else
                        Cells[x, y].MineCount = 0;
                }

            // now ready the rest of the board
            for (int x = 0; x < Cells.GetLength(0); x++)
                for (int y = 0; y < Cells.GetLength(1); y++)
                {
                    var cell = Cells[x, y];

                    cell.IsRevealed = false;
                    cell.IsMarked = false;
                    cell.HasMine = Random.NextDouble() < Fill;

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

        private void RevealCell(BoardCell cell)
        {
            if (cell.IsRevealed)
                return;

            cell.IsRevealed = true;
            HiddenCount--;
        }

        private (int Min, int Max) GetMinMax(int num, int dim) =>
            (Clamp(num - 1, dim), Clamp(num + 1, dim));

        private void RevealZeroCounts(int x, int y)
        {
            var processing = new Queue<(int X, int Y)>();
            processing.Enqueue((x, y));

            while(processing.Count > 0)
            {
                // get the next cell from the queue
                var coord = processing.Dequeue();
                var cell = Cells[coord.X, coord.Y];

                if (cell.IsRevealed)
                    continue;

                // reveal the cell
                RevealCell(cell);


                // reveal no further if we are near mines
                if (cell.MineCount != 0)
                    continue;

                // queue neighbors for processing
                var (minX, maxX) = GetMinMax(coord.X, 0);
                var (minY, maxY) = GetMinMax(coord.Y, 1);

                for (int ix = minX; ix <= maxX; ix++)
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
            var cell = Cells[x, y];

            // no marking revealed cells
            if (cell.IsRevealed)
                return;

            cell.IsMarked = !cell.IsMarked;
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


        public int WrapX(int x) => Wrap(x, Cells.GetLength(0));
        public int WrapY(int y) => Wrap(y, Cells.GetLength(1));
        private int Wrap(int num, int size)
        {
            if (num < 0)
                return size + (num % size);

            if (num >= size)
                return num % size;

            return num;
        }

        private void IncrementMineCountsAround(int x, int y)
        {
            var (minX, maxX) = GetMinMax(x, 0);
            var (minY, maxY) = GetMinMax(y, 1);

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





    }
}

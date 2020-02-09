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

        public CoordinateManager2D Coordinates { get; }

        public int Width => Cells.GetLength(0);
        public int Height => Cells.GetLength(1);

        public int MineCount { get; private set; }
        public int HiddenCount { get; private set; }

        public BoardCell this[int x, int y]
        {
            get
            {
                Coordinates.ValidateCoordinates(x, y);
                return Cells[x, y];
            }
        }

        public Board(int width, int height, double fill)
        {
            Cells = new BoardCell[width, height];
            Coordinates = new CoordinateManager2D(Cells);
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

                        // increment mine counts in adjacent cells
                        ProcessAdjacentCells(x, y, (x, y) => Cells[x, y].MineCount++);
                    }
                }
        }


        public void Reveal(int x, int y)
        {
            Coordinates.ValidateCoordinates(x, y);
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
                        RevealCell(Cells[x, y]);
        }

        private void RevealCell(BoardCell cell)
        {
            // we only reveal once
            if (cell.IsRevealed)
                return;

            // we do not reveal marked cells
            if (cell.IsMarked)
                return;

            cell.IsRevealed = true;

            // we do not count revealed mines
            if (cell.HasMine)
                return;

            // count is used to determine when the game is won
            HiddenCount--;
        }

        private (int Min, int Max) GetAdjacentLoopBounds(int num, int dim) =>
            (Coordinates.Clamp(num - 1, Cells.GetUpperBound(dim)),
             Coordinates.Clamp(num + 1, Cells.GetUpperBound(dim)));

        private void RevealZeroCounts(int x, int y)
        {
            var processing = new Queue<(int X, int Y)>();
            processing.Enqueue((x, y));

            while(processing.Count > 0)
            {
                // get the next cell from the queue
                var coord = processing.Dequeue();
                var cell = Cells[coord.X, coord.Y];

                // skip cells that have since been revealed
                if (cell.IsRevealed)
                    continue;

                // reveal the cell
                RevealCell(cell);

                // reveal no further if we are near mines
                if (cell.MineCount != 0)
                    continue;

                // queue neighbors for processing
                ProcessAdjacentCells(coord.X, coord.Y,
                    (x, y) =>
                    {
                        if(!Cells[x, y].IsRevealed) // don't queue revealed cells
                        {
                            processing.Enqueue((x, y));
                        }
                    });
            }
        }

        private void ProcessAdjacentCells(int x, int y, Action<int, int> process)
        {
            var (minX, maxX) = GetAdjacentLoopBounds(x, 0);
            var (minY, maxY) = GetAdjacentLoopBounds(y, 1);

            for (int ix = minX; ix <= maxX; ix++)
                for (int iy = minY; iy <= maxY; iy++)
                {
                    // skip the center cell
                    if (ix == x & iy == y)
                        continue;

                    process(ix, iy);
                }
        }

        public void ToggleMark(int x, int y)
        {
            Coordinates.ValidateCoordinates(x, y);
            var cell = Cells[x, y];

            // no marking revealed cells
            if (cell.IsRevealed)
                return;

            cell.IsMarked = !cell.IsMarked;
        }
    }
}

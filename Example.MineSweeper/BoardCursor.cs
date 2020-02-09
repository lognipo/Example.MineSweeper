using System;
using System.Collections.Generic;
using System.Text;

namespace Example.MineSweeper
{
    public class BoardCursor
    {
        private Board Board { get; }
        public int X { get; private set; }
        public int Y { get; private set; }

        public BoardCursor(Board board)
        {
            Board = board;
            Reset();
        }

        public void SetPosition(int x, int y)
        {
            X = Board.ClampX(x);
            Y = Board.ClampY(y);
        }

        public void MoveLeft() => SetPosition(X - 1, Y);
        public void MoveRight() => SetPosition(X + 1, Y);
        public void MoveUp() => SetPosition(X, Y - 1);
        public void MoveDown() => SetPosition(X, Y + 1);

        public void Reset() => SetPosition(Board.Width / 2, Board.Height / 2);
    }
}

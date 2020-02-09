using System;
using System.Collections.Generic;
using System.Text;

namespace Example.MineSweeper
{
    public class BoardCursor
    {
        private CoordinateManager2D Coordinates { get; }
        public int X { get; private set; }
        public int Y { get; private set; }

        public BoardCursor(CoordinateManager2D coordinates)
        {
            Coordinates = coordinates;
            Reset();
        }

        public void SetPosition(int x, int y)
        {
            X = Coordinates.WrapX(x);
            Y = Coordinates.WrapY(y);
        }

        public void MoveLeft() => SetPosition(X - 1, Y);
        public void MoveRight() => SetPosition(X + 1, Y);
        public void MoveUp() => SetPosition(X, Y - 1);
        public void MoveDown() => SetPosition(X, Y + 1);

        public void Reset() => SetPosition(Coordinates.Width / 2, Coordinates.Height / 2);
    }
}

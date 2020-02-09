using System;
using System.Collections.Generic;
using System.Text;

namespace Example.MineSweeper
{
    public class CoordinateManager2D
    {
        private Array Array { get; }

        public int Width => Array.GetLength(0);
        public int Height => Array.GetLength(1);

        public CoordinateManager2D(Array array)
        {
            Array = array ?? throw new ArgumentNullException(nameof(array));
            if (Array.Rank != 2)
                throw new Exception($"{nameof(CoordinateManager2D)} only works with 2D arrays.  You supplied an array with {array.Rank} dimensions.");
        }

        public void ValidateCoordinates(int x, int y)
        {
            if (x < 0 || x > Array.GetUpperBound(0))
                throw new IndexOutOfRangeException($"X ({x}) must be between 0 and {Array.GetUpperBound(0)}.");

            if (y < 0 || y > Array.GetUpperBound(0))
                throw new IndexOutOfRangeException($"Y ({y}) must be between 0 and {Array.GetUpperBound(1)}.");
        }

        public int Clamp(int num, int max)
        {
            if (num < 0)
                return 0;

            if (num > max)
                return max;

            return num;
        }

        public int ClampX(int x) => Clamp(x, Array.GetUpperBound(0));
        public int ClampY(int y) => Clamp(y, Array.GetUpperBound(1));

        public int WrapX(int x) => Wrap(x, Array.GetLength(0));
        public int WrapY(int y) => Wrap(y, Array.GetLength(1));

        

        private int Wrap(int num, int size)
        {
            if (num < 0)
                return size + (num % size);

            if (num >= size)
                return num % size;

            return num;
        }





    }
}

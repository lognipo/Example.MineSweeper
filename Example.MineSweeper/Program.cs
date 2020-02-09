using System;

namespace Example.MineSweeper
{
    class Program
    {
        static void Main(string[] args)
        {
            var graphics = new ConsoleGraphics();
            var board = new Board(64, 32, 0.1);
            var game = new MineSweeperGame(graphics, board);

            game.Play();
        }
    }
}

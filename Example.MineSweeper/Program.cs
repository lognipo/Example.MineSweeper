using System;

namespace Example.MineSweeper
{
    class Program
    {
        static void Main(string[] args)
        {
            var game = new MineSweeperGame(64, 32, 0.10f);
            game.Play();
        }
    }
}

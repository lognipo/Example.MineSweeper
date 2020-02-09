using System;

namespace Example.MineSweeper
{
    class Program
    {
        static void Main(string[] args)
        {
            var game = new MineSweeperGame(10, 10, 0.05f);
            game.Play();
        }
    }
}

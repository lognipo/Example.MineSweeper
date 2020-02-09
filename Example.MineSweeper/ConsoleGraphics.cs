using System;
using System.Collections.Generic;
using System.Text;

namespace Example.MineSweeper
{
    public class ConsoleGraphics : IGraphics
    {
        public int BoardOffsetY { get; private set; }

        public ConsoleGraphics()
        {

        }

        public void Draw(MineSweeperGame game)
        {
            var stateText = game.GetGameStateText() + "\r\n";
            BoardOffsetY = GetLineCount(stateText);

            Clear();

            // status text
            Console.ForegroundColor =
                game.HasFailed ? ConsoleColor.Red
                : game.HasWon ? ConsoleColor.Blue
                : ConsoleColor.Yellow;
            Console.WriteLine(stateText);

            // board text
            Console.ForegroundColor =
                game.HasFailed ? ConsoleColor.Red
                : game.HasWon ? ConsoleColor.Cyan
                : ConsoleColor.White;
            Console.WriteLine(GetBoardString(game.Board));

        }

        public void Clear()
        {
            Console.Clear();
        }


        private char GetCellCharacter(BoardCell cell)
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

        private string GetBoardString(Board board)
        {
            var builder = new StringBuilder();

            for (int y = 0; y < board.Height; y++)
            {
                for (int x = 0; x < board.Width; x++)
                {
                    builder.Append(GetCellCharacter(board[x, y]));
                }
                builder.Append("\r\n");
            }

            return builder.ToString();
        }


        private int GetLineCount(string text)
        {
            int count = 0,
                nextIndex = 0,
                lastIndex;

            while (nextIndex >= 0 && nextIndex < text.Length)
            {
                lastIndex = text.IndexOf("\r\n", nextIndex);
                if (lastIndex == -1)
                    break;

                count++;
                nextIndex = lastIndex + 2;
            }

            return count + 1;
        }



    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Example.MineSweeper
{
    public class MineSweeperGame
    {
        private Board Board { get; }
        private BoardCursor Cursor { get; }
        private Dictionary<ConsoleKey, Action> KeyBindings { get; }

        private bool IsPlaying { get; set; }
        private bool HasFailed { get; set; }
        private bool HasWon { get; set; }

        public MineSweeperGame(int width, int height, float fill)
        {
            Board = new Board(width, height, fill);
            Cursor = new BoardCursor(Board);

            KeyBindings = new Dictionary<ConsoleKey, Action>
            {
                { ConsoleKey.UpArrow, Cursor.MoveUp },
                { ConsoleKey.DownArrow, Cursor.MoveDown },
                { ConsoleKey.LeftArrow, Cursor.MoveLeft },
                { ConsoleKey.RightArrow, Cursor.MoveRight },
                { ConsoleKey.Spacebar, Reveal },
                { ConsoleKey.M, ToggleMark },
                { ConsoleKey.Escape, Exit },
                { ConsoleKey.R, Reset }
            };
        }

        public void Reset()
        {
            HasFailed = false;
            Board.Reset();
            Cursor.Reset();
        }

        public void Exit()
        {
            IsPlaying = false;
        }

        private void ToggleMark()
        {
            // we can't mark if we fail
            if (HasFailed)
                return;

            Board.ToggleMark(Cursor.X, Cursor.Y);
        }

        private void Reveal()
        {
            // we can't reveal cells after we fail
            if (HasFailed)
                return;

            var cell = Board[Cursor.X, Cursor.Y];

            // prevent unhappy accidents
            if (cell.IsMarked)
                return;

            Board.Reveal(Cursor.X, Cursor.Y);
            if (cell.HasMine)
            {
                // game over!
                Board.RevealMines();
                HasFailed = true;
            }
            else if(Board.HiddenCount == Board.MineCount)
            {
                HasWon = true;
            }
        }

        private string GetGameStateText()
        {
            if (HasFailed)
                return "Esc: Quit\r\nR: Reset\r\n\r\nGAME OVER!";

            return "Arrows: Move Cursor\r\nSpace: Reveal/Clear\r\nM: Mark\r\nEsc: Quit\r\nR: Reset\r\n\r\nCHOOSE WISELY";
        }


        private int GetLineCount(string text)
        {
            int count = 0,
                nextIndex = 0,
                lastIndex;

            while(nextIndex >= 0 && nextIndex < text.Length)
            {
                lastIndex = text.IndexOf("\r\n", nextIndex);
                if (lastIndex == -1)
                    break;

                count++;
                nextIndex = lastIndex + 2;
            }

            return count + 1;
        }

        private void DrawBoard()
        {
            var stateText = GetGameStateText() + "\r\n";
            var stateTextLineCount = GetLineCount(stateText);

            Console.Clear();

            // status text
            Console.ForegroundColor = 
                HasFailed
                ? ConsoleColor.Red
                : ConsoleColor.Yellow;
            Console.WriteLine(stateText);

            // board text
            Console.ForegroundColor = 
                HasFailed
                ? ConsoleColor.Red
                : ConsoleColor.White;
            Console.WriteLine(Board.GetBoardString());

            Console.SetCursorPosition(Cursor.X, Cursor.Y + stateTextLineCount);
        }

        public void Play()
        {
            Reset();
            

            IsPlaying = true;
            while(IsPlaying)
            {
                DrawBoard();

                var key = Console.ReadKey(true).Key;

                if(!KeyBindings.TryGetValue(key, out Action action))
                {
                    // do nothing?
                    continue;
                }

                action();
            }

        }
    }
}

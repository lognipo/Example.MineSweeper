using System;
using System.Collections.Generic;
using System.Text;

namespace Example.MineSweeper
{
    public class MineSweeperGame
    {
        public Board Board { get; }
        public BoardCursor Cursor { get; }
        private IGraphics Graphics { get; }
        private Dictionary<ConsoleKey, Action> KeyBindings { get; }

        private bool IsPlaying { get; set; }
        public bool HasFailed { get; set; }
        public bool HasWon { get; set; }

        private bool SkipRedraw { get; set; }

        public MineSweeperGame(IGraphics graphics, Board board)
        {
            Board = board;
            Cursor = new BoardCursor(Board);
            Graphics = graphics;

            Action NoRedraw(Action action) =>
                () =>
                {
                    action();
                    SkipRedraw = true;
                };

            KeyBindings = new Dictionary<ConsoleKey, Action>
            {
                { ConsoleKey.UpArrow, NoRedraw(Cursor.MoveUp) },
                { ConsoleKey.DownArrow, NoRedraw(Cursor.MoveDown) },
                { ConsoleKey.LeftArrow, NoRedraw(Cursor.MoveLeft) },
                { ConsoleKey.RightArrow, NoRedraw(Cursor.MoveRight) },
                { ConsoleKey.Spacebar, Reveal },
                { ConsoleKey.M, ToggleMark },
                { ConsoleKey.Escape, Exit },
                { ConsoleKey.R, Reset }
            };
        }

        public void Reset()
        {
            HasFailed = false;
            HasWon = false;
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

        public string GetGameStateText()
        {
            if (HasFailed)
                return "Esc: Quit\r\nR: Reset\r\n\r\nGAME OVER!";

            if (HasWon)
                return "Esc: Quit\r\nR: Reset\r\n\r\nA WINNER IS YOU!";

            return "Arrows: Move Cursor\r\nSpace: Reveal/Clear\r\nM: Mark\r\nEsc: Quit\r\nR: Reset\r\n\r\nCHOOSE WISELY";
        }



        public void Play()
        {
            Reset();
            

            IsPlaying = true;
            while(IsPlaying)
            {
                if (SkipRedraw)
                    SkipRedraw = false;
                else
                    Graphics.Draw(this);

                Console.SetCursorPosition(Cursor.X, Cursor.Y + Graphics.BoardOffsetY);

                var key = Console.ReadKey(true).Key;
                

                if (!KeyBindings.TryGetValue(key, out Action action))
                {
                    // do nothing?
                    continue;
                }

                action();
            }

        }
    }
}

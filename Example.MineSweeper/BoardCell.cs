using System;
using System.Collections.Generic;
using System.Text;

namespace Example.MineSweeper
{
    public class BoardCell
    {
        public byte MineCount { get; set; } = 0;
        public bool IsRevealed { get; set; } = false;
        public bool IsMarked { get; set; } = false;
        public bool HasMine { get; set; } = false;
        
    }
}

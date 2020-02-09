using System;
using System.Collections.Generic;
using System.Text;

namespace Example.MineSweeper
{
    public interface IGraphics
    {
        int BoardOffsetY { get; }

        void Draw(MineSweeperGame game);
        void Clear();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris_wpf
{
    public class Position
    {
        public int Row { set; get; }
        public int Column { set; get; }
        public Position(int row, int column)
        {
            Row = row;
            Column = column;
        }
    }
}

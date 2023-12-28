using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Graphics
{
    public class Glyphe
    {
        public Char Character { get; set; }
        public int Row { get; set; }
        public int Column { get; set; }

        public Glyphe(char character, int row, int column)
        {
            Character = character;
            Row = row;
            Column = column;
        }
    }
}

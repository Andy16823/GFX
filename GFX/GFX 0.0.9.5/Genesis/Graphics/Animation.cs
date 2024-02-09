using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Graphics
{
    public class Animation
    {
        public String Name { get; set; }
        public int Cell { get; set; }
        public int Row { get; set; }
        public int Frames { get; set; }

        public Animation()
        {

        }

        public Animation(String name, int cell, int row, int frames)
        {
            this.Name = name;
            this.Cell= cell;
            this.Row = row; 
            this.Frames = frames;   
        }
    }
}

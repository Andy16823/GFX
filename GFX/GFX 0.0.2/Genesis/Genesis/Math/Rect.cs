using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Math
{
    public class Rect
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }

        public Rect(float x, float y, float width, float height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public Rect()
        {

        }

        public bool Contains(float x, float y)
        {
            if(x > this.X && x < this.X + this.Width && y > this.Y && y < this.Y + this.Height)
            {
                return true;
            }
            return false;
        }

        public bool Intersects(Rect rect)
        {
            if (rect.X > X + Width || rect.Y + rect.Height < Y || rect.X + rect.Width < X || rect.Y > Y + Height)
            {
                return false;
            }
            return true;
        }

    }
}

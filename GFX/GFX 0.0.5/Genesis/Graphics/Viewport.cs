using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Graphics
{
    public class Viewport
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }

        public Viewport()
        {

        }

        public Viewport(float width, float height)
        {
            this.X = 0;
            this.Y = 0;
            this.Width = width; 
            this.Height = height;   
        }

        public Viewport(float x, float y, float width, float height) : this(width, height)
        {
            this.X = x;
            this.Y = y;
        }

        public void SetNewViewport(float width, float height)
        {
            this.Width = width;
            this.Height = height;
        }
    }
}

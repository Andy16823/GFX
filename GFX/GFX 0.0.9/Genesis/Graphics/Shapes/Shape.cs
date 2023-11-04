using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Graphics.Shapes
{
    public abstract class Shape
    {
        public int vbo { get; set; }

        public abstract float[] GetShape();
    }
}

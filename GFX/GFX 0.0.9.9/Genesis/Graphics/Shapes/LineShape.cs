using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Graphics.Shapes
{
    public class LineShape : Shape
    {
        public override float[] GetShape()
        {
            float[] vertices =
            {
               0f, 0f, 0f,
               1f, 1f, 1f
            };
            return vertices;
        }
    }
}

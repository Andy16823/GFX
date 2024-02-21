using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Graphics.Shapes
{
    public class FrameShape : Shape
    {
        public override float[] GetShape()
        {
            float[] verticies =
            {
                -1f, -1f, 0.0f,
                -1f, 1f, 0.0f,
                1f, 1f, 0.0f,

                -1f, -1f, 0.0f,
                1f, 1f, 0.0f,
                1f, -1f, 0.0f,

                //Tex Coords
                0.0f, 0.0f,
                0.0f, 1.0f,
                1.0f, 1.0f,

                0.0f, 0.0f,
                1.0f, 1.0f,
                1.0f, 0.0f
            };
            return verticies;
        }
    }
}

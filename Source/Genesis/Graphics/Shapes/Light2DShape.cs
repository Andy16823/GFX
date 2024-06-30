using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Graphics.Shapes
{
    public class Light2DShape : Shape
    {
        public override float[] GetShape()
        {
            float[] vericies =
            {
                -0.5f, -0.5f, 0.0f,
                -0.5f, 0.5f, 0.0f,
                0.5f, 0.5f, 0.0f,

                -0.5f, -0.5f, 0.0f,
                0.5f, 0.5f, 0.0f,
                0.5f, -0.5f, 0.0f,

                0.0f, 0.0f,
                0.0f, 1.0f,
                1.0f, 1.0f,

                0.0f, 0.0f,
                1.0f, 1.0f,
                1.0f, 0.0f
            };

            return vericies;
        }
    }
}

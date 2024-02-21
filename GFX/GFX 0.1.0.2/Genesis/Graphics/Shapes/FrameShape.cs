using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Graphics.Shapes
{
    /// <summary>
    /// Represents a shape that defines a frame or rectangle.
    /// </summary>
    public class FrameShape : Shape
    {
        /// <summary>
        /// Gets the vertices defining the frame shape.
        /// </summary>
        /// <returns>An array of floating-point values representing the vertices and texture coordinates.</returns>
        /// <remarks>
        /// The vertices are defined in the following order:
        /// - Vertex positions (x, y, z)
        /// - Texture coordinates (u, v)
        /// </remarks>
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

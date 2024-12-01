using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Graphics.Shapes
{
    /// <summary>
    /// Represents a shape for a buffered sprite in a graphics context.
    /// </summary>
    public class BufferedSpriteShape : Shape
    {
        /// <summary>
        /// Gets the vertices defining the buffered sprite shape.
        /// </summary>
        /// <returns>An array of floating-point values representing the vertices, colors, and texture coordinates of the buffered sprite.</returns>
        /// <remarks>
        /// The vertices are defined in groups of three for each triangle face:
        /// - Vertex positions (x, y, z)
        /// - Colors (r, g, b)
        /// - Texture coordinates (u, v)
        /// </remarks>
        public override float[] GetShape()
        {
            float[] verticies =
            {
                //Verticies
                -0.5f, -0.5f, 0.0f,
                -0.5f, 0.5f, 0.0f,
                0.5f, 0.5f, 0.0f,

                -0.5f, -0.5f, 0.0f,
                0.5f, 0.5f, 0.0f,
                0.5f, -0.5f, 0.0f,

                //Colors
                1.0f, 1.0f, 1.0f,
                1.0f, 1.0f, 1.0f,
                1.0f, 1.0f, 1.0f,

                1.0f, 1.0f, 1.0f,
                1.0f, 1.0f, 1.0f,
                1.0f, 1.0f, 1.0f,

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

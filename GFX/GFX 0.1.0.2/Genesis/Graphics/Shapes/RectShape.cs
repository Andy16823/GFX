using OpenObjectLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Graphics.Shapes
{
    /// <summary>
    /// Represents a shape that defines a rectangle in two-dimensional space.
    /// </summary>
    public class RectShape : Shape
    {
        /// <summary>
        /// Gets the vertices defining the rectangle shape.
        /// </summary>
        /// <returns>An array of floating-point values representing the vertices and colors of the rectangle.</returns>
        /// <remarks>
        /// The vertices are defined in the following order:
        /// - Vertex positions (x, y, z)
        /// - Colors (r, g, b)
        /// </remarks>
        public override float[] GetShape()
        {
            float[] verticies =
            {
                -0.5f, -0.5f, 0.0f,
                -0.5f, 0.5f, 0.0f,
                0.5f, 0.5f, 0.0f,

                -0.5f, -0.5f, 0.0f,
                0.5f, 0.5f, 0.0f,
                0.5f, -0.5f, 0.0f,

                1.0f, 1.0f, 1.0f,
                1.0f, 1.0f, 1.0f,
                1.0f, 1.0f, 1.0f,

                1.0f, 1.0f, 1.0f,
                1.0f, 1.0f, 1.0f,
                1.0f, 1.0f, 1.0f,
            };
            return verticies;
        }
    }
}

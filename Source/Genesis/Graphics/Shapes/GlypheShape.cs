using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Graphics.Shapes
{
    /// <summary>
    /// Represents a shape that defines a glyphe (glyph or character).
    /// </summary>
    public class GlypheShape : Shapes.Shape
    {
        /// <summary>
        /// Gets the vertices defining the glyphe shape.
        /// </summary>
        /// <returns>An array of floating-point values representing the vertices, colors, and texture coordinates.</returns>
        /// <remarks>
        /// The vertices are defined in the following order:
        /// - Vertex positions (x, y, z)
        /// - Colors (r, g, b)
        /// - Texture coordinates (u, v)
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

                0.0f, 1.0f,
                0.0f, 0.0f,
                1.0f, 0.0f,

                0.0f, 1.0f,
                1.0f, 0.0f,
                1.0f, 1.0f
            };
            return verticies;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Graphics.Shapes
{
    /// <summary>
    /// Represents a shape that defines a line segment.
    /// </summary>
    public class LineShape : Shape
    {
        /// <summary>
        /// Gets the vertices defining the line segment shape.
        /// </summary>
        /// <returns>An array of floating-point values representing the start and end points of the line.</returns>
        /// <remarks>
        /// The vertices are defined in the following order:
        /// - Start point (x, y, z)
        /// - End point (x, y, z)
        /// </remarks>
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

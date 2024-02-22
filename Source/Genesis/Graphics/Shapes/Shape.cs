using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Graphics.Shapes
{
    /// <summary>
    /// Represents an abstract base class for defining shapes in a graphics context.
    /// </summary>
    public abstract class Shape
    {
        /// <summary>
        /// Gets or sets the Vertex Buffer Object (VBO) associated with the shape.
        /// </summary>
        public int vbo { get; set; }

        /// <summary>
        /// Gets the vertices defining the shape.
        /// </summary>
        /// <returns>An array of floating-point values representing the vertices of the shape.</returns>
        public abstract float[] GetShape();
    }
}

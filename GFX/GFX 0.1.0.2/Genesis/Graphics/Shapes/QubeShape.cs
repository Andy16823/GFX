using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Graphics.Shapes
{
    /// <summary>
    /// Represents a shape that defines a cube in three-dimensional space.
    /// </summary>
    public class QubeShape : Shape
    {
        /// <summary>
        /// Gets the vertices defining the cube shape.
        /// </summary>
        /// <returns>An array of floating-point values representing the vertices of the cube.</returns>
        /// <remarks>
        /// The vertices are defined in groups of three for each triangle face.
        /// </remarks>
        public override float[] GetShape()
        {
            float[] vertices = {
                -0.5f, -0.5f, -0.5f,  
                 0.5f, -0.5f, -0.5f,
                 0.5f,  0.5f, -0.5f,  
                 0.5f,  0.5f, -0.5f,  
                -0.5f,  0.5f, -0.5f,  
                -0.5f, -0.5f, -0.5f,  

                -0.5f, -0.5f,  0.5f,  
                 0.5f, -0.5f,  0.5f,  
                 0.5f,  0.5f,  0.5f,  
                 0.5f,  0.5f,  0.5f,  
                -0.5f,  0.5f,  0.5f,  
                -0.5f, -0.5f,  0.5f,  

                -0.5f,  0.5f,  0.5f,
                -0.5f,  0.5f, -0.5f,
                -0.5f, -0.5f, -0.5f,
                -0.5f, -0.5f, -0.5f,
                -0.5f, -0.5f,  0.5f,
                -0.5f,  0.5f,  0.5f,

                 0.5f,  0.5f,  0.5f,
                 0.5f,  0.5f, -0.5f,
                 0.5f, -0.5f, -0.5f,
                 0.5f, -0.5f, -0.5f,
                 0.5f, -0.5f,  0.5f,
                 0.5f,  0.5f,  0.5f,

                -0.5f, -0.5f, -0.5f,
                 0.5f, -0.5f, -0.5f,
                 0.5f, -0.5f,  0.5f,
                 0.5f, -0.5f,  0.5f,
                -0.5f, -0.5f,  0.5f,
                -0.5f, -0.5f, -0.5f,

                -0.5f,  0.5f, -0.5f,
                 0.5f,  0.5f, -0.5f,
                 0.5f,  0.5f,  0.5f,
                 0.5f,  0.5f,  0.5f,
                -0.5f,  0.5f,  0.5f,
                -0.5f,  0.5f, -0.5f
            };
            return vertices;
        }

        /// <summary>
        /// Gets the normals defining the cube shape.
        /// </summary>
        /// <returns>An array of floating-point values representing the normals of the cube faces.</returns>
        /// <remarks>
        /// The normals are defined in groups of three for each triangle face.
        /// </remarks>
        public float[] GetNormals()
        {
            float[] normals = {
                0.0f,  0.0f, -1.0f,
                0.0f,  0.0f, -1.0f,
                0.0f,  0.0f, -1.0f,
                0.0f,  0.0f, -1.0f,
                0.0f,  0.0f, -1.0f,
                0.0f,  0.0f, -1.0f,

                0.0f,  0.0f, 1.0f,
                0.0f,  0.0f, 1.0f,
                0.0f,  0.0f, 1.0f,
                0.0f,  0.0f, 1.0f,
                0.0f,  0.0f, 1.0f,
                0.0f,  0.0f, 1.0f,

                -1.0f,  0.0f,  0.0f,
                -1.0f,  0.0f,  0.0f,
                -1.0f,  0.0f,  0.0f,
                -1.0f,  0.0f,  0.0f,
                -1.0f,  0.0f,  0.0f,
                -1.0f,  0.0f,  0.0f,

                1.0f,  0.0f,  0.0f,
                1.0f,  0.0f,  0.0f,
                1.0f,  0.0f,  0.0f,
                1.0f,  0.0f,  0.0f,
                1.0f,  0.0f,  0.0f,
                1.0f,  0.0f,  0.0f,

                0.0f, -1.0f,  0.0f,
                0.0f, -1.0f,  0.0f,
                0.0f, -1.0f,  0.0f,
                0.0f, -1.0f,  0.0f,
                0.0f, -1.0f,  0.0f,
                0.0f, -1.0f,  0.0f,

                0.0f,  1.0f,  0.0f,
                0.0f,  1.0f,  0.0f,
                0.0f,  1.0f,  0.0f,
                0.0f,  1.0f,  0.0f,
                0.0f,  1.0f,  0.0f,
                0.0f,  1.0f,  0.0f
            };
            return normals;
        }
    }
}

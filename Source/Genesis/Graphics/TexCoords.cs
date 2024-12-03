using Genesis.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Graphics
{
    /// <summary>
    /// Represents values for texture coordinates defining a rectangle.
    /// </summary>
    public class TexCoords
    {
        /// <summary>
        /// Gets or sets the texture coordinate for the top-left corner.
        /// </summary>
        public Vec3 TopLeft { get; set; } = new Vec3(0f, 0f);

        /// <summary>
        /// Gets or sets the texture coordinate for the top-right corner.
        /// </summary>
        public Vec3 TopRight { get; set; } = new Vec3(1f, 0f);

        /// <summary>
        /// Gets or sets the texture coordinate for the bottom-right corner.
        /// </summary>
        public Vec3 BottomRight { get; set; } = new Vec3(1f, 1f);

        /// <summary>
        /// Gets or sets the texture coordinate for the bottom-left corner.
        /// </summary>
        public Vec3 BottomLeft { get; set; } = new Vec3(0f, 1f);

        /// <summary>
        /// Default constructor for the TexCoords class.
        /// </summary>
        public TexCoords()
        {

        }

        /// <summary>
        /// Constructor for the TexCoords class that initializes the texture coordinates.
        /// </summary>
        /// <param name="topLeft">The texture coordinate for the top-left corner.</param>
        /// <param name="topRight">The texture coordinate for the top-right corner.</param>
        /// <param name="bottomRight">The texture coordinate for the bottom-right corner.</param>
        /// <param name="bottomLeft">The texture coordinate for the bottom-left corner.</param>
        public TexCoords(Vec3 topLeft, Vec3 topRight, Vec3 bottomRight, Vec3 bottomLeft) 
        {
            this.TopLeft = topLeft;
            this.TopRight = topRight;
            this.BottomRight = bottomRight;
            this.BottomLeft = bottomLeft;
        }

        /// <summary>
        /// Gets an array of floats representing the texture coordinates in the order (X, Y).
        /// </summary>
        /// <returns>An array of floats representing the texture coordinates.</returns>
        public float[] GetFloats()
        {
            return new float[] {
                TopLeft.X, TopLeft.Y,
                BottomLeft.X, BottomLeft.Y,
                BottomRight.X, BottomRight.Y,
                TopRight.X, TopRight.Y
            };
        }
    }
}

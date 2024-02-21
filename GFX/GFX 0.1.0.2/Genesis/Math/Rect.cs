using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Math
{
    /// <summary>
    /// Represents a rectangle with position and dimensions.
    /// </summary>
    public class Rect
    {
        /// <summary>
        /// Gets or sets the X-coordinate of the rectangle.
        /// </summary>
        public float X { get; set; }

        /// <summary>
        /// Gets or sets the Y-coordinate of the rectangle.
        /// </summary>
        public float Y { get; set; }

        /// <summary>
        /// Gets or sets the width of the rectangle.
        /// </summary>
        public float Width { get; set; }

        /// <summary>
        /// Gets or sets the height of the rectangle.
        /// </summary>
        public float Height { get; set; }

        /// <summary>
        /// Constructor for the Rect class that initializes the rectangle with specified position and dimensions.
        /// </summary>
        /// <param name="x">The X-coordinate of the rectangle.</param>
        /// <param name="y">The Y-coordinate of the rectangle.</param>
        /// <param name="width">The width of the rectangle.</param>
        /// <param name="height">The height of the rectangle.</param>
        public Rect(float x, float y, float width, float height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        /// <summary>
        /// Default constructor for the Rect class.
        /// </summary>
        public Rect()
        {

        }

        /// <summary>
        /// Checks if a point with specified coordinates is contained within the rectangle.
        /// </summary>
        /// <param name="x">The X-coordinate of the point.</param>
        /// <param name="y">The Y-coordinate of the point.</param>
        /// <returns>True if the point is contained within the rectangle, otherwise false.</returns>
        public bool Contains(float x, float y)
        {
            if(x > this.X && x < this.X + this.Width && y > this.Y && y < this.Y + this.Height)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Checks if the current rectangle intersects with another rectangle.
        /// </summary>
        /// <param name="rect">The other rectangle to check for intersection.</param>
        /// <returns>True if the rectangles intersect, otherwise false.</returns>
        public bool Intersects(Rect rect)
        {
            if (rect.X > X + Width || rect.Y + rect.Height < Y || rect.X + rect.Width < X || rect.Y > Y + Height)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Converts the rectangle to a string representation in the format "X;Y;Width;Height".
        /// </summary>
        /// <returns>A string representation of the rectangle.</returns>
        public override string ToString()
        {
            return X.ToString() + ";" + Y.ToString() + ";" + Width.ToString() + ";" + Height.ToString();
        }

    }
}

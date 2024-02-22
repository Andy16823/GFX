using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Graphics
{
    /// <summary>
    /// Represents a viewport used in graphics rendering.
    /// </summary>
    public class Viewport
    {
        /// <summary>
        /// Gets or sets the X-coordinate of the viewport.
        /// </summary>
        public float X { get; set; }

        /// <summary>
        /// Gets or sets the Y-coordinate of the viewport.
        /// </summary>
        public float Y { get; set; }

        /// <summary>
        /// Gets or sets the width of the viewport.
        /// </summary>
        public float Width { get; set; }

        /// <summary>
        /// Gets or sets the height of the viewport.
        /// </summary>
        public float Height { get; set; }

        /// <summary>
        /// Default constructor for the Viewport class.
        /// </summary>
        public Viewport()
        {

        }

        /// <summary>
        /// Constructor for the Viewport class that initializes the viewport with a specified width and height.
        /// </summary>
        /// <param name="width">The width of the viewport.</param>
        /// <param name="height">The height of the viewport.</param>
        public Viewport(float width, float height)
        {
            this.X = 0;
            this.Y = 0;
            this.Width = width; 
            this.Height = height;   
        }

        /// <summary>
        /// Constructor for the Viewport class that initializes the viewport with a specified X-coordinate, Y-coordinate, width, and height.
        /// </summary>
        /// <param name="x">The X-coordinate of the viewport.</param>
        /// <param name="y">The Y-coordinate of the viewport.</param>
        /// <param name="width">The width of the viewport.</param>
        /// <param name="height">The height of the viewport.</param>
        public Viewport(float x, float y, float width, float height) : this(width, height)
        {
            this.X = x;
            this.Y = y;
        }

        /// <summary>
        /// Sets a new width and height for the viewport.
        /// </summary>
        /// <param name="width">The new width of the viewport.</param>
        /// <param name="height">The new height of the viewport.</param>
        public void SetNewViewport(float width, float height)
        {
            this.Width = width;
            this.Height = height;
        }

        /// <summary>
        /// Gets the size of the viewport as a System.Drawing.Size.
        /// </summary>
        /// <returns>The size of the viewport as a System.Drawing.Size.</returns>
        public Size GetSize()
        {
            return new Size((int)this.Width, (int)this.Height);
        }

        /// <summary>
        /// Gets the size of the viewport as a System.Drawing.SizeF.
        /// </summary>
        /// <returns>The size of the viewport as a System.Drawing.SizeF.</returns>
        public SizeF GetSizeF()
        {
            return new SizeF(this.Width, this.Height);
        }

    }
}

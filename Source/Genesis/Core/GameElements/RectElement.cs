using Genesis.Graphics;
using Genesis.Math;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Core.GameElements
{
    /// <summary>
    /// Simple rectangle element
    /// </summary>
    public class RectElement : GameElement
    {
        /// <summary>
        /// Gets or sets the color of the rectangle border.
        /// </summary>
        public Color BorderColor { get; set; } = Color.GreenYellow;

        /// <summary>
        /// Gets or sets the width of the rectangle border.
        /// </summary>
        public float BorderWidth { get; set; } = 1.0f;

        /// <summary>
        /// Gets or sets a value indicating whether the rectangle has a border.
        /// </summary>
        public bool HasBorder { get; set; } = true;

        /// <summary>
        /// Gets or sets the fill color of the rectangle.
        /// </summary>
        public Color Fill { get; set; } = Color.Red;

        /// <summary>
        /// Gets or sets a value indicating whether the rectangle has fill color.
        /// </summary>
        public bool HasFill { get; set; } = true;

        /// <summary>
        /// Creates a new rectangle
        /// </summary>
        /// <param name="name">The name of the rectangle.</param>
        /// <param name="location">The location of the rectangle.</param>
        /// <param name="size">The size of the rectangle.</param>
        public RectElement(String name, Vec3 location, Vec3 size)
        {
            this.Name = name;
            this.Location= location;
            this.Size = size;   
        }

        /// <summary>
        /// Renders the rectangle
        /// </summary>
        /// <param name="game">The game instance.</param>
        /// <param name="renderDevice">The render device used for rendering.</param>
        public override void OnRender(Game game, IRenderDevice renderDevice)
        {
            base.OnRender(game, renderDevice);
            Rect rect = new Rect(Location.X, Location.Y, Size.X, Size.Y);
            renderDevice.ModelViewMatrix();
            renderDevice.PushMatrix();
            renderDevice.Translate(Location.X, Location.Y, 0.0f);
            //renderDevice.Rotate(15, new Vec3(1f, 0f, 0f));
            if(this.HasFill)
            {
                renderDevice.FillRect(rect, Fill);
            }
            if(this.HasBorder)
            {
                renderDevice.DrawRect(rect, BorderColor, BorderWidth);
            }
            renderDevice.PopMatrix();
        }
    }
}

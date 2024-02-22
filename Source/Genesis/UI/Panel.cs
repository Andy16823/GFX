using Genesis.Core;
using Genesis.Graphics;
using Genesis.Math;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.UI
{
    /// <summary>
    /// Represents a UI panel that can have a background image, background color, or both.
    /// </summary>
    public class Panel : Widget
    {
        /// <summary>
        /// Gets or sets the background image of the panel.
        /// </summary>
        public Texture BackgroundImage { get; set; }

        /// <summary>
        /// Gets or sets the background color of the panel.
        /// </summary>
        public Color BackgroundColor { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the panel has a background color.
        /// </summary>
        public bool HasBackgroundColor { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Panel"/> class with the specified name, location, and size.
        /// </summary>
        /// <param name="name">The name of the panel.</param>
        /// <param name="location">The location of the panel.</param>
        /// <param name="size">The size of the panel.</param>
        public Panel(String name, Vec3 location, Vec3 size) 
            : base()
        {
            this.Name = name;
            this.Location = location;
            this.Size = size;
        }

        /// <summary>
        /// Renders the panel, considering the background image and color.
        /// </summary>
        /// <param name="game">The game instance.</param>
        /// <param name="renderDevice">The render device used for rendering.</param>
        /// <param name="scene">The scene in which the panel is rendered.</param>
        /// <param name="canvas">The canvas to which the panel belongs.</param>
        public override void OnRender(Game game, IRenderDevice renderDevice, Scene scene, Canvas canvas)
        {
            Vec3 loc = GetRelativePos(canvas);

            if(this.HasBackgroundColor)
            {
                if (this.BackgroundColor != null)
                {
                    renderDevice.FillRect(new Rect(loc.X, loc.Y, Size.X, Size.Y), BackgroundColor);
                }
            }

            if(this.BackgroundImage != null)
            {
                renderDevice.DrawSprite(loc, this.Size, BackgroundImage);
            }
            base.OnRender(game, renderDevice, scene, canvas);
        }

    }
}

using Genesis.Core;
using Genesis.Graphics;
using Genesis.Math;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Genesis.UI
{
    /// <summary>
    /// Represents a progress bar UI element.
    /// </summary>
    public class ProgressBar : Widget
    {
        /// <summary>
        /// Gets or sets the maximum value of the progress bar.
        /// </summary>
        public float MaxValue { get; set; } = 100;

        /// <summary>
        /// Gets or sets the current value of the progress bar.
        /// </summary>
        public float Value { get; set; } = 50;

        /// <summary>
        /// Gets or sets the background color of the progress bar.
        /// </summary>
        public Color BackgroundColor { get; set; } = Color.FromArgb(86, 86, 86);

        /// <summary>
        /// Gets or sets the color of the progress bar.
        /// </summary>
        public Color BarColor { get; set; } = Color.Green;

        /// <summary>
        /// Gets or sets the color of the progress bar border.
        /// </summary>
        public Color BorderColor { get; set; } = Color.Black;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProgressBar"/> class with the specified name, location, and size.
        /// </summary>
        /// <param name="name">The name of the progress bar.</param>
        /// <param name="location">The location of the progress bar.</param>
        /// <param name="size">The size of the progress bar.</param>
        public ProgressBar(String name, Vec3 location, Vec3 size)
        {
            this.Name = name;
            this.Location = location;
            this.Size = size;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProgressBar"/> class with the specified name, location, and size.
        /// </summary>
        /// <param name="name">The name of the progress bar.</param>
        /// <param name="location">The location of the progress bar.</param>
        /// <param name="size">The size of the progress bar.</param>
        /// <param name="anchor">The anchor for the progress bar</param>
        public ProgressBar(String name, Vec3 location, Vec3 size, WidgetAnchor anchor)
        {
            this.Name = name;
            this.Location = location;
            this.Size = size;
            this.Anchor = anchor;
        }

        /// <summary>
        /// Renders the progress bar with its background, bar, and border.
        /// </summary>
        /// <param name="game">The game instance.</param>
        /// <param name="renderDevice">The render device used for rendering.</param>
        /// <param name="scene">The scene in which the progress bar is rendered.</param>
        /// <param name="canvas">The canvas to which the progress bar belongs.</param>
        public override void OnRender(Game game, IRenderDevice renderDevice, Scene scene, Canvas canvas)
        {
            base.OnRender(game, renderDevice, scene, canvas);
            var bounds = TransformBounds(new Rect(GetRelativePos(canvas), this.Size), this.Anchor);
            Vec3 loc = bounds.GetLocation();


            float hpct = Value / MaxValue * 100;
            float barWidth = this.Size.X * hpct / 100;
            float barX = loc.X - ((Size.X / 2) - (barWidth / 2));

            game.RenderDevice.FillRect(new Rect(loc.X, loc.Y, this.Size.X, this.Size.Y), BackgroundColor);
            game.RenderDevice.FillRect(new Rect(barX, loc.Y, barWidth, this.Size.Y), BarColor);
            game.RenderDevice.DrawRect(new Rect(loc.X, loc.Y, this.Size.X, this.Size.Y), BorderColor, 0.1f);
        }
    }
}

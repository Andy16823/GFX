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
    /// ProgressBar Class
    /// </summary>
    public class ProgressBar : Widget
    {
        public float MaxValue { get; set; } = 100;
        public float Value { get; set; } = 50;
        public Color BackgroundColor { get; set; } = Color.FromArgb(86, 86, 86);
        public Color BarColor { get; set; } = Color.Green;
        public Color BorderColor { get; set; } = Color.Black;

        /// <summary>
        /// Creates a new progress bar
        /// </summary>
        /// <param name="name"></param>
        /// <param name="location"></param>
        /// <param name="size"></param>
        public ProgressBar(String name, Vec3 location, Vec3 size)
        {
            this.Name = name;
            this.Location = location;
            this.Size = size;
        }

        /// <summary>
        /// Renders the progress bar
        /// </summary>
        /// <param name="game"></param>
        /// <param name="renderDevice"></param>
        /// <param name="scene"></param>
        /// <param name="canvas"></param>
        public override void OnRender(Game game, IRenderDevice renderDevice, Scene scene, Canvas canvas)
        {
            base.OnRender(game, renderDevice, scene, canvas);
            Vec3 loc = GetRelativePos(canvas);

            float hpct = Value / MaxValue * 100;
            float barWidth = this.Size.X * hpct / 100;
            float barX = loc.X - ((Size.X / 2) - (barWidth / 2));

            game.RenderDevice.FillRect(new Rect(loc.X, loc.Y, this.Size.X, this.Size.Y), BackgroundColor);
            game.RenderDevice.FillRect(new Rect(barX, loc.Y, barWidth, this.Size.Y), BarColor);
            game.RenderDevice.DrawRect(new Rect(loc.X, loc.Y, this.Size.X, this.Size.Y), BorderColor, 0.1f);
        }
    }
}

using Genesis.Graphics;
using Genesis.Math;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Core.Prefabs
{
    /// <summary>
    /// Simple rectangle element
    /// </summary>
    public class RectElement : GameElement
    {
        public Color BorderColor { get; set; } = Color.GreenYellow;
        public float BorderWidth { get; set; } = 1.0f;
        public bool HasBorder { get; set; } = true;
        public Color Fill { get; set; } = Color.Red;
        public bool HasFill { get; set; } = true;

        /// <summary>
        /// Creates a new rectangle
        /// </summary>
        /// <param name="name"></param>
        /// <param name="location"></param>
        /// <param name="size"></param>
        public RectElement(String name, Vec3 location, Vec3 size)
        {
            this.Name = name;
            this.Location= location;
            this.Size = size;   
        }

        /// <summary>
        /// Renders the rectangle
        /// </summary>
        /// <param name="game"></param>
        /// <param name="renderDevice"></param>
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

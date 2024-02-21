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
    public class Panel : Widget
    {
        public Texture BackgroundImage { get; set; }
        public Color BackgroundColor { get; set; }
        public bool HasBackgroundColor { get; set; }

        public Panel(String name, Vec3 location, Vec3 size) 
            : base()
        {
            this.Name = name;
            this.Location = location;
            this.Size = size;
        }

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

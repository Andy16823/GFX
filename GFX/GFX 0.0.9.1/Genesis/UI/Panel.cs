using Genesis.Core;
using Genesis.Graphics;
using Genesis.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.UI
{
    public class Panel : Entity
    {
        public Texture BackgroundImage { get; set; }

        public override void OnRender(Game game, IRenderDevice renderDevice, Scene scene, Canvas canvas)
        {
            Vec3 loc = GetRelativePos(canvas);
            if(this.BackgroundImage != null)
            {
                renderDevice.DrawSprite(loc, this.Size, BackgroundImage);
            }
            base.OnRender(game, renderDevice, scene, canvas);
        }

    }
}

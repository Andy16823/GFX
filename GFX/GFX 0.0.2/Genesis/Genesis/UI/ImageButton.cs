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
    public class ImageButton : Entity
    {
        public Texture NormalTexture { get; set; }
        public Texture HoverTexture { get; set; }

        public ImageButton(String name, Vec3 location, Vec3 size, Texture normalTexture, Texture hoverTexture)
        {
            this.Name= name;
            this.Location= location;    
            this.Size= size;
            this.NormalTexture= normalTexture;
            this.HoverTexture= hoverTexture;
        }

        public override void OnRender(Game game, IRenderDevice renderDevice, Scene scene, Canvas canvas)
        {
            base.OnRender(game, renderDevice, scene, canvas);
            Vec3 loc = GetRelativePos(canvas);
            if (IsHover(game,scene,canvas))
            {
                renderDevice.DrawSprite(loc, this.Size, this.HoverTexture);
            }
            else
            {
                renderDevice.DrawSprite(loc, this.Size, this.NormalTexture);
            }
        }

    }
}

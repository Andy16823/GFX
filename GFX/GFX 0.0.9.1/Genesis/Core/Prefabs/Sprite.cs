using Genesis.Graphics;
using Genesis.Math;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Core.Prefabs
{
    public class Sprite : GameElement
    {
        public Texture Texture { get; set; }
        public Color Color { get; set; } = Color.White;
        public TexCoords TexCoords { get; set; } = new TexCoords();
        public Boolean OcclusionCulling { get; set; }

        public Sprite(String name, Vec3 location, Vec3 size, Texture texture)
        {
            this.Name = name;
            this.Location= location;    
            this.Size= size;
            this.Texture= texture;
            this.OcclusionCulling = true;
        }

        public override void Init(Game game, IRenderDevice renderDevice)
        {
            base.Init(game, renderDevice);
            renderDevice.InitSprite(this);
            if(Texture.RenderID== 0 )
            {
                renderDevice.LoadTexture(Texture);
            }
        }

        public override void OnRender(Game game, IRenderDevice renderDevice)
        {
            base.OnRender(game, renderDevice);

            if(game.SelectedScene.Camera.GetRect().Intersects(this.GetBounds2D()))
            {
                renderDevice.DrawSprite(this);
            }
        }

        public override void OnDestroy(Game game)
        {
            base.OnDestroy(game);
            game.RenderDevice.DisposeTexture(Texture);
        }

        public Rect GetBounds2D()
        {
            return new Rect(Location.X, Location.Y, Size.X, Size.Y);
        }

        public Vec3 GetCenterLocation()
        {
            return new Vec3(Location.X + (Size.X / 2), Location.Y + (Size.Y / 2));
        }

        public float[] CalculateVerticies()
        {
            float LeftX = this.Location.X - (Size.X / 2);
            float RightX = this.Location.X + (Size.X / 2);
            float top = this.Location.Y + (Size.Y / 2);
            float bottom = this.Location.Y - (Size.Y / 2);
            
            return new float[]
            {
                LeftX, bottom, 0.0f,
                LeftX, top, 0.0f,
                RightX, top, 0.0f,

                LeftX, bottom, 0.0f,
                RightX, top, 0.0f,
                RightX, bottom, 0.0f
            };
        }

    }
}

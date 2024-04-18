using Genesis.Graphics;
using Genesis.Math;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Core
{
    public class Light2D : GameElement
    {
        public Texture LightShape { get; set; }
        public Color LightColor { get; set; } = Color.White;

        public Light2D(Vec3 location, Vec3 size)
        {
            this.Location = location;
            this.Size = size;
        }

        public Light2D(String name, Vec3 location, Vec3 size, Texture texture)
        {
            this.Name = name;
            this.Location = location;
            this.Size = size;
            this.LightShape = texture;
        }

        public override void Init(Game game, IRenderDevice renderDevice)
        {
            base.Init(game, renderDevice);
        }

        public override void OnRender(Game game, IRenderDevice renderDevice)
        {
            renderDevice.DrawGameElement(this);
            base.OnRender(game, renderDevice);
        }

        public override void OnDestroy(Game game)
        {
            base.OnDestroy(game);
            game.RenderDevice.DisposeElement(this);
        }

        public Vec3 GetColor()
        {
            var colors = Utils.ConvertColor(LightColor);
            return new Vec3(colors[0], colors[1], colors[2]);
        }

        public static float[] GetVericies()
        {
            float[] vericies =
            {
                -0.5f, -0.5f, 0.0f,
                -0.5f, 0.5f, 0.0f,
                0.5f, 0.5f, 0.0f,

                -0.5f, -0.5f, 0.0f,
                0.5f, 0.5f, 0.0f,
                0.5f, -0.5f, 0.0f,
            };
            return vericies;
        }

        public static float[] GetTexCoords()
        {
            float[] texcoords =
            {
                 0.0f, 0.0f,
                0.0f, 1.0f,
                1.0f, 1.0f,

                0.0f, 0.0f,
                1.0f, 1.0f,
                1.0f, 0.0f
            };
            return texcoords;
        }

        public void TestLight2D()
        {
            Console.WriteLine("Test");
        }

    }
}

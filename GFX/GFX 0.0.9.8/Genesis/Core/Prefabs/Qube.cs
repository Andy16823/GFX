using Genesis.Graphics;
using Genesis.Graphics.Shaders.OpenGL;
using Genesis.Graphics.Shapes;
using Genesis.Math;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Core.Prefabs
{
    public class Qube : GameElement
    {
        public ShaderProgram Shader { get; set; }
        public QubeShape Shape { get; set; }
        public Color Color { get; set; }

        public Qube(String name, Vec3 location)
        {
            this.Name = name;
            this.Location = location;
            this.Size = new Vec3(1f);
            this.Rotation = Vec3.Zero();
            this.Shape = new QubeShape();
            this.Shader = new DiffuseSolidShader();
            this.Color = Color.White;
        }

        public Qube(String Name, Vec3 location, Vec3 size)
        {
            this.Name = Name;
            this.Location = location;
            this.Size = size;
            this.Rotation = Vec3.Zero();
            this.Shape = new QubeShape();
            this.Shader = new DiffuseSolidShader();
            this.Color = Color.White;
        }

        public Qube(String name, Vec3 location, Vec3 size, Vec3 rotation)
        {
            this.Name = name;
            this.Location = location;
            this.Size = size;
            this.Rotation = rotation;
            this.Shape = new QubeShape();
            this.Shader = new DiffuseSolidShader();
            this.Color = Color.White;
        }

        public static float[] GetColors(Color color)
        {
            float r = (float)color.R / 255;
            float g = (float)color.G / 255;
            float b = (float)color.B / 255;

            float[] colorArray =
            {
                // Front
                r, g, b,
                r, g, b,
                r, g, b,
                r, g, b,
                r, g, b,
                r, g, b,

                //Back
                r, g, b,
                r, g, b,
                r, g, b,
                r, g, b,
                r, g, b,
                r, g, b,

                //Right
                r, g, b,
                r, g, b,
                r, g, b,
                r, g, b,
                r, g, b,
                r, g, b,

                //Left
                r, g, b,
                r, g, b,
                r, g, b,
                r, g, b,
                r, g, b,
                r, g, b,

                //Top
                r, g, b,
                r, g, b,
                r, g, b,
                r, g, b,
                r, g, b,
                r, g, b,

                //Bottom
                r, g, b,
                r, g, b,
                r, g, b,
                r, g, b,
                r, g, b,
                r, g, b
            };
            return colorArray;
        }

        public override void Init(Game game, IRenderDevice renderDevice)
        {
            base.Init(game, renderDevice);
        }

        public override void OnRender(Game game, IRenderDevice renderDevice)
        {
            base.OnRender(game, renderDevice);
            renderDevice.DrawGameElement(this);
        }
    }
}

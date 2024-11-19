using Genesis.Graphics;
using Genesis.Graphics.Shapes;
using Genesis.Math;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Core.GameElements
{
    public class Sphere : GameElement
    {
        public ShaderProgram Shader { get; set; }
        public SphereShape Shape { get; set; }
        public Material Material { get; set; }
        public Color Color
        {
            get => Material.DiffuseColor;
            set => Material.DiffuseColor = value;
        }

        public Sphere(String name, Vec3 location, Vec3 size, Vec3 rotation)
        {
            this.Name = name;
            this.Location = location;
            this.Size = size;
            this.Rotation = rotation;
            this.Material = new Material();
            this.Color = Color.Green;
            this.Shape = new SphereShape();
            this.Shader = new Graphics.Shaders.OpenGL.DiffuseLightning();
        }

        public Sphere(String name, Vec3 location, Vec3 size, Vec3 rotation, int latitudebands =  20, int longitudebands = 20, float radius = 0.5f)
        {
            this.Name = name;
            this.Location = location;
            this.Size = size;
            this.Rotation = rotation;
            this.Material = new Material();
            this.Color = Color.Green;
            this.Shape = new SphereShape();
            this.Shape.LatitudeBands = latitudebands;
            this.Shape.LongitudeBands = longitudebands;
            this.Shape.Radius = radius;
            this.Shader = new Graphics.Shaders.OpenGL.DiffuseLightning();
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

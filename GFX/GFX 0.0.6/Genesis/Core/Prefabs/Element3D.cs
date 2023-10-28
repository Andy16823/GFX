using Genesis.Graphics;
using Genesis.Math;
using GlmSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Core.Prefabs
{
    public class Element3D : GameElement
    {
        public Vec3 Location { get; set; }
        public Vec3 Rotation { get; set; }
        public Vec3 Scale { get; set; }
        public OpenObjectLoader.Model Model { get; set; }

        public Element3D(String name, String path, Vec3 location, Vec3 rotation, Vec3 scale)
        {
            this.Name = name;
            this.Location = location;
            this.Rotation = rotation;
            this.Scale = scale;
            this.Model = new OpenObjectLoader.WavefrontLoader().LoadModel(path);
            this.Model.Propertys.Add("path", new System.IO.FileInfo(path).DirectoryName);
        }

        public override void Init(Game game, IRenderDevice renderDevice)
        {
            base.Init(game, renderDevice);
            renderDevice.InitElement3D(this);
        }

        public override void OnRender(Game game, IRenderDevice renderDevice)
        {
            base.OnRender(game, renderDevice);
            renderDevice.DrawElement3D(this);
        }

    }
}

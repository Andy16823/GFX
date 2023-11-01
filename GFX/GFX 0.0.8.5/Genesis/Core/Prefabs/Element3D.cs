using BulletSharp;
using BulletSharp.SoftBody;
using Genesis.Graphics;
using Genesis.Math;
using GlmSharp;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BulletSharp.Math;
using System.Xml.Linq;

namespace Genesis.Core.Prefabs
{
    public class Element3D : GameElement
    {
        public ShaderProgram Shader { get; set; }
        public OpenObjectLoader.Model Model { get; set; }

        public Element3D(String name, String path, Vec3 location, Vec3 rotation, Vec3 scale)
        {
            this.Name = name;
            this.Location = location;
            this.Rotation = rotation;
            this.Size = scale;
            this.Model = new OpenObjectLoader.WavefrontLoader().LoadModel(path);
            this.Model.Propertys.Add("path", new System.IO.FileInfo(path).DirectoryName);
        }


        public override void OnUpdate(Game game, IRenderDevice renderDevice)
        {
            base.OnUpdate(game, renderDevice);
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

        public override void OnDestroy(Game game)
        {
            base.OnDestroy(game);
            game.RenderDevice.DisposeElement3D(this);
        }

    }
}

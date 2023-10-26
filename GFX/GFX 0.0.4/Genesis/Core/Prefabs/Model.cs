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
    public class Model : GameElement
    {
        public Mesh Mesh { get; set; }

        public Model(Mesh mesh) { 
            this.Mesh = mesh;
        }

        public override void Init(Game game, IRenderDevice renderDevice)
        {
            base.Init(game, renderDevice);
            this.Mesh.InitMesh(renderDevice);
        }

        public override void OnRender(Game game, IRenderDevice renderDevice)
        {
            base.OnRender(game, renderDevice);
            float tX = Location.X;
            float tY = Location.Y;

            renderDevice.ModelViewMatrix();
            renderDevice.PushMatrix();
            renderDevice.Translate(tX, tY, 0.0f);
            //renderDevice.Rotate(0f, new Vec3(0f, 0f, 200f));
            //renderDevice.Translate(-tX, -tY, 0.0f);
            renderDevice.DrawMesh(this.Mesh, Color.White);
            renderDevice.PopMatrix();
        }
    }
}

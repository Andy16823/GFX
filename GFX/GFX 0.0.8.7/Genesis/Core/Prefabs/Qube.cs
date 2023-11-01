using Genesis.Graphics;
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
        private Vec3[] vecs;
        public Qube(String Name, Vec3 location, Vec3 size)
        {
            this.Name = Name;
            this.Location = location;
            this.Size = size;
            vecs = GetVerticies();
        }

        public Vec3[] GetVerticies()
        {
            Vec3[] vecs =
            {
                // Front
                new Vec3(Location.X, Location.Y, Location.Z),
                new Vec3(Location.X, Location.Y + Size.Y, Location.Z),
                new Vec3(Location.X + Size.X, Location.Y + Size.Y, Location.Z),
                new Vec3(Location.X + Size.X, Location.Y, Location.Z),
                //Left
                new Vec3(Location.X, Location.Y, Location.Z),
                new Vec3(Location.X, Location.Y, Location.Z - Size.Z),
                new Vec3(Location.X, Location.Y + Size.Y, Location.Z - Size.Z),
                new Vec3(Location.X, Location.Y + Size.Y, Location.Z),
                // Back
                new Vec3(Location.X, Location.Y, Location.Z - Size.Z),
                new Vec3(Location.X, Location.Y + Size.Y, Location.Z - Size.Z),
                new Vec3(Location.X + Size.X, Location.Y + Size.Y, Location.Z - Size.Z),
                new Vec3(Location.X + Size.X, Location.Y, Location.Z - Size.Z),
                //Right
                new Vec3(Location.X + Size.X, Location.Y, Location.Z),
                new Vec3(Location.X + Size.X, Location.Y, Location.Z - Size.Z),
                new Vec3(Location.X + Size.X, Location.Y + Size.Y, Location.Z - Size.Z),
                new Vec3(Location.X + Size.X, Location.Y + Size.Y, Location.Z)
            };
            return vecs;
        }

        public override void OnRender(Game game, IRenderDevice renderDevice)
        {
            base.OnRender(game, renderDevice);
            renderDevice.ModelViewMatrix();
            renderDevice.PushMatrix();
            renderDevice.Translate(Location.X, Location.Y, 0.0f);
            //renderDevice.Rotate(15, new Vec3(1f, 0f, 0f));
            renderDevice.DrawVectors(vecs, Color.Green);
            renderDevice.PopMatrix();
        }
    }
}

using Genesis.Core;
using Genesis.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Graphics
{
    public enum CameraType
    {
        Ortho,
        Perspective
    }

    public class Camera
    {
        public Vec3 Location { get; set; }
        public Vec3 Size { get; set; }
        public float Near { get; set; }
        public float Far { get; set; }
        public CameraType Type { get; set; }

        public Camera(Vec3 location, Vec3 size, float near, float far)
        {
            Location = location;
            Size = size;
            Near = near;
            Far = far;
        }

        public void LookAt(GameElement element)
        {
            this.Location.X = element.Location.X + element.Size.X / 2;
            this.Location.Y = element.Location.Y + element.Size.Y / 2;
        }

    }
}

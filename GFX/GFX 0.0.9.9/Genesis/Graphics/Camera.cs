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
        public Vec3 Rotation { get; set; }
        public float Near { get; set; }
        public float Far { get; set; }
        public CameraType Type { get; set; }

        public Camera(Vec3 location, Vec3 size, float near, float far)
        {
            Location = location;
            Size = size;
            Near = near;
            Far = far;
            this.Rotation = new Vec3(0f);
        }

        public void LookAt(GameElement element)
        {
            this.Location.X = element.Location.X + element.Size.X / 2;
            this.Location.Y = element.Location.Y + element.Size.Y / 2;
        }

        public void LookAt(GameElement element, bool centerOffset)
        {
            if(centerOffset)
            {
                LookAt(element);
            }
            else
            {
                this.Location.X = element.Location.X;
                this.Location.Y = element.Location.Y;
            }
        }

        public Rect GetRect()
        {
            return new Rect(Location.X - (Size.X / 2), Location.Y - (Size.Y / 2), Size.X, Size.Y);
        }

    }
}

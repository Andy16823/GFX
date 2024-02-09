using Genesis.Graphics;
using Genesis.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Core.Prefabs
{
    public class CameraElement : GameElement
    {
        public Camera Camera { get; set; }

        public Vec3 Location
        {
            get => Camera.Location;
            set => Camera.Location = value;
        }

        public Vec3 Rotation
        {
            get => Camera.Rotation;
            set => Camera.Rotation = value;
        }

        public Vec3 Size
        {
            get => Camera.Size;
            set => Camera.Size = value;
        }

        public CameraElement(String name, Vec3 location, Vec3 cameraSize, float near, float far, CameraType type)
        {
            this.Name = name;
            this.Camera = new Camera(location, cameraSize, near, far);
            this.Camera.Type = type;
        }

        public CameraElement(String name, Camera camera)
        {
            this.Name = name;
            this.Camera = camera;
        }

    }
}

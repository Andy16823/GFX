using Genesis.Core;
using Genesis.Math;
using GlmSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Graphics
{
    public class PerspectiveCamera : Camera
    {
        public PerspectiveCamera(Vec3 location, Vec3 size, float near, float far) : base(location, size, near, far)
        {
            this.Type = CameraType.Perspective;
        }

        public Vec3 CameraFront()
        {
            return Utils.CalculateCameraFront2(this);
        }

        public void MoveForward(float value)
        {
            this.Location += Utils.CalculateCameraFront2(this) * value;
        }

        public void MoveBackward(float value)
        {
            this.Location -= Utils.CalculateCameraFront2(this) * value;
        }

        public void MoveUp(float value)
        {
            this.Location += new Vec3(0.0f, value, 0.0f);
        }

        public void MoveDown(float value)
        {
            this.Location -= new Vec3(0.0f, value, 0.0f);
        }

        public void MoveLeft(float value)
        {
            this.Location -= Vec3.Cross(Utils.CalculateCameraFront2(this), new Vec3(0.0f, 1.0f, 0.0f)) * value;
        }

        public void MoveRight(float value)
        {
            this.Location += Vec3.Cross(Utils.CalculateCameraFront2(this), new Vec3(0.0f, 1.0f, 0.0f)) * value;
        } 
        public Vec3 Forward(float distance)
        {
            return this.Location + Utils.CalculateCameraFront2(this) * distance;
        }
    }
}

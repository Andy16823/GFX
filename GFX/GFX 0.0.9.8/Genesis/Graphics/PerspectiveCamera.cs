using Genesis.Core;
using Genesis.Graphics.RenderDevice;
using Genesis.Math;
using GlmSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Genesis.Graphics
{
    public class PerspectiveCamera : Camera
    {
        private Framebuffer _frameBuffer;
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

        public void RenderToTexture(Game game, IRenderDevice renderer, Texture renderTarget, Vec3 resolution)
        {
            this.RenderToTexture(game, renderer, renderTarget.RenderID, resolution);
        }

        public void RenderToTexture(Game game, IRenderDevice renderer, int renderTarget, Vec3 resolution)
        {
            if (game.SelectedScene != null)
            {
                if (_frameBuffer == null)
                {
                    _frameBuffer = renderer.BuildFramebuffer((int)resolution.X, (int)resolution.Y, renderTarget);
                }
                renderer.Viewport(game.Viewport.X, game.Viewport.Y, (int)resolution.X, (int)resolution.Y);
                renderer.SetCamera(this);
                renderer.UpdateFramebufferSize(_frameBuffer, (int)resolution.X, (int)resolution.Y);
                renderer.SetFramebuffer(_frameBuffer);
                foreach (var layer in game.SelectedScene.Layer)
                {
                    layer.OnRender(game, renderer);
                }
                renderer.Viewport(game.Viewport.X, game.Viewport.Y, game.Viewport.Width, game.Viewport.Height);
                renderer.SetCamera(game.SelectedScene.Camera);
            }
        }
    }
}

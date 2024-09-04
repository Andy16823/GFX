using Assimp;
using Genesis.Core;
using Genesis.Graphics.RenderDevice;
using Genesis.Math;
using GlmSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Genesis.Graphics
{
    /// <summary>
    /// Represents a perspective camera derived from the Camera class.
    /// </summary>
    public class PerspectiveCamera : Camera
    {
        private Framebuffer _frameBuffer;

        /// <summary>
        /// Constructor for the PerspectiveCamera class.
        /// </summary>
        /// <param name="location">The position of the camera.</param>
        /// <param name="size">The size of the camera.</param>
        /// <param name="near">The near clipping plane of the camera.</param>
        /// <param name="far">The far clipping plane of the camera.</param>
        public PerspectiveCamera(Vec3 location, Vec3 size, float near, float far) : base(location, size, near, far)
        {
            this.Type = CameraType.Perspective;
        }

        /// <summary>
        /// Gets the front direction vector of the camera.
        /// </summary>
        /// <returns>The front direction vector of the camera.</returns>
        public Vec3 CameraFront()
        {
            return Utils.CalculateCameraFront2(this);
        }

        public Vec3 CameraRight()
        {
            var forward = this.CameraFront().ToGlmVec3();
            var right = vec3.Cross(new vec3(0, 1, 0), forward).Normalized;
            return new Vec3(right);
        }

        public Vec3 CameraUp()
        {
            var forward = this.CameraFront().ToGlmVec3();
            var right = vec3.Cross(new vec3(0, 1, 0), forward).Normalized;
            var up = vec3.Cross(forward, right).Normalized;

            return new Vec3(up);
        }

        /// <summary>
        /// Moves the camera forward by a specified value.
        /// </summary>
        /// <param name="value">The distance to move the camera forward.</param>
        public void MoveForward(float value)
        {
            this.Location += Utils.CalculateCameraFront2(this) * value;
        }

        /// <summary>
        /// Moves the camera backward by a specified value.
        /// </summary>
        /// <param name="value">The distance to move the camera backward.</param>
        public void MoveBackward(float value)
        {
            this.Location -= Utils.CalculateCameraFront2(this) * value;
        }

        /// <summary>
        /// Moves the camera up by a specified value.
        /// </summary>
        /// <param name="value">The distance to move the camera up.</param>
        public void MoveUp(float value)
        {
            this.Location += new Vec3(0.0f, value, 0.0f);
        }

        /// <summary>
        /// Moves the camera down by a specified value.
        /// </summary>
        /// <param name="value">The distance to move the camera down.</param>
        public void MoveDown(float value)
        {
            this.Location -= new Vec3(0.0f, value, 0.0f);
        }

        /// <summary>
        /// Moves the camera left by a specified value.
        /// </summary>
        /// <param name="value">The distance to move the camera left.</param>
        public void MoveLeft(float value)
        {
            this.Location -= Vec3.Cross(Utils.CalculateCameraFront2(this), new Vec3(0.0f, 1.0f, 0.0f)) * value;
        }

        /// <summary>
        /// Moves the camera right by a specified value.
        /// </summary>
        /// <param name="value">The distance to move the camera right.</param>
        public void MoveRight(float value)
        {
            this.Location += Vec3.Cross(Utils.CalculateCameraFront2(this), new Vec3(0.0f, 1.0f, 0.0f)) * value;
        }

        /// <summary>
        /// Calculates a point in front of the camera at a specified distance.
        /// </summary>
        /// <param name="distance">The distance from the camera.</param>
        /// <returns>The calculated point in front of the camera.</returns>
        public Vec3 Forward(float distance)
        {
            return this.Location + Utils.CalculateCameraFront2(this) * distance;
        }

        /// <summary>
        /// Renders the scene to a texture using a specified render target and resolution.
        /// </summary>
        /// <param name="game">The Game instance.</param>
        /// <param name="renderer">The render device.</param>
        /// <param name="renderTarget">The render target texture.</param>
        /// <param name="resolution">The resolution of the rendered texture.</param>
        public void RenderToTexture(Game game, IRenderDevice renderer, Texture renderTarget, Vec3 resolution)
        {
            this.RenderToTexture(game, renderer, renderTarget.RenderID, resolution);
        }

        /// <summary>
        /// Renders the scene to a texture using a specified render target ID and resolution.
        /// </summary>
        /// <param name="game">The Game instance.</param>
        /// <param name="renderer">The render device.</param>
        /// <param name="renderTarget">The ID of the render target.</param>
        /// <param name="resolution">The resolution of the rendered texture.</param>
        public void RenderToTexture(Game game, IRenderDevice renderer, int renderTarget, Vec3 resolution)
        {
            if (game.SelectedScene != null)
            {
                if (_frameBuffer == null)
                {
                    _frameBuffer = renderer.BuildFramebuffer((int)resolution.X, (int)resolution.Y, renderTarget);
                }
                renderer.SetViewport(game.Viewport);
                renderer.SetCamera(game.Viewport, this);
                renderer.UpdateFramebufferSize(_frameBuffer, (int)resolution.X, (int)resolution.Y);
                renderer.SetFramebuffer(_frameBuffer);
                foreach (var layer in game.SelectedScene.Layer)
                {
                    layer.OnRender(game, renderer);
                }
                renderer.SetViewport(game.Viewport);
                renderer.SetCamera(game.Viewport, game.SelectedScene.Camera);
            }
        }

        /// <summary>
        /// Adjusts the camera's orientation to look at a specified position.
        /// </summary>
        /// <param name="vec3">The position to look at.</param>
        public void LookAt(Vec3 vec3)
        {
            Utils.LookAt(this, vec3);
        }

        /// <summary>
        /// ToDo: Change FOV
        /// </summary>
        /// <param name="viewport"></param>
        /// <returns></returns>
        public Frustum GetFrustum(Viewport viewport)
        {
            var cUp = this.CameraUp().ToGlmVec3();
            var cRight = this.CameraRight().ToGlmVec3(); 

            float correction = this.CalculateScreenCorrection(viewport);
            float aspectRatio = (viewport.Width * correction) / (viewport.Height * correction);
            float fov = glm.Radians(this.FOV);
            float nearDist = this.Near;
            float farDist = this.Far;
            float hNear = (float)(2 * System.Math.Tan(fov / 2) * nearDist);
            float wNear = hNear * aspectRatio;
            float hFar = (float)(2 * System.Math.Tan(fov / 2) * farDist);
            float wFar = hFar * aspectRatio;

            Frustum frustum = new Frustum();
            frustum.centerFar = this.Location.ToGlmVec3() + this.CameraFront().ToGlmVec3() * farDist;
            frustum.topLeftFar = frustum.centerFar + (cUp * hFar /2) - (cRight * wFar /2);
            frustum.topRightFar = frustum.centerFar + (cUp * hFar/2) + (cRight * wFar /2);
            frustum.bottomLeftFar = frustum.centerFar - (cUp * hFar / 2) - (cRight * wFar /2);
            frustum.bottomRightFar = frustum.centerFar - (cUp * hFar / 2) + (cRight * wFar /2);
            frustum.centerNear = this.Location.ToGlmVec3() + this.CameraFront().ToGlmVec3() * nearDist;
            frustum.topLeftNear = frustum.centerNear + (cUp * hNear/2) - (cRight * wNear /2);
            frustum.topRightNear = frustum.centerNear + (cUp * hNear/2) + (cRight * wNear /2);
            frustum.bottomLeftNear = frustum.centerNear - (cUp * hNear/2) - (cRight * wNear /2);
            frustum.bottomRightNear = frustum.centerNear - (-cUp * hNear/2) + (cRight *wNear/2);
            frustum.center = (frustum.centerFar - frustum.centerNear) * 0.5f;

            return frustum;
        }

    }
}

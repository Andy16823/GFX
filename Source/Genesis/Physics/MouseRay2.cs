using BulletSharp.Math;
using Genesis.Core;
using Genesis.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Genesis.Physics
{
    /// <summary>
    /// Represents a class for calculating a ray based on mouse input.
    /// </summary>
    public class MouseRay2
    {
        /// <summary>
        /// Gets the ray direction from the camera's location to a specified point on the screen.
        /// </summary>
        /// <param name="point">The screen point</param>
        /// <param name="camera">The camera used for perspective</param>
        /// <param name="viewport">The viewport representing the screen size</param>
        /// <returns>The direction of the ray in world coordinates</returns>
        public Vector3 GetRayTo(Point point, PerspectiveCamera camera, Viewport viewport)
        {
            float aspect;

            Vector3 cameraFront = Utils.CalculateCameraFront2(camera).ToBulletVec3();
            Vector3 eye = camera.Location.ToBulletVec3();
            Vector3 target = Vector3.Add(eye, cameraFront);
            float fov = camera.FOV;

            Vector3 rayForward = target - eye;
            rayForward.Normalize();
            const float farPlane = 10000.0f;
            rayForward *= farPlane;

            Vector3 vertical = new Vector3(0.0f, 1.0f, 0.0f);

            Vector3 hor = Vector3.Cross(rayForward, vertical);
            hor.Normalize();
            vertical = Vector3.Cross(hor, rayForward);
            vertical.Normalize();

            float tanFov = (float)System.Math.Tan(fov / 2);
            hor *= 2.0f * farPlane * tanFov;
            vertical *= 2.0f * farPlane * tanFov;

            Size clientSize = viewport.GetSize();
            if (clientSize.Width > clientSize.Height)
            {
                aspect = (float)clientSize.Width / (float)clientSize.Height;
                hor *= aspect;
            }
            else
            {
                aspect = (float)clientSize.Height / (float)clientSize.Width;
                vertical *= aspect;
            }

            Vector3 rayToCenter = eye + rayForward;
            Vector3 dHor = hor / (float)clientSize.Width;
            Vector3 dVert = vertical / (float)clientSize.Height;

            Vector3 rayTo = rayToCenter - 0.5f * hor + 0.5f * vertical;
            rayTo += (clientSize.Width - point.X) * dHor;
            rayTo -= point.Y * dVert;
            return rayTo;
        }
    }
}

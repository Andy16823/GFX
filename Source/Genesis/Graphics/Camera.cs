using Genesis.Core;
using Genesis.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Graphics
{
    public enum CameraType
    {
        Ortho,
        Perspective
    }

    /// <summary>
    /// Represents a camera in a graphics context for rendering 2D scenes.
    /// </summary>
    public class Camera
    {
        /// <summary>
        /// Gets or sets the location of the camera in 3D space.
        /// </summary>
        public Vec3 Location { get; set; }

        /// <summary>
        /// Gets or sets the size of the camera.
        /// </summary>
        public Vec3 Size { get; set; }

        /// <summary>
        /// Gets or sets the rotation of the camera in 3D space.
        /// </summary>
        public Vec3 Rotation { get; set; }

        /// <summary>
        /// Gets or sets the distance to the near clipping plane of the camera.
        /// </summary>
        public float Near { get; set; }

        /// <summary>
        /// Gets or sets the distance to the far clipping plane of the camera.
        /// </summary>
        public float Far { get; set; }

        /// <summary>
        /// Gets or sets the type of the camera, either orthographic or perspective.
        /// </summary>
        public CameraType Type { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Camera"/> class with the specified location, size, near and far distances.
        /// </summary>
        /// <param name="location">The initial location of the camera in 3D space.</param>
        /// <param name="size">The size of the camera.</param>
        /// <param name="near">The distance to the near clipping plane of the camera.</param>
        /// <param name="far">The distance to the far clipping plane of the camera.</param>
        public Camera(Vec3 location, Vec3 size, float near, float far)
        {
            Location = location;
            Size = size;
            Near = near;
            Far = far;
            this.Rotation = new Vec3(0f);
        }

        /// <summary>
        /// Adjusts the camera to look at the specified game element (only 2D).
        /// </summary>
        /// <param name="element">The game element to look at.</param>
        public void LookAt(GameElement element)
        {
            this.Location.X = element.Location.X + element.Size.X / 2;
            this.Location.Y = element.Location.Y + element.Size.Y / 2;
        }

        /// <summary>
        /// Adjusts the camera to look at the specified game element, with an option to center the view on the element.
        /// </summary>
        /// <param name="element">The game element to look at.</param>
        /// <param name="centerOffset">If true, centers the view on the element; otherwise, positions the camera at the element's location.</param>
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

        /// <summary>
        /// Gets the rectangular region represented by the camera's location and size.
        /// </summary>
        /// <returns>A <see cref="Rect"/> object representing the camera's view region.</returns>
        public Rect GetRect()
        {
            return new Rect(Location.X - (Size.X / 2), Location.Y - (Size.Y / 2), Size.X, Size.Y);
        }

        /// <summary>
        /// Projects the mouse coords into screen coords
        /// </summary>
        /// <param name="camera">The camera</param>
        /// <param name="viewport">The viewport</param>
        /// <param name="mouseX">the screen x coordinate</param>
        /// <param name="mouseY">the screen y coordinate</param>
        /// <returns></returns>
        public static Vec3 ProjectMouse2D(Camera camera, Viewport viewport, int mouseX, int mouseY)
        {
            var x = mouseX - (viewport.Width / 2) + camera.Location.X;
            var y = mouseY - (viewport.Height / 2) + camera.Location.Y;
            return new Vec3(x, y);
        }

    }
}

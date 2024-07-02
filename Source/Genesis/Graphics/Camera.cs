using Assimp;
using Genesis.Core;
using Genesis.Math;
using GlmSharp;
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
        /// Initializes a new instance of the <see cref="Camera"/> class with the specified location, size, near and far distances.
        /// </summary>
        /// <param name="viewport">The viewport for the location and resoulution.</param>
        /// <param name="near">The distance to the near clipping plane of the camera.</param>
        /// <param name="far">The distance to the far clipping plane of the camera.</param>
        public Camera(Viewport viewport, float near, float far)
        {
            Location = new Vec3(viewport.X, viewport.Y);
            Size = new Vec3(viewport.Width, viewport.Height);
            Near = near;
            Far = far;
            this.Rotation = Vec3.Zero();
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
        /// Calculates the screen correction factor.
        /// </summary>
        /// <param name="screenWidth">The width of the screen.</param>
        /// <param name="screenHeight">The height of the screen.</param>
        /// <returns>
        /// The correction factor to be applied to the screen size,
        /// based on the smaller ratio of screen width to desired width (Size.X)
        /// and screen height to desired height (Size.Y).
        /// </returns>
        public float CalculateScreenCorrection(float screenWidth, float screenHeight)
        {
            return System.Math.Min(screenWidth / Size.X, screenHeight / Size.Y);
        }

        /// <summary>
        /// Calculates the screen correction factor using the dimensions of a viewport.
        /// </summary>
        /// <param name="viewport">The viewport containing the screen dimensions.</param>
        /// <returns>
        /// The correction factor to be applied to the screen size,
        /// based on the smaller ratio of viewport width to desired width (Size.X)
        /// and viewport height to desired height (Size.Y).
        /// </returns>
        public float CalculateScreenCorrection(Viewport viewport)
        {
            return CalculateScreenCorrection(viewport.Width, viewport.Height);
        }

        /// <summary>
        /// Converts screen coordinates to world coordinates using an orthographic projection.
        /// </summary>
        /// <param name="viewport">The viewport object that defines the dimensions of the screen space.</param>
        /// <param name="x">The x-coordinate of the screen position to convert.</param>
        /// <param name="y">The y-coordinate of the screen position to convert.</param>
        /// <returns>A Vec3 object representing the corresponding world coordinates.</returns>
        /// <remarks>
        /// This method assumes an orthographic projection and calculates the world coordinates
        /// based on the screen position, the camera's position, and the size of the viewport.
        /// The screen coordinates are first normalized to a range of [-1, 1], then adjusted
        /// according to the camera's position and size.
        /// </remarks>
        public Vec3 ConvertScreenToWorldOrtho(Viewport viewport, float x, float y)
        {
            float correction = this.CalculateScreenCorrection(viewport);
            float halfWidth = (viewport.Width / 2) / correction;
            float halfHeight = (viewport.Height / 2) / correction;

            float left = this.Location.X - halfWidth;
            float right = this.Location.X + halfWidth;
            float bottom = this.Location.Y - halfHeight;
            float top = this.Location.Y + halfHeight;

            var p_mat = mat4.Ortho(left, right, bottom, top, 0.1f, 100.0f);
            var ivmat = p_mat.Inverse;

            // Normalisierung der Bildschirmkoordinaten (0 bis 1)
            float normalizedX = (x - viewport.X) / viewport.Width;
            float normalizedY = (y - viewport.Y) / viewport.Height;

            // Umwandlung der normalisierten Koordinaten in den Bereich (-1 bis 1)
            float ndcX = normalizedX * 2 - 1;
            float ndcY = 1 - normalizedY * 2; // Y-Achse invertiert (oben nach unten)

            // Erstellen eines Vektors in NDC-Koordinaten
            vec4 ndcPosition = new vec4(ndcX, ndcY, 0, 1);

            // Transformation in Weltkoordinaten
            vec4 worldPosition = ivmat * ndcPosition;

            return new Vec3(worldPosition.x, worldPosition.y, worldPosition.z);
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

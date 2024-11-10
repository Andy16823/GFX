using Assimp;
using BulletSharp.SoftBody;
using Genesis.Core;
using Genesis.Math;
using GlmSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Genesis.Graphics
{
    public enum CameraType
    {
        Ortho,
        Perspective
    }

    /// <summary>
    /// Represents a camera in a graphics context for rendering.
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
        /// Gets or sets the field of view (FOV) for the camera in degrees.
        /// </summary>
        public float FOV { get; set; } = 45.0f;

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
            float x = element.Location.X + element.Size.X / 2;
            float y = element.Location.Y + element.Size.Y / 2;
            float z = this.Location.Z;

            this.Location = new Vec3(x, y, z);
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
                float x = element.Location.X;
                float y = element.Location.Y;
                float z = this.Location.Z;

                this.Location = new Vec3(x, y, z);
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
        /// Projects the mouse coordinates into 3D world space, returning the direction vector.
        /// </summary>
        /// <param name="camera">The camera used for the projection.</param>
        /// <param name="viewport">The viewport for the camera's dimensions.</param>
        /// <param name="sX">The x-coordinate of the mouse in screen space.</param>
        /// <param name="sY">The y-coordinate of the mouse in screen space.</param>
        /// <returns>A <see cref="Vec3"/> representing the direction of the ray in 3D space.</returns>
        public static Vec3 ScreenToWorldDirection3D(PerspectiveCamera camera, Viewport viewport, float sX, float sY)
        {
            var projectionMatrix = PerspectiveCamera.GetProjectionMatrix(camera);
            var viewMatrix = PerspectiveCamera.GetViewMatrix(camera);

            float x = (2.0f * sX) / viewport.Width - 1.0f;
            float y = 1.0f - (2.0f * sY) / viewport.Height;
            float z = 1.0f;

            vec3 nds = new vec3(x, y, z);
            vec4 clip = new vec4(nds.xy, -1.0f, 1.0f);
            vec4 eye = projectionMatrix.Inverse * clip;
            eye = new vec4(eye.xy, -1.0f, 0.0f);

            vec3 worldCords = (viewMatrix.Inverse * eye).xyz;
            // don't forget to normalise the vector at some point
            worldCords = worldCords.Normalized;

            Vec3 mouseWorldPosition = new Vec3(worldCords.x, worldCords.y, worldCords.z);
            return mouseWorldPosition;
        }

        /// <summary>
        /// Converts screen space coordinates (2D) into world space coordinates (3D).
        /// </summary>
        /// <param name="camera">The perspective camera used for projection and view matrix calculations.</param>
        /// <param name="viewport">The viewport that defines the screen's width and height.</param>
        /// <param name="sX">The x-coordinate of the mouse position in screen space (pixels).</param>
        /// <param name="sY">The y-coordinate of the mouse position in screen space (pixels).</param>
        /// <returns>
        /// A <see cref="Vec3"/> representing the corresponding world space coordinates.
        /// This is the position in the 3D world corresponding to the input screen coordinates, 
        /// taking into account the perspective projection and camera view matrix.
        /// </returns>
        /// <remarks>
        /// The method first normalizes the screen coordinates to the range [-1, 1], then applies 
        /// the inverse of the combined projection and view matrix to map the 2D screen coordinates 
        /// back into 3D world coordinates. The result is a position in the world space, representing 
        /// the direction of a ray starting from the camera's near plane and passing through the 
        /// specified screen coordinates.
        /// </remarks>
        public static Vec3 ScreenToWorldPosition3D(PerspectiveCamera camera, Viewport viewport, float sX, float sY)
        {
            var projectionMatrix = PerspectiveCamera.GetProjectionMatrix(camera);
            var viewMatrix = PerspectiveCamera.GetViewMatrix(camera);

            float x = ((float)sX / (float)viewport.Width) * 2.0f - 1.0f;
            float y = 1.0f - ((float)sY / (float)viewport.Height) * 2.0f;
            vec4 ndc = new vec4(x, y, -1.0f, 1.0f);

            // Faster way (just one inverse)
            mat4 m = (projectionMatrix * viewMatrix).Inverse;
            vec4 world = m * ndc;
            world /= world.w;

            return new Vec3(world.x, world.y, world.z);
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

        /// <summary>
        /// Calculates the projection matrix for this camera based on the specified viewport.
        /// </summary>
        /// <param name="viewport">The viewport used to calculate the projection matrix.</param>
        /// <returns>A <see cref="mat4"/> representing the projection matrix.</returns>
        public mat4 GetProjectionMatrix(Viewport viewport)
        {
            return Camera.GetProjectionMatrix(this, viewport);
        }

        /// <summary>
        /// Calculates the projection matrix for the specified camera based on the viewport and field of view.
        /// </summary>
        /// <param name="camera">The camera for which to calculate the projection matrix.</param>
        /// <param name="viewport">The viewport used to calculate the projection matrix.</param>
        /// <param name="fov">The field of view in degrees. Defaults to 45.0f.</param>
        /// <returns>A <see cref="mat4"/> representing the projection matrix.</returns>
        public static mat4 GetProjectionMatrix(Camera camera, Viewport viewport)
        {
            float correction = camera.CalculateScreenCorrection(viewport);

            if (camera.Type == CameraType.Ortho)
            {
                float halfWidth = (viewport.Width / 2) / correction;
                float halfHeight = (viewport.Height / 2) / correction;
                float left = camera.Location.X - halfWidth;
                float right = camera.Location.X + halfWidth;
                float bottom = camera.Location.Y - halfHeight;
                float top = camera.Location.Y + halfHeight;
                var p_mat = mat4.Ortho(left, right, bottom, top, 0.1f, 100.0f);
                return p_mat;
            }
            else
            {
                float aspectRatio = (viewport.Width * correction) / (viewport.Height * correction);
                vec3 cameraPosition = camera.Location.ToGlmVec3();
                Vec3 cameraFront = Utils.CalculateCameraFront2(camera);
                var p_mat = mat4.Perspective(Utils.ToRadians(camera.FOV), aspectRatio, camera.Near, camera.Far);
                return p_mat;
            }
        }

        /// <summary>
        /// Calculates the view matrix for this camera.
        /// </summary>
        /// <returns>A <see cref="mat4"/> representing the view matrix.</returns>
        public mat4 GetViewMatrix()
        {
            return Camera.GetViewMatrix(this);
        }

        /// <summary>
        /// Calculates the view matrix for the specified camera.
        /// </summary>
        /// <param name="camera">The camera for which to calculate the view matrix.</param>
        /// <returns>A <see cref="mat4"/> representing the view matrix.</returns>
        public static mat4 GetViewMatrix(Camera camera)
        {
            if (camera.Type == CameraType.Ortho)
            {
                var v_mat = mat4.LookAt(new vec3(0f, 0f, 1f), new vec3(0f, 0f, 0f), new vec3(0f, 1f, 0f));
                return v_mat;
            }
            else
            {
                vec3 cameraPosition = camera.Location.ToGlmVec3();
                Vec3 cameraFront = Utils.CalculateCameraFront2(camera);
                var v_mat = mat4.LookAt(cameraPosition, cameraPosition + cameraFront.ToGlmVec3(), new vec3(0.0f, 1.0f, 0.0f));
                return v_mat;
            }
        }
    }
}

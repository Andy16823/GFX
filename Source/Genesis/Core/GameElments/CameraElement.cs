using Genesis.Graphics;
using Genesis.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Core.GameElements
{
    /// <summary>
    /// Represents a game element that serves as a camera within the game world.
    /// </summary>
    public class CameraElement : GameElement
    {
        /// <summary>
        /// Gets or sets the camera associated with this element.
        /// </summary>
        public Camera Camera { get; set; }

        /// <summary>
        /// Gets or sets the location of the camera within the game world.
        /// </summary>
        public Vec3 Location
        {
            get => Camera.Location;
            set => Camera.Location = value;
        }

        /// <summary>
        /// Gets or sets the rotation of the camera.
        /// </summary>
        public Vec3 Rotation
        {
            get => Camera.Rotation;
            set => Camera.Rotation = value;
        }

        /// <summary>
        /// Gets or sets the size of the camera.
        /// </summary>
        public Vec3 Size
        {
            get => Camera.Size;
            set => Camera.Size = value;
        }

        /// <summary>
        /// Initializes a new instance of the CameraElement class with specified parameters.
        /// </summary>
        /// <param name="name">The name of the camera element.</param>
        /// <param name="location">The initial location of the camera.</param>
        /// <param name="cameraSize">The size of the camera.</param>
        /// <param name="near">The near clipping plane distance of the camera.</param>
        /// <param name="far">The far clipping plane distance of the camera.</param>
        /// <param name="type">The type of the camera (perspective or orthographic).</param>
        public CameraElement(String name, Vec3 location, Vec3 cameraSize, float near, float far, CameraType type)
        {
            this.Name = name;
            this.Camera = new Camera(location, cameraSize, near, far);
            this.Camera.Type = type;
        }

        /// <summary>
        /// Initializes a new instance of the CameraElement class with an existing camera.
        /// </summary>
        /// <param name="name">The name of the camera element.</param>
        /// <param name="camera">The camera object to associate with this element.</param>
        public CameraElement(String name, Camera camera)
        {
            this.Name = name;
            this.Camera = camera;
        }

    }
}

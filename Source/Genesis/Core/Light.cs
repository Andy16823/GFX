using Genesis.Graphics;
using Genesis.Math;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Core
{
    /// <summary>
    /// Represents a light source in the Genesis framework.
    /// </summary>
    public class Light : GameElement
    {
        /// <summary>
        /// Creates a new instance of the Light class with the specified name and location.
        /// </summary>
        /// <param name="name">The name of the light.</param>
        /// <param name="location">The 3D location of the light.</param>
        public Light(String name, Vec3 location)
        {
            this.Name = name;
            this.Location = location;
        }

        /// <summary>
        /// Gets or sets the color of the light.
        /// </summary>
        public Color LightColor { get; set; } = Color.FromArgb(255, 255, 255);

        /// <summary>
        /// Gets or sets the intensity of the light.
        /// </summary>
        public float Intensity { get; set; }

        /// <summary>
        /// Returns the direction vector from the light to the camera.
        /// </summary>
        /// <param name="camera">The camera to which the direction is calculated.</param>
        /// <returns>The normalized vector representing the light direction.</returns>
        public Vec3 GetLightDirection(Camera camera)
        {
            Vec3 camPos = camera.Location;
            Vec3 lightDirection = Vec3.Normalized(this.Location -  camPos);
            return lightDirection;
        }

        /// <summary>
        /// Returns the light color in RGB values normalized between 0 and 1.
        /// </summary>
        /// <returns>A Vec3 representing the normalized RGB values of the light color.</returns>
        public Vec3 GetLightColor()
        {
            float r = (float)LightColor.R / 255;
            float g = (float)LightColor.G / 255;
            float b = (float)LightColor.B / 255;
            return new Vec3(r, g, b);
        }

    }
}

using Genesis.Graphics;
using Genesis.Math;
using GlmSharp;
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
        /// Gets or sets a value indicating whether the light should cast shadows.
        /// </summary>
        /// <value>
        /// <c>true</c> if the light casts shadows; otherwise, <c>false</c>.
        /// This property controls whether shadows are rendered for this object during the shadow mapping process.
        /// </value>
        public bool CastShadows { get; set; }

        /// <summary>
        /// Creates a new instance of the Light class with the specified name and location.
        /// </summary>
        /// <param name="name">The name of the light.</param>
        /// <param name="location">The 3D location of the light.</param>
        public Light(String name, Vec3 location, bool castShadows = true)
        {
            this.Name = name;
            this.Location = location;
            this.CastShadows = castShadows;
        }

        /// <summary>
        /// Creates a new instance of the Light class with the specified name, location and intensity.
        /// </summary>
        /// <param name="name">The name of the light.</param>
        /// <param name="location">The 3D location of the light.</param>
        /// <param name="intensity">The intensity of the light.</param>
        public Light(String name, Vec3 location, float intensity, bool castShadows = true) 
        {
            this.Name=name;
            this.Location = location;
            this.Intensity = intensity;
            this.CastShadows = castShadows;
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

        public static mat4 GetLightViewMatrix(Light lightSource)
        {
            mat4 lightView = mat4.LookAt(lightSource.Location.ToGlmVec3(), new vec3(0), new vec3(0.0f, 1.0f, 0.0f));
            return lightView;
        }

        public static mat4 GetLightProjectionMatrix(Light lightSource, PerspectiveCamera camera, Viewport viewport)
        {
            mat4 lightView = Light.GetLightViewMatrix(lightSource);
            var frustumCorners = camera.GetFrustum(viewport).ToList(lightView);

            vec3 min = new vec3();
            vec3 max = new vec3();

            for (int i = 0; i < frustumCorners.Count; i++)
            {
                if (frustumCorners[i].x < min.x)
                    min.x = frustumCorners[i].x;
                if (frustumCorners[i].y < min.y)
                    min.y = frustumCorners[i].y;
                if (frustumCorners[i].z < min.z)
                    min.z = frustumCorners[i].z;

                if (frustumCorners[i].x > max.x)
                    max.x = frustumCorners[i].x;
                if (frustumCorners[i].y > max.y)
                    max.y = frustumCorners[i].y;
                if (frustumCorners[i].z > max.z)
                    max.z = frustumCorners[i].z;
            }

            float l = min.x - 10f;
            float r = max.x + 10f;
            float b = min.y - 10f;
            float t = max.y + 10f;

            float n = -max.z;
            float f = -min.z;

            mat4 lightProjection = mat4.Ortho(l, r, b, t, n, f);

            return lightProjection;
        }

    }
}

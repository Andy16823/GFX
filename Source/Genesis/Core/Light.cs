using Genesis.Graphics;
using Genesis.Math;
using GlmSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Core
{
    /// <summary>
    /// Represents a light source in the Genesis framework.
    /// </summary>
    public abstract class Light : GameElement
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
        /// Gets or sets the shadow map for the light.
        /// </summary>
        /// <value>
        /// A <see cref="Framebuffer"/> object that stores the shadow map used for shadow calculations.
        /// </value>
        public Framebuffer Shadowmap { get; set; }

        /// <summary>
        /// Gets or sets the resolution of the shadow map.
        /// </summary>
        /// <value>
        /// A <see cref="Vec3"/> object specifying the width and height of the shadow map. Default resolution is 2048x2048.
        /// </value>
        public Vec3 ShadowResolution { get; set; } = new Vec3(2048, 2048);

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

        /// <summary>
        /// Initializes the light with the provided game and render device.
        /// This method prepares the shadow map for rendering.
        /// </summary>
        /// <param name="game">The game object that this light belongs to.</param>
        /// <param name="renderDevice">The render device used to build the shadow map.</param>
        public override void Init(Game game, IRenderDevice renderDevice)
        {
            this.Shadowmap = renderDevice.BuildShadowMap((int)ShadowResolution.X, (int)ShadowResolution.Y);
        }

        /// <summary>
        /// Gets the light view matrix for rendering.
        /// This abstract method must be implemented in subclasses to return the appropriate light view matrix.
        /// </summary>
        /// <returns>
        /// A <see cref="mat4"/> representing the view matrix for this light.
        /// </returns>
        public abstract mat4 GetLightViewMatrix();

        /// <summary>
        /// Gets the light projection matrix for the specified camera and viewport.
        /// This abstract method must be implemented in subclasses to return the appropriate light projection matrix.
        /// </summary>
        /// <param name="camera">The perspective camera used for the calculation.</param>
        /// <param name="viewport">The viewport that defines the rendering area.</param>
        /// <returns>
        /// A <see cref="mat4"/> representing the projection matrix for this light.
        /// </returns>
        public abstract mat4 GetLightProjectionMatrix(PerspectiveCamera camera, Viewport viewport);
    }
}

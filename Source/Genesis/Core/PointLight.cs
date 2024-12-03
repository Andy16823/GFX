using Genesis.Graphics;
using Genesis.Math;
using GlmSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Core
{
    /// <summary>
    /// Represents a point light source in the scene, which emits light in all directions from a specific position.
    /// </summary>
    public class PointLight : Light
    {
        /// <summary>
        /// Gets or sets the range of the shadow for the point light.
        /// </summary>
        public float ShadowRange { get; set; } = 25.0f;

        /// <summary>
        /// Initializes a new instance of the <see cref="PointLight"/> class.
        /// </summary>
        /// <param name="name">The name of the light.</param>
        /// <param name="location">The position of the light source.</param>
        /// <param name="intensity">The intensity of the light.</param>
        /// <param name="castShadows">Indicates whether the light casts shadows.</param>
        public PointLight(string name, Vec3 location, float intensity, bool castShadows = true) : base(name, location, intensity, castShadows)
        {
        }

        /// <summary>
        /// Calculates the projection matrix for the point light based on its shadow range.
        /// </summary>
        /// <param name="camera">The camera used for the scene (not directly utilized here).</param>
        /// <param name="viewport">The viewport dimensions (not directly utilized here).</param>
        /// <returns>A 4x4 orthographic projection matrix for the light.</returns>
        public override mat4 GetLightProjectionMatrix(PerspectiveCamera camera, Viewport viewport)
        {
            float left = this.Location.X - this.ShadowRange;
            float right = this.Location.X + this.ShadowRange;
            float top = this.Location.Z + this.ShadowRange;
            float bottom = this.Location.Z - this.ShadowRange;

            float near_plane = 1.0f, far_plane = this.Location.Y + 1.0f;
            mat4 lightProjection = mat4.Ortho(left, right, bottom, top, near_plane, far_plane);
            return lightProjection;
        }

        /// <summary>
        /// Computes the view matrix for the point light.
        /// </summary>
        /// <returns>A 4x4 view matrix for the light.</returns>
        public override mat4 GetLightViewMatrix()
        {
            mat4 lightView = mat4.LookAt(this.Location.ToGlmVec3(), new vec3(0), new vec3(0.0f, 1.0f, 0.0f));
            return lightView;
        }
    }
}

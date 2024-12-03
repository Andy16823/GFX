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
    /// Represents a directional light source in the scene, such as sunlight.
    /// </summary>
    public class DirectionalLight : Light
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DirectionalLight"/> class.
        /// </summary>
        /// <param name="name">The name of the light.</param>
        /// <param name="location">The position of the light source.</param>
        /// <param name="intensity">The intensity of the light.</param>
        /// <param name="castShadows">Indicates whether the light casts shadows.</param>
        public DirectionalLight(string name, Vec3 location, float intensity, bool castShadows = true) : base(name, location, intensity, castShadows)
        {
        }

        /// <summary>
        /// Calculates the projection matrix for the directional light 
        /// based on the provided camera and viewport.
        /// </summary>
        /// <param name="camera">The camera used to define the view frustum.</param>
        /// <param name="viewport">The viewport dimensions.</param>
        /// <returns>A 4x4 projection matrix for the light.</returns>
        public override mat4 GetLightProjectionMatrix(PerspectiveCamera camera, Viewport viewport)
        {
            mat4 lightView = this.GetLightViewMatrix();
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

        /// <summary>
        /// Computes the view matrix for the directional light.
        /// </summary>
        /// <returns>A 4x4 view matrix for the light.</returns>
        public override mat4 GetLightViewMatrix()
        {
            mat4 lightView = mat4.LookAt(this.Location.ToGlmVec3(), new vec3(0), new vec3(0.0f, 1.0f, 0.0f));
            return lightView;
        }
    }
}

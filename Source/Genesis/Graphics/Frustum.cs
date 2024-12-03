using GlmSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Graphics
{
    /// <summary>
    /// Represents a viewing frustum in 3D space, defined by its corners in both the near and far planes.
    /// </summary>
    public struct Frustum
    {
        /// <summary>
        /// Gets or sets the center point of the far plane of the frustum.
        /// </summary>
        public vec3 centerFar;

        /// <summary>
        /// Gets or sets the top-left corner of the far plane of the frustum.
        /// </summary>
        public vec3 topLeftFar;

        /// <summary>
        /// Gets or sets the top-right corner of the far plane of the frustum.
        /// </summary>
        public vec3 topRightFar;

        /// <summary>
        /// Gets or sets the bottom-right corner of the far plane of the frustum.
        /// </summary>
        public vec3 bottomRightFar;

        /// <summary>
        /// Gets or sets the bottom-left corner of the far plane of the frustum.
        /// </summary>
        public vec3 bottomLeftFar;

        /// <summary>
        /// Gets or sets the center point of the near plane of the frustum.
        /// </summary>
        public vec3 centerNear;

        /// <summary>
        /// Gets or sets the top-left corner of the near plane of the frustum.
        /// </summary>
        public vec3 topLeftNear;

        /// <summary>
        /// Gets or sets the top-right corner of the near plane of the frustum.
        /// </summary>
        public vec3 topRightNear;

        /// <summary>
        /// Gets or sets the bottom-right corner of the near plane of the frustum.
        /// </summary>
        public vec3 bottomRightNear;

        /// <summary>
        /// Gets or sets the bottom-left corner of the near plane of the frustum.
        /// </summary>
        public vec3 bottomLeftNear;

        /// <summary>
        /// Gets or sets the center point of the frustum.
        /// </summary>
        public vec3 center;

        /// <summary>
        /// Transforms the frustum corners using the specified transformation matrix and returns them as a list of <see cref="vec3"/> vectors.
        /// </summary>
        /// <param name="matrix">The transformation matrix to apply to the frustum corners.</param>
        /// <returns>A <see cref="List{vec3}"/> containing the transformed corners of the frustum.</returns>
        public List<vec3> ToList(mat4 matrix)
        {
            List<vec3> list = new List<vec3>();
            list.Add((vec3)(matrix * new vec4(bottomRightNear, 1.0f)));
            list.Add((vec3)(matrix * new vec4(topRightNear, 1.0f)));
            list.Add((vec3)(matrix * new vec4(bottomLeftNear, 1.0f)));
            list.Add((vec3)(matrix * new vec4(topLeftFar, 1.0f)));
            list.Add((vec3)(matrix * new vec4(bottomRightFar, 1.0f)));
            list.Add((vec3)(matrix * new vec4(topRightFar, 1.0f)));
            list.Add((vec3)(matrix * new vec4(bottomLeftFar, 1.0f)));
            list.Add((vec3)(matrix * new vec4(topLeftFar, 1.0f)));
            return list;
        }

    }
}

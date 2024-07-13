using BulletSharp;
using Genesis.Core;
using Genesis.Math;
using GlmSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Physics
{
    /// <summary>
    /// Represents an Axis-Aligned Bounding Box (AABB) in 3D space.
    /// </summary>
    public class Aabb
    {
        /// <summary>
        /// Gets or sets the maximum corner of the AABB.
        /// </summary>
        public Vec3 Max { get; set; }

        /// <summary>
        /// Gets or sets the minimum corner of the AABB.
        /// </summary>
        public Vec3 Min { get; set; }

        /// <summary>
        /// Creates a new AABB with default values.
        /// </summary>
        public Aabb()
        {
            this.Max = new Vec3(0);
            this.Min = new Vec3(0);
        }

        /// <summary>
        /// Creates a new AABB with specified minimum and maximum values.
        /// </summary>
        /// <param name="min">Minimum corner</param>
        /// <param name="max">Maximum corner</param>
        public Aabb(BulletSharp.Math.Vector3 min, BulletSharp.Math.Vector3 max)
        {
            this.Max = new Vec3(max.X, max.Y, max.Z);
            this.Min = new Vec3(min.X, min.Y, min.Z);
        }

        /// <summary>
        /// Creates an new Aabb for an game element using the elements dimensions
        /// </summary>
        /// <param name="element">The element for the Aabb</param>
        public Aabb(GameElement element)
        {
            this.Min = Vec3.Zero();
            this.Min.X = element.Location.X - (element.Size.X / 2);
            this.Min.Y = element.Location.Y - (element.Size.Y / 2);
            this.Min.Z = element.Location.Z - (element.Size.Z / 2);

            this.Max = Vec3.Zero();
            this.Max.X = element.Location.X + (element.Size.X / 2);
            this.Max.Y = element.Location.Y + (element.Size.Y / 2);
            this.Max.Z = element.Location.Z + (element.Size.Z / 2);
        }

        /// <summary>
        /// Returns the AABB of a Bullet rigid body.
        /// </summary>
        /// <param name="rigidBody">Bullet RigidBody</param>
        /// <returns>Aabb representing the bounding box of the rigid body</returns>
        public static Aabb FromBulletRigidBody(BulletSharp.RigidBody rigidBody)
        {
            var min = new BulletSharp.Math.Vector3();
            var max = new BulletSharp.Math.Vector3();
            rigidBody.GetAabb(out min, out max);
            return new Aabb(min, max);
        }

        /// <summary>
        /// Checks if the AABB contains a specified point.
        /// </summary>
        /// <param name="v">Point to check</param>
        /// <returns>True if the point is inside the AABB, otherwise false</returns>
        public bool Contains(Vec3 v) {

            if (v.X > Min.X && v.X < Max.X && v.Y > Min.Y && v.Y < Max.Y && v.Z > Min.Z && v.Z < Max.Z)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Checks if a ray intersects with the AABB.
        /// </summary>
        /// <param name="rayOrigin">Origin of the ray</param>
        /// <param name="rayDirection">Direction of the ray</param>
        /// <param name="aabbMin">Minimum corner of the AABB</param>
        /// <param name="aabbMax">Maximum corner of the AABB</param>
        /// <returns>True if the ray intersects with the AABB, otherwise false</returns>
        public static bool IntersectRay(Vec3 rayOrigin, Vec3 rayDirection, Vec3 aabbMin, Vec3 aabbMax)
        {
            float t1 = (aabbMin.X - rayOrigin.X) / rayDirection.X;
            float t2 = (aabbMax.X - rayOrigin.X) / rayDirection.X;

            float t3 = (aabbMin.Y - rayOrigin.Y) / rayDirection.Y;
            float t4 = (aabbMax.Y - rayOrigin.Y) / rayDirection.Y;

            float t5 = (aabbMin.Z - rayOrigin.Z) / rayDirection.Z;
            float t6 = (aabbMax.Z - rayOrigin.Z) / rayDirection.Z;

            float tmin = glm.Max(glm.Min(t1, t2), glm.Min(t3, t4));
            float tmax = glm.Min(glm.Max(t1, t2), glm.Max(t3, t4));

            return tmax >= tmin;
        }
    }
}

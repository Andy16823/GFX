using BulletSharp;
using Genesis.Math;
using GlmSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Physics
{
    public class Aabb
    {
        public Vec3 Max { get; set; }
        public Vec3 Min { get; set; }

        /// <summary>
        /// Creates a new Aabb
        /// </summary>
        public Aabb()
        {
            this.Max = new Vec3(0);
            this.Min = new Vec3(0);
        }

        /// <summary>
        /// Creates a new Aabb
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public Aabb(BulletSharp.Math.Vector3 min, BulletSharp.Math.Vector3 max)
        {
            this.Max = new Vec3(max.X, max.Y, max.Z);
            this.Min = new Vec3(min.X, min.Y, min.Z);
        }

        /// <summary>
        /// Returns the aabb from a bullet rigidbody
        /// </summary>
        /// <param name="rigidBody"></param>
        /// <returns></returns>
        public static Aabb FromBulletRigidBody(BulletSharp.RigidBody rigidBody)
        {
            var min = new BulletSharp.Math.Vector3();
            var max = new BulletSharp.Math.Vector3();
            rigidBody.GetAabb(out min, out max);
            return new Aabb(min, max);
        }

        /// <summary>
        /// Checks if the aabb contains a point
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public bool Contains(Vec3 v) {

            if (v.X > Min.X && v.X < Max.X && v.Y > Min.Y && v.Y < Max.Y && v.Z > Min.Z && v.Z < Max.Z)
            {
                return true;
            }
            return false;
        }

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

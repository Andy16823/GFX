using Genesis.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Physics
{
    public class Raycast2D
    {
        /// <summary>
        /// Checks if a 2D ray intersects with an Axis-Aligned Bounding Box (AABB).
        /// </summary>
        /// <param name="ray">The 2D ray to test.</param>
        /// <param name="aabb">The AABB defined by its minimum and maximum points.</param>
        /// <param name="intersectionPoint">If there is an intersection, this will contain the intersection point.</param>
        /// <returns>True if the ray intersects with the AABB, false otherwise.</returns>
        public static bool RayIntersectsAABB(Ray2D ray, Aabb aabb, out Vec3 intersectionPoint)
        {
            return RayIntersects(ray, aabb.Min, aabb.Max, out intersectionPoint);
        }

        /// <summary>
        /// Performs the actual intersection test between a 2D ray and an AABB defined by its minimum and maximum points.
        /// </summary>
        /// <param name="ray">The 2D ray to test.</param>
        /// <param name="min">The minimum point of the AABB.</param>
        /// <param name="max">The maximum point of the AABB.</param>
        /// <param name="intersectionPoint">If there is an intersection, this will contain the intersection point.</param>
        /// <returns>True if the ray intersects with the AABB, false otherwise.</returns>
        public static bool RayIntersects(Ray2D ray, Vec3 min, Vec3 max, out Vec3 intersectionPoint)
        {
            intersectionPoint = Vec3.Zero();

            // Berechnung der t-Werte für die x-Achse
            float tMin = (min.X - ray.Origin.X) / ray.Direction.X;
            float tMax = (max.X - ray.Origin.X) / ray.Direction.X;

            if (tMin > tMax)
            {
                float temp = tMin;
                tMin = tMax;
                tMax = temp;
            }

            // Berechnung der t-Werte für die y-Achse
            float tMinY = (min.Y - ray.Origin.Y) / ray.Direction.Y;
            float tMaxY = (max.Y - ray.Origin.Y) / ray.Direction.Y;

            if (tMinY > tMaxY)
            {
                float temp = tMinY;
                tMinY = tMaxY;
                tMaxY = temp;
            }

            // Überprüfung auf Überschneidung der Intervalle
            if ((tMin > tMaxY) || (tMinY > tMax))
            {
                return false;
            }

            if (tMinY > tMin)
            {
                tMin = tMinY;
            }

            if (tMaxY < tMax)
            {
                tMax = tMaxY;
            }

            if (tMin < 0)
            {
                return false;
            }

            intersectionPoint = ray.Origin + ray.Direction * tMin;
            return true;
        }
    }
}

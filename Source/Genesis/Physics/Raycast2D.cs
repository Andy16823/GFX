using Genesis.Core;
using Genesis.Math;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Physics
{
    /// <summary>
    /// Represents the result of a raycast hit in 2D space, containing information about
    /// the intersection point, distance from the origin, the hit element, and its bounding box.
    /// </summary>
    public struct HitResult2D
    {
        /// <summary>
        /// The intersection point of the raycast hit.
        /// </summary>
        public Vec3 intersectionPoint;

        /// <summary>
        /// The distance from the ray's origin to the intersection point.
        /// </summary>
        public float distance;

        /// <summary>
        /// The game element that was hit by the raycast.
        /// </summary>
        public GameElement hit;

        /// <summary>
        /// The axis-aligned bounding box (AABB) of the hit element.
        /// </summary>
        public Aabb aabb;
    }

    /// <summary>
    /// Provides methods to perform raycasting in a 2D scene and retrieve intersection information.
    /// </summary>
    public class Raycast2D
    {
        /// <summary>
        /// Casts a ray in a scene and returns all hit results for elements intersected by the ray.
        /// </summary>
        /// <param name="ray">The 2D ray to cast.</param>
        /// <param name="scene">The scene containing elements to test for intersections.</param>
        /// <returns>A list of <see cref="HitResult2D"/> representing all intersection results.</returns>
        public static List<HitResult2D> PerformCastAll(Ray2D ray, Scene scene)
        {
            var elements = scene.Layer.SelectMany(layer => layer.Elements).ToArray();
            return PerformCastAll(ray, elements);
        }

        /// <summary>
        /// Casts a ray against an array of game elements and returns all hit results.
        /// </summary>
        /// <param name="ray">The 2D ray to cast.</param>
        /// <param name="elements">An array of game elements to test for intersections.</param>
        /// <returns>A list of <see cref="HitResult2D"/> containing all intersection results.</returns>
        public static List<HitResult2D> PerformCastAll(Ray2D ray, GameElement[] elements)
        {
            ConcurrentBag<HitResult2D> hits = new ConcurrentBag<HitResult2D>();
            Parallel.ForEach(elements, (element) =>
            {
                if (element.Enabled)
                {
                    var elementAABB = new Aabb(element);

                    Vec3 hitLocation;
                    if (RayIntersectsAABB(ray, elementAABB, out hitLocation))
                    {
                        var result = new HitResult2D()
                        {
                            intersectionPoint = hitLocation,
                            distance = ray.Origin.Distance(hitLocation),
                            hit = element,
                            aabb = elementAABB
                        };
                        hits.Add(result);
                    }
                }
            });
            return hits.ToList();
        }

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

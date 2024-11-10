using BulletSharp;
using BulletSharp.Math;
using Genesis.Core;
using Genesis.Core.Behaviors.Physics3D;
using Genesis.Graphics;
using Genesis.Math;
using GlmSharp;
using GlmSharp.Swizzle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static BulletSharp.DiscreteCollisionDetectorInterface;

namespace Genesis.Physics
{
    /// <summary>
    /// Represents a set of matrices used in raycasting.
    /// </summary>
    public struct MatrixSet
    {
        public mat4 viewMatrix;
        public mat4 projectionMatrix;
    }

    /// <summary>
    /// Represents the result of a raycasting hit.
    /// </summary>
    public struct HitResult
    {
        public bool hit;
        public Vec3 rayStart; 
        public Vec3 rayEnd;
        public CollisionObject collisionObject;
        public GameElement hitElement;
        public Vec3 hitLocation;
    }

    /// <summary>
    /// Represents a class for performing raycasting in 3D space.
    /// </summary>
    public class Raycast
    {
        /// <summary>
        /// Gets or sets the camera used for raycasting.
        /// </summary>
        public Camera Camera { get; set; }

        /// <summary>
        /// Gets or sets the viewport associated with the raycasting.
        /// </summary>
        public Viewport Viewport { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Raycast"/> class.
        /// </summary>
        /// <param name="camera">The camera used for raycasting.</param>
        /// <param name="viewport">The viewport associated with the raycasting.</param>
        public Raycast(Camera camera, Viewport viewport)
        {
            this.Camera = camera;
            this.Viewport = viewport;
        }

        /// <summary>
        /// Gets the view and projection matrices for raycasting based on a given camera.
        /// </summary>
        /// <param name="camera">The camera for which matrices are calculated.</param>
        /// <returns>MatrixSet containing the view and projection matrices.</returns>
        public static MatrixSet GetViewProjectionMatrix(Camera camera)
        {
            MatrixSet matrixSet;

            vec3 cameraPosition = camera.Location.ToGlmVec3();
            Vec3 cameraFront = Utils.CalculateCameraFront2(camera);

            matrixSet.projectionMatrix = mat4.Perspective(Utils.ToRadians(camera.FOV), camera.Size.X / camera.Size.Y, camera.Near, camera.Far);
            matrixSet.viewMatrix = mat4.LookAt(cameraPosition, cameraPosition + cameraFront.ToGlmVec3(), new vec3(0.0f, 1.0f, 0.0f));

            return matrixSet;
        }

        /// <summary>
        /// Gets the world position of the mouse cursor.
        /// </summary>
        /// <param name="mouseX">X-coordinate of the mouse cursor.</param>
        /// <param name="mouseY">Y-coordinate of the mouse cursor.</param>
        /// <returns>World position of the mouse cursor.</returns>
        public Vec3 GetMouseWorldPosition(int mouseX, int mouseY)
        {
            return GetMouseWorldPosition(this.Camera, this.Viewport, mouseX, mouseY);
        }

        /// <summary>
        /// Gets the world position of the mouse cursor based on a given camera and viewport.
        /// </summary>
        /// <param name="camera">The camera used for raycasting.</param>
        /// <param name="viewport">The viewport associated with the raycasting.</param>
        /// <param name="mouseX">X-coordinate of the mouse cursor.</param>
        /// <param name="mouseY">Y-coordinate of the mouse cursor.</param>
        /// <returns>World position of the mouse cursor.</returns>
        public static Vec3 GetMouseWorldPosition(Camera camera, Viewport viewport, int mouseX, int mouseY)
        {
            var matrixSet = GetViewProjectionMatrix(camera);

            float x = (2.0f * mouseX) / viewport.Width - 1.0f;
            float y = 1.0f - (2.0f * mouseY) / viewport.Height;
            float z = 1.0f;
            vec3 ray_nds = new vec3(x, y, z);
            vec4 ray_clip = new vec4(ray_nds.xy, -1.0f, 1.0f);
            vec4 ray_eye = matrixSet.projectionMatrix.Inverse * ray_clip;
            ray_eye = new vec4(ray_eye.xy, -1.0f, 0.0f);

            vec3 ray_wor = (matrixSet.viewMatrix.Inverse * ray_eye).xyz;
            // don't forget to normalise the vector at some point
            ray_wor = ray_wor.Normalized;

            Vec3 mouseWorldPosition = new Vec3(ray_wor.x, ray_wor.y, ray_wor.z);
            return mouseWorldPosition;
        }

        /// <summary>
        /// Performs a raycast and returns the hit result based on the mouse cursor position.
        /// </summary>
        /// <param name="physicHandler">The physics handler used for raycasting.</param>
        /// <param name="posX">X-coordinate of the mouse cursor.</param>
        /// <param name="posY">Y-coordinate of the mouse cursor.</param>
        /// <returns>HitResult containing information about the raycasting hit.</returns>
        public HitResult PerformCast(PhysicHandler physicHandler, int posX, int posY)
        {
            return PerformCast(this.Camera, this.Viewport, physicHandler, posX, posY);
        }

        public static HitResult PerformCast(Vec3 start, Vec3 end, PhysicHandler physicHandler)
        {
            var result = new HitResult();
            var btStart = start.ToBulletVec3();
            var btEnd = end.ToBulletVec3();

            PhysicsHandler3D physicsHandler3D = (PhysicsHandler3D) physicHandler;
            using(var cb = new ClosestRayResultCallback(ref btStart, ref btEnd))
            {
                physicsHandler3D.PhysicsWorld.RayTest(btStart, btEnd, cb);
                if(cb.HasHit)
                {
                    result.hit = true;
                    result.hitElement = (GameElement) cb.CollisionObject.UserObject;
                    result.collisionObject = cb.CollisionObject;
                    result.hitLocation = new Vec3(cb.HitPointWorld.X, cb.HitPointWorld.Y, cb.HitPointWorld.Z);
                }
                return result;
            }
        }

        /// <summary>
        /// Performs a raycast and returns the hit result based on the mouse cursor position.
        /// </summary>
        /// <param name="camera">The camera used for raycasting.</param>
        /// <param name="viewport">The viewport associated with the raycasting.</param>
        /// <param name="physicHandler">The physics handler used for raycasting.</param>
        /// <param name="posX">X-coordinate of the mouse cursor.</param>
        /// <param name="posY">Y-coordinate of the mouse cursor.</param>
        /// <returns>HitResult containing information about the raycasting hit.</returns>
        public static HitResult PerformCast(Camera camera, Viewport viewport, PhysicHandler physicHandler, int posX, int posY)
        {
            HitResult result = new HitResult();
            var btStart = Raycast.GetStartVec(camera, viewport, posX, posY);
            var btEnd = Raycast.GetEndVec(camera, viewport, posX, posY);
            var direction = Raycast.GetRayDir(btStart, btEnd);
            vec3 out_end = btStart.xyz - (direction * 1000.0f);

            var _start = new Vector3(btStart.x, btStart.y, btEnd.z);
            var _end = new Vector3(out_end.x, out_end.y, out_end.z);

            result.rayStart = new Vec3(btStart.xyz);
            result.rayEnd = new Vec3(out_end.xyz);

            PhysicsHandler3D physics = (PhysicsHandler3D) physicHandler;
            using (var cb = new ClosestRayResultCallback(ref _start, ref _end))
            {
                physics.PhysicsWorld.RayTest(_start, _end, cb);
                if (cb.HasHit)
                {
                    result.hit = true;
                    result.collisionObject = cb.CollisionObject;
                    result.hitLocation = new Vec3(cb.HitPointWorld.X, cb.HitPointWorld.Y, cb.HitPointWorld.Z);
                    result.hitElement = (GameElement) cb.CollisionObject.UserObject;
                }
            }
            return result;
        }

        public static HitResult PerformCastFiltered(Camera camera, Viewport viewport, PhysicHandler physicHandler, int posX, int posY, int collisionGroup = -1, int collisionMask = -1)
        {
            HitResult result = new HitResult();
            var btStart = Raycast.GetStartVec(camera, viewport, posX, posY);
            var btEnd = Raycast.GetEndVec(camera, viewport, posX, posY);
            var direction = Raycast.GetRayDir(btStart, btEnd);
            vec3 out_end = btStart.xyz - (direction * 1000.0f);

            var _start = new Vector3(btStart.x, btStart.y, btEnd.z);
            var _end = new Vector3(out_end.x, out_end.y, out_end.z);

            result.rayStart = new Vec3(btStart.xyz);
            result.rayEnd = new Vec3(out_end.xyz);

            PhysicsHandler3D physics = (PhysicsHandler3D)physicHandler;
            using (var cb = new ClosestRayResultCallback(ref _start, ref _end))
            {
                cb.CollisionFilterGroup = collisionGroup;
                cb.CollisionFilterMask = collisionMask;

                physics.PhysicsWorld.RayTest(_start, _end, cb);
                if (cb.HasHit)
                {
                    result.hit = true;
                    result.collisionObject = cb.CollisionObject;
                    result.hitLocation = new Vec3(cb.HitPointWorld.X, cb.HitPointWorld.Y, cb.HitPointWorld.Z);
                    result.hitElement = (GameElement)cb.CollisionObject.UserObject;
                }
            }
            return result;
        }

        public static List<HitResult> PerformCastAll(Camera camera, Viewport viewport, PhysicHandler physicHandler, int posX, int posY)
        {
            var results = new List<HitResult>();

            var btStart = Raycast.GetStartVec(camera, viewport, posX, posY);
            var btEnd = Raycast.GetEndVec(camera, viewport, posX, posY);
            var direction = Raycast.GetRayDir(btStart, btEnd);
            vec3 out_end = btStart.xyz - (direction * 1000.0f);

            var _start = new Vector3(btStart.x, btStart.y, btEnd.z);
            var _end = new Vector3(out_end.x, out_end.y, out_end.z);

            

            PhysicsHandler3D physics = (PhysicsHandler3D)physicHandler;
            using (var cb = new AllHitsRayResultCallback(_start, _end))
            {
                physics.PhysicsWorld.RayTest(_start, _end, cb);
                if (cb.HasHit)
                {
                    for(int i = 0; i < cb.CollisionObjects.Count; i++)
                    {
                        Vector3 location = cb.HitPointWorld[i];
                        var collisionObject = cb.CollisionObjects[i];

                        HitResult result = new HitResult();
                        result.hit = true;
                        result.rayStart = new Vec3(btStart.xyz);
                        result.rayEnd = new Vec3(out_end.xyz);
                        result.hitLocation = new Vec3(location.X, location.Y, location.Z);
                        result.collisionObject = collisionObject;
                        result.hitElement = (GameElement) collisionObject.UserObject;
                        results.Add(result);
                    }
                }
            }
            return results;
        }

        /// <summary>
        /// Gets the start vector for raycasting based on the mouse cursor position.
        /// </summary>
        /// <param name="camera">The camera used for raycasting.</param>
        /// <param name="vp">The viewport associated with the raycasting.</param>
        /// <param name="posX">X-coordinate of the mouse cursor.</param>
        /// <param name="posY">Y-coordinate of the mouse cursor.</param>
        /// <returns>The start vector for raycasting.</returns>
        public static vec4 GetStartVec(Camera camera, Viewport vp, int posX, int posY)
        {
            var matrixSet = GetViewProjectionMatrix(camera);
            float x = ((float)posX / (float)vp.Width) * 2.0f - 1.0f;
            float y = 1.0f - ((float)posY / (float)vp.Height) * 2.0f;
            vec4 lRayStart_NDC = new vec4(x, y, -1.0f, 1.0f);

            // Faster way (just one inverse)
            mat4 M = (matrixSet.projectionMatrix * matrixSet.viewMatrix).Inverse;
            vec4 lRayStart_world = M * lRayStart_NDC; 
            lRayStart_world/=lRayStart_world.w;
            //glm::vec4 lRayEnd_world   = M * lRayEnd_NDC  ; lRayEnd_world  /=lRayEnd_world.w;

            return lRayStart_world;
        }

        /// <summary>
        /// Gets the end vector for raycasting based on the mouse cursor position.
        /// </summary>
        /// <param name="camera">The camera used for raycasting.</param>
        /// <param name="vp">The viewport associated with the raycasting.</param>
        /// <param name="posX">X-coordinate of the mouse cursor.</param>
        /// <param name="posY">Y-coordinate of the mouse cursor.</param>
        /// <returns>The end vector for raycasting.</returns>
        public static vec4 GetEndVec(Camera camera, Viewport vp, int posX, int posY)
        {
            var matrixSet = GetViewProjectionMatrix(camera);
            float x = ((float)posX / (float)vp.Width) * 2.0f - 1.0f;
            float y = 1.0f - ((float)posY / (float)vp.Height) * 2.0f;
            vec4 lRayEnd_NDC = new vec4(x, y, 0.0f, 1.0f);

            // Faster way (just one inverse)
            mat4 M = (matrixSet.projectionMatrix * matrixSet.viewMatrix).Inverse;
            vec4 lRayEnd_world = M * lRayEnd_NDC; 
            lRayEnd_world  /=lRayEnd_world.w;

            return lRayEnd_world;
        }

        /// <summary>
        /// Gets the direction vector for a ray based on start and end vectors.
        /// </summary>
        /// <param name="start">The start vector of the ray.</param>
        /// <param name="end">The end vector of the ray.</param>
        /// <returns>The direction vector of the ray.</returns>
        public static vec3 GetRayDir(vec4 start, vec4 end)
        {
            vec3 lRayDir_world = (start - end).xyz;
            lRayDir_world = lRayDir_world.Normalized;
            return lRayDir_world;
        }
    }
}

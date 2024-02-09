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

namespace Genesis.Physics
{
    public struct MatrixSet
    {
        public mat4 viewMatrix;
        public mat4 projectionMatrix;
    }

    public struct HitResult
    {
        public Vec3 rayStart; 
        public Vec3 rayEnd;
        public RigidBody rigidBody;
        public Vec3 hitLocation;
    }

    public class MouseRay
    {
        public Camera Camera { get; set; }
        public Viewport Viewport { get; set; }

        public MouseRay(Camera camera, Viewport viewport)
        {
            this.Camera = camera;
            this.Viewport = viewport;
        }

        public static MatrixSet GetViewProjectionMatrix(Camera camera)
        {
            MatrixSet matrixSet;

            vec3 cameraPosition = camera.Location.ToGlmVec3();
            Vec3 cameraFront = Utils.CalculateCameraFront2(camera);

            matrixSet.projectionMatrix = mat4.Perspective(Utils.ToRadians(45.0f), camera.Size.X / camera.Size.Y, camera.Near, camera.Far);
            matrixSet.viewMatrix = mat4.LookAt(cameraPosition, cameraPosition + cameraFront.ToGlmVec3(), new vec3(0.0f, 1.0f, 0.0f));

            return matrixSet;
        }

        public Vec3 GetMouseWorldPosition(int mouseX, int mouseY)
        {
            return GetMouseWorldPosition(this.Camera, this.Viewport, mouseX, mouseY);
        }

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


        public HitResult PerformCast(PhysicHandler physicHandler, int posX, int posY)
        {
            return PerformCast(this.Camera, this.Viewport, physicHandler, posX, posY);
        }

        public static HitResult PerformCast(Camera camera, Viewport viewport, PhysicHandler physicHandler, int posX, int posY)
        {
            HitResult result = new HitResult();
            var btStart = MouseRay.GetStartVec(camera, viewport, posX, posY);
            var btEnd = MouseRay.GetEndVec(camera, viewport, posX, posY);
            var direction = MouseRay.GetRayDir(btStart, btEnd);
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
                    result.rigidBody = (RigidBody) cb.CollisionObject;
                    result.hitLocation = new Vec3(cb.HitPointWorld.X, cb.HitPointWorld.Y, cb.HitPointWorld.Z);
                }
            }
            return result;
        }


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

        public static vec3 GetRayDir(vec4 start, vec4 end)
        {
            vec3 lRayDir_world = (start - end).xyz;
            lRayDir_world = lRayDir_world.Normalized;
            return lRayDir_world;
        }
    }
}

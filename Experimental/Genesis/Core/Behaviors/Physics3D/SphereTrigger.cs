using BulletSharp;
using Genesis.Math;
using Genesis.Physics;
using GlmSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Core.Behaviors.Physics3D
{
    /// <summary>
    /// Represents a Sphere trigger behavior for 3D physics.
    /// </summary>
    public class SphereTrigger : TriggerBehavior3D
    {
        public SphereTrigger(PhysicHandler physicHandler) : base(physicHandler)
        {
        }

        /// <summary>
        /// Creates a trigger with a sphere shape using the default radius (half of the parent's size).
        /// </summary>
        /// <param name="physicHandler">The physics handler to manage this element.</param>
        public override void CreateTrigger(int collisionGroup = -1, int collisionMask = -1)
        {
            this.CreateTrigger(this.Parent.Size.X / 2, collisionGroup, collisionMask);
        }

        /// <summary>
        /// Creates a trigger with a sphere shape at the origin.
        /// </summary>
        /// <param name="radius">The radius of the sphere.</param>
        public void CreateTrigger(float radius, int collisionGroup = -1, int collisionMask = -1)
        {
            this.CreateTrigger(Vec3.Zero(), radius, collisionGroup, collisionMask);
        }

        /// <summary>
        /// Creates a trigger with a sphere shape at the specified offset.
        /// </summary>
        /// <param name="offset">The offset from the parent element's location.</param>
        /// <param name="radius">The radius of the sphere.</param>
        public void CreateTrigger(Vec3 offset, float radius, int collisionGroup = -1, int collisionMask = -1)
        {
            this.Offset = offset;

            var element = this.Parent;
            SphereShape sphereShape = new SphereShape(radius);

            Vec3 location = Utils.GetElementWorldLocation(element) + Offset;
            Vec3 rotation = Utils.GetElementWorldRotation(element);

            var btTranslation = BulletSharp.Math.Matrix.Translation(location.ToBulletVec3());
            var btRotation = BulletSharp.Math.Matrix.RotationX(rotation.X) * BulletSharp.Math.Matrix.RotationY(rotation.Y) * BulletSharp.Math.Matrix.RotationZ(rotation.Z);
            var btStartTransform = btTranslation * btRotation;

            Trigger = new GhostObject();
            Trigger.UserObject = element;
            Trigger.CollisionShape = sphereShape;
            Trigger.WorldTransform = btStartTransform;
            PhysicHandler.ManageElement(this, collisionGroup, collisionMask);
        }
    }
}

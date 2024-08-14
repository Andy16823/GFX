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
    /// Represents a Sphere RigidBody behavior for 3D physics.
    /// </summary>
    public class SphereRigidBody : RigidBodyBehavior3D
    {
        /// <summary>
        /// Constructor for creating a SphereRigidBody behavior.
        /// </summary>
        /// <param name="handler">The physics handler managing this rigid body.</param>
        public SphereRigidBody(PhysicHandler handler) : base(handler)
        {
        }

        /// <summary>
        /// Creates a rigid body with a sphere shape using the default radius (half of the parent's size).
        /// </summary>
        /// <param name="mass">The mass of the sphere rigid body.</param>
        public override void CreateRigidBody(float mass, int collisionGroup = -1, int collisionMask = -1)
        {
            this.CreateRigidBody(this.Parent.Size.X / 2, mass, collisionGroup, collisionMask);
        }

        /// <summary>
        /// Creates a rigid body with a sphere shape at the origin.
        /// </summary>
        /// <param name="radius">The radius of the sphere.</param>
        /// <param name="mass">The mass of the sphere rigid body.</param>
        public void CreateRigidBody(float radius, float mass, int collisionGroup = -1, int collisionMask = -1)
        {
            this.CreateRigidBody(Vec3.Zero(), radius, mass, collisionGroup, collisionMask);
        }

        /// <summary>
        /// Creates a rigid body with a sphere shape at the specified offset.
        /// </summary>
        /// <param name="offset">The offset from the parent element's location.</param>
        /// <param name="radius">The radius of the sphere.</param>
        /// <param name="mass">The mass of the sphere rigid body.</param>
        public void CreateRigidBody(Vec3 offset, float radius, float mass, int collisionGroup = -1, int collisionMask = -1)
        {
            this.Offset = offset;

            var element = this.Parent;
            SphereShape sphereShape = new SphereShape(radius);
            RigidBodyConstructionInfo constructionInfo = new RigidBodyConstructionInfo(mass, null, sphereShape);

            Vec3 location = Utils.GetElementWorldLocation(element) + Offset;
            Vec3 rotation = Utils.GetElementWorldRotation(element);

            var btTranslation = BulletSharp.Math.Matrix.Translation(location.ToBulletVec3());
            var btRotation = BulletSharp.Math.Matrix.RotationX(rotation.X) * BulletSharp.Math.Matrix.RotationY(rotation.Y) * BulletSharp.Math.Matrix.RotationZ(rotation.Z);
            var btStartTransform = btTranslation * btRotation;

            constructionInfo.MotionState = new DefaultMotionState(btStartTransform);

            RigidBody = new RigidBody(constructionInfo);
            RigidBody.UserObject = element;
            RigidBody.ApplyGravity();

            PhysicHandler.ManageElement(this, collisionGroup, collisionMask);
        }
    }
}

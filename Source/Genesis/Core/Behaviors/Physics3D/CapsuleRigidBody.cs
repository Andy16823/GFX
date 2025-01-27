using BulletSharp;
using BulletSharp.SoftBody;
using Genesis.Math;
using Genesis.Physics;
using GlmSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BulletSharp.Dbvt;

namespace Genesis.Core.Behaviors.Physics3D
{
    /// <summary>
    /// Represents a Capsule RigidBody behavior for 3D physics.
    /// </summary>
    public class CapsuleRigidBody : Physics.RigidBodyBehavior3D
    {
        /// <summary>
        /// Constructor for creating a CapsuleRigidBody behavior.
        /// </summary>
        /// <param name="handler">The physics handler managing this rigid body.</param>
        public CapsuleRigidBody(PhysicHandler handler) : base(handler)
        {
        }

        /// <summary>
        /// Creates a RigidBody with default capsule dimensions.
        /// </summary>
        /// <param name="mass">The mass of the capsule.</param>
        public override void CreateRigidBody(float mass, int collisionGroup = -1, int collisionMask = -1)
        {
            this.CreateRigidBody(1.0f, 2.0f, mass, new Vec3(), collisionGroup, collisionMask);
        }

        /// <summary>
        /// Creates a RigidBody with a capsule shape.
        /// </summary>
        /// <param name="radius">The radius of the capsule.</param>
        /// <param name="height">The height of the capsule.</param>
        /// <param name="mass">The mass of the capsule.</param>
        /// <param name="offset">The offset of the capsule. Default is Vec3.Zero().</param>
        public void CreateRigidBody(float radius, float height, float mass, Vec3 offset = new Vec3(), int collisionGroup = -1, int collisionMask = -1)
        {
            this.Offset = offset;
            CapsuleShape capsuleShape = new CapsuleShape(radius, height);
            RigidBodyConstructionInfo constructionInfo = new RigidBodyConstructionInfo(mass, null, capsuleShape, capsuleShape.CalculateLocalInertia(mass));
            var btTransform = Utils.GetBtTransform(Parent, Offset);

            constructionInfo.MotionState = new DefaultMotionState(btTransform);

            RigidBody = new RigidBody(constructionInfo);
            RigidBody.UserObject = Parent;
            RigidBody.ApplyGravity();

            PhysicHandler.ManageElement(this, collisionGroup, collisionMask);
            constructionInfo.Dispose();
        }
    }
}

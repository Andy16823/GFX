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
    /// Defines a box-shaped rigid body behavior for 3D physics simulations.
    /// </summary>
    public class BoxRigidBody : RigidBodyBehavior3D
    {
        /// <summary>
        /// Constructor for creating a BoxRigidBody behavior.
        /// </summary>
        /// <param name="handler">The physics handler managing this rigid body.</param>
        public BoxRigidBody(PhysicHandler handler) : base(handler)
        {
        }

        /// <summary>
        /// Creates a box-shaped rigid body using the parent's size and the specified mass.
        /// </summary>
        /// <param name="mass">The mass of the rigid body.</param>
        public override void CreateRigidBody(float mass, int collisionGroup = -1, int collisionMask = -1)
        {
            this.CreateRigidBody(this.Parent.Size.Half(), mass, collisionGroup, collisionMask);
        }

        /// <summary>
        /// Creates a box-shaped rigid body with the specified half extends, mass, and physics handler.
        /// </summary>
        /// <param name="boxHalfExtends">Half extends of the box rigid body.</param>
        /// <param name="mass">Mass of the rigid body.</param>
        public void CreateRigidBody(Vec3 boxHalfExtends, float mass, int collisionGroup = -1, int collisionMask = -1)
        {
            var element = this.Parent;
            BoxShape boxShape = new BoxShape(boxHalfExtends.ToBulletVec3());
            RigidBodyConstructionInfo info = new RigidBodyConstructionInfo(mass, null, boxShape, boxShape.CalculateLocalInertia(mass));

            Vec3 location = Utils.GetElementWorldLocation(element) + Offset;
            Vec3 rotation = Utils.GetElementWorldRotation(element);

            var btTranslation = BulletSharp.Math.Matrix.Translation(location.ToBulletVec3());
            var btRotation = BulletSharp.Math.Matrix.RotationX(rotation.X) * BulletSharp.Math.Matrix.RotationY(rotation.Y) * BulletSharp.Math.Matrix.RotationZ(rotation.Z);
            var btStartTransform = btTranslation * btRotation;

            info.MotionState = new DefaultMotionState(btStartTransform);
            RigidBody = new RigidBody(info);
            RigidBody.UserObject = element;
            RigidBody.ApplyGravity();
            
            PhysicHandler.ManageElement(this, collisionGroup, collisionMask);
        }
    }
}

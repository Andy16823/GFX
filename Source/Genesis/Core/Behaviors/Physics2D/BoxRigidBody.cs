using BulletSharp;
using Genesis.Math;
using Genesis.Physics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace Genesis.Core.Behaviors.Physics2D
{
    /// <summary>
    /// Represents a 2D box rigid body behavior for game elements.
    /// </summary>
    public class BoxRigidBody : RigidBodyBehavior2D
    {
        /// <summary>
        /// Gets or sets the linear factor for the rigid body.
        /// </summary>
        public Vec3 LinearFactor { get; set; } = new Vec3(1, 1, 0);

        /// <summary>
        /// Gets or sets the angular factor for the rigid body.
        /// </summary>
        public Vec3 AngularFactor { get; set; } = new Vec3(0, 0, 0);

        /// <summary>
        /// Creates the rigid body with the specified mass.
        /// </summary>
        /// <param name="mass">The mass of the rigid body.</param>
        public override void CreateRigidBody(float mass, int collisionGroup = -1, int collisionMask = -1)
        {
            this.CreateRigidBody(mass, Parent.Size.Half(), collisionGroup, collisionMask);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BoxRigidBody"/> class with the specified physics handler.
        /// </summary>
        /// <param name="handler">The physics handler.</param>
        public BoxRigidBody(PhysicHandler handler) : base(handler)
        {
        }

        /// <summary>
        /// Creates the rigid body with the specified mass and half extents.
        /// </summary>
        /// <param name="mass">The mass of the rigid body.</param>
        /// <param name="halfextends">The half extents of the box shape.</param>
        public void CreateRigidBody(float mass, Vec3 halfextends, int collisionGroup = -1, int collisionMask = -1)
        {
            //var capsuleShape = new CapsuleShape(Parent.Size.X / 2, 1.1f);
            var boxShape = new Box2DShape(halfextends.ToBulletVec3());
            //var shape = new Box2DShape(Parent.Size.ToVector3() / 2);
            RigidBodyConstructionInfo info = new RigidBodyConstructionInfo(mass, null, boxShape, boxShape.CalculateLocalInertia(mass));


            BulletSharp.Math.Matrix transform = BulletSharp.Math.Matrix.Translation(Parent.Location.ToBulletVec3() + this.Offset.ToBulletVec3());
            BulletSharp.Math.Matrix rotationMatrix = BulletSharp.Math.Matrix.RotationYawPitchRoll(Parent.Rotation.X, Parent.Rotation.Y, Parent.Rotation.Z);
            BulletSharp.Math.Matrix startTransform = transform * rotationMatrix;

            info.MotionState = new DefaultMotionState(startTransform);
            RigidBody = new BulletSharp.RigidBody(info);
            RigidBody.LinearFactor = this.LinearFactor.ToBulletVec3();
            RigidBody.AngularFactor = this.AngularFactor.ToBulletVec3();
            RigidBody.UserObject = this.Parent;
            this.RigidBody.ApplyGravity();
            PhysicHandler.ManageElement(this, collisionGroup, collisionMask);
            info.Dispose();
        }
    }
}

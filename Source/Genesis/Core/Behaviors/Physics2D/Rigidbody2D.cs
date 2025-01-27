using BulletSharp;
using BulletSharp.Math;
using BulletSharp.SoftBody;
using Genesis.Math;
using Genesis.Physics;
using GlmSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;

namespace Genesis.Core.Behaviors.Physics2D
{
    /// <summary>
    /// Represents a 2D physics behavior for game elements.
    /// </summary>
    public class Rigidbody2D : RigidBodyBehavior2D
    {
        /// <summary>
        /// Gets or sets the linear factor for the rigid body.
        /// </summary>
        public Vec3 LinearFactor { get; set; } = new Vec3(1, 1, 0);

        /// <summary>
        /// Gets or sets the angular factor for the rigid body.
        /// </summary>
        public Vec3 AngularFactor { get; set; } = new Vec3(0, 1, 0);

        /// <summary>
        /// Gets or sets whether physics is enabled.
        /// </summary>
        public bool EnablePhysic { get; set; } = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="Rigidbody2D"/> class with the specified physics handler.
        /// </summary>
        /// <param name="handler">The physics handler.</param>
        public Rigidbody2D(PhysicHandler handler) : base(handler)
        {
        }

        /// <summary>
        /// Creates the rigid body with the specified mass.
        /// </summary>
        /// <param name="mass">The mass of the rigid body.</param>
        public override void CreateRigidBody(float mass, int collisionGroup = -1, int collisionMask = -1)
        {
            this.CreateRigidbody(mass, 0.5f, 1.0f, collisionGroup, collisionMask);
        }

        /// <summary>
        /// Creates the rigid body with the specified mass, capsule radius, and capsule height.
        /// </summary>
        /// <param name="mass">The mass of the rigid body.</param>
        /// <param name="capsuleRadius">The radius of the capsule shape.</param>
        /// <param name="capsuleHeight">The height of the capsule shape.</param>
        public void CreateRigidbody(float mass, float capsuleRadius, float capsuleHeight, int collisionGroup = -1, int collisionMask = -1)
        {
            var capsuleShape = new CapsuleShape(capsuleRadius, capsuleHeight);
            var shape = new Convex2DShape(capsuleShape);
            RigidBodyConstructionInfo info = new RigidBodyConstructionInfo(mass, null, shape, shape.CalculateLocalInertia(mass));

            //Create the start matrix
            BulletSharp.Math.Matrix transform = BulletSharp.Math.Matrix.Translation(Parent.Location.ToBulletVec3());
            Matrix startTransform = transform;

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

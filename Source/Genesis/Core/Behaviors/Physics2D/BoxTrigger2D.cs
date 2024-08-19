using BulletSharp;
using BulletSharp.SoftBody;
using Genesis.Math;
using Genesis.Physics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Core.Behaviors.Physics2D
{
    /// <summary>
    /// Represents a 2D box trigger for physics interactions.
    /// </summary>
    /// <remarks>
    /// Provides functionality to create and manage a box-shaped trigger for detecting collisions in a 2D physics world.
    /// </remarks>
    public class BoxTrigger2D : TriggerBehavior2D
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BoxTrigger2D"/> class with the specified physics handler.
        /// </summary>
        /// <param name="handler">The physics handler to associate with this box trigger.</param>
        public BoxTrigger2D(PhysicHandler handler) : base(handler)
        {

        }

        /// <summary>
        /// Creates the trigger using the parent's size half extents.
        /// </summary>
        public override void CreateTrigger(int collisionGroup = -1, int collisionMask = -1)
        {
            this.CreateTrigger(Parent.Size.Half(), collisionGroup, collisionMask);
        }

        /// <summary>
        /// Creates the trigger with the specified half extents.
        /// </summary>
        /// <param name="halfExtends">The half extents of the box trigger.</param>
        public void CreateTrigger(Vec3 halfExtends, int collisionGroup = -1, int collisionMask = -1)
        {
            Box2DShape box2DShape = new Box2DShape(halfExtends.ToBulletVec3());

            Vec3 location = this.Parent.Location + this.Offset;
            Vec3 rotation = this.Parent.Rotation;

            var btTranslation = BulletSharp.Math.Matrix.Translation(location.ToBulletVec3());
            var btRotation = BulletSharp.Math.Matrix.RotationX(rotation.X) * BulletSharp.Math.Matrix.RotationY(rotation.Y) * BulletSharp.Math.Matrix.RotationZ(rotation.Z);
            var btStartTransform = btTranslation * btRotation;

            this.Trigger = new GhostObject();
            this.Trigger.CollisionShape = box2DShape;
            this.Trigger.WorldTransform = btStartTransform;
            this.Trigger.CollisionFlags = CollisionFlags.NoContactResponse;
            this.Trigger.UserObject = this.Parent;

            this.PhysicHandler.ManageElement(this, collisionGroup, collisionMask);
        }
    }
}

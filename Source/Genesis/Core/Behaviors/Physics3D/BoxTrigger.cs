using BulletSharp;
using Genesis.Math;
using Genesis.Physics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BulletSharp.Dbvt;

namespace Genesis.Core.Behaviors.Physics3D
{
    /// <summary>
    /// Defines a box trigger behavior for 3D physics simulations.
    /// </summary>
    public class BoxTrigger : TriggerBehavior3D
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BoxTrigger"/> class with the specified physics handler.
        /// </summary>
        /// <param name="physicHandler">The physics handler to associate with this box trigger.</param>
        public BoxTrigger(PhysicHandler physicHandler) : base(physicHandler)
        {
        }

        /// <summary>
        /// Creates the trigger using the parent's size half extents.
        /// </summary>
        public override void CreateTrigger(int collisionGroup = -1, int collisionMask = -1)
        {
            this.CreateTrigger(new Vec3(0.5f, 0.5f, 0.5f), collisionGroup, collisionMask);
        }

        /// <summary>
        /// Creates a box trigger with the specified parameters.
        /// </summary>
        /// <param name="boxHalfExtends">The half extends of the box trigger.</param>
        public void CreateTrigger(Vec3 boxHalfExtends, int collisionGroup = -1, int collisionMask = -1)
        {
            var element = this.Parent;
            BoxShape boxShape = new BoxShape(boxHalfExtends.ToBulletVec3());
            var btStartTransform = Utils.GetBtTransform(element, Offset);

            Trigger = new GhostObject();
            Trigger.UserObject = element;
            Trigger.CollisionShape = boxShape;
            Trigger.WorldTransform = btStartTransform;
            Trigger.CollisionFlags = CollisionFlags.NoContactResponse;
            Trigger.CollisionShape.LocalScaling = element.Size.ToBulletVec3();

            PhysicHandler.ManageElement(this, collisionGroup, collisionMask);
        }
    }
}

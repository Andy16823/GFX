using BulletSharp;
using Genesis.Math;
using Genesis.Physics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Core.Behaviors.Physics3D
{
    /// <summary>
    /// Defines a box trigger behavior for 3D physics simulations.
    /// </summary>
    public class BoxTrigger : TriggerBehavior3D
    {
        public override void CreateTrigger(PhysicHandler handler)
        {
            this.CreateTrigger(handler, Parent.Size.Half());
        }

        /// <summary>
        /// Creates a box trigger with the specified parameters.
        /// </summary>
        /// <param name="handler">The physics handler managing this trigger.</param>
        /// <param name="boxHalfExtends">The half extends of the box trigger.</param>
        public void CreateTrigger(PhysicHandler handler, Vec3 boxHalfExtends)
        {
            var element = this.Parent;
            BoxShape boxShape = new BoxShape(boxHalfExtends.ToBulletVec3());

            Vec3 location = Utils.GetElementWorldLocation(element);
            Vec3 rotation = Utils.GetElementWorldRotation(element);

            var btTranslation = BulletSharp.Math.Matrix.Translation(location.ToBulletVec3());
            var btRotation = BulletSharp.Math.Matrix.RotationX(rotation.X) * BulletSharp.Math.Matrix.RotationY(rotation.Y) * BulletSharp.Math.Matrix.RotationZ(rotation.Z);
            var btStartTransform = btTranslation * btRotation;

            Trigger = new GhostObject();
            Trigger.CollisionShape = boxShape;
            Trigger.WorldTransform = btStartTransform;
            Trigger.CollisionFlags = CollisionFlags.NoContactResponse;

            handler.ManageElement(this);
        }
    }
}

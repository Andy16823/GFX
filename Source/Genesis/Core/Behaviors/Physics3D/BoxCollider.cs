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

namespace Genesis.Core.Behaviors.Physics3D
{
    /// <summary>
    /// Defines a box collider behavior for 3D physics simulations.
    /// </summary>

    public class BoxCollider : ColliderBehavior3D
    {

        /// <summary>
        /// Creates a box collider with default half extends.
        /// </summary>
        /// <param name="handler">The physics handler to manage this collider.</param>
        public override void CreateCollider(PhysicHandler handler)
        {
            this.CreateCollider(handler, this.Parent.Size.Half());
        }

        /// <summary>
        /// Creates a box collider with specified half extends.
        /// </summary>
        /// <param name="handler">The physics handler to manage this collider.</param>
        /// <param name="boxHalfExtends">Half extends of the box collider.</param>
        public void CreateCollider(PhysicHandler handler, Vec3 boxHalfExtends)
        {
            var element = this.Parent;
            BoxShape boxShape = new BoxShape(boxHalfExtends.ToBulletVec3());

            Vec3 location = Utils.GetElementWorldLocation(element);
            Vec3 rotation = Utils.GetElementWorldRotation(element);

            var btTranslation = BulletSharp.Math.Matrix.Translation(location.ToBulletVec3());            
            var btRotation = BulletSharp.Math.Matrix.RotationX(rotation.X) * BulletSharp.Math.Matrix.RotationY(rotation.Y) * BulletSharp.Math.Matrix.RotationZ(rotation.Z);
            var btStartTransform = btTranslation * btRotation;

            Collider = new CollisionObject();
            Collider.UserObject = element;
            Collider.CollisionShape = boxShape;
            Collider.WorldTransform = btStartTransform;

            handler.ManageElement(this);
        }
    }
}

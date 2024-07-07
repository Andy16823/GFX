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
    public class BoxCollider : ColliderBehavior2D
    {
        /// <summary>
        /// Creates a collider using the provided PhysicHandler.
        /// </summary>
        /// <param name="handler">The PhysicHandler responsible for managing physics elements.</param>
        public override void CreateCollider(PhysicHandler physicHandler)
        {
            //var shape = new CapsuleShape(Parent.Size.X / 2, 1.5f);
            var shape = new Box2DShape(Parent.Size.ToBulletVec3() / 2);

            Vec3 rotation = this.Parent.Rotation;
            var btTranslation = BulletSharp.Math.Matrix.Translation(Parent.Location.ToBulletVec3() + Offset.ToBulletVec3());
            var btRotation = BulletSharp.Math.Matrix.RotationX(rotation.X) * BulletSharp.Math.Matrix.RotationY(rotation.Y) * BulletSharp.Math.Matrix.RotationZ(rotation.Z);
            var btStartTransform = btTranslation * btRotation;

            Collider = new BulletSharp.CollisionObject();
            Collider.CollisionShape = shape;
            Collider.UserObject = this.Parent;
            Collider.WorldTransform = btStartTransform;
            physicHandler.ManageElement(this);
        }
    }
}

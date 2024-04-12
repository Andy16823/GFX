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
    public class BoxTrigger2D : PhysicsBehavior
    {
        public GhostObject Trigger { get; set; }

        public void CreateCollider(PhysicHandler handler, Vec3 halfExtends)
        {
            Box2DShape box2DShape = new Box2DShape(halfExtends.ToBulletVec3());

            Vec3 location = Utils.GetElementWorldLocation(this.Parent);
            Vec3 rotation = Utils.GetElementWorldRotation(this.Parent);

            var btTranslation = BulletSharp.Math.Matrix.Translation(location.ToBulletVec3());
            var btRotation = BulletSharp.Math.Matrix.RotationX(rotation.X) * BulletSharp.Math.Matrix.RotationY(rotation.Y) * BulletSharp.Math.Matrix.RotationZ(rotation.Z);
            var btStartTransform = btTranslation * btRotation;

            this.Trigger = new GhostObject();
            this.Trigger.CollisionShape = box2DShape;
            this.Trigger.WorldTransform = btStartTransform;
            this.Trigger.CollisionFlags = CollisionFlags.NoContactResponse;
            this.Trigger.UserObject = this.Parent;

            handler.ManageElement(this);
        }

        public override object GetPhysicsObject()
        {
            return Trigger;
        }

        public override T GetPhysicsObject<T>()
        {
            return (T)(object)Trigger;
        }

        public override void OnDestroy(Game game, GameElement parent)
        {
            
        }

        public override void OnInit(Game game, GameElement parent)
        {
            
        }

        public override void OnRender(Game game, GameElement parent)
        {
            
        }

        public override void OnUpdate(Game game, GameElement parent)
        {
            
        }
    }
}

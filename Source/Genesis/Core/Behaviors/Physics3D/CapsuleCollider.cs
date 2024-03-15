using BulletSharp;
using BulletSharp.Math;
using Genesis.Math;
using Genesis.Physics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Core.Behaviors.Physics3D
{
    public class CapsuleCollider : PhysicsBehavior
    {
        public RigidBody RigidBody { get; set; }
        public Vec3 Offset { get; set; }

        public void CreateRigidBody(PhysicHandler handler, float radius, float height, float mass)
        {
            this.CreateRigidBody(handler, Vec3.Zero(), radius, height, mass);
        }

        public void CreateRigidBody(PhysicHandler handler, Vec3 offset, float radius, float height, float mass)
        {
            this.Offset = offset;
            var element = this.Parent;
            CapsuleShape capsuleShape = new CapsuleShape(radius, height);
            RigidBodyConstructionInfo info = new RigidBodyConstructionInfo(mass, null, capsuleShape, capsuleShape.CalculateLocalInertia(mass));

            Vec3 location = Utils.GetElementWorldLocation(element);
            Vec3 rotation = Utils.GetElementWorldRotation(element);

            var btTranslation = BulletSharp.Math.Matrix.Translation(location.ToBulletVec3() + Offset.ToBulletVec3());
            var btRotation = BulletSharp.Math.Matrix.RotationX(rotation.X) * BulletSharp.Math.Matrix.RotationY(rotation.Y) * BulletSharp.Math.Matrix.RotationZ(rotation.Z);
            var btStartTransform = btTranslation * btRotation;

            info.MotionState = new DefaultMotionState(btStartTransform);
            RigidBody = new RigidBody(info);
            RigidBody.UserObject = element;
            RigidBody.ApplyGravity();

            handler.ManageElement(this);
        }

        public override object GetPhysicsObject()
        {
            return RigidBody;
        }

        public override T GetPhysicsObject<T>()
        {
            return (T)(object)RigidBody;
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
            if (RigidBody.InvMass > 0)
            {
                Vector3 position = RigidBody.WorldTransform.Origin;
                Vec3 newLocation = Utils.GetModelSpaceLocation(Parent, new Vec3(position.X, position.Y, position.Z));
                parent.Location = newLocation - Offset;
            }
        }
    }
}

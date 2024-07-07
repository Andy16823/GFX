using BulletSharp;
using Genesis.Core;
using Genesis.Math;
using GlmSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Physics
{
    public abstract class ColliderBehavior2D : PhysicsBehavior
    {
        public CollisionObject Collider { get; set; }
        public Vec3 Offset { get; set; } = Vec3.Zero();

        public abstract void CreateCollider(PhysicHandler physicHandler);

        public void Translate(Vec3 value)
        {
            this.Translate(value.X, value.Y);
        }

        public void Translate(float x, float y)
        {
            // Translation
            var translationMatrix = BulletSharp.Math.Matrix.Translation(x, y, 0);

            // Rotation
            var rotation = BulletSharp.Math.Quaternion.RotationMatrix(Collider.WorldTransform);
            BulletSharp.Math.Matrix rotationMatrix;
            BulletSharp.Math.Matrix.RotationQuaternion(ref rotation, out rotationMatrix);

            this.Collider.WorldTransform = translationMatrix * rotationMatrix;
        }

        public void Rotate(Vec3 value)
        {
            this.Rotate(value.Z);
        }

        public void Rotate(float z)
        {
            // Translation
            var translation = Collider.WorldTransform.Origin;
            var translationMatrix = BulletSharp.Math.Matrix.Translation(translation);

            // Rotation
            var rotation = BulletSharp.Math.Quaternion.RotationMatrix(Collider.WorldTransform);
            BulletSharp.Math.Matrix rotationMatrix;
            BulletSharp.Math.Matrix.RotationYawPitchRoll(0, 0, z, out rotationMatrix);

            this.Collider.WorldTransform = translationMatrix * rotationMatrix;
        }

        public override object GetPhysicsObject()
        {
            return this.Collider;
        }

        public override T GetPhysicsObject<T>()
        {
            return (T)(object)this.Collider;
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
            var position = Collider.WorldTransform.Origin;

            var rotation = BulletSharp.Math.Quaternion.RotationMatrix(Collider.WorldTransform);
            vec3 rotationVector = (vec3)glm.EulerAngles(new quat(rotation.X, rotation.Y, rotation.Z, rotation.W));

            parent.Location = new Vec3(position.X, position.Y, position.Z) - Offset;
            parent.Rotation = new Vec3(Utils.ToDegrees(rotationVector.x), Utils.ToDegrees(rotationVector.y), Utils.ToDegrees(rotationVector.z));
            Collider.Activate(true);
        }
    }
}

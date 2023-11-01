using BulletSharp;
using BulletSharp.Math;
using BulletSharp.SoftBody;
using Genesis.Core.Prefabs;
using Genesis.Physics;
using GlmSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Core.Behaviors.Physics3D
{
    /// <summary>
    /// A ConvexHull Rigidbody for Element3D.
    /// </summary>
    public class ConvexHullBehavior : PhysicsBehavior
    {
        public RigidBody RigidBody { get; set; }

        public void CreateRigidBody(PhysicHandler handler, float mass)
        {
            if(this.Parent.GetType() == typeof(Element3D))
            {
                //Create the shape 
                Element3D element = (Element3D)this.Parent;
                ConvexHullShape shape = new ConvexHullShape(element.Model.GetMesh());
                RigidBodyConstructionInfo info = new RigidBodyConstructionInfo(mass, null, shape, shape.CalculateLocalInertia(mass)); 
                //Create the start matrix
                BulletSharp.Math.Matrix transform = BulletSharp.Math.Matrix.Translation(element.Location.X, element.Location.Y, element.Location.Z);
                BulletSharp.Math.Matrix rotation = BulletSharp.Math.Matrix.RotationX(element.Rotation.X) * BulletSharp.Math.Matrix.RotationY(element.Rotation.Y) * BulletSharp.Math.Matrix.RotationZ(element.Rotation.Z);
                Matrix startTransform = rotation * transform;
                info.MotionState = new DefaultMotionState(startTransform);
                //Create the rigid body
                this.RigidBody = new BulletSharp.RigidBody(info);
                this.RigidBody.ApplyGravity();
                //Scale it
                this.RigidBody.CollisionShape.LocalScaling = new Vector3(element.Size.X, element.Size.Y, element.Size.Z);
                handler.ManageElement(this);
            }
            else
            {
                throw new InvalidOperationException("Invalid element for this Behavior");
            }
        }

        public override void Collide(Scene scene, Game game, RigidBody collisionObject)
        {
            base.Collide(scene, game, collisionObject);

        }

        public void UpdateRigidBody()
        {
            BulletSharp.Math.Matrix transform = BulletSharp.Math.Matrix.Translation(Parent.Location.X, Parent.Location.Y, Parent.Location.Z);
            BulletSharp.Math.Matrix rotation = BulletSharp.Math.Matrix.RotationX(Parent.Rotation.X) * BulletSharp.Math.Matrix.RotationY(Parent.Rotation.Y) * BulletSharp.Math.Matrix.RotationZ(Parent.Rotation.Z);
            this.RigidBody.MotionState = new DefaultMotionState(rotation * transform);
        }
        public override void OnDestroy(Game game, GameElement parent)
        {
        }

        public override void OnInit(Game game, GameElement parent)
        {
            if (parent.GetType() != typeof(Element3D))
            {
                throw new InvalidOperationException("The Physics3D Behavior can only be attached to an Element3D.");
            }
        }

        public override void OnRender(Game game, GameElement parent)
        {

        }

        public override void OnUpdate(Game game, GameElement parent)
        {
            if (this.RigidBody != null)
            {
                Vector3 position = RigidBody.WorldTransform.Origin;
                parent.Location.X = position.X;
                parent.Location.Y = position.Y;
                parent.Location.Z = position.Z;

                Quaternion rotation = Quaternion.RotationMatrix(RigidBody.WorldTransform);
                vec3 vec = (vec3)glm.EulerAngles(new quat(rotation.X, rotation.Y, rotation.Z, rotation.W));

                parent.Rotation.X = vec.x;
                parent.Rotation.Y = vec.y;
                parent.Rotation.Z = vec.z;
            }
        }

        public override object GetPhysicsObject() 
        {
            return RigidBody;
        }

        public override T GetPhysicsObject<T>()
        {
            return (T)(object)RigidBody;
        }
    }
}

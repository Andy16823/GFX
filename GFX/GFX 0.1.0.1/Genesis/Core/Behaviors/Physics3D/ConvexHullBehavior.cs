using BulletSharp;
using BulletSharp.Math;
using BulletSharp.SoftBody;
using Genesis.Core.GameElements;
using Genesis.Math;
using Genesis.Physics;
using GlmSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

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
                ConvexHullShape shape = new ConvexHullShape(element.GetShape());
                RigidBodyConstructionInfo info = new RigidBodyConstructionInfo(mass, null, shape, shape.CalculateLocalInertia(mass)); 
                //Create the start matrix
                Vec3 location = Utils.GetElementWorldLocation(element);
                Vec3 rotation = Utils.GetElementWorldRotation(element);
                Vec3 scale = Utils.GetElementWorldScale(element);

                BulletSharp.Math.Matrix transform = BulletSharp.Math.Matrix.Translation(location.X, location.Y, location.Z);
                BulletSharp.Math.Matrix rotMat = BulletSharp.Math.Matrix.RotationX(rotation.X) * BulletSharp.Math.Matrix.RotationY(rotation.Y) * BulletSharp.Math.Matrix.RotationZ(rotation.Z);
                Matrix startTransform = rotMat * transform;
                info.MotionState = new DefaultMotionState(startTransform);
                //Create the rigid body
                this.RigidBody = new BulletSharp.RigidBody(info);
                this.RigidBody.ApplyGravity();
                //Scale it
                this.RigidBody.CollisionShape.LocalScaling = new Vector3(scale.X, scale.Y, scale.Z);
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
            Vec3 location = Utils.GetElementWorldLocation(Parent);
            Vec3 rotation = Utils.GetElementWorldRotation(Parent);

            quat quat = new quat(new vec3(Utils.ToRadians(rotation.X), Utils.ToRadians(rotation.Y), Utils.ToRadians(rotation.Z)));
            mat4 rotMat = new mat4(quat);

            BulletSharp.Math.Matrix transform = BulletSharp.Math.Matrix.Translation(location.X, location.Y, location.Z);
            BulletSharp.Math.Matrix btrotMat = new Matrix(rotMat.ToArray());
            this.RigidBody.MotionState = new DefaultMotionState(btrotMat * transform);

            Vec3 scale = Utils.GetElementWorldScale(this.Parent);
            this.RigidBody.CollisionShape.LocalScaling = new Vector3(scale.X, scale.Y, scale.Z);
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
            if (this.RigidBody != null && this.RigidBody.InvMass > 0)
            {
                Vector3 position = RigidBody.WorldTransform.Origin;
                Quaternion rotation = Quaternion.RotationMatrix(RigidBody.WorldTransform);
                vec3 vec = (vec3)glm.EulerAngles(new quat(rotation.X, rotation.Y, rotation.Z, rotation.W));

                Vec3 newLocation = Utils.GetModelSpaceLocation(Parent, new Vec3(position.X, position.Y, position.Z));
                Vec3 newRotation = Utils.GetModelSpaceRotation(Parent, new Vec3(vec));

                parent.Location = newLocation;
                parent.Rotation = newRotation;
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

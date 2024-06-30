using BulletSharp;
using BulletSharp.Math;
using Genesis.Core.GameElements;
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
    /// Represents a Physics behavior for a 3D cube with a convex hull shape.
    /// </summary>
    public class QubeConvexHull : PhysicsBehavior
    {
        /// <summary>
        /// Gets or sets the RigidBody associated with this behavior.
        /// </summary>
        public RigidBody RigidBody { get; set; }

        /// <summary>
        /// Creates a RigidBody for the QubeConvexHull behavior.
        /// </summary>
        /// <param name="handler">Physics handler responsible for managing elements.</param>
        /// <param name="mass">Mass of the rigid body.</param>
        public void CreateRigidBody(PhysicHandler handler, float mass)
        {
            if (this.Parent.GetType() == typeof(Qube))
            {
                //Create the shape 
                Qube element = (Qube)this.Parent;
                ConvexHullShape shape = new ConvexHullShape(element.Shape.GetShape());
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

        /// <summary>
        /// Updates the RigidBody's position and rotation based on the associated parent element.
        /// </summary>
        public void UpdateRigidBody()
        {
            Vec3 location = Utils.GetElementWorldLocation(Parent);
            Vec3 rotation = Utils.GetElementWorldRotation(Parent);

            BulletSharp.Math.Matrix transform = BulletSharp.Math.Matrix.Translation(location.X, location.Y, location.Z);
            BulletSharp.Math.Matrix rotMat = BulletSharp.Math.Matrix.RotationX(rotation.X) * BulletSharp.Math.Matrix.RotationY(rotation.Y) * BulletSharp.Math.Matrix.RotationZ(rotation.Z);
            this.RigidBody.MotionState = new DefaultMotionState(rotMat * transform);
        }

        /// <summary>
        /// Gets the associated RigidBody as an object.
        /// </summary>
        public override object GetPhysicsObject()
        {
            return RigidBody;
        }

        /// <summary>
        /// Gets the associated RigidBody with the specified type.
        /// </summary>
        public override T GetPhysicsObject<T>()
        {
            return (T)(object)RigidBody;
        }

        /// <summary>
        /// Called when the associated game element is destroyed.
        /// </summary>
        public override void OnDestroy(Game game, GameElement parent)
        {
            
        }

        /// <summary>
        /// Called when the associated game element is initialized.
        /// </summary>
        public override void OnInit(Game game, GameElement parent)
        {
            
        }

        /// <summary>
        /// Called during the rendering phase.
        /// </summary>
        public override void OnRender(Game game, GameElement parent)
        {
            
        }

        /// <summary>
        /// Called during the update phase.
        /// </summary>
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
    }
}

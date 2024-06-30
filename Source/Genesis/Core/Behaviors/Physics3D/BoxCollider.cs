using BulletSharp;
using BulletSharp.SoftBody;
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
    /// Defines a box collider behavior for 3D physics simulations.
    /// </summary>

    public class BoxCollider : PhysicsBehavior
    {
        /// <summary>
        /// Gets or sets the rigid body associated with this box collider.
        /// </summary>
        public RigidBody RigidBody { get; set; }

        /// <summary>
        /// Creates a box collider with the specified parameters.
        /// </summary>
        /// <param name="handler">The physics handler managing this collider.</param>
        /// <param name="boxHalfExtends">The half extends of the box collider.</param>
        /// <param name="mass">The mass of the box collider.</param>
        public void CreateCollider(PhysicHandler handler, Vec3 boxHalfExtends, float mass)
        {
            var element = this.Parent;
            BoxShape boxShape = new BoxShape(boxHalfExtends.ToBulletVec3());
            RigidBodyConstructionInfo info = new RigidBodyConstructionInfo(mass, null, boxShape, boxShape.CalculateLocalInertia(mass));

            Vec3 location = Utils.GetElementWorldLocation(element);
            Vec3 rotation = Utils.GetElementWorldRotation(element);

            var btTranslation = BulletSharp.Math.Matrix.Translation(location.ToBulletVec3());
            var btRotation = BulletSharp.Math.Matrix.RotationX(rotation.X) * BulletSharp.Math.Matrix.RotationY(rotation.Y) * BulletSharp.Math.Matrix.RotationZ(rotation.Z);
            var btStartTransform = btTranslation * btRotation;

            info.MotionState = new DefaultMotionState(btStartTransform);
            RigidBody = new RigidBody(info);
            RigidBody.UserObject = element;
            RigidBody.ApplyGravity();
            
            handler.ManageElement(this);
        }

        /// <summary>
        /// Retrieves the physics object associated with this box collider.
        /// </summary>
        /// <returns>The physics object associated with this collider.</returns>
        public override object GetPhysicsObject()
        {
            return RigidBody;
        }

        /// <summary>
        /// Retrieves the physics object associated with this box collider, cast to the specified type.
        /// </summary>
        /// <typeparam name="T">The type to cast the physics object to.</typeparam>
        /// <returns>The physics object associated with this collider, cast to the specified type.</returns>
        public override T GetPhysicsObject<T>()
        {
            return (T)(object)RigidBody;
        }

        /// <summary>
        /// Called when the collider is destroyed.
        /// </summary>
        /// <param name="game">The current game instance.</param>
        /// <param name="parent">The parent game element.</param>
        public override void OnDestroy(Game game, GameElement parent)
        {
            
        }

        /// <summary>
        /// Called when the collider is initialized.
        /// </summary>
        /// <param name="game">The current game instance.</param>
        /// <param name="parent">The parent game element.</param>
        public override void OnInit(Game game, GameElement parent)
        {
            
        }

        /// <summary>
        /// Called when the collider needs to be rendered.
        /// </summary>
        /// <param name="game">The current game instance.</param>
        /// <param name="parent">The parent game element.</param>
        public override void OnRender(Game game, GameElement parent)
        {
            
        }

        /// <summary>
        /// Called when the collider needs to be updated.
        /// </summary>
        /// <param name="game">The current game instance.</param>
        /// <param name="parent">The parent game element.</param>
        public override void OnUpdate(Game game, GameElement parent)
        {
            
        }
    }
}

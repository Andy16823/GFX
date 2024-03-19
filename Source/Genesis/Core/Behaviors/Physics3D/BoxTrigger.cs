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
    public class BoxTrigger : PhysicsBehavior
    {
        /// <summary>
        /// Gets or sets the ghost object associated with this box trigger.
        /// </summary>
        public GhostObject GhostObject { get; set; }

        /// <summary>
        /// Creates a box trigger with the specified parameters.
        /// </summary>
        /// <param name="handler">The physics handler managing this trigger.</param>
        /// <param name="boxHalfExtends">The half extends of the box trigger.</param>
        public void CreateCollider(PhysicHandler handler, Vec3 boxHalfExtends)
        {
            var element = this.Parent;
            BoxShape boxShape = new BoxShape(boxHalfExtends.ToBulletVec3());

            Vec3 location = Utils.GetElementWorldLocation(element);
            Vec3 rotation = Utils.GetElementWorldRotation(element);

            var btTranslation = BulletSharp.Math.Matrix.Translation(location.ToBulletVec3());
            var btRotation = BulletSharp.Math.Matrix.RotationX(rotation.X) * BulletSharp.Math.Matrix.RotationY(rotation.Y) * BulletSharp.Math.Matrix.RotationZ(rotation.Z);
            var btStartTransform = btTranslation * btRotation;

            GhostObject = new GhostObject();
            GhostObject.CollisionShape = boxShape;
            GhostObject.WorldTransform = btStartTransform;
            GhostObject.CollisionFlags = CollisionFlags.NoContactResponse;

            handler.ManageElement(this);
        }

        /// <summary>
        /// Retrieves the physics object associated with this box trigger.
        /// </summary>
        /// <returns>The physics object associated with this trigger.</returns>
        public override object GetPhysicsObject()
        {
            return GhostObject;
        }

        /// <summary>
        /// Retrieves the physics object associated with this box trigger, cast to the specified type.
        /// </summary>
        /// <typeparam name="T">The type to cast the physics object to.</typeparam>
        /// <returns>The physics object associated with this trigger, cast to the specified type.</returns>
        public override T GetPhysicsObject<T>()
        {
            return (T)(object)GhostObject;
        }

        /// <summary>
        /// Called when the trigger is destroyed.
        /// </summary>
        /// <param name="game">The current game instance.</param>
        /// <param name="parent">The parent game element.</param>
        public override void OnDestroy(Game game, GameElement parent)
        {

        }

        /// <summary>
        /// Called when the trigger is initialized.
        /// </summary>
        /// <param name="game">The current game instance.</param>
        /// <param name="parent">The parent game element.</param>
        public override void OnInit(Game game, GameElement parent)
        {

        }

        /// <summary>
        /// Called when the trigger needs to be rendered.
        /// </summary>
        /// <param name="game">The current game instance.</param>
        /// <param name="parent">The parent game element.</param>
        public override void OnRender(Game game, GameElement parent)
        {

        }

        /// <summary>
        /// Called when the trigger needs to be updated.
        /// </summary>
        /// <param name="game">The current game instance.</param>
        /// <param name="parent">The parent game element.</param>
        public override void OnUpdate(Game game, GameElement parent)
        {

        }
    }
}

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
    /// <summary>
    /// Represents a 2D box trigger for physics interactions.
    /// </summary>
    public class BoxTrigger2D : PhysicsBehavior
    {
        /// <summary>
        /// Gets or sets the ghost object representing the trigger.
        /// </summary>
        public GhostObject Trigger { get; set; }

        /// <summary>
        /// Creates a collider for the box trigger.
        /// </summary>
        /// <param name="handler">The physics handler.</param>
        /// <param name="halfExtends">Half extends of the box.</param>
        public void CreateCollider(PhysicHandler handler, Vec3 halfExtends)
        {
            Box2DShape box2DShape = new Box2DShape(halfExtends.ToBulletVec3());

            Vec3 location = this.Parent.Location;
            Vec3 rotation = this.Parent.Rotation;

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

        /// <summary>
        /// Gets the physics object associated with this behavior.
        /// </summary>
        /// <returns>The ghost object representing the trigger.</returns>
        public override object GetPhysicsObject()
        {
            return Trigger;
        }

        /// <summary>
        /// Gets the physics object associated with this behavior, cast to the specified type.
        /// </summary>
        /// <typeparam name="T">The type to cast to.</typeparam>
        /// <returns>The ghost object representing the trigger.</returns>
        public override T GetPhysicsObject<T>()
        {
            return (T)(object)Trigger;
        }

        /// <summary>
        /// Called when the behavior is being destroyed.
        /// </summary>
        /// <param name="game">The game instance.</param>
        /// <param name="parent">The parent game element.</param>
        public override void OnDestroy(Game game, GameElement parent)
        {
            
        }

        /// <summary>
        /// Called when the behavior is initialized.
        /// </summary>
        /// <param name="game">The game instance.</param>
        /// <param name="parent">The parent game element.</param>
        public override void OnInit(Game game, GameElement parent)
        {
            
        }

        /// <summary>
        /// Called when the behavior needs to render something.
        /// </summary>
        /// <param name="game">The game instance.</param>
        /// <param name="parent">The parent game element.</param>
        public override void OnRender(Game game, GameElement parent)
        {
            
        }

        /// <summary>
        /// Called when the behavior needs to update its state.
        /// </summary>
        /// <param name="game">The game instance.</param>
        /// <param name="parent">The parent game element.</param>
        public override void OnUpdate(Game game, GameElement parent)
        {
            
        }
    }
}

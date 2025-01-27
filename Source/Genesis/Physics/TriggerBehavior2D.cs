using BulletSharp;
using Genesis.Core;
using Genesis.Math;
using GlmSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Physics
{
    /// <summary>
    /// Abstract base class for 2D trigger behaviors in the physics system.
    /// </summary>
    public abstract class TriggerBehavior2D : PhysicsBehavior
    {
        /// <summary>
        /// Gets or sets the offset position for the trigger.
        /// </summary>
        public Vec3 Offset { get; set; } = Vec3.Zero();

        /// <summary>
        /// Gets or sets the ghost object representing the trigger.
        /// </summary>
        public GhostObject Trigger { get; set; }

        /// <summary>
        /// Gets or sets the physics handler associated with this trigger behavior.
        /// </summary>
        /// <value>
        /// The <see cref="PhysicHandler"/> instance associated with this trigger behavior.
        /// </value>
        public PhysicHandler PhysicHandler { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TriggerBehavior2D"/> class with the specified physics handler.
        /// </summary>
        /// <param name="handler">The physics handler to associate with this trigger behavior.</param>
        public TriggerBehavior2D(PhysicHandler handler)
        {
            this.PhysicHandler = handler;
        }

        /// <summary>
        /// Creates the trigger object using the specified physics handler.
        /// </summary>
        public abstract void CreateTrigger(int collisionGroup = -1, int collisionMask = -1);

        /// <summary>
        /// Translates the trigger by the specified vector.
        /// </summary>
        /// <param name="value">The vector specifying the translation values.</para
        public virtual void Translate(Vec3 value)
        {
            this.Translate(value.X, value.Y);
        }

        /// <summary>
        /// Translates the trigger by the specified x and y values.
        /// </summary>
        /// <param name="x">The translation value along the x-axis.</param>
        /// <param name="y">The translation value along the y-axis.</param>
        public virtual void Translate(float x, float y)
        {
            var translationMatrix = BulletSharp.Math.Matrix.Translation(x, y, 0);
            var rotation = BulletSharp.Math.Quaternion.RotationMatrix(Trigger.WorldTransform);
            var rotationMatrix = BulletSharp.Math.Matrix.RotationQuaternion(rotation);

            this.Trigger.WorldTransform = rotationMatrix * translationMatrix;
        }

        /// <summary>
        /// Rotates the trigger by the specified vector.
        /// </summary>
        /// <param name="value">The vector specifying the rotation values.</param>
        public virtual void Rotate(Vec3 value)
        {
            this.Rotate(value.Z);
        }

        /// <summary>
        /// Rotates the trigger by the specified z value.
        /// </summary>
        /// <param name="z">The rotation value around the z-axis.</param>
        public virtual void Rotate(float z)
        {
            var translation = this.Trigger.WorldTransform.Origin;
            var translationMatrix = BulletSharp.Math.Matrix.Translation(translation);

            var rotationMatrix = BulletSharp.Math.Matrix.RotationYawPitchRoll(0, 0, z);
            this.Trigger.WorldTransform = rotationMatrix * translationMatrix;
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
        /// <typeparam name="T">The type to cast the physics object to.</typeparam>
        /// <returns>The ghost object representing the trigger, cast to the specified type.</returns>
        public override T GetPhysicsObject<T>()
        {
            return (T)(object)Trigger;
        }

        /// <summary>
        /// Updates the trigger's position and rotation each frame.
        /// </summary>
        /// <param name="game">The game instance.</param>
        /// <param name="parent">The parent game element.</param>
        public override void OnUpdate(Game game, GameElement parent)
        {
            // Translation
            var position = Trigger.WorldTransform.Origin;

            // Rotation
            var rotation = BulletSharp.Math.Quaternion.RotationMatrix(Trigger.WorldTransform);
            vec3 rotationVector = (vec3)glm.EulerAngles(new quat(rotation.X, rotation.Y, rotation.Z, rotation.W));

            // Set new values
            parent.Location = new Vec3(position.X, position.Y, position.Z) - Offset;
            parent.Rotation = new Vec3(Utils.ToDegrees(rotationVector.x), Utils.ToDegrees(rotationVector.y), Utils.ToDegrees(rotationVector.z));
            Trigger.Activate(true);
        }

        /// <summary>
        /// Called when the trigger is destroyed.
        /// </summary>
        /// <param name="game">The game instance.</param>
        /// <param name="parent">The parent game element.</param>
        public override void OnDestroy(Game game, GameElement parent)
        {
            this.RemoveTrigger();
        }

        /// <summary>
        /// Removes the Trigger from the physics world
        /// </summary>
        public virtual void RemoveTrigger()
        {
            this.PhysicHandler.RemoveElement(this);
            this.Trigger.CollisionShape.Dispose();
            this.Trigger.Dispose();
        }

        /// <summary>
        /// Called when the trigger is initialized.
        /// </summary>
        /// <param name="game">The game instance.</param>
        /// <param name="parent">The parent game element.</param>
        public override void OnInit(Game game, GameElement parent)
        {

        }

        /// <summary>
        /// Called when the trigger is rendered.
        /// </summary>
        /// <param name="game">The game instance.</param>
        /// <param name="parent">The parent game element.</param>
        public override void OnRender(Game game, GameElement parent)
        {

        }

        public override void OnCollide(Collision collision, GameElement parent)
        {
            
        }
    }
}

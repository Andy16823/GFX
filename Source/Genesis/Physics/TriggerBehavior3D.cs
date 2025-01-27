using BulletSharp;
using Genesis.Core;
using Genesis.Math;
using GlmSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BulletSharp.Dbvt;

namespace Genesis.Physics
{
    /// <summary>
    /// Abstract base class for defining trigger behaviors in 3D physics simulations.
    /// </summary>
    public abstract class TriggerBehavior3D : PhysicsBehavior
    {
        /// <summary>
        /// Gets or sets the ghost object representing the trigger.
        /// </summary>
        public GhostObject Trigger { get; set; }

        /// <summary>
        /// Gets or sets the offset of the trigger from its parent's location.
        /// </summary>
        public Vec3 Offset { get; set; } = Vec3.Zero();

        /// <summary>
        /// Gets or sets the physics handler associated with this 3D trigger behavior.
        /// </summary>
        /// <value>
        /// The <see cref="PhysicHandler"/> instance associated with this 3D trigger behavior.
        /// </value>
        public PhysicHandler PhysicHandler { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TriggerBehavior3D"/> class with the specified physics handler.
        /// </summary>
        /// <param name="physicHandler">The physics handler to associate with this 3D trigger behavior.</param>
        public TriggerBehavior3D(PhysicHandler physicHandler)
        {
            this.PhysicHandler = physicHandler;
        }

        /// <summary>
        /// Creates the trigger associated with this behavior.
        /// </summary>
        /// <param name="handler">The physics handler to manage this trigger.</param>
        public abstract void CreateTrigger(int collisionGroup = -1, int collisionMask = -1);

        /// <summary>
        /// Retrieves the physics object associated with this trigger.
        /// </summary>
        /// <returns>The physics object associated with this trigger.</returns>
        public override object GetPhysicsObject()
        {
            return Trigger;
        }

        /// <summary>
        /// Retrieves the physics object associated with this trigger, cast to the specified type.
        /// </summary>
        /// <typeparam name="T">The type to cast the physics object to.</typeparam>
        /// <returns>The physics object associated with this trigger, cast to the specified type.</returns>
        public override T GetPhysicsObject<T>()
        {
            return (T)(object)Trigger;
        }

        /// <summary>
        /// Rotates the trigger by the specified vector of Euler angles.
        /// </summary>
        /// <param name="value">A vector containing rotation angles for X, Y, and Z axes.</param>
        public virtual void Rotate(Vec3 value)
        {
            this.Rotate(value.X, value.Y, value.Z);
        }

        /// <summary>
        /// Rotates the trigger by the specified Euler angles.
        /// </summary>
        /// <param name="x">The rotation angle around the X-axis.</param>
        /// <param name="y">The rotation angle around the Y-axis.</param>
        /// <param name="z">The rotation angle around the Z-axis.</param>
        public virtual void Rotate(float x, float y, float z)
        {
            // Translation
            var translation = Trigger.WorldTransform.Origin;
            var translationMatrix = BulletSharp.Math.Matrix.Translation(translation);

            // Rotation
            var rotation = BulletSharp.Math.Quaternion.RotationMatrix(Trigger.WorldTransform);
            BulletSharp.Math.Matrix rotationMatrix;
            BulletSharp.Math.Matrix.RotationYawPitchRoll(x, y, z, out rotationMatrix);

            this.Trigger.WorldTransform = rotationMatrix * translationMatrix;
        }

        /// <summary>
        /// Translates the trigger by the specified vector.
        /// </summary>
        /// <param name="value">The translation vector.</param>
        public virtual void Translate(Vec3 value)
        {
            this.Translate(value.X, value.Y, value.Z);
        }

        /// <summary>
        /// Translates the trigger by the specified distances.
        /// </summary>
        /// <param name="x">The distance to translate along the X-axis.</param>
        /// <param name="y">The distance to translate along the Y-axis.</param>
        /// <param name="z">The distance to translate along the Z-axis.</param>
        public virtual void Translate(float x, float y, float z)
        {
            // Translation
            var translationMatrix = BulletSharp.Math.Matrix.Translation(x, y, z);

            // Rotation
            var rotation = BulletSharp.Math.Quaternion.RotationMatrix(Trigger.WorldTransform);
            BulletSharp.Math.Matrix rotationMatrix;
            BulletSharp.Math.Matrix.RotationQuaternion(ref rotation, out rotationMatrix);

            this.Trigger.WorldTransform = rotationMatrix * translationMatrix;
        }

        /// <summary>
        /// Called when updating the game state.
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

            // New Location
            Vec3 newLocation = Utils.GetModelSpaceLocation(Parent, new Vec3(position.X, position.Y, position.Z));
            Vec3 newRotation = Utils.GetModelSpaceRotation(Parent, new Vec3(rotationVector));

            parent.Location = newLocation - Offset;
            parent.Rotation = new Vec3(Utils.ToDegrees(newRotation.X), Utils.ToDegrees(newRotation.Y), Utils.ToDegrees(newRotation.Z));
            Trigger.Activate(true);
        }

        /// <summary>
        /// Called when destroying the trigger.
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
        /// Called when initializing the trigger.
        /// </summary>
        /// <param name="game">The game instance.</param>
        /// <param name="parent">The parent game element.</param>
        public override void OnInit(Game game, GameElement parent)
        {
            
        }

        /// <summary>
        /// Called when rendering the trigger.
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

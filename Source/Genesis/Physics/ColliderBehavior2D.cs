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
    /// <summary>
    /// Represents a 2D collider behavior for a physics object in the Genesis game engine.
    /// </summary>
    /// <remarks>
    /// Provides methods for creating and manipulating a collider object, including translation and rotation.
    /// </remarks>
    public abstract class ColliderBehavior2D : PhysicsBehavior
    {
        /// <summary>
        /// Gets or sets the collision object associated with this collider behavior.
        /// </summary>
        public CollisionObject Collider { get; set; }

        /// <summary>
        /// Gets or sets the offset position for the collider.
        /// </summary>
        public Vec3 Offset { get; set; } = Vec3.Zero();

        /// <summary>
        /// Gets or sets the physics handler associated with this collider behavior.
        /// </summary>
        public PhysicHandler PhysicHandler { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ColliderBehavior2D"/> class with the specified physics handler.
        /// </summary>
        /// <param name="physicHandler">The physics handler to associate with this collider behavior.</param>
        public ColliderBehavior2D(PhysicHandler physicHandler)
        {
            this.PhysicHandler = physicHandler;
        }

        /// <summary>
        /// Creates the collider object.
        /// </summary>
        public abstract void CreateCollider(int collisionGroup = -1, int collisionMask = -1);

        /// <summary>
        /// Translates the collider by the specified vector.
        /// </summary>
        /// <param name="value">The vector specifying the translation values.</param>
        public void Translate(Vec3 value)
        {
            this.Translate(value.X, value.Y);
        }

        /// <summary>
        /// Translates the collider by the specified x and y values.
        /// </summary>
        /// <param name="x">The translation value along the x-axis.</param>
        /// <param name="y">The translation value along the y-axis.</param>
        public void Translate(float x, float y)
        {
            // Translation
            var translationMatrix = BulletSharp.Math.Matrix.Translation(x, y, 0);

            // Rotation
            var rotation = BulletSharp.Math.Quaternion.RotationMatrix(Collider.WorldTransform);
            BulletSharp.Math.Matrix rotationMatrix;
            BulletSharp.Math.Matrix.RotationQuaternion(ref rotation, out rotationMatrix);

            this.Collider.WorldTransform = rotationMatrix * translationMatrix;
        }

        /// <summary>
        /// Rotates the collider by the specified vector.
        /// </summary>
        /// <param name="value">The vector specifying the rotation values.</param>
        public void Rotate(Vec3 value)
        {
            this.Rotate(value.Z);
        }

        /// <summary>
        /// Rotates the collider by the specified z value.
        /// </summary>
        /// <param name="z">The rotation value around the z-axis.</param>
        public void Rotate(float z)
        {
            // Translation
            var translation = Collider.WorldTransform.Origin;
            var translationMatrix = BulletSharp.Math.Matrix.Translation(translation);

            // Rotation
            var rotation = BulletSharp.Math.Quaternion.RotationMatrix(Collider.WorldTransform);
            BulletSharp.Math.Matrix rotationMatrix;
            BulletSharp.Math.Matrix.RotationYawPitchRoll(0, 0, z, out rotationMatrix);

            this.Collider.WorldTransform = rotationMatrix * translationMatrix;
        }

        /// <summary>
        /// Gets the physics object associated with this behavior.
        /// </summary>
        /// <returns>The collision object representing the collider.</returns>
        public override object GetPhysicsObject()
        {
            return this.Collider;
        }

        /// <summary>
        /// Gets the physics object associated with this behavior, cast to the specified type.
        /// </summary>
        /// <typeparam name="T">The type to cast the physics object to.</typeparam>
        /// <returns>The collision object representing the collider, cast to the specified type.</returns>
        public override T GetPhysicsObject<T>()
        {
            return (T)(object)this.Collider;
        }

        /// <summary>
        /// Called when the collider is destroyed.
        /// </summary>
        /// <param name="game">The game instance.</param>
        /// <param name="parent">The parent game element.</param>
        public override void OnDestroy(Game game, GameElement parent)
        {
            this.RemoveCollider();
        }

        /// <summary>
        /// Removes the Collider from the physics world
        /// </summary>
        public virtual void RemoveCollider()
        {
            this.PhysicHandler.RemoveElement(this);
            this.Collider.CollisionShape.Dispose();
            this.Collider.Dispose();
        }

        /// <summary>
        /// Called when the collider is initialized.
        /// </summary>
        /// <param name="game">The game instance.</param>
        /// <param name="parent">The parent game element.</param>
        public override void OnInit(Game game, GameElement parent)
        {

        }

        /// <summary>
        /// Called when the collider is rendered.
        /// </summary>
        /// <param name="game">The game instance.</param>
        /// <param name="parent">The parent game element.</param>
        public override void OnRender(Game game, GameElement parent)
        {

        }

        /// <summary>
        /// Updates the collider's position and rotation each frame.
        /// </summary>
        /// <param name="game">The game instance.</param>
        /// <param name="parent">The parent game element.</param>
        public override void OnUpdate(Game game, GameElement parent)
        {
            var position = Collider.WorldTransform.Origin;

            var rotation = BulletSharp.Math.Quaternion.RotationMatrix(Collider.WorldTransform);
            vec3 rotationVector = (vec3)glm.EulerAngles(new quat(rotation.X, rotation.Y, rotation.Z, rotation.W));

            parent.Location = new Vec3(position.X, position.Y, position.Z) - Offset;
            parent.Rotation = new Vec3(Utils.ToDegrees(rotationVector.x), Utils.ToDegrees(rotationVector.y), Utils.ToDegrees(rotationVector.z));
            Collider.Activate(true);
        }

        public override void OnCollide(Collision collision, GameElement parent)
        {
            
        }
    }
}

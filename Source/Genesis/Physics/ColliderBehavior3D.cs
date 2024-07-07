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
    public abstract class ColliderBehavior3D : PhysicsBehavior
    {
        public CollisionObject Collider { get; set; }
        public Vec3 Offset { get; set; } = Vec3.Zero();

        /// <summary>
        /// Abstract method to create the collider.
        /// Implement this method in derived classes to define the specific behavior for creating a collider.
        /// </summary>
        /// <param name="physicHandler">The physics handler responsible for managing physics-related tasks.</param>
        public abstract void CreateCollider(PhysicHandler physicHandler);

        /// <summary>
        /// Translates the collider by the specified vector.
        /// </summary>
        /// <param name="value">The translation vector.</param>
        public virtual void Translate(Vec3 value)
        {
            this.Translate(value.X, value.Y, value.Z);
        }

        /// <summary>
        /// Translates the collider by the specified distances.
        /// </summary>
        /// <param name="x">The distance to translate along the X-axis.</param>
        /// <param name="y">The distance to translate along the Y-axis.</param>
        /// <param name="z">The distance to translate along the Z-axis.</param>
        public virtual void Translate(float x, float y, float z)
        {
            // Translation
            var translationMatrix = BulletSharp.Math.Matrix.Translation(x, y, z);

            // Rotation
            var rotation = BulletSharp.Math.Quaternion.RotationMatrix(Collider.WorldTransform);
            BulletSharp.Math.Matrix rotationMatrix;
            BulletSharp.Math.Matrix.RotationQuaternion(ref rotation, out rotationMatrix);

            this.Collider.WorldTransform = translationMatrix * rotationMatrix;
        }

        /// <summary>
        /// Rotates the collider by the specified vector of Euler angles.
        /// </summary>
        /// <param name="value">A vector containing rotation angles for X, Y, and Z axes.</param>
        public virtual void Rotate(Vec3 value)
        {
            this.Rotate(value.X, value.Y, value.Z);
        }

        /// <summary>
        /// Rotates the collider by the specified Euler angles.
        /// </summary>
        /// <param name="x">The rotation angle around the X-axis.</param>
        /// <param name="y">The rotation angle around the Y-axis.</param>
        /// <param name="z">The rotation angle around the Z-axis.</param>
        public virtual void Rotate(float x, float y, float z) 
        {
            // Translation
            var translation = Collider.WorldTransform.Origin;
            var translationMatrix = BulletSharp.Math.Matrix.Translation(translation);

            // Rotation
            var rotation = BulletSharp.Math.Quaternion.RotationMatrix(Collider.WorldTransform);
            BulletSharp.Math.Matrix rotationMatrix;
            BulletSharp.Math.Matrix.RotationYawPitchRoll(x, y, z, out rotationMatrix);

            this.Collider.WorldTransform = translationMatrix * rotationMatrix;
        }

        /// <summary>
        /// Updates the collider's position and rotation based on its current state in the physics simulation.
        /// </summary>
        /// <param name="game">The game instance.</param>
        /// <param name="parent">The parent game element.</param>
        public override void OnUpdate(Game game, GameElement parent)
        {
            // Translation
            var position = Collider.WorldTransform.Origin;
            
            // Rotation
            var rotation = BulletSharp.Math.Quaternion.RotationMatrix(Collider.WorldTransform);
            vec3 rotationVector = (vec3)glm.EulerAngles(new quat(rotation.X, rotation.Y, rotation.Z, rotation.W));

            // New Location
            Vec3 newLocation = Utils.GetModelSpaceLocation(Parent, new Vec3(position.X, position.Y, position.Z));
            Vec3 newRotation = Utils.GetModelSpaceRotation(Parent, new Vec3(rotationVector));

            parent.Location = newLocation - Offset;
            parent.Rotation = new Vec3(Utils.ToDegrees(newRotation.X), Utils.ToDegrees(newRotation.Y), Utils.ToDegrees(newRotation.Z));
            Collider.Activate(true);
        }

        /// <summary>
        /// Retrieves the physics object associated with this behavior.
        /// </summary>
        /// <returns>The rigid body associated with this behavior.</returns>
        public override object GetPhysicsObject()
        {
            return Collider;
        }

        /// <summary>
        /// Retrieves the physics object associated with this behavior, cast to the specified type.
        /// </summary>
        /// <typeparam name="T">The type to cast the physics object to.</typeparam>
        /// <returns>The rigid body associated with this behavior, cast to the specified type.</returns>
        public override T GetPhysicsObject<T>()
        {
            return (T)(object)Collider;
        }

        /// <summary>
        /// Initialization method called when the behavior is initialized.
        /// Override this method to provide custom initialization logic.
        /// </summary>
        /// <param name="game">The game instance.</param>
        /// <param name="parent">The parent game element.</param>
        public override void OnInit(Game game, GameElement parent)
        {
            
        }

        /// <summary>
        /// Cleanup method called when the behavior is destroyed.
        /// Override this method to provide custom cleanup logic.
        /// </summary>
        /// <param name="game">The game instance.</param>
        /// <param name="parent">The parent game element.</param>
        public override void OnRender(Game game, GameElement parent)
        {
            
        }

        /// <summary>
        /// Rendering method called during the render phase.
        /// Override this method to provide custom rendering logic.
        /// </summary>
        /// <param name="game">The game instance.</param>
        /// <param name="parent">The parent game element.</param>
        public override void OnDestroy(Game game, GameElement parent)
        {
            
        }
    }
}

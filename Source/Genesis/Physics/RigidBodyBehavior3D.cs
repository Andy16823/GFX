using BulletSharp;
using Genesis.Core;
using Genesis.Math;
using GlmSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BulletSharp.Dbvt;

namespace Genesis.Physics
{
    /// <summary>
    /// Abstract base class for defining behaviors of 3D rigid bodies in a physics simulation.
    /// </summary>
    public abstract class RigidBodyBehavior3D : PhysicsBehavior
    {
        /// <summary>
        /// The BulletSharp RigidBody associated with this behavior.
        /// </summary>
        public RigidBody RigidBody { get; set; }

        /// <summary>
        /// Offset from the parent element's location to apply to the rigid body.
        /// </summary>
        public Vec3 Offset { get; set; } = Vec3.Zero();

        /// <summary>
        /// The physics handler responsible for managing this rigid body.
        /// </summary>
        public PhysicHandler PhysicHandler { get; set; }

        /// <summary>
        /// Constructor for initializing the RigidBodyBehavior3D.
        /// </summary>
        /// <param name="handler">The physics handler managing this rigid body.</param>
        public RigidBodyBehavior3D(PhysicHandler handler)
        {
            this.PhysicHandler = handler;
        }

        /// <summary>
        /// Abstract method to create the rigid body. 
        /// Implement this method in derived classes to define the specific behavior for creating a rigid body.
        /// </summary>
        public abstract void CreateRigidBody(float mass, int collisionGroup = -1, int collisionMask = -1);

        /// <summary>
        /// Rotates the rigid body by the specified vector of Euler angles.
        /// </summary>
        /// <param name="value">A vector containing rotation angles for X, Y, and Z axes.</param>
        public virtual void Rotate(Vec3 value)
        {
            this.Rotate(value.X, value.Y, value.Z);
        }

        /// <summary>
        /// Rotates the rigid body by the specified Euler angles.
        /// </summary>
        /// <param name="x">The rotation angle around the X-axis.</param>
        /// <param name="y">The rotation angle around the Y-axis.</param>
        /// <param name="z">The rotation angle around the Z-axis.</param>
        public virtual void Rotate(float x, float y, float z)
        {
            // Translation
            var translation = RigidBody.WorldTransform.Origin;
            var translationMatrix = BulletSharp.Math.Matrix.Translation(translation);

            // Rotation
            var rotation = BulletSharp.Math.Quaternion.RotationMatrix(RigidBody.WorldTransform);
            BulletSharp.Math.Matrix rotationMatrix;
            BulletSharp.Math.Matrix.RotationYawPitchRoll(x, y, z, out rotationMatrix);

            this.RigidBody.WorldTransform = rotationMatrix * translationMatrix;
        }

        /// <summary>
        /// Translates the rigid body by the specified vector.
        /// </summary>
        /// <param name="value">The translation vector.</param>
        public virtual void Translate(Vec3 value)
        {
            this.Translate(value.X, value.Y, value.Z);
        }

        /// <summary>
        /// Translates the rigid body by the specified distances.
        /// </summary>
        /// <param name="x">The distance to translate along the X-axis.</param>
        /// <param name="y">The distance to translate along the Y-axis.</param>
        /// <param name="z">The distance to translate along the Z-axis.</param>
        public virtual void Translate(float x, float y, float z)
        {
            // Translation
            var translationMatrix = BulletSharp.Math.Matrix.Translation(x, y, z);

            // Rotation
            var rotation = BulletSharp.Math.Quaternion.RotationMatrix(RigidBody.WorldTransform);
            BulletSharp.Math.Matrix rotationMatrix;
            BulletSharp.Math.Matrix.RotationQuaternion(ref rotation, out rotationMatrix);

            this.RigidBody.WorldTransform = rotationMatrix * translationMatrix;
        }

        /// <summary>
        /// Sets the angular velocity of the rigid body.
        /// </summary>
        /// <param name="x">Angular velocity around the X-axis.</param>
        /// <param name="y">Angular velocity around the Y-axis.</param>
        /// <param name="z">Angular velocity around the Z-axis.</param>
        public virtual void AngularVelocity(float x, float y, float z)
        {
            this.RigidBody.AngularVelocity = new BulletSharp.Math.Vector3(x, y, z);
        }

        /// <summary>
        /// Sets the angular velocity of the rigid body.
        /// </summary>
        /// <param name="value">Angular velocity vector.</param>
        public virtual void AngularVelocity(Vec3 value)
        {
            this.AngularVelocity(value.X, value.Y, value.Z);
        }

        /// <summary>
        /// Sets the linear velocity of the rigid body.
        /// </summary>
        /// <param name="x">Linear velocity along the X-axis.</param>
        /// <param name="y">Linear velocity along the Y-axis.</param>
        /// <param name="z">Linear velocity along the Z-axis.</param>
        public virtual void LinearVelocity(float x, float y, float z)
        {
            this.RigidBody.LinearVelocity = new BulletSharp.Math.Vector3(x, y, z);
        }

        /// <summary>
        /// Sets the linear velocity of the rigid body.
        /// </summary>
        /// <param name="value">Linear velocity vector.</param>
        public virtual void LinearVelocity(Vec3 value)
        {
            this.LinearVelocity(value.X, value.Y, value.Z);
        }

        /// <summary>
        /// Gets the current linear velocity of the rigid body.
        /// </summary>
        /// <returns>A vector representing the linear velocity.</returns>
        public Vec3 GetLinearVelocity()
        {
            return new Vec3(this.RigidBody.LinearVelocity.X, this.RigidBody.LinearVelocity.Y, this.RigidBody.LinearVelocity.Z);
        }

        /// <summary>
        /// Gets the current rotation of the rigid body as a quaternion.
        /// </summary>
        /// <returns>The quaternion representing the rotation of the rigid body.</returns>
        public BulletSharp.Math.Quaternion GetRotation()
        {
            var rotation = RigidBody.WorldTransform.Basis;
            return BulletSharp.Math.Quaternion.RotationMatrix(rotation);
        }

        /// <summary>
        /// Gets the current location of the rigid body.
        /// </summary>
        /// <returns>A vector representing the position of the rigid body.</returns>
        public Vec3 GetLocation()
        {
            var position = RigidBody.WorldTransform.Origin;
            return new Vec3(position.X, position.Y, position.Z);
        }

        /// <summary>
        /// Calculates the forward vector of the rigid body based on its rotation and a specified distance.
        /// </summary>
        /// <param name="distance">The distance to scale the forward vector.</param>
        /// <returns>The forward vector scaled by the specified distance.</returns>
        public Vec3 CalculateForwardVector(float distance)
        {
            var rotation = GetRotation();
            vec3 forward = new vec3(0, 0, -1);
            quat quaternion = new quat(rotation.X, rotation.Y, rotation.Z, rotation.W);
            forward = quaternion * forward;

            forward *= distance;
            return new Vec3(forward);
        }

        /// <summary>
        /// Calculates the right vector of the rigid body based on its rotation and a specified distance.
        /// </summary>
        /// <param name="distance">The distance to scale the right vector.</param>
        /// <returns>The right vector scaled by the specified distance.</returns>
        public Vec3 CalculateRightVector(float distance)
        {
            var rotation = GetRotation();
            var right = new vec3(1, 0, 0);
            quat quaternion = new quat(rotation.X, rotation.Y, rotation.Z, rotation.W);
            right = quaternion * right;

            right *= distance;
            return new Vec3(right);
        }

        /// <summary>
        /// Calculates the up vector of the rigid body based on its rotation and a specified distance.
        /// </summary>
        /// <param name="distance">The distance to scale the up vector.</param>
        /// <returns>The up vector scaled by the specified distance.</returns>
        public Vec3 CalculateUpVector(float distance)
        {
            var rotation = GetRotation();
            var up = new vec3(0, 1, 0);
            quat quaternion = new quat(rotation.X, rotation.Y, rotation.Z, rotation.W);
            up = quaternion * up;

            up *= distance;
            return new Vec3(up);
        }

        /// <summary>
        /// Updates the rigid body's position and rotation based on its current state in the physics simulation.
        /// </summary>
        /// <param name="game">The game instance.</param>
        /// <param name="parent">The parent game element.</param>
        public override void OnUpdate(Game game, GameElement parent)
        {
            // Translation
            var position = RigidBody.WorldTransform.Origin;
            var rotationQuat = this.GetRotation();
            vec3 eulerAngles = (vec3) glm.EulerAngles(new quat(rotationQuat.X, rotationQuat.Y, rotationQuat.Z, rotationQuat.W));
            vec3 rotationVector = new vec3(Utils.ToDegrees(eulerAngles.x), Utils.ToDegrees(eulerAngles.y), Utils.ToDegrees(eulerAngles.z));

            // New Location
            Vec3 newLocation = Utils.GetModelSpaceLocation(Parent, new Vec3(position.X, position.Y, position.Z));
            Vec3 newRotation = Utils.GetModelSpaceRotation(Parent, new Vec3(rotationVector));

            parent.Location = newLocation - Offset;
            parent.Rotation = newRotation;
            RigidBody.Activate(true);
        }

        /// <summary>
        /// Retrieves the physics object associated with this behavior.
        /// </summary>
        /// <returns>The rigid body associated with this behavior.</returns>
        public override object GetPhysicsObject()
        {
            return RigidBody;
        }

        /// <summary>
        /// Retrieves the physics object associated with this behavior, cast to the specified type.
        /// </summary>
        /// <typeparam name="T">The type to cast the physics object to.</typeparam>
        /// <returns>The rigid body associated with this behavior, cast to the specified type.</returns>
        public override T GetPhysicsObject<T>()
        {
            return (T)(object)RigidBody;
        }

        /// <summary>
        /// Cleanup method called when the behavior is destroyed.
        /// Override this method to provide custom cleanup logic.
        /// </summary>
        /// <param name="game">The game instance.</param>
        /// <param name="parent">The parent game element.</param>
        public override void OnDestroy(Game game, GameElement parent)
        {
            PhysicHandler.RemoveElement(this);
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
        /// Rendering method called during the render phase.
        /// Override this method to provide custom rendering logic.
        /// </summary>
        /// <param name="game">The game instance.</param>
        /// <param name="parent">The parent game element.</param>
        public override void OnRender(Game game, GameElement parent)
        {

        }
    }
}

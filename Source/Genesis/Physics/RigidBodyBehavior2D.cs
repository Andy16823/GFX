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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace Genesis.Physics
{
    /// <summary>
    /// Represents a 2D rigid body behavior in a physics simulation.
    /// </summary>
    public abstract class RigidBodyBehavior2D : PhysicsBehavior
    {
        /// <summary>
        /// Gets or sets the rigid body associated with this behavior.
        /// </summary>
        public RigidBody RigidBody { get; set; }

        /// <summary>
        /// Gets or sets the offset vector.
        /// </summary>
        public Vec3 Offset { get; set; } = Vec3.Zero();

        /// <summary>
        /// Gets or sets the physics handler.
        /// </summary>
        public PhysicHandler PhysicHandler { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RigidBodyBehavior2D"/> class with the specified physics handler.
        /// </summary>
        /// <param name="handler">The physics handler.</param>
        public RigidBodyBehavior2D(PhysicHandler handler)
        {
            this.PhysicHandler = handler;
        }

        /// <summary>
        /// Creates a rigid body with the specified mass.
        /// </summary>
        /// <param name="mass">The mass of the rigid body.</param>
        public abstract void CreateRigidBody(float mass, int collisionGroup = -1, int collisionMask = -1);

        /// <summary>
        /// Rotates the rigid body by the specified vector.
        /// </summary>
        /// <param name="value">The rotation vector.</param>
        public virtual void Rotate(Vec3 value)
        {
            this.Rotate(value.Z);
        }

        /// <summary>
        /// Rotates the rigid body by the specified angle around the Z-axis.
        /// </summary>
        /// <param name="z">The angle to rotate by, in radians.</param>
        public virtual void Rotate(float z)
        {
            // Translation
            var translation = RigidBody.WorldTransform.Origin;
            var translationMatrix = BulletSharp.Math.Matrix.Translation(translation);

            // Rotation
            var rotation = BulletSharp.Math.Quaternion.RotationMatrix(RigidBody.WorldTransform);
            BulletSharp.Math.Matrix rotationMatrix;
            BulletSharp.Math.Matrix.RotationYawPitchRoll(0, 0, z, out rotationMatrix);

            this.RigidBody.WorldTransform = rotationMatrix * translationMatrix;
        }

        /// <summary>
        /// Translates the rigid body by the specified vector.
        /// </summary>
        /// <param name="value">The translation vector.</param>
        public virtual void Translate(Vec3 value)
        {
            this.Translate(value.X, value.Y);
        }

        /// <summary>
        /// Translates the rigid body by the specified X and Y values.
        /// </summary>
        /// <param name="x">The translation along the X-axis.</param>
        /// <param name="y">The translation along the Y-axis.</param>
        public virtual void Translate(float x, float y)
        {
            // Translation
            var translationMatrix = BulletSharp.Math.Matrix.Translation(x, y, 0);

            // Rotation
            var rotation = BulletSharp.Math.Quaternion.RotationMatrix(RigidBody.WorldTransform);
            BulletSharp.Math.Matrix rotationMatrix;
            BulletSharp.Math.Matrix.RotationQuaternion(ref rotation, out rotationMatrix);

            this.RigidBody.WorldTransform = rotationMatrix * translationMatrix;
        }

        /// <summary>
        /// Sets the angular velocity of the rigid body around the Z-axis.
        /// </summary>
        /// <param name="z">The angular velocity around the Z-axis.</param>
        public virtual void AngularVelocity(float z)
        {
            this.RigidBody.AngularVelocity = new  BulletSharp.Math.Vector3(0, 0, z);
        }

        /// <summary>
        /// Sets the angular velocity of the rigid body using a vector.
        /// </summary>
        /// <param name="value">The angular velocity vector.</param>
        public virtual void AngularVelocity(Vec3 value)
        {
            this.AngularVelocity(value.Z);
        }

        /// <summary>
        /// Sets the linear velocity of the rigid body using X and Y values.
        /// </summary>
        /// <param name="x">The linear velocity along the X-axis.</param>
        /// <param name="y">The linear velocity along the Y-axis.</param>
        public virtual void LinearVelocity(float x, float y)
        {
            this.RigidBody.LinearVelocity = new BulletSharp.Math.Vector3(x, y, 0);
        }

        /// <summary>
        /// Sets the linear velocity of the rigid body using a vector.
        /// </summary>
        /// <param name="value">The linear velocity vector.</param>
        public virtual void LinearVelocity(Vec3 value)
        {
            this.LinearVelocity(value.X, value.Y);
        }

        /// <summary>
        /// Gets the physics object associated with this behavior.
        /// </summary>
        /// <returns>The physics object.</returns>
        public override object GetPhysicsObject()
        {
            return RigidBody;
        }

        /// <summary>
        /// Gets the physics object associated with this behavior, cast to the specified type.
        /// </summary>
        /// <typeparam name="T">The type to cast the physics object to.</typeparam>
        /// <returns>The physics object cast to the specified type.</returns>
        public override T GetPhysicsObject<T>()
        {
            return (T)(Object)RigidBody;
        }

        /// <summary>
        /// Updates the physics behavior.
        /// </summary>
        /// <param name="game">The game instance.</param>
        /// <param name="parent">The parent game element.</param>
        public override void OnUpdate(Game game, GameElement parent)
        {
            var position = RigidBody.WorldTransform.Origin;

            var rotation = BulletSharp.Math.Quaternion.RotationMatrix(RigidBody.WorldTransform);
            vec3 rotationVector = (vec3)glm.EulerAngles(new quat(rotation.X, rotation.Y, rotation.Z, rotation.W));

            parent.Location = new Vec3(position.X, position.Y, position.Z) - Offset;
            parent.Rotation = new Vec3(Utils.ToDegrees(rotationVector.x), Utils.ToDegrees(rotationVector.y), Utils.ToDegrees(rotationVector.z));
            RigidBody.Activate(true);
        }

        /// <summary>
        /// Destroys the physics behavior.
        /// </summary>
        /// <param name="game">The game instance.</param>
        /// <param name="parent">The parent game element.</param>
        public override void OnDestroy(Game game, GameElement parent)
        {
            PhysicHandler.RemoveElement(this);
        }

        /// <summary>
        /// Initializes the physics behavior.
        /// </summary>
        /// <param name="game">The game instance.</param>
        /// <param name="parent">The parent game element.</param>
        public override void OnInit(Game game, GameElement parent)
        {

        }

        /// <summary>
        /// Renders the physics behavior.
        /// </summary>
        /// <param name="game">The game instance.</param>
        /// <param name="parent">The parent game element.</param>
        public override void OnRender(Game game, GameElement parent)
        {

        }
    }
}

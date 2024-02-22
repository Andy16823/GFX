using BulletSharp;
using BulletSharp.Math;
using BulletSharp.SoftBody;
using Genesis.Math;
using Genesis.Physics;
using GlmSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;

namespace Genesis.Core.Behaviors.Physics2D
{
    /// <summary>
    /// Represents a 2D physics behavior for game elements.
    /// </summary>
    public class Rigidbody2D : Physics.PhysicsBehavior
    {
        /// <summary>
        /// Gets or sets the RigidBody associated with this 2D physics behavior.
        /// </summary>
        public RigidBody RigidBody { get; set; }

        /// <summary>
        /// Gets or sets the linear factor for the RigidBody's motion.
        /// </summary>
        public Vec3 LinearFactor { get; set; } = new Vec3(1, 1, 0);

        /// <summary>
        /// Gets or sets the angular factor for the RigidBody's rotation.
        /// </summary>
        public Vec3 AngularFactor { get; set; } = new Vec3(0, 1, 0);

        /// <summary>
        /// Creates a RigidBody with the specified mass using the provided PhysicHandler.
        /// </summary>
        /// <param name="handler">The PhysicHandler responsible for managing physics elements.</param>
        /// <param name="mass">The mass of the RigidBody.</param>
        public void CreateRigidbody(PhysicHandler handler, float mass)
        {
            var shape = new Box2DShape(Parent.Size.ToBulletVec3() / 2);
            RigidBodyConstructionInfo info = new RigidBodyConstructionInfo(mass, null, shape, shape.CalculateLocalInertia(mass));

            //Create the start matrix
            //Vec3 rotation = Utils.GetElementWorldRotation(this.Parent);
            BulletSharp.Math.Matrix transform = BulletSharp.Math.Matrix.Translation(Parent.Location.ToBulletVec3());
            //BulletSharp.Math.Matrix rotMat = BulletSharp.Math.Matrix.RotationX(rotation.X) * BulletSharp.Math.Matrix.RotationY(rotation.Y) * BulletSharp.Math.Matrix.RotationZ(rotation.Z);
            Matrix startTransform = transform;

            info.MotionState = new DefaultMotionState(startTransform);
            RigidBody = new BulletSharp.RigidBody(info);
            RigidBody.LinearFactor = this.LinearFactor.ToBulletVec3();
            RigidBody.AngularFactor = this.AngularFactor.ToBulletVec3();
            this.RigidBody.ApplyGravity();
            handler.ManageElement(this);
        }

        /// <summary>
        /// Updates the RigidBody's motion state based on the parent's location.
        /// </summary>
        public void UpdateRigidBody()
        {
            Matrix transform = BulletSharp.Math.Matrix.Translation(Parent.Location.ToBulletVec3());
            RigidBody.MotionState = new DefaultMotionState(transform);
            RigidBody.Activate(true);
            //physicsBehavior.RigidBody.ActivationState = BulletSharp.ActivationState.DisableDeactivation;
        }

        /// <summary>
        /// Called when the associated game element is being destroyed.
        /// </summary>
        /// <param name="game">The current game instance.</param>
        /// <param name="parent">The parent game element associated with this behavior.</param>
        public override void OnDestroy(Game game, GameElement parent)
        {
            
        }

        /// <summary>
        /// Called when the associated game element is being initialized.
        /// </summary>
        /// <param name="game">The current game instance.</param>
        /// <param name="parent">The parent game element associated with this behavior.</param>
        public override void OnInit(Game game, GameElement parent)
        {
            
        }

        /// <summary>
        /// Called during the rendering phase of the game update cycle.
        /// </summary>
        /// <param name="game">The current game instance.</param>
        /// <param name="parent">The parent game element associated with this behavior.</param>
        public override void OnRender(Game game, GameElement parent)
        {
            
        }

        /// <summary>
        /// Updates the behavior during the game's update cycle.
        /// </summary>
        /// <param name="game">The current game instance.</param>
        /// <param name="parent">The parent game element associated with this behavior.</param>
        public override void OnUpdate(Game game, GameElement parent)
        {
            if (this.RigidBody != null && this.RigidBody.InvMass > 0)
            {
                Vector3 position = RigidBody.WorldTransform.Origin;
                Vec3 newLocation = Utils.GetModelSpaceLocation(Parent, new Vec3(position.X, position.Y, position.Z));
                parent.Location = newLocation;
            }
        }

        /// <summary>
        /// Gets the physics object associated with this behavior.
        /// </summary>
        /// <returns>The RigidBody physics object.</returns>
        public override object GetPhysicsObject()
        {
            return RigidBody;
        }

        /// <summary>
        /// Gets the physics object associated with this behavior, cast to the specified type.
        /// </summary>
        /// <typeparam name="T">The type to which the physics object is cast.</typeparam>
        /// <returns>The RigidBody physics object cast to the specified type.</returns>
        public override T GetPhysicsObject<T>()
        {
            return (T)(object)RigidBody;
        }
    }
}

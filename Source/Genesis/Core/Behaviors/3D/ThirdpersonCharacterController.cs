using BulletSharp;
using BulletSharp.Math;
using Genesis.Core.Behaviors.Physics3D;
using Genesis.Core.GameElements;
using Genesis.Graphics;
using Genesis.Math;
using Genesis.Physics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Genesis.Core.Behaviors._3D
{
    /// <summary>
    /// Defines the stance of the character.
    /// </summary>
    public enum Stance
    {
        Walk,
        Run,
        Idle
    }

    /// <summary>
    /// Controls the behavior of a third-person character in a game.
    /// </summary>
    public class ThirdpersonCharacterController : IGameBehavior
    {
        /// <summary>
        /// Gets or sets the collider for the character.
        /// </summary>
        public CapsuleRigidBody RigidBody { get; set; }

        /// <summary>
        /// Gets or sets the speed of walking.
        /// </summary>
        public float WalkSpeed { get; set; } = 0.3f;

        /// <summary>
        /// Gets or sets the speed of running.
        /// </summary>
        public float RunSpeed { get; set; } = 0.7f;

        /// <summary>
        /// Gets or sets the speed of jumping.
        /// </summary>
        public float JumpSpeed { get; set; } = 7.5f;

        /// <summary>
        /// Gets or sets a value indicating whether the character is running.
        /// </summary>
        public bool IsRunning { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the character is colliding.
        /// </summary>
        public bool IsColliding { get; set; }

        /// <summary>
        /// Gets or sets the name of the walking animation.
        /// </summary>
        public String WalkAnimation { get; set; } = "walk";

        /// <summary>
        /// Gets or sets the name of the idle animation.
        /// </summary>
        public String IdleAnimation { get; set; } = "idle";

        /// <summary>
        /// Gets or sets the name of the running animation.
        /// </summary>
        public String RunAnimation { get; set; } = "run";

        /// <summary>
        /// Gets or sets a value indicating whether the mouse movement is inverted.
        /// </summary>
        public bool InvertMouse { get; set; }

        /// <summary>
        /// Gets or sets the stance of the character.
        /// </summary>
        public Stance Stance { get; set; }

        private Vec3 inputVector;
        private long jumpCooldown = 250;
        private long lastJump = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="ThirdpersonCharacterController"/> class.
        /// </summary>
        public ThirdpersonCharacterController()
        {

        }

        /// <summary>
        /// Creates the physics for the character.
        /// </summary>
        /// <param name="handler">The physics handler.</param>
        /// <param name="radius">The radius.</param>
        /// <param name="height">The height.</param>
        /// <param name="mass">The mass.</param>
        public void CreatePhysics(PhysicHandler handler, float radius, float height, float mass)
        {
            this.CreatePhysics(handler, Vec3.Zero(), radius, height, mass);
        }

        /// <summary>
        /// Creates the physics for the character.
        /// </summary>
        /// <param name="handler">The physics handler.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="radius">The radius.</param>
        /// <param name="height">The height.</param>
        /// <param name="mass">The mass.</param>
        public void CreatePhysics(PhysicHandler handler, Vec3 offset, float radius, float height, float mass)
        {
            this.RigidBody = new CapsuleRigidBody(handler);
            this.Parent.AddBehavior(this.RigidBody);
            this.RigidBody.CreateRigidBody(radius, height, mass, offset);
            this.RigidBody.RigidBody.AngularFactor = new BulletSharp.Math.Vector3(0);
        }

        /// <summary>
        /// Called when the game element is destroyed.
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
        /// Called when the game element should be rendered.
        /// </summary>
        /// <param name="game">The game instance.</param>
        /// <param name="parent">The parent game element.</param>
        public override void OnRender(Game game, GameElement parent)
        {

        }

        /// <summary>
        /// Sets the input vector for the character.
        /// </summary>
        /// <param name="inputVector">The input vector to set for the character.</param>
        /// <remarks>
        /// This method assigns the provided input vector to the 'inputVector' property of the character.
        /// </remarks>
        public void SetInput(Vec3 inputVector)
        {
            this.inputVector = inputVector;
        }

        /// <summary>
        /// Called when the game element should be updated.
        /// </summary>
        /// <param name="game">The game instance.</param>
        /// <param name="parent">The parent game element.</param>
        public override void OnUpdate(Game game, GameElement parent)
        {
            // Convert the parent game element to an model game element
            Model model = (Model)this.Parent;
            model.AnimationSpeed = (float)game.DeltaTime * 0.001f;

            // Set the model rotation
            Vec3 currentCursoPos = Input.GetMousePos();
            Input.SetCursorPos((int)(game.Viewport.Width / 2), (int)(game.Viewport.Height / 2), game.RenderDevice.GetHandle());
            Vec3 diff = currentCursoPos - Input.GetMousePos();
            if (this.InvertMouse)
            {
                model.Rotation = model.Rotation.AddY((diff.X / 100));
            }
            else
            {
                model.Rotation = model.Rotation.SubY((diff.X / 100));
            }

            // Let the camera follow the player
            PerspectiveCamera camera = (PerspectiveCamera)game.SelectedScene.Camera;
            camera.Location = Utils.GetRelativePosition(model, new Vec3(0f, 2.5f, -4f));
            camera.Rotation = camera.Rotation.AddX(-20f);
            camera.LookAt(model.Location + new Vec3(0, 1, 0));


            // Setup the Velocity for the player movement.
            Vector3 velocity = new Vector3(0, RigidBody.RigidBody.LinearVelocity.Y, 0f);

            // Setup the movement speed
            float speed = (float)game.DeltaTime * WalkSpeed;

            // Move the player
            if (Input.IsKeyDown(Keys.W) || Input.IsKeyDown(Keys.S))
            {
                if (Input.IsKeyDown(Keys.W))
                {
                    if (Input.IsKeyDown(Keys.ShiftKey))
                    {
                        speed = (float)game.DeltaTime * RunSpeed;
                        IsRunning = true;
                    }
                    var fwd = Utils.GetForwardDirection(model.Rotation);
                    velocity += (fwd.ToBulletVec3() * speed);
                }
                else if (Input.IsKeyDown(Keys.S))
                {
                    var fwd = Utils.GetForwardDirection(model.Rotation);
                    velocity -= (fwd.ToBulletVec3() * speed);
                }
                if (this.IsRunning)
                {
                    this.Stance = Stance.Run;

                }
                else
                {
                    this.Stance = Stance.Walk;

                }
            }
            else
            {
                this.Stance = Stance.Idle;

            }

            // Handle the jump
            if (Input.IsKeyDown(Keys.Space))
            {
                var now = Utils.GetCurrentTimeMillis();
                if ((now > lastJump + jumpCooldown) && !this.IsAirborn())
                {
                    float jumpSpeed = this.JumpSpeed;
                    velocity.Y += jumpSpeed;
                    lastJump = now;
                }
            }

            // Play the animation for the selected stance
            switch (this.Stance)
            {
                case Stance.Walk:
                    if (model.Animator.CurrentAnimation.Name != WalkAnimation)
                    {
                        model.PlayAnimation(WalkAnimation);
                    }
                    break;
                case Stance.Run:
                    if (model.Animator.CurrentAnimation.Name != RunAnimation)
                    {
                        model.PlayAnimation(RunAnimation);
                    }
                    break;
                case Stance.Idle:
                    if (model.Animator.CurrentAnimation.Name != IdleAnimation)
                    {
                        model.PlayAnimation(IdleAnimation);
                    }
                    break;
                default:
                    if (model.Animator.CurrentAnimation.Name != IdleAnimation)
                    {
                        model.PlayAnimation(IdleAnimation);
                    }
                    break;
            }

            // Add the input vector to the velocity
            velocity += inputVector.ToBulletVec3();

            // Set data for the next frame
            RigidBody.RigidBody.Activate(true);
            RigidBody.RigidBody.LinearVelocity = velocity;
            IsRunning = false;
            IsColliding = false;
        }

        /// <summary>
        /// Determines whether the character is airborne.
        /// </summary>
        /// <returns><c>true</c> if the character is airborne; otherwise, <c>false</c>.</returns>
        public bool IsAirborn()
        {
            if (this.IsColliding)
            {
                return false;
            }
            return true;
        }

        public override void OnCollide(Collision collision, GameElement parent)
        {
            var physicsBehavior = collision.collidingElement.GetBehavior<PhysicsBehavior>();
            var btCollisionObject = (CollisionObject)physicsBehavior.GetPhysicsObject();
            if (btCollisionObject.GetType() != typeof(GhostObject))
            {
                IsColliding = true;
            }
        }
    }
}

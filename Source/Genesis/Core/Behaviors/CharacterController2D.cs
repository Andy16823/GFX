using BulletSharp;
using Genesis.Core.Behaviors.Physics2D;
using Genesis.Core.GameElements;
using Genesis.Math;
using Genesis.Physics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Genesis.Core.Behaviors
{
    /// <summary>
    /// Enumeration for defining movement flags.
    /// </summary>
    public enum MovementFlags
    {
        CharacterMoveHorizontal,
        CharacterMoveVertical,
        CharacterMoveVerticalAndHorizontal,
    }

    /// <summary>
    /// Enumeration for defining camera flags.
    /// </summary>
    public enum CameraFlags
    {
        CameraLookAtSprite,
        CameraLookAtSpriteRounded,
        IgnoreCamera
    }

    /// <summary>
    /// Enumeration for defining controller presets.
    /// </summary>
    public enum ControllerPreset
    {
        TopDownController,
        SideScrollerController
    }

    /// <summary>
    /// Enumeration for the move direction
    /// </summary>
    public enum MoveDirection
    {
        North,
        East,
        South,
        West
    }

    /// <summary>
    /// Class representing a 2D character controller as a game behavior.
    /// </summary>
    public class CharacterController2D : IGameBehavior
    {
        /// <summary>
        /// Gets or sets the mass of the character.
        /// </summary>
        public float Mass { get; set; } = 5.0f;

        /// <summary>
        /// Gets or sets the speed of the character movement.
        /// </summary>
        public float Speed { get; set; } = 0.1f;

        /// <summary>
        /// Gets or sets the jump speed of the character.
        /// </summary>
        public float JumpSpeed { get; set; } = 5.0f;

        /// <summary>
        /// Gets or sets the key for moving the character up.
        /// </summary>
        public Keys UpKey { get; set; } = Keys.W;

        /// <summary>
        /// Gets or sets the key for moving the character down.
        /// </summary>
        public Keys DownKey { get; set; } = Keys.S;

        /// <summary>
        /// Gets or sets the key for moving the character right.
        /// </summary>
        public Keys LeftKey { get; set; } = Keys.A;

        /// <summary>
        /// Gets or sets the key for making the character jump.
        /// </summary>
        public Keys RightKey { get; set; } = Keys.D;

        /// <summary>
        /// Gets or sets whether jumping is allowed.
        /// </summary>
        public Keys JumpKey { get; set; } = Keys.Space;

        /// <summary>
        /// Gets or sets the jump cooldown duration in milliseconds.
        /// </summary>
        public bool AllowJump { get; set; } = false;

        /// <summary>
        /// Gets or sets the jump cooldown in milliseconds
        /// </summary>
        public long JumpCooldown { get; set; } = 1000;

        /// <summary>
        /// Gets or sets the camera flags for character control.
        /// </summary>
        public CameraFlags CameraFlags { get; set; } = CameraFlags.CameraLookAtSprite;

        /// <summary>
        /// Gets or sets the movement flags for character control.
        /// </summary>
        public MovementFlags MovementFlags { get; set; } = MovementFlags.CharacterMoveVerticalAndHorizontal;

        /// <summary>
        /// Gets the last movement direction from the character
        /// </summary>
        public MoveDirection MoveDirection { get; set; }

        /// <summary>
        /// Gets or sets the Rigidbody2D component for character physics.
        /// </summary>
        public Rigidbody2D Rigidbody { get; set; }

        /// <summary>
        /// Gets or sets the collider radius
        /// </summary>
        public float ColliderRadius { get; set; }

        /// <summary>
        /// Gets or sets the follider height
        /// </summary>
        public float ColliderHeight { get; set; }

        private Sprite m_player;
        private bool m_collide = false;
        private Vec3 m_lastPosition;
        private long m_lastJump = 0;

        /// <summary>
        /// Default constructor for CharacterController2D.
        /// </summary>
        public CharacterController2D()
        {
         
        }

        /// <summary>
        /// Initialization method called when the game starts.
        /// </summary>
        /// <param name="game">The game instance.</param>
        /// <param name="parent">The parent GameElement of the behavior.</param>
        public override void OnInit(Game game, GameElement parent)
        {
            m_lastPosition = new Vec3(this.Parent.Location.X, this.Parent.Location.Y);
            m_player = (Sprite)this.Parent;

        }

        /// <summary>
        /// Creates physics for the character based on the specified controller preset.
        /// </summary>
        /// <param name="physicsHandler">The physics handler for creating the physics.</param>
        /// <param name="controllerPreset">The preset for the controller.</param>
        public void CreatePhysics(PhysicHandler physicsHandler, ControllerPreset controllerPreset)
        {
            switch (controllerPreset)
            {
                case ControllerPreset.TopDownController:
                    this.CameraFlags = CameraFlags.CameraLookAtSprite;
                    this.MovementFlags = MovementFlags.CharacterMoveVerticalAndHorizontal;
                    this.AllowJump = false;
                    this.Speed = 10f;
                    this.CreatePhysics(physicsHandler, new Vec3(1, 1, 0), Vec3.Zero(), true);
                    break;
                case ControllerPreset.SideScrollerController:
                    this.MovementFlags = MovementFlags.CharacterMoveHorizontal;
                    this.AllowJump = true;
                    this.JumpSpeed = 50f;
                    this.Speed = 15f;
                    this.CreatePhysics(physicsHandler, new Vec3(1, 1, 0), new Vec3(0, 0, 0), true);
                    break;
            }
        }

        /// <summary>
        /// Creates default physics for the character with no specified factors.
        /// </summary>
        /// <param name="physicHandler">The physics handler for creating the physics.</param>
        public void CreatePhysics(PhysicHandler physicHandler)
        {
            this.CreatePhysics(physicHandler, Vec3.Zero(), Vec3.Zero(), false);
        }

        /// <summary>
        /// Creates physics for the character with specified factors and physics enabling.
        /// </summary>
        /// <param name="physicHandler">The physics handler for creating the physics.</param>
        /// <param name="linearFactor">The linear factor for the physics.</param>
        /// <param name="angularFactor">The angular factor for the physics.</param>
        /// <param name="enablePhysics">Specifies whether physics should be enabled initially.</param>
        public void CreatePhysics(PhysicHandler physicHandler, Vec3 linearFactor, Vec3 angularFactor, bool enablePhysics)
        {
            this.Rigidbody = new Rigidbody2D(physicHandler);

            if (this.ColliderRadius == 0)
            {
                this.ColliderRadius = Parent.Size.X / 2;
            }
            if(this.ColliderHeight == 0)
            {
                this.ColliderHeight = 1.1f;
            }

            this.Parent.AddBehavior(Rigidbody);
            this.Rigidbody.LinearFactor = linearFactor;
            this.Rigidbody.AngularFactor = angularFactor;
            this.Rigidbody.EnablePhysic = enablePhysics;
            Rigidbody.CreateRigidbody(this.Mass, ColliderRadius, ColliderHeight);
            Rigidbody.OnCollide += (s, g, collision) =>
            {
                if (!m_collide)
                {
                    m_collide = true;
                }
            };
        }

        /// <summary>
        /// Updates the character state on each frame.
        /// </summary>
        /// <param name="game">The game instance.</param>
        /// <param name="parent">The parent GameElement of the behavior.</param>
        public override void OnUpdate(Game game, GameElement parent)
        {
            CharacterProcess(game);
        }

        /// <summary>
        /// Processes character movement and behavior based on user input.
        /// </summary>
        /// <param name="game">The game instance.</param>
        public void CharacterProcess(Game game)
        {
            float speed = Speed;
            var btRigidBody = Rigidbody.GetPhysicsObject<BulletSharp.RigidBody>();
            float vX = 0f;
            float vY = btRigidBody.LinearVelocity.Y;
            float vZ = 0f;

            switch (this.MovementFlags)
            {
                case MovementFlags.CharacterMoveHorizontal:
                    {
                        if (Input.IsKeyDown(this.LeftKey))
                        {
                            vX = -speed;
                            this.MoveDirection = MoveDirection.West;
                        }
                        else if (Input.IsKeyDown(this.RightKey))
                        {
                            vX = speed;
                            this.MoveDirection = MoveDirection.East;
                        }
                        break;
                    }
                case MovementFlags.CharacterMoveVertical:
                    {
                        if (Input.IsKeyDown(this.UpKey))
                        {
                            vY = speed;
                            this.MoveDirection = MoveDirection.North;
                        }
                        else if (Input.IsKeyDown(this.DownKey))
                        {
                            vY = -speed;
                            this.MoveDirection = MoveDirection.South;
                        }
                        break;
                    }
                case MovementFlags.CharacterMoveVerticalAndHorizontal:
                    {
                        vY = 0f;
                        if (Input.IsKeyDown(this.LeftKey))
                        {
                            vX = -speed;
                            this.MoveDirection = MoveDirection.West;
                        }
                        else if (Input.IsKeyDown(this.RightKey))
                        {
                            vX = speed;
                            this.MoveDirection = MoveDirection.East;
                        }
                        else if (Input.IsKeyDown(this.UpKey))
                        {
                            vY = speed;
                            this.MoveDirection = MoveDirection.North;
                        }
                        else if (Input.IsKeyDown(this.DownKey))
                        {
                            vY = -speed;
                            this.MoveDirection = MoveDirection.South;
                        }
                        break;
                    }
            }

            if (this.AllowJump)
            {
                if (Input.IsKeyDown(this.JumpKey))
                {
                    var now = Utils.GetCurrentTimeMillis();
                    if(now > m_lastJump + JumpCooldown)
                    {
                        float jumpSpeed = (float)game.DeltaTime * 0.3f;
                        vY = btRigidBody.LinearVelocity.Y + this.JumpSpeed;
                        m_lastJump = now;   
                    }
                }
            }

            btRigidBody.LinearVelocity = new BulletSharp.Math.Vector3(vX, vY, vZ);

            switch (CameraFlags)
            {
                case CameraFlags.CameraLookAtSprite:
                    {
                        game.SelectedScene.Camera.LookAt(m_player, true);
                        break;
                    }
                case CameraFlags.CameraLookAtSpriteRounded:
                    {
                        game.SelectedScene.Camera.LookAt(m_player, true);
                        game.SelectedScene.Camera.Location = Vec3.Round(game.SelectedScene.Camera.Location);
                        break;
                    }
                case CameraFlags.IgnoreCamera:
                    {
                        break;
                    }
            }
            m_collide = false;
            btRigidBody.Activate(true);
        }

        /// <summary>
        /// Destruction method called when the game ends or the behavior is removed.
        /// </summary>
        /// <param name="game">The game instance.</param>
        /// <param name="parent">The parent GameElement of the behavior.</param>
        public override void OnDestroy(Game game, GameElement parent)
        {

        }

        /// <summary>
        /// Rendering method called during the rendering phase.
        /// </summary>
        /// <param name="game">The game instance.</param>
        /// <param name="parent">The parent GameElement of the behavior.</param>
        public override void OnRender(Game game, GameElement parent)
        {

        }
    }
}

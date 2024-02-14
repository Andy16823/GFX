using Genesis.Core;
using Genesis.Graphics.RenderDevice;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Genesis.Math;
using Genesis.Core.GameElements;
using Genesis.Physics;
using Genesis.Core.Behaviors.Physics2D;
using Genesis.Graphics.Physics;
using System.Management.Instrumentation;
using Genesis.Core.Behaviors;
using Genesis.Graphics;

namespace Physics2DTest
{
    public partial class Form1 : Form
    {
        // Game instance for managing the game loop and scenes
        private Game m_game;

        // Constructor for the main form
        public Form1()
        {
            InitializeComponent();

            // Initialize the game and set up rendering and viewport
            m_game = new Game(new GLRenderer(this.Handle), new Genesis.Graphics.Viewport(this.ClientSize.Width, this.ClientSize.Height));
            m_game.TargetFPS = 60;
            m_game.AssetManager.LoadTextures();

            // Create a test scene
            var testScene = new Scene("TestScene");

            // Set up the camera for the test scene
            testScene.Camera = new Genesis.Graphics.Camera(new Genesis.Math.Vec3(0f, 0f), new Genesis.Math.Vec3(this.ClientSize.Width, this.ClientSize.Height), -10, 10);
            testScene.AddLayer("BaseLayer");

            // Set up the physics handler for the scene
            var physicsHandler = new PhysicsHandler2D(0f, -10f);
            testScene.PhysicHandler = physicsHandler;

            // Create a player sprite with physics and animation behavior
            var player = new Sprite("Player", new Vec3(-300, 0), new Vec3(32, 32), m_game.AssetManager.GetTexture("player.png"));
           
            var animationBehavior = player.AddBehavior<AnimationBehavior>(new AnimationBehavior(5, 3, 100, m_game.AssetManager.GetTexture("CharacterAnimations.png")));
            var walkRight = new Animation("MoveRight", 0, 0, 5);
            animationBehavior.AddAnimation(walkRight);
            var walkLeft = new Animation("MoveLeft", 0, 1, 5);
            animationBehavior.AddAnimation(walkLeft);
            var idle = new Animation("Idle", 0, 2, 5);
            animationBehavior.AddAnimation(idle);
            animationBehavior.SelectedAnimation = idle;
    
            var physicsBehavior = player.AddBehavior(new Rigidbody2D());
            physicsBehavior.CreateRigidbody(testScene.PhysicHandler, 1f);
            physicsBehavior.RigidBody.AngularFactor = new Vec3(0f, 0f, 0f).ToBulletVec3();
            testScene.AddGameElement("BaseLayer", player);

            // Event handler for sprite collision
            physicsBehavior.OnCollide += (scene, game, collisionObject) =>
            {
                foreach(GameElement element in scene.GetLayer("BaseLayer").Elements)
                {
                    Rigidbody2D rigidbody2D = (Rigidbody2D) element.GetBehavior<Rigidbody2D>();
                    if(rigidbody2D != null)
                    {
                        if(rigidbody2D.RigidBody == collisionObject)
                        {
                            Console.WriteLine("Collision with " + rigidbody2D.Parent.Name);
                        }
                    }
                }
            };

            // Create and add several block sprites to the scene
            var spacing = 120f;
            for (int i = 0; i < 5; i++)
            {
                var x = -300 + ((64.0f * i) + i * spacing);
                var colObject = new Sprite("ColObject_" + i, new Vec3(x, -150), new Vec3(64, 64), m_game.AssetManager.GetTexture("block.png"));
                var colPhysicsBehavior = colObject.AddBehavior(new Rigidbody2D());
                colPhysicsBehavior.CreateRigidbody(testScene.PhysicHandler, 0f);
                testScene.AddGameElement("BaseLayer", colObject);
            }

            // Event handler for game initialization
            m_game.OnInit += (game, renderer) =>
            {
                animationBehavior.Play();
                PhysicsHandler2D physicsHandler2D = (PhysicsHandler2D)testScene.PhysicHandler;
                physicsHandler2D.PhysicsWorld.DebugDrawer = new BulletDebugRenderer(m_game.RenderDevice);
            };

            // Event handler for rendering the debug information
            m_game.OnRenderEnd += (game, renderer) =>
            {
                PhysicsHandler2D physicsHandler2D = (PhysicsHandler2D)testScene.PhysicHandler;
                physicsHandler2D.PhysicsWorld.DebugDrawWorld();
                Console.WriteLine(m_game.FPS);
            };

            // Event handler for player movement based on keyboard input
            m_game.OnUpdate += (game, renderer) =>
            {
                float jumpSpeed = (float)game.DeltaTime * 0.3f;
                float moveSpeed = (float)game.DeltaTime * 0.3f;
                if (Input.IsKeyDown(Keys.Space))
                {
                    player.Location.Y += jumpSpeed;
                    physicsBehavior.UpdateRigidBody();
                }
                if (Input.IsKeyDown(Keys.A))
                {
                    player.Location.X -= moveSpeed;
                    physicsBehavior.UpdateRigidBody();
                    if (!animationBehavior.SelectedAnimation.Name.Equals("MoveLeft"))
                    {
                        animationBehavior.LoadAnimation("MoveLeft");
                    }
                    animationBehavior.Play();
                }
                else if (Input.IsKeyDown(Keys.D))
                {
                    player.Location.X += moveSpeed;
                    physicsBehavior.UpdateRigidBody();
                    if (!animationBehavior.SelectedAnimation.Name.Equals("MoveRight"))
                    {
                        animationBehavior.LoadAnimation("MoveRight");
                    }
                    animationBehavior.Play();
                }
                else
                {
                    animationBehavior.Stop();
                }
                testScene.Camera.LookAt(player);
            };

            // Add the test scene to the game, load the scene, and start the game loop
            m_game.AddScene(testScene);
            m_game.LoadScene("TestScene");
            m_game.Start();
        }

        // Placeholder method for form load event
        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}

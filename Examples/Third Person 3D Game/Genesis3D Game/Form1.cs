using BulletSharp;
using BulletSharp.Math;
using Genesis.Core;
using Genesis.Core.Behaviors;
using Genesis.Core.Behaviors.Physics3D;
using Genesis.Core.GameElements;
using Genesis.Graphics;
using Genesis.Graphics.RenderDevice;
using Genesis.Graphics.Shaders.OpenGL;
using Genesis.Math;
using Genesis.Physics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Genesis3D_Game
{
    // Main form of the game
    public partial class Form1 : Form
    {
        private Game game; // Game instance

        public Form1()
        {
            InitializeComponent();

            // Create the game instance with GLRenderer
            this.game = new Game(new GLRenderer(this.Handle), new Viewport(this.ClientSize.Width, this.ClientSize.Height));
            this.game.TargetFPS = 200;

            // Create sunlight
            Light sun = new Light("Sun", new Vec3(10.0f, 10f, 10.0f));
            sun.Intensity = 0.2f;

            // Create a new 3D scene with a base layer
            var scene = new Scene3D("Scene", sun);
            scene.Camera = new PerspectiveCamera(new Vec3(0.0f, 0.0f, 0.0f), new Vec3(this.ClientSize.Width, this.ClientSize.Height), 0.1f, 100f);
            scene.Camera.Rotation.Y = 90.0f;
            scene.AddLayer("BaseLayer");


            // Create a Physics Handler
            PhysicsHandler3D physicsHandler = new PhysicsHandler3D(0f, -9.81f, 0f);
            scene.PhysicHandler = physicsHandler;

            // Load the Skydome
            String modelspath = new System.IO.FileInfo(System.Reflection.Assembly.GetEntryAssembly().Location).Directory + "\\Resources";
            scene.Skybox = new Skybox("skydome", modelspath + "\\Assets\\Skydome1\\skydome.obj", new Vec3(0), new Vec3(0), new Vec3(75));

            // Create a large cube for the ground with a BoxCollider
            Qube qube1 = new Qube("Qube_1", new Vec3(0f, 0f, 0f));
            qube1.Size = new Vec3(50f, 0.5f, 50f);
            qube1.Shader = new DiffuseSolidShader();
            qube1.Color = Color.White;
            scene.AddGameElement("BaseLayer", qube1);

            var boxCollider = qube1.AddBehavior(new BoxCollider());
            boxCollider.CreateCollider(physicsHandler, qube1.Size.Half(), 0f);

            // Create the player model with a character controller.
            Model model = new Model("Character", new Vec3(2f, 2f, 0f), modelspath + "\\Assets\\Animation\\Human.fbx");
            model.PlayAnimation("Armature|idle");
            scene.AddGameElement("BaseLayer", model);

            var controller = model.AddBehavior(new Genesis.Core.Behaviors._3D.ThirdpersonCharacterController());
            controller.CreatePhysics(physicsHandler, new Vec3(0f, 0.75f, 0f), 0.5f, 0.5f, 50f);
            controller.RunAnimation = "Armature|run";
            controller.IdleAnimation = "Armature|idle";
            

            // Create an second Qube with an collider
            Qube qube2 = new Qube("Q2", new Vec3(2f, 0f, 0f));
            qube2.Shader = new DiffuseSolidShader();
            qube2.Color = Color.Green;
            var qubeCollider = qube2.AddBehavior(new BoxCollider());
            qubeCollider.CreateCollider(physicsHandler, qube2.Size.Half(), 0);
            scene.AddGameElement("BaseLayer", qube2);

            // Add the scene to the game
            game.AddScene(scene);
            game.SelectedScene = scene;

            // OnInit event: create the debug drawer
            game.OnInit += (game, renderer) =>
            {
                physicsHandler.PhysicsWorld.DebugDrawer = new Genesis.Graphics.Physics.BulletDebugRenderer((GLRenderer)this.game.RenderDevice);
            };

            // After rendering, debug draw the physics
            game.OnRenderEnd += (game, renderer) =>
            {
                physicsHandler.PhysicsWorld.DebugDrawWorld();
            };

            // Game Update logic
            game.OnUpdate += (game, renderer) =>
            {
                Console.WriteLine("FPS " + game.FPS.ToString());
            };

            // After each update, check if the Escape key is pressed to exit the game
            game.AfterUpdate += (game, renderer) =>
            {
                if (Input.IsKeyDown(Keys.Escape))
                {
                    game.Stop();
                    Application.Exit();
                }
            };

            // Start the game
            game.Start();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        // Update the viewport
        private void Form1_Resize(object sender, EventArgs e)
        {
            game.Viewport.SetNewViewport(ClientSize.Width, ClientSize.Height);
            game.SelectedScene.Camera.Size = new Vec3(ClientSize.Width, ClientSize.Height);
        }

        // End the game when the for get closed
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            game.Stop();
        }
    }
}

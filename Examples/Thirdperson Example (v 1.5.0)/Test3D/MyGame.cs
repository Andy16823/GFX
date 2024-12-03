using BulletSharp;
using Genesis.Core;
using Genesis.Core.Behaviors.Physics3D;
using Genesis.Core.GameElements;
using Genesis.Graphics;
using Genesis.Graphics.Physics;
using Genesis.Graphics.Shaders.OpenGL;
using Genesis.Math;
using Genesis.Physics;
using Genesis.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Test3D;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace GFXNetFrameworkTemplate
{
    public class MyGame : Genesis.Core.Game
    {
        private Window window;

        // Constructor to initialize the game with a render device, viewport, and window
        public MyGame(IRenderDevice renderDevice, Viewport viewport, Window window) : base(renderDevice, viewport)
        {
            this.window = window;

            // Load fonts and textures for the game
            var font = Genesis.Graphics.Font.LoadSystemFont("Arial");
            this.AssetManager.AddFont(font);
            this.AssetManager.LoadTextures();

            // Load materials for the game objects
            var wallMaterial = AssetManager.LoadMaterial("Wall_1", "wall_orange.png");
            var floorMaterial = AssetManager.LoadMaterial("Wall_2", "wall_blue.png");
            var platformMaterial = AssetManager.LoadMaterial("Wall_2", "wall_gray.png");

            // Set target frames per second for smooth performance
            this.TargetFPS = 200;

            // Create a directional light (sun) for the scene and set shadow properties
            var sun = new DirectionalLight("Sun", new Vec3(-2.0f, 5f, -1.0f), 0.5f);
            sun.CastShadows = true;
            sun.ShadowResolution = new Vec3(4096, 4096);

            // Initialize a physics handler with gravity settings
            var physicsHandler = new PhysicsHandler3D(0.0f, -10.0f, 0.0f);

            // Create the 3D scene with lighting, camera, and physics
            var scene = new Scene3D("MyTestScene", sun);
            scene.AddLayer("BaseLayer");
            scene.Camera = new PerspectiveCamera(new Vec3(0.0f, 0.0f, -2.0f), new Vec3(1920, 1080), 0.1f, 50f);
            scene.Camera.Rotation = scene.Camera.Rotation.SetY(90.0f);
            scene.PhysicHandler = physicsHandler;
            scene.ShadowResolution = new Vec3(4096, 4096);
            scene.Skybox = new Skybox("Skydome", "Resources/Models/Skydome1/skydome.obj", Vec3.Zero(), Vec3.Zero(), new Vec3(5.0f, 5.0f, 5.0f));

            // Create a player character model and configure its properties
            var playerCharacter = new Model("Girly", new Vec3(5.0f, -0.5f, 5.0f), "Resources/Models/Girly/girly.fbx");
            playerCharacter.AnimationSpeed = 0.1f;
            playerCharacter.Size = new Vec3(0.5f, 0.5f, 0.5f);

            // Add a rigid body for physics interactions
            var playerRigidBody = playerCharacter.AddBehavior<CapsuleRigidBody>(new CapsuleRigidBody(scene.PhysicHandler));
            playerRigidBody.CreateRigidBody(0.25f, 0.5f, 50.0f, new Vec3(0.0f, 0.5f, 0.0f));

            // Attach a third-person controller to the player character
            playerCharacter.AddBehavior<ThirdPersonController>(new ThirdPersonController(playerRigidBody){
                IdleAnimation = "Idle",
                RunningAnimation = "Run",
                JumpingAnimation = "Jump",
                WalkAnimation = "Walk",
                StrafeLeftAnimation = "StrafeLeft",
                StrafeRightAnimation = "StrafeRight"
            });

            // Add the player character to the scene
            scene.AddGameElement("BaseLayer", playerCharacter);

            // Create a ground cube with a collider
            var groundQube = new Qube("GroundQube", new Vec3(0.0f, -1.0f, 0.0f), new Vec3(25.0f, 0.25f, 25.0f));
            groundQube.AddBehavior<BoxCollider>(new BoxCollider(scene.PhysicHandler)).CreateCollider();
            groundQube.Material = floorMaterial;
            groundQube.Shader = new Genesis.Graphics.Shaders.OpenGL.SpecularShader();
            scene.AddGameElement("BaseLayer", groundQube);

            // Create walls to define the game area
            // Left wall
            var sideQube1 = new Qube("SideQube1", new Vec3(12.5f, -1.0f, 0.0f), new Vec3(0.25f, 5.0f, 25.0f));
            sideQube1.AddBehavior<BoxCollider>(new BoxCollider(scene.PhysicHandler)).CreateCollider();
            sideQube1.Material = wallMaterial;
            sideQube1.Shader = new Genesis.Graphics.Shaders.OpenGL.SpecularShader();
            scene.AddGameElement("BaseLayer", sideQube1 );

            // Right wall
            var sideQube2 = new Qube("SideQube2", new Vec3(-12.5f, -1.0f, 0.0f), new Vec3(0.25f, 5.0f, 25.0f));
            sideQube2.AddBehavior<BoxCollider>(new BoxCollider(scene.PhysicHandler)).CreateCollider();
            sideQube2.Material = wallMaterial;
            sideQube2.Shader = new Genesis.Graphics.Shaders.OpenGL.SpecularShader();
            scene.AddGameElement("BaseLayer", sideQube2);

            // Back wall
            var sideQube3 = new Qube("SideQube3", new Vec3(0.0f, -1.0f, 12.5f), new Vec3(25.0f, 5.0f, 0.25f));
            sideQube3.AddBehavior<BoxCollider>(new BoxCollider(scene.PhysicHandler)).CreateCollider();
            sideQube3.Material = wallMaterial;
            sideQube3.Shader = new Genesis.Graphics.Shaders.OpenGL.SpecularShader();
            scene.AddGameElement("BaseLayer", sideQube3);

            // Front wall
            var sideQube4 = new Qube("sideQube4", new Vec3(0.0f, -1.0f, -12.5f), new Vec3(25.0f, 5.0f, 0.25f));
            sideQube4.AddBehavior<BoxCollider>(new BoxCollider(scene.PhysicHandler)).CreateCollider();
            sideQube4.Material = wallMaterial;
            sideQube4.Shader = new Genesis.Graphics.Shaders.OpenGL.SpecularShader();
            scene.AddGameElement("BaseLayer", sideQube4);

            //Create an Qube
            var demoQube = new Qube("Test", new Vec3(0f, 1f, 2f), new Vec3(1f, 1f, 1f), new Vec3(45, 45, 0));
            demoQube.Shader = new Genesis.Graphics.Shaders.OpenGL.SpecularShader();
            demoQube.Material = floorMaterial;
            var demoQubeRigidBody = demoQube.AddBehavior<BoxCollider>(new BoxCollider(physicsHandler));
            demoQubeRigidBody.CreateCollider();
            scene.AddGameElement("BaseLayer", demoQube);

            //Create an demoElement3D element3d with the new CompoundMeshCollider
            var demoElement3D = new Element3D("Testwalls", "Resources/Models/test_walls.fbx", new Vec3(-2, -1f, 0), new Vec3(0f, 0f, 0f), new Vec3(0.5f, 0.5f, 0.5f));
            var demoElement3DCollider = demoElement3D.AddBehavior<CompoundMeshCollider>(new CompoundMeshCollider(physicsHandler));
            demoElement3DCollider.CreateCollider("Resources/Models/test_walls.fbx");
            demoElement3DCollider.OnCollide += (s, g, e) =>
            {
                Debug.WriteLine($"Collide2 with {e.Name}");  
            };
            demoElement3D.Shader = new SpecularShader();
            scene.AddGameElement("BaseLayer", demoElement3D);

            //Create render instance container and add instances
            var demoInstanceContainer = Qube.CreateInstanceContainer(platformMaterial, true);
            scene.AddGameElement("BaseLayer", demoInstanceContainer);
            var jumpQube = demoInstanceContainer.AddInstance(new Vec3(0.0f, 1.0f, 0.0f), new Vec3(3.0f, 0.25f, 3.0f));
            jumpQube.AddBehavior<BoxCollider>(new BoxCollider(scene.PhysicHandler)).CreateCollider();
            var testInstance1 = demoInstanceContainer.AddInstance(new Vec3(-2.5f, 1f, 2.5f), new Vec3(1.0f, 1.0f, 1.0f), new Vec3(90, 45, 45));
            testInstance1.AddBehavior<BoxCollider>(new BoxCollider(physicsHandler)).CreateCollider();
            var testInstance2 = demoInstanceContainer.AddInstance(new Vec3(2.5f, 1f, 2.5f), new Vec3(2.0f, 2.0f, 2.0f));
            testInstance2.AddBehavior<BoxRigidBody>(new BoxRigidBody(physicsHandler)).CreateRigidBody(25f);

            //Create an sphere with an SphereReigidBody
            var demoSphere = new Sphere("Spehere", new Vec3(2f, 1f, -2f), new Vec3(1f, 1f, 1f), new Vec3());
            demoSphere.AddBehavior<SphereRigidBody>(new SphereRigidBody(physicsHandler)).CreateRigidBody(10);
            demoSphere.Material = wallMaterial;
            demoSphere.Shader = new SpecularShader();
            scene.AddGameElement("BaseLayer", demoSphere);

            // Add a UI canvas for displaying game information (e.g., FPS counter)
            Canvas canvas = new Canvas("MainUI", new Vec3(0, 0), new Vec3(window.WindowSize.X, window.WindowSize.Y));
            var debugLabel = new Genesis.UI.Label("FPS", new Vec3(15, 15), "FPS:", font, Color.Green);
            canvas.AddWidget(debugLabel);
            scene.AddCanvas(canvas);

            // Load and display the scene
            this.AddScene(scene);
            this.LoadScene("MyTestScene");

            // Initialize physics debugging (optional)
            this.OnInit += (g, r) =>
            {
                physicsHandler.PhysicsWorld.DebugDrawer = new BulletDebugRenderer(renderDevice);
                physicsHandler.PhysicsWorld.DebugDrawer.DebugMode = DebugDrawModes.DrawWireframe;
            };

            // Render the physics
            this.OnRenderEnd += (g, r) =>
            {
                //physicsHandler.PhysicsWorld.DebugDrawWorld();
                //physicsHandler.PhysicsWorld.DebugDrawObject(demoElement3DCollider.Collider.WorldTransform, demoElement3DCollider.Collider.CollisionShape, new BulletSharp.Math.Vector3(0.5f, 0.5f, 0.5f));
            };

            // Update game state and handle input
            this.OnUpdate += (game, renderer) =>
            {
                debugLabel.Text = $"FPS: {game.FPS}";

                var mousePos = Input.GetRefMousePos(window.Handle);
                var ndcMousePos = PerspectiveCamera.ScreenToWorldPosition3D((PerspectiveCamera) scene.Camera, viewport, mousePos.X, mousePos.Y);

                // Perform raycasting
                var start = playerCharacter.Location + new Vec3(0f, 0.5f, 0f);
                var end = Utils.GetTransformedForwardVector(playerCharacter.Rotation, -100f) + start;
                var result = Raycast.PerformRaycast(start, end, physicsHandler);
                if (result.hit)
                {
                    Debug.WriteLine($"Raycast hit {result.hitElement.Name}");
                }

                // Exit game on Escape key press
                if (Input.IsKeyDown(Input.Keys.Escape))
                {
                    Input.ShowCursor();
                    game.Stop();
                    window.RequestClose();
                }
            };
        }
    }
}

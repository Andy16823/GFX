using Genesis.Core;
using Genesis.Core.Behaviors.Physics2D;
using Genesis.Core.GameElements;
using Genesis.Graphics;
using Genesis.Graphics.Physics;
using Genesis.Math;
using Genesis.Physics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTutorialGame
{
    public class MyGame : Game
    {
        public MyGame(IRenderDevice renderDevice, Viewport viewport) : base(renderDevice, viewport)
        {
            // Load the textures and set the target fps
            this.AssetManager.LoadTextures();
            this.TargetFPS = 200;

            // Create an physics handler for the scene
            var physicsHandler = new PhysicsHandler2D(0f, 0f);

            // Setup the Scene with an sun, layer, camera and physics handler
            var scene = new Scene("MyTestScene");
            scene.AddLayer("BaseLayer");
            scene.Camera = new Camera(viewport, -1f, 1f);
            scene.PhysicHandler = physicsHandler;

            // Create your game elements
            var sprite = new Sprite("sprite", new Vec3(0, 0), new Vec3(100, 100), this.AssetManager.GetTexture("gfx.png"));
            var collider = sprite.AddBehavior<BoxCollider>(new BoxCollider(physicsHandler));
            collider.CreateCollider();
            scene.AddGameElement("BaseLayer", sprite);

            // Add the Scene to the game and load it.
            this.AddScene(scene);
            this.LoadScene("MyTestScene");

            this.OnUpdate += (g, r) =>
            {
                Console.WriteLine("Hello World");
            };
        }
    }
}

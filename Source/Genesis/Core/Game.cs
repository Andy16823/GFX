using Genesis.Graphics;
using Genesis.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Genesis.Core
{
    public delegate void GameEventHandler(Game game, IRenderDevice renderDevice);

    /// <summary>
    /// Represents the main game class responsible for managing game loops, scenes, rendering, and updates.
    /// </summary>
    public class Game
    {
        /// <summary>
        /// Gets or sets a value indicating whether the game loop is running.
        /// </summary>
        public bool Run { get; set; }

        /// <summary>
        /// Gets or sets the rendering device used by the game.
        /// </summary>
        public IRenderDevice RenderDevice { get; set; }

        /// <summary>
        /// Gets or sets the list of scenes in the game.
        /// </summary>
        public List<Scene> Scenes { get; set; }

        /// <summary>
        /// Gets or sets the currently selected scene.
        /// </summary>
        public Scene SelectedScene { get; set; }

        /// <summary>
        /// Gets or sets the asset manager for handling game assets.
        /// </summary>
        public AssetManager AssetManager { get; set; }

        /// <summary>
        /// Gets or sets the viewport configuration for rendering.
        /// </summary>
        public Viewport Viewport { get; set; }

        /// <summary>
        /// Gets or sets the target frames per second for the game loop.
        /// </summary>
        public int TargetFPS { get; set; } = 60;

        /// <summary>
        /// Gets or sets the current frames per second achieved by the game loop.
        /// </summary>
        public double FPS { get; set; }

        /// <summary>
        /// Gets or sets the timestamp of the last frame.
        /// </summary>
        public long LastFrame { get; set; }

        /// <summary>
        /// Gets or sets the time elapsed since the last frame in milliseconds.
        /// </summary>
        public double DeltaTime { get; set; }

        /// <summary>
        /// Gets or sets the storage object for managing game data.
        /// </summary>
        public Storage Storage { get; set; }

        /// <summary>
        /// Event triggered when the game initializes.
        /// </summary>
        public event GameEventHandler OnInit;

        /// <summary>
        /// Event triggered before the game update.
        /// </summary>
        public event GameEventHandler BeforeUpdate;

        /// <summary>
        /// Event triggered during the game update.
        /// </summary>
        public event GameEventHandler OnUpdate;

        /// <summary>
        /// Event triggered after the game update.
        /// </summary>
        public event GameEventHandler AfterUpdate;

        /// <summary>
        /// Event triggered before rendering.
        /// </summary>
        public event GameEventHandler BeforeRender;

        /// <summary>
        /// Event triggered at the beginning of rendering.
        /// </summary>
        public event GameEventHandler OnRenderBeginn;

        /// <summary>
        /// Event triggered at the end of rendering.
        /// </summary>
        public event GameEventHandler OnRenderEnd;

        /// <summary>
        /// Event triggered after rendering.
        /// </summary>
        public event GameEventHandler AfterRender;

        /// <summary>
        /// Event triggered when the game is disposed.
        /// </summary>
        public event GameEventHandler OnDispose;

        /// <summary>
        /// Creates a new instance of the Game class.
        /// </summary>
        /// <param name="renderDevice">The rendering device to use.</param>
        /// <param name="viewport">The viewport configuration.</param>
        public Game(IRenderDevice renderDevice, Viewport viewport)
        {
            this.RenderDevice = renderDevice;
            this.Scenes = new List<Scene>();
            this.AssetManager = new AssetManager();
            this.Storage = new Storage();
            Viewport = viewport;
        }

        /// <summary>
        /// Starts the main game loop.
        /// </summary>
        public void Start()
        {
            this.Run = true;
            Thread thread = new Thread(() => { this.Loop(); });
            thread.Start();
        }

        /// <summary>
        /// Main game loop. Handles updates and rendering.
        /// </summary>
        public void Loop()
        {
            long timestamp = Utils.GetCurrentTimeMillis() / 1000;

            RenderDevice.Init();
            AssetManager.Init(RenderDevice);
            foreach (var scene in this.Scenes)
            {
                scene.Init(this, RenderDevice);
            }
            if(OnInit != null)
            {
                OnInit(this, RenderDevice);
            }
            LastFrame = Utils.GetCurrentTimeMillis();
            while(Run)
            {
                long currentFrame = Utils.GetCurrentTimeMillis();
                long currentTime = currentFrame / 1000;
                double frameTime = 1000 / (double) TargetFPS;

                if(currentFrame > LastFrame + frameTime)
                {
                    // Update
                    if (this.SelectedScene != null)
                    {
                        if (OnUpdate != null)
                        {
                            OnUpdate(this, RenderDevice);
                        }
                        this.SelectedScene.OnUpdate(this, RenderDevice);
                        if(AfterUpdate != null) AfterUpdate(this, RenderDevice);
                    }
                    // Rise before render event
                    if (BeforeRender != null)
                    {
                        BeforeRender(this, RenderDevice);
                    }
                    //Render
                    RenderDevice.Viewport(Viewport.X, Viewport.Y, Viewport.Width, Viewport.Height);
                    RenderDevice.Begin();
                    if (OnRenderBeginn != null)
                    {
                        OnRenderBeginn(this, RenderDevice);
                    }
                    if (this.SelectedScene != null)
                    {
                        this.SelectedScene.OnRender(this, RenderDevice);
                    }
                    if(OnRenderEnd != null)
                    {
                        OnRenderEnd(this, RenderDevice);
                    }
                    RenderDevice.End();
                    // Call after render event
                    if (AfterRender != null)
                    {
                        AfterRender(this, RenderDevice);
                    }

                    this.Storage.Process(this, SelectedScene);

                    long deltaTimeLong = currentFrame - LastFrame;
                    DeltaTime = (double) deltaTimeLong;

                    FPS = 1000 / DeltaTime;
                    LastFrame = currentFrame;
                }
            }
            this.AssetManager.DisposeTextures(this);
            foreach (var item in Scenes)
            {
                item.OnDestroy(this);
            }
            if(this.OnDispose != null)
            {
                this.OnDispose(this, RenderDevice);
            }
            RenderDevice.Dispose();
        }

        /// <summary>
        /// Stops the main game loop.
        /// </summary>
        public void Stop()
        {
            this.Run = false;
        }

        /// <summary>
        /// Get the screen coordinates of the given element.
        /// </summary>
        /// <param name="element">The GameElement to get the screen coordinates for.</param>
        /// <returns>A Vec3 representing the screen coordinates of the element.</returns>
        public Vec3 GetElementScreenLocation2D(GameElement element)
        {
            if (this.SelectedScene != null)
            {
                if(this.SelectedScene.Camera != null)
                {
                    float X = (SelectedScene.Camera.Size.X / 2) - SelectedScene.Camera.Location.X + element.Location.X;
                    float Y = (SelectedScene.Camera.Size.Y / 2) - SelectedScene.Camera.Location.Y + element.Location.Y;

                    return new Vec3(X, Y, 0f);
                }
            }
            return null;
        }

        /// <summary>
        /// Returns the screen location of the given element with a center anchor.
        /// </summary>
        /// <param name="element">The GameElement to get the centered screen coordinates for.</param>
        /// <returns>A Vec3 representing the centered screen coordinates of the element.</returns>
        public Vec3 GetElementCenterScreenLocation2D(GameElement element)
        {
            if (this.SelectedScene != null)
            {
                if (this.SelectedScene.Camera != null)
                {
                    float X = (SelectedScene.Camera.Size.X / 2) - SelectedScene.Camera.Location.X + element.Location.X + (element.Size.X / 2);
                    float Y = (SelectedScene.Camera.Size.Y / 2) - SelectedScene.Camera.Location.Y + element.Location.Y + (element.Size.Y / 2);

                    return new Vec3(X, Y, 0f);
                }
            }
            return null;
        }

        /// <summary>
        /// Transforms a display vector to a vector inside the scene.
        /// </summary>
        /// <param name="location">The display vector to transform.</param>
        /// <returns>A Vec3 representing the transformed vector inside the scene.</returns>
        public Vec3 GetSceneCord(Vec3 location)
        {
            float x = SelectedScene.Camera.Location.X - (SelectedScene.Camera.Size.X / 2) + location.X;
            float y = SelectedScene.Camera.Location.Y - (SelectedScene.Camera.Size.Y / 2) + location.Y;
            return new Vec3(x, y);
        }

        /// <summary>
        /// Adds a scene to the list of scenes.
        /// </summary>
        /// <param name="scene">The Scene object to be added.</param>
        /// <returns>The added Scene object.</returns>
        public Scene AddScene(Scene scene)
        {
            this.Scenes.Add(scene);
            return scene;
        }

        /// <summary>
        /// Initializes a GameElement within the game environment.
        /// </summary>
        /// <param name="element">The GameElement to be initialized.</param>
        public void InitGameElement(GameElement element)
        {
            element.Init(this, RenderDevice);
        }

        /// <summary>
        /// Loads a scene with the given name.
        /// </summary>
        /// <param name="name">The name of the scene to be loaded.</param>
        public void LoadScene(String name)
        {
            foreach (var scene in Scenes)
            {
                if(scene.Name.Equals(name))
                {
                    this.LoadScene(scene);
                }
            }
        }

        /// <summary>
        /// Loads a scene
        /// </summary>
        /// <param name="scene">The scene to be loaded.</param>
        public void LoadScene(Scene scene)
        {
            this.SelectedScene = scene;
        }

        /// <summary>
        /// Search for the scene with the given name
        /// </summary>
        /// <param name="name">The name for the scene</param>
        /// <returns></returns>
        public Scene FindScene(String name)
        {
            foreach (var scene in Scenes)
            {
                if (scene.Name.Equals(name))
                {
                    return scene;
                }
            }
            return null;
        }


        public T GetScene<T>() where T : Scene
        {
            foreach (var scene in Scenes)
            {
                if (scene.GetType().Equals(typeof(T)))
                {
                    return (T)scene;
                }
            }
            return null;
        }
    }
}

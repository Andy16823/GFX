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

    public class Game
    {
        public bool Run { get; set; }
        public IRenderDevice RenderDevice { get; set; }
        public List<Scene> Scenes { get; set; }
        public Scene SelectedScene { get; set; }
        public AssetManager AssetManager { get; set; }
        public Viewport Viewport { get; set; }
        public int TargetFPS { get; set; } = 60;
        public double FPS { get; set; }
        public long LastFrame { get; set; }
        public double DeltaTime { get; set; }
        public Storage Storage { get; set; }

        public event GameEventHandler OnInit;
        public event GameEventHandler BeforeUpdate;
        public event GameEventHandler OnUpdate;
        public event GameEventHandler AfterUpdate;
        public event GameEventHandler BeforeRender;
        public event GameEventHandler OnRenderBeginn;
        public event GameEventHandler OnRenderEnd;
        public event GameEventHandler AfterRender;
        public event GameEventHandler OnDispose;

        /// <summary>
        /// Creates a new instance of a the Game class
        /// </summary>
        /// <param name="renderDevice"></param>
        /// <param name="viewport"></param>
        public Game(IRenderDevice renderDevice, Viewport viewport)
        {
            this.RenderDevice = renderDevice;
            this.Scenes = new List<Scene>();
            this.AssetManager = new AssetManager();
            this.Storage = new Storage();
            Viewport = viewport;
        }

        /// <summary>
        /// Starts the main loop
        /// </summary>
        public void Start()
        {
            this.Run = true;
            Thread thread = new Thread(() => { this.Loop(); });
            thread.Start();
        }

        /// <summary>
        /// Main Loop. Handles the updates and the rendering
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
                    if (SelectedScene != null)
                    {
                        if (SelectedScene.Camera != null)
                        {
                            RenderDevice.SetCamera(SelectedScene.Camera);
                        }
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

                    DeltaTime = currentFrame - LastFrame;
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
        /// Stopt the main loop
        /// </summary>
        public void Stop()
        {
            this.Run = false;
        }

        /// <summary>
        /// Get the screen cords of the given element
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
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
        /// Returns the screen location from the given element with a center anchor.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
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
        /// <param name="location"></param>
        /// <returns></returns>
        public Vec3 GetSceneCord(Vec3 location)
        {
            float x = SelectedScene.Camera.Location.X - (SelectedScene.Camera.Size.X / 2) + location.X;
            float y = SelectedScene.Camera.Location.Y - (SelectedScene.Camera.Size.Y / 2) + location.Y;
            return new Vec3(x, y);
        }

        public Scene AddScene(Scene scene)
        {
            this.Scenes.Add(scene);
            return scene;
        }

    }

    

}

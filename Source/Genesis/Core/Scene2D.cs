using Genesis.Graphics;
using Genesis.Graphics.RenderDevice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Core
{
    /// <summary>
    /// Represents a 2D scene in the game.
    /// </summary>
    public class Scene2D : Scene
    {
        /// <summary>
        /// Gets or sets a value indicating whether to render the lightmap.
        /// </summary>
        public bool RenderLightmap { get; set; } = true;

        /// <summary>
        /// Gets or sets the framebuffer used for rendering the lightmap.
        /// </summary>
        public Framebuffer Framebuffer { get; set; }

        /// <summary>
        /// Gets or sets the intensity of the lightmap.
        /// </summary>
        public float LightmapIntensity { get; set; } = 0.7f;

        /// <summary>
        /// Gets the list of 2D lights in the scene.
        /// </summary>
        public List<Light2D> Lights { get; set; }

        /// <summary>
        /// Event that occurs before lightmap preparation.
        /// </summary>
        public event SceneEventHandler BeforeLightmapPreparation;

        /// <summary>
        /// Event that occurs after lightmap rendering.
        /// </summary>
        public event SceneEventHandler AfterLightmapRendering;

        /// <summary>
        /// Initializes a new instance of the <see cref="Scene2D"/> class.
        /// </summary>
        public Scene2D()
        {
            Lights = new List<Light2D>();
        }

        /// <summary>
        /// Adds a 2D light to the scene.
        /// </summary>
        /// <param name="light">The light to add.</param>
        public void AddLight(Light2D light)
        {
            this.Lights.Add(light);
        }

        /// <summary>
        /// Removes a 2D light from the scene.
        /// </summary>
        /// <param name="game">The game instance.</param>
        /// <param name="light">The light to remove.</param>
        public void RemoveLight(Game game, Light2D light)
        {
            this.Lights.Remove(light);
            light.OnDestroy(game);
        }

        /// <summary>
        /// Initializes the scene.
        /// </summary>
        /// <param name="game">The game instance.</param>
        /// <param name="renderDevice">The render device.</param>
        public override void Init(Game game, IRenderDevice renderDevice)
        {
            base.Init(game, renderDevice);
            foreach (var light in Lights)
            {
                light.Init(game, renderDevice);
            }
            this.Framebuffer = renderDevice.BuildFramebuffer(100, 100);
        }

        /// <summary>
        /// Called when the scene needs to update.
        /// </summary>
        /// <param name="game">The game instance.</param>
        /// <param name="renderDevice">The render device.</param>
        public override void OnUpdate(Game game, IRenderDevice renderDevice)
        {
            base.OnUpdate(game, renderDevice);
            foreach (var light in Lights)
            {
                light.OnUpdate(game, renderDevice);
            }
        }

        /// <summary>
        /// Called when the scene needs to render.
        /// </summary>
        /// <param name="game">The game instance.</param>
        /// <param name="renderDevice">The render device.</param>
        public override void OnRender(Game game, IRenderDevice renderDevice)
        {
            if (this.Camera != null)
                renderDevice.SetCamera(game.Viewport, this.Camera);

            // Before scene preperation even
            if (this.BeforeScenePreperation != null)
                this.BeforeScenePreperation(this, game, renderDevice);

            // Scene rendering
            renderDevice.PrepareSceneRendering(this);

            if (this.BeforeSceneRender != null)
                this.BeforeSceneRender(this, game, renderDevice);

            foreach (var item in Layer)
            {
                item.OnRender(game, renderDevice);
            }

            if (this.AfterSceneRender != null)
                this.AfterSceneRender(this, game, renderDevice);

            renderDevice.FinishSceneRendering(this);


            if(this.RenderLightmap)
            {
                if(this.BeforeLightmapPreparation != null)
                    this.BeforeLightmapPreparation(this, game, renderDevice);

                renderDevice.PrepareLightmap2D(this, this.Framebuffer);
                foreach (var light in Lights)
                {
                    light.OnRender(game, renderDevice);
                }
                renderDevice.FinishLightmap2D(this, this.Framebuffer);

                if(this.AfterLightmapRendering != null)
                    this.AfterLightmapRendering(this, game, renderDevice);
            }

            // Canvas canvas preperation
            if (this.BeforeCanvasPreperation != null)
                this.BeforeCanvasPreperation(this, game, renderDevice);

            // Cavas rendering
            renderDevice.PrepareCanvasRendering(this, null);

            if (this.BeforeCanvasRender != null)
                this.BeforeCanvasRender(this, game, renderDevice);

            foreach (var item in Canvas)
            {
                item.OnRender(game, renderDevice, this);
            }

            if (this.AfterCanvasRender != null)
                this.AfterCanvasRender(this, game, renderDevice);

            renderDevice.FinishCanvasRendering(this, null);
        }

        /// <summary>
        /// Called when the scene is being destroyed.
        /// </summary>
        /// <param name="game">The game instance.</param>
        public override void OnDestroy(Game game)
        {
            base.OnDestroy(game);
            for(int i = Lights.Count -1; i >= 0; i--)
            {
                var light = Lights[i];
                this.RemoveLight(game, light);
            }
        }
    }
}

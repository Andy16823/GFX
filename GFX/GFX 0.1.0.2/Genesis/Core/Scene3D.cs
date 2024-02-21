using Genesis.Core.GameElements;
using Genesis.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Core
{
    /// <summary>
    /// Represents a 3D scene for rendering, including lighting and an optional skybox.
    /// </summary>
    public class Scene3D : Scene
    {
        /// <summary>
        /// Gets or sets the light source (sun) in the scene.
        /// </summary>
        public Light Sun { get; set; }

        /// <summary>
        /// Gets or sets the skybox used in the scene (optional).
        /// </summary>
        public Skybox Skybox { get; set; }

        /// <summary>
        /// Initializes a new instance of the Scene3D class.
        /// </summary>
        /// <param name="name">The name of the scene.</param>
        /// <param name="sun">The light source (sun) in the scene.</param>
        public Scene3D(String name, Light sun)
        {
            this.Sun = sun;
            this.Name = name;
        }

        /// <summary>
        /// Initializes the 3D scene.
        /// </summary>
        /// <param name="game">The game instance.</param>
        /// <param name="renderDevice">The render device used for rendering.</param>
        public override void Init(Game game, IRenderDevice renderDevice)
        {
            base.Init(game, renderDevice);
            renderDevice.SetLightSource(this.Sun);
            if(this.Skybox != null)
            {
                Skybox.Init(game, renderDevice);
            }
        }

        /// <summary>
        /// Called during the update phase of the game loop.
        /// </summary>
        /// <param name="game">The game instance.</param>
        /// <param name="renderDevice">The render device used for rendering.</param>
        public override void OnUpdate(Game game, IRenderDevice renderDevice)
        {
            if (this.Skybox != null)
            {
                this.Skybox.Location = this.Camera.Location;
            }
            base.OnUpdate(game, renderDevice);
        }

        /// <summary>
        /// Called during the rendering phase of the game loop.
        /// </summary>
        /// <param name="game">The game instance.</param>
        /// <param name="renderDevice">The render device used for rendering.</param>
        public override void OnRender(Game game, IRenderDevice renderDevice)
        {
            if(this.Camera != null)
               renderDevice.SetCamera(this.Camera);

            if (this.Skybox != null)
                renderDevice.DrawSkyBox(this.Skybox);

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
        /// Called when the scene is destroyed.
        /// </summary>
        /// <param name="game">The game instance.</param>
        public override void OnDestroy(Game game)
        {
            base.OnDestroy(game);
            if(this.Skybox != null)
            {
                this.Skybox.OnDestroy(game);
            }
        }
    }
}

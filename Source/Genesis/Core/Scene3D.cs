using Genesis.Core.GameElements;
using Genesis.Graphics;
using Genesis.Math;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        /// Gets or sets the framebuffer used for storing the shadow map.
        /// </summary>
        /// <value>
        /// The framebuffer object that holds the shadow map texture. This is used during the shadow mapping process to store depth information from the perspective of the light source.
        /// </value>
        public Framebuffer ShadowMap { get; set; }

        /// <summary>
        /// Gets or sets the resolution of the shadow map.
        /// </summary>
        /// <value>
        /// A <see cref="Vec3"/> representing the width, height, and depth (unused) of the shadow map in pixels.
        /// The default value is (1024, 1024, 0), where the third component is typically not used.
        /// </value>
        public Vec3 ShadowResolution { get; set; } = new Vec3(1024, 1024, 0);

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
            ShadowMap = renderDevice.BuildShadowMap((int) ShadowResolution.X, (int) ShadowResolution.Y);
            Debug.WriteLine("Shadowmap created with error " + renderDevice.GetError());
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
            var lightspaceMatrix = renderDevice.GenerateLightspaceMatrix(Camera, game.Viewport, Sun);
            renderDevice.PrepareShadowPass(ShadowMap, lightspaceMatrix);
            if (this.Sun.CastShadows)
            {
                renderDevice.RenderShadowmap(ShadowMap, lightspaceMatrix, this);
            }
            renderDevice.FinishShadowPass(game.Viewport);
            Debug.WriteLine($"Rendered Shadowmap with error: {renderDevice.GetError()}");

            // Old code
            if (this.Camera != null)
               renderDevice.SetCamera(game.Viewport, this.Camera);

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

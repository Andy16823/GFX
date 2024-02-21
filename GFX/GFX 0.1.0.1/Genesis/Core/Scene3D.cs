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
    /// Creates a scene for 3D rendering. This scene contains
    /// lightning and a optional skybox.
    /// </summary>
    public class Scene3D : Scene
    {
        public Light Sun { get; set; }
        public Skybox Skybox { get; set; }

        public Scene3D(String name, Light sun)
        {
            this.Sun = sun;
            this.Name = name;
        }

        public override void Init(Game game, IRenderDevice renderDevice)
        {
            base.Init(game, renderDevice);
            renderDevice.SetLightSource(this.Sun);
            if(this.Skybox != null)
            {
                Skybox.Init(game, renderDevice);
            }
        }

        public override void OnUpdate(Game game, IRenderDevice renderDevice)
        {
            if (this.Skybox != null)
            {
                this.Skybox.Location = this.Camera.Location;
            }
            base.OnUpdate(game, renderDevice);
        }

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

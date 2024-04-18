using Genesis.Graphics;
using Genesis.Graphics.RenderDevice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Core
{
    public class Scene2D : Scene
    {
        public bool RenderLightmap { get; set; } = true;
        public Framebuffer Framebuffer { get; set; }
        public float LightmapIntensity { get; set; } = 0.7f;
        public List<Light2D> Lights { get; set; }

        public event SceneEventHandler BeforeLightmapPreparation;
        public event SceneEventHandler AfterLightmapRendering;

        public Scene2D()
        {
            Lights = new List<Light2D>();
        }

        public void AddLight(Light2D light)
        {
            this.Lights.Add(light);
        }

        public void RemoveLight(Game game, Light2D light)
        {
            this.Lights.Remove(light);
            light.OnDestroy(game);
        }

        public override void Init(Game game, IRenderDevice renderDevice)
        {
            base.Init(game, renderDevice);
            foreach (var light in Lights)
            {
                light.Init(game, renderDevice);
            }
            this.Framebuffer = renderDevice.BuildFramebuffer(100, 100);
        }

        public override void OnUpdate(Game game, IRenderDevice renderDevice)
        {
            base.OnUpdate(game, renderDevice);
            foreach (var light in Lights)
            {
                light.OnUpdate(game, renderDevice);
            }
        }

        public override void OnRender(Game game, IRenderDevice renderDevice)
        {
            if (this.Camera != null)
                renderDevice.SetCamera(this.Camera);

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

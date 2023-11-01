using Genesis.Core.Prefabs;
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
            if(this.Skybox != null)
            {
                renderDevice.DrawSkyBox(this.Skybox);
            }
            base.OnRender(game, renderDevice);
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

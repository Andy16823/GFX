using Genesis.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Core
{
    public class LightScene : Scene
    {
        public Light Sun { get; set; }

        public LightScene(String name, Light sun)
        {
            this.Sun = sun;
            this.Name = name;
        }

        public override void Init(Game game, IRenderDevice renderDevice)
        {
            base.Init(game, renderDevice);
            renderDevice.SetLightSource(this.Sun);
        }
    }
}

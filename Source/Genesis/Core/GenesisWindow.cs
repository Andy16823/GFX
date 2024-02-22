using Genesis.Graphics;
using Genesis.Graphics.RenderDevice;
using Genesis.Math;
using NetGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Genesis.Core
{
    public class GenesisWindow : Form
    {
        public Game Game { get; set; }


        public GenesisWindow()
        {
            this.Width = 800;
            this.Height = 600;
            this.Game = new Game(new ClassicGL(Handle), new Viewport(this.ClientSize.Width, this.ClientSize.Height));
            this.Game.OnUpdate += (game, renderer) =>
            {
                this.Update(game);
            };
        }

        protected override void OnResize(EventArgs e)
        {
            if(Game != null)
            {
                Game.Viewport.SetNewViewport(ClientSize.Width, ClientSize.Height);
            }
            base.OnResize(e);
        }

        public virtual void Update(Game game)
        {

        }

        public virtual void Init(Game game)
        {

        }

        public virtual void Start(Game game)
        {
            
        }

    }
}

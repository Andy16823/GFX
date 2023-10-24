using Genesis.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Core
{
    public class Layer
    {
        public String Name { get; set; }
        public List<GameElement> Elements { get; set; }

        public Layer()
        {
            Elements= new List<GameElement>();
        }

        public Layer(String name)
        {
            this.Name = name;
            Elements= new List<GameElement>();
        }

        public void Init(Game game, IRenderDevice renderDevice)
        {
            foreach (var item in Elements)
            {
                item.Init(game, renderDevice);
            }
        }

        public void OnRender(Game game, IRenderDevice renderDevice)
        {
            foreach (var item in Elements)
            {
                item.OnRender(game, renderDevice);
            }
        }

        public void OnUpdate(Game game, IRenderDevice renderDevice)
        {
            foreach (var item in Elements)
            {
                item.OnUpdate(game, renderDevice);
            }
        }

        public void OnDestroy(Game game)
        {
            foreach (var item in Elements)
            {
                item.OnDestroy(game);
            }
        }

    }
}

using Genesis.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Core
{
    public abstract class IGameBehavior
    {
        public GameElement Parent { get; set; }

        public abstract void OnInit(Game game, GameElement parent);
        public abstract void OnUpdate(Game game, GameElement parent);
        public abstract void OnRender(Game game, GameElement parent);
        public abstract void OnDestroy(Game game, GameElement parent);
    }
}

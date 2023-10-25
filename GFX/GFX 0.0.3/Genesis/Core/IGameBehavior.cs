using Genesis.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Core
{
    public interface IGameBehavior
    {
        void OnInit(Game game, GameElement parent);
        void OnUpdate(Game game, GameElement parent);
        void OnRender(Game game, GameElement parent);
        void OnDestroy(Game game, GameElement parent);
    }
}

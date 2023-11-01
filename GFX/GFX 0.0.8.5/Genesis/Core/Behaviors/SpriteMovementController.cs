using Genesis.Core.Prefabs;
using Genesis.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Genesis.Core.Behaviors
{
    public class SpriteMovementController : IGameBehavior
    {
        public bool Automove { get; set; }
        public float MoveSpeed { get; set; } = 5f;
        private Sprite parent;
        private Game game;

        public SpriteMovementController()
        {

        }

        public override void OnDestroy(Game game, GameElement parent)
        {

        }

        public override void OnInit(Game game, GameElement parent)
        {
            this.game = game;
            this.parent = (Sprite)parent;
        }

        public override void OnRender(Game game, GameElement parent)
        {
            
        }

        public override void OnUpdate(Game game, GameElement parent)
        {
            if(this.Automove)
            {
                if (Input.IsKeyDown(Keys.W))
                {
                    MoveUp();
                }
                if (Input.IsKeyDown(Keys.S))
                {
                    MoveDown();
                }
                if (Input.IsKeyDown(Keys.A))
                {
                    MoveLeft();
                }
                if (Input.IsKeyDown(Keys.D))
                {
                    MoveRight();
                }
            }
        }

        public void MoveUp()
        {
            parent.Location.Y -= MoveSpeed * (float)game.DeltaTime;
        }

        public void MoveDown()
        {
            parent.Location.Y += MoveSpeed * (float)game.DeltaTime;
        }

        public void MoveLeft() { 
            parent.Location.X -= MoveSpeed * (float)game.DeltaTime;
        }

        public void MoveRight()
        {
            parent.Location.X += MoveSpeed * (float)game.DeltaTime;
        }
    }
}

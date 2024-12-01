using Genesis.Core.GameElements;
using Genesis.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Genesis.Core.Behaviors
{
    /// <summary>
    /// Represents a behavior for controlling the movement of a sprite.
    /// </summary>
    public class SpriteMovementController : IGameBehavior
    {
        /// <summary>
        /// Gets or sets a flag indicating whether automatic movement is enabled.
        /// </summary>
        public bool Automove { get; set; }

        /// <summary>
        /// Gets or sets the speed of the sprite movement.
        /// </summary>
        public float MoveSpeed { get; set; } = 5f;

        private Sprite parent;
        private Game game;

        /// <summary>
        /// Initializes a new instance of the SpriteMovementController class.
        /// </summary>
        public SpriteMovementController()
        {

        }

        /// <summary>
        /// Called when the behavior is destroyed.
        /// </summary>
        /// <param name="game"></param>
        /// <param name="parent"></param>
        public override void OnDestroy(Game game, GameElement parent)
        {

        }

        /// <summary>
        /// Called when the behavior is initialized.
        /// </summary>
        /// <param name="game"></param>
        /// <param name="parent"></param>
        public override void OnInit(Game game, GameElement parent)
        {
            this.game = game;
            this.parent = (Sprite)parent;
        }

        /// <summary>
        /// Called during the rendering phase.
        /// </summary>
        /// <param name="game"></param>
        /// <param name="parent"></param>
        public override void OnRender(Game game, GameElement parent)
        {
            
        }

        /// <summary>
        /// Called during the update phase.
        /// </summary>
        /// <param name="game"></param>
        /// <param name="parent"></param>
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

        /// <summary>
        /// Moves the sprite upwards.
        /// </summary>
        public void MoveUp()
        {
            parent.Location = parent.Location.SubY(MoveSpeed * (float)game.DeltaTime);
        }

        /// <summary>
        /// Moves the sprite downwards.
        /// </summary>
        public void MoveDown()
        {
            parent.Location = parent.Location.AddY(MoveSpeed * (float)game.DeltaTime);
        }

        /// <summary>
        /// Moves the sprite to the left.
        /// </summary>
        public void MoveLeft() {
            parent.Location = parent.Location.SubX(MoveSpeed * (float)game.DeltaTime);
        }

        /// <summary>
        /// Moves the sprite to the right.
        /// </summary>
        public void MoveRight()
        {
            parent.Location = parent.Location.AddX(MoveSpeed * (float)game.DeltaTime);
        }
    }
}

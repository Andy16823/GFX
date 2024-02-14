using Genesis.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Core
{
    /// <summary>
    /// Represents the base class for game behaviors in the Genesis framework.
    /// </summary>
    public abstract class IGameBehavior
    {
        /// <summary>
        /// Gets or sets the parent game element to which this behavior is attached.
        /// </summary>
        public GameElement Parent { get; set; }

        /// <summary>
        /// Called when the game behavior is initialized.
        /// </summary>
        /// <param name="game">The game instance.</param>
        /// <param name="parent">The parent game element.</param>
        public abstract void OnInit(Game game, GameElement parent);

        /// <summary>
        /// Called when the game behavior is updated.
        /// </summary>
        /// <param name="game">The game instance.</param>
        /// <param name="parent">The parent game element.</param>
        public abstract void OnUpdate(Game game, GameElement parent);

        /// <summary>
        /// Called when the game behavior is rendered.
        /// </summary>
        /// <param name="game">The game instance.</param>
        /// <param name="parent">The parent game element.</param>
        public abstract void OnRender(Game game, GameElement parent);

        /// <summary>
        /// Called when the game behavior is being destroyed.
        /// </summary>
        /// <param name="game">The game instance.</param>
        /// <param name="parent">The parent game element.</param>
        public abstract void OnDestroy(Game game, GameElement parent);
    }
}

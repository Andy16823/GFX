using Genesis.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Core
{
    /// <summary>
    /// Represents a layer containing game elements in the Genesis framework.
    /// </summary>
    public class Layer
    {
        /// <summary>
        /// Gets or sets the name of the layer.
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// Gets or sets the list of game elements within the layer.
        /// </summary>
        public List<GameElement> Elements { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the elements within the layer should be updated.
        /// </summary>
        public bool UpdateElements { get; set; } = true;

        /// <summary>
        /// Creates a new instance of the Layer class.
        /// </summary>
        public Layer()
        {
            Elements= new List<GameElement>();
        }

        /// <summary>
        /// Creates a new instance of the Layer class with the specified name.
        /// </summary>
        /// <param name="name">The name of the layer.</param>
        public Layer(String name)
        {
            this.Name = name;
            Elements= new List<GameElement>();
        }

        /// <summary>
        /// Creates a new instance of the Layer class with the specified name and updateElements flag.
        /// </summary>
        /// <param name="name">The name of the layer.</param>
        /// <param name="updateElements">Flag indicating whether the elements within the layer should be updated.</param>
        public Layer(String name, bool updateElements)
        {
            this.Name = name;
            this.UpdateElements = updateElements;
            Elements = new List<GameElement>();
        }

        /// <summary>
        /// Initializes all game elements within the layer.
        /// </summary>
        /// <param name="game">The game instance.</param>
        /// <param name="renderDevice">The rendering device.</param>
        public void Init(Game game, IRenderDevice renderDevice)
        {
            foreach (var item in Elements)
            {
                item.Init(game, renderDevice);
            }
        }

        /// <summary>
        /// Renders all game elements within the layer.
        /// </summary>
        /// <param name="game">The game instance.</param>
        /// <param name="renderDevice">The rendering device.</param>
        public void OnRender(Game game, IRenderDevice renderDevice)
        {
            foreach (var item in Elements)
            {
                item.OnRender(game, renderDevice);
            }
        }

        /// <summary>
        /// Updates all game elements within the layer if the UpdateElements flag is true.
        /// </summary>
        /// <param name="game">The game instance.</param>
        /// <param name="renderDevice">The rendering device.</param>
        public void OnUpdate(Game game, IRenderDevice renderDevice)
        {
            if(this.UpdateElements)
            {
                Parallel.ForEach(Elements, item =>
                {
                    item.OnUpdate(game, renderDevice);
                });
            }
        }

        /// <summary>
        /// Destroys all game elements within the layer.
        /// </summary>
        /// <param name="game">The game instance.</param>
        public void OnDestroy(Game game)
        {
            foreach (var item in Elements)
            {
                item.OnDestroy(game);
            }
        }

    }
}

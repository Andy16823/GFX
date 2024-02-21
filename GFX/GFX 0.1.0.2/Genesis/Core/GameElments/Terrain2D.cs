using Genesis.Graphics;
using Genesis.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Core.GameElements
{
    /// <summary>
    /// Represents a 2D terrain element.
    /// </summary>
    public class Terrain2D : GameElement
    {
        /// <summary>
        /// Gets or sets the texture of the terrain.
        /// </summary>
        public Texture Texture { get; set; }

        /// <summary>
        /// Gets or sets the number of cells in the X direction.
        /// </summary>
        public float CellsX { get; set; }

        /// <summary>
        /// Gets or sets the number of cells in the Y direction.
        /// </summary>
        public float CellsY { get; set; }

        /// <summary>
        /// Gets or sets the size of each cell.
        /// </summary>
        public float CellSize { get; set; }


        private float texRepeatX;
        private float texRepeatY;

        /// <summary>
        /// Initializes a new instance of the Terrain2D class with specified name, location, cell counts, cell size, and texture.
        /// </summary>
        /// <param name="name">The name of the terrain.</param>
        /// <param name="location">The initial location of the terrain.</param>
        /// <param name="cellsX">The number of cells in the X direction.</param>
        /// <param name="cellsY">The number of cells in the Y direction.</param>
        /// <param name="cellSize">The size of each cell.</param>
        /// <param name="texture">The texture for the terrain.</param>
        public Terrain2D(String name, Vec3 location, float cellsX, float cellsY, float cellSize, Texture texture)
        {
            this.Name = name;
            this.Location = location;
            this.CellsX = cellsX;
            this.CellsY = cellsY;
            this.CellSize= cellSize;
            this.Texture = texture;
            this.Size = new Vec3(cellsX * cellSize, cellsY * cellSize);
            texRepeatX = Size.X / cellSize;
            texRepeatY = Size.Y / cellSize;  
        }

        /// <summary>
        /// Initializes the terrain element.
        /// </summary>
        /// <param name="game">The game instance.</param>
        /// <param name="renderDevice">The render device used for rendering.</param>
        public override void Init(Game game, IRenderDevice renderDevice)
        {
            base.Init(game, renderDevice);
            if (Texture.RenderID == 0)
            {
                renderDevice.LoadTexture(Texture);
            }
        }

        /// <summary>
        /// Renders the terrain element.
        /// </summary>
        /// <param name="game">The game instance.</param>
        /// <param name="renderDevice">The render device used for rendering.</param>
        public override void OnRender(Game game, IRenderDevice renderDevice)
        {
            base.OnRender(game, renderDevice);

            float tX = Location.X + (Size.X / 2);
            float tY = Location.Y + (Size.Y / 2);

            renderDevice.ModelViewMatrix();
            renderDevice.PushMatrix();
            renderDevice.DrawTexture(Location, Size, texRepeatX, texRepeatY, Texture);
            renderDevice.PopMatrix();
        }

        /// <summary>
        /// Handles cleanup and resource disposal when the terrain is destroyed.
        /// </summary>
        /// <param name="game">The game instance.</param>
        public override void OnDestroy(Game game)
        {
            base.OnDestroy(game);
            game.RenderDevice.DisposeTexture(Texture);
        }

        /// <summary>
        /// Gets the 2D bounds of the terrain.
        /// </summary>
        /// <returns>A rectangular region representing the 2D bounds of the terrain.</returns>
        public Rect GetBounds()
        {
            return new Rect(Location.X, Location.Y, Size.X, Size.Y);
        }

    }
}

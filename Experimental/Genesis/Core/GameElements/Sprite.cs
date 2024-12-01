using Genesis.Graphics;
using Genesis.Math;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Core.GameElements
{
    /// <summary>
    /// Represents a sprite element in a 2D or 3D environment.
    /// </summary>
    public class Sprite : GameElement
    {
        /// <summary>
        /// Gets or sets the texture of the sprite.
        /// </summary>
        public Texture Texture { get; set; }

        /// <summary>
        /// Gets or sets the color of the sprite.
        /// </summary>
        public Color Color { get; set; } = Color.White;

        /// <summary>
        /// Gets or sets the texture coordinates of the sprite.
        /// </summary>
        public TexCoords TexCoords { get; set; } = new TexCoords();

        /// <summary>
        /// Gets or sets a value indicating whether occlusion culling is enabled for the sprite.
        /// </summary>
        public Boolean OcclusionCulling { get; set; }

        /// <summary>
        /// Initializes a new instance of the Sprite class with specified name, location, size, and texture.
        /// </summary>
        /// <param name="name">The name of the sprite.</param>
        /// <param name="location">The initial location of the sprite.</param>
        /// <param name="size">The size of the sprite.</param>
        /// <param name="texture">The texture for the sprite.</param>
        public Sprite(String name, Vec3 location, Vec3 size, Texture texture)
        {
            this.Name = name;
            this.Location= location;    
            this.Size= size;
            this.Texture= texture;
            this.OcclusionCulling = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Sprite"/> class.
        /// </summary>
        /// <param name="name">The name of the sprite.</param>
        /// <param name="location">The location of the sprite.</param>
        /// <param name="size">The size of the sprite.</param>
        /// <param name="spriteSheet">The sprite sheet from which this sprite is sourced.</param>
        /// <param name="col">The column index of the sprite in the sprite sheet.</param>
        /// <param name="row">The row index of the sprite in the sprite sheet.</param>
        public Sprite(String name, Vec3 location, Vec3 size, SpriteSheet spriteSheet, int col, int row)
        {
            this.Name = name;
            this.Location = location;
            this.Size = size;
            this.Texture = spriteSheet.Texture;
            this.OcclusionCulling = true;
            this.TexCoords = spriteSheet.GetSprite(col, row);
        }

        /// <summary>
        /// Initializes the sprite element.
        /// </summary>
        /// <param name="game">The game instance.</param>
        /// <param name="renderDevice">The render device used for rendering.</param>
        public override void Init(Game game, IRenderDevice renderDevice)
        {
            base.Init(game, renderDevice);
            renderDevice.InitSprite(this);
            if(Texture != null)
            {
                if (Texture.RenderID == 0)
                {
                    renderDevice.LoadTexture(Texture);
                }
            }
        }

        /// <summary>
        /// Renders the sprite element.
        /// </summary>
        /// <param name="game">The game instance.</param>
        /// <param name="renderDevice">The render device used for rendering.</param>
        public override void OnRender(Game game, IRenderDevice renderDevice)
        {
            base.OnRender(game, renderDevice);

            if(game.SelectedScene.Camera.GetRect().Intersects(this.GetBounds2D()))
            {
                renderDevice.DrawSprite(this);
            }
        }

        /// <summary>
        /// Handles cleanup and resource disposal when the sprite is destroyed.
        /// </summary>
        /// <param name="game">The game instance.</param>
        public override void OnDestroy(Game game)
        {
            base.OnDestroy(game);
            game.RenderDevice.DisposeTexture(Texture);
        }

        /// <summary>
        /// Gets the 2D bounds of the sprite.
        /// </summary>
        /// <returns>A rectangular region representing the 2D bounds of the sprite.</returns>
        public Rect GetBounds2D()
        {
            return new Rect(Location.X, Location.Y, Size.X, Size.Y);
        }

        /// <summary>
        /// Gets the center location of the sprite.
        /// </summary>
        /// <returns>The center location of the sprite.</returns>
        public Vec3 GetCenterLocation()
        {
            return new Vec3(Location.X + (Size.X / 2), Location.Y + (Size.Y / 2));
        }

        /// <summary>
        /// Calculates the vertex coordinates of the sprite.
        /// </summary>
        /// <returns>An array containing the vertex coordinates of the sprite.</returns>
        public float[] CalculateVerticies()
        {
            float LeftX = this.Location.X - (Size.X / 2);
            float RightX = this.Location.X + (Size.X / 2);
            float top = this.Location.Y + (Size.Y / 2);
            float bottom = this.Location.Y - (Size.Y / 2);
            
            return new float[]
            {
                LeftX, bottom, 0.0f,
                LeftX, top, 0.0f,
                RightX, top, 0.0f,

                LeftX, bottom, 0.0f,
                RightX, top, 0.0f,
                RightX, bottom, 0.0f
            };
        }
    }
}

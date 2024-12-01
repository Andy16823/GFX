using Genesis.Graphics;
using Genesis.Math;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Core.GameElements
{
    /// <summary>
    /// Represents the definition of the shape for a sprite, including its location and size.
    /// </summary>
    public struct SpriteShapeDeffinition
    {    
        public SpriteShapeDeffinition(Vec3 loc, Vec3 size)
        {
            this.locX = loc.X;
            this.locY = loc.Y;
            this.sizeX = size.X;
            this.sizeY = size.Y;
        }

        public float locX;
        public float locY;
        public float sizeX;
        public float sizeY;
    }


    /// <summary>
    /// Represents a game element that creates a buffered sprite with vertices, colors, and texture coordinates.
    /// </summary>
    public class BufferedSprite : GameElement
    {
        /// <summary>
        /// Gets or sets the list of vertices for the sprite.
        /// </summary>
        public List<float> Verticies { get; set; }

        /// <summary>
        /// Gets or sets the list of colors for the sprite.
        /// </summary>
        public List<float> Colors { get; set; }

        /// <summary>
        /// Gets or sets the list of texture coordinates for the sprite.
        /// </summary>
        public List<float> TexCoords { get; set; }

        /// <summary>
        /// Gets or sets the list of shape definitions for the sprite.
        /// </summary>
        public List<SpriteShapeDeffinition> ShapeDeffinitions { get; set; }

        /// <summary>
        /// Gets or sets the texture applied to the sprite.
        /// </summary>
        public Texture Texture { get; set; }

        /// <summary>
        /// Creates a new buffered sprite with the specified name, location, and texture.
        /// </summary>
        /// <param name="name">The name of the game element.</param>
        /// <param name="location">The location of the game element.</param>
        /// <param name="texture">The texture applied to the sprite.</param>
        public BufferedSprite(String name, Vec3 location, Texture texture)
        {
            this.Name = name;
            this.Location = location;
            this.Texture = texture;
            this.Verticies = new List<float>();
            this.Colors = new List<float>();
            this.TexCoords = new List<float>();
            this.ShapeDeffinitions = new List<SpriteShapeDeffinition>();
        }

        /// <summary>
        /// Adds a new rectangular shape at the given location and with the given size to the sprite.
        /// </summary>
        /// <param name="location">The location for the sprite.</param>
        /// <param name="size">The size for the sprite.</param>
        public void AddShape(Vec3 location, Vec3 size)
        {
            float LeftX = location.X - (size.X / 2);
            float RightX = location.X + (size.X / 2);
            float top = location.Y + (size.Y / 2);
            float bottom = location.Y - (size.Y / 2);

            float[] verticies =
            {
                LeftX, bottom, 0.0f,
                LeftX, top, 0.0f,
                RightX, top, 0.0f,

                LeftX, bottom, 0.0f,
                RightX, top, 0.0f,
                RightX, bottom, 0.0f
            };
            this.Verticies.AddRange(verticies);

            float[] color =
            {
                1f, 1f, 1f,
                1f, 1f, 1f,
                1f, 1f, 1f,

                1f, 1f, 1f,
                1f, 1f, 1f,
                1f, 1f, 1f
            };
            this.Colors.AddRange(color);
            
            float[] textCoordsf =
            {
                0.0f, 1.0f,
                0.0f, 0.0f,
                1.0f, 0.0f,

                0.0f, 1.0f,
                1.0f, 0.0f,
                1.0f, 1.0f
            };
            this.TexCoords.AddRange(textCoordsf);

            SpriteShapeDeffinition deffinition = new SpriteShapeDeffinition();
            deffinition.locX = location.X;
            deffinition.locY = location.Y;
            deffinition.sizeX = size.X;
            deffinition.sizeY = size.Y;
            this.ShapeDeffinitions.Add(deffinition);
        }

        /// <summary>
        /// Adds a new rectangular shape at the given location, size, and texture coordinates to the sprite.
        /// </summary>
        /// <param name="location">The location for the sprite.</param>
        /// <param name="size">The size for the sprite.</param>
        /// <param name="texCoords">The texture coordinates for the sprite.</param>
        public void AddShape(Vec3 location, Vec3 size, TexCoords texCoords)
        {
            float LeftX = location.X - (size.X / 2);
            float RightX = location.X + (size.X / 2);
            float top = location.Y + (size.Y / 2);
            float bottom = location.Y - (size.Y / 2);

            float[] verticies =
            {
                LeftX, bottom, 0.0f,
                LeftX, top, 0.0f,
                RightX, top, 0.0f,

                LeftX, bottom, 0.0f,
                RightX, top, 0.0f,
                RightX, bottom, 0.0f
            };
            this.Verticies.AddRange(verticies);

            float[] color =
            {
                1f, 1f, 1f,
                1f, 1f, 1f,
                1f, 1f, 1f,

                1f, 1f, 1f,
                1f, 1f, 1f,
                1f, 1f, 1f
            };
            this.Colors.AddRange(color);

            float[] textCoordsf =
            {
                texCoords.BottomLeft.X, texCoords.BottomLeft.Y, // Left bottom
                texCoords.TopLeft.X, texCoords.TopLeft.Y,
                texCoords.TopRight.X, texCoords.TopRight.Y,
               
                texCoords.BottomLeft.X, texCoords.BottomLeft.Y,
                texCoords.TopRight.X, texCoords.TopRight.Y,
                texCoords.BottomRight.X,texCoords.BottomRight.Y
            };
            this.TexCoords.AddRange(textCoordsf);

            SpriteShapeDeffinition deffinition = new SpriteShapeDeffinition();
            deffinition.locX = location.X;
            deffinition.locY = location.Y;
            deffinition.sizeX = size.X;
            deffinition.sizeY = size.Y;
            this.ShapeDeffinitions.Add(deffinition);
        }

        /// <summary>
        /// Initializes the game element.
        /// </summary>
        /// <param name="game">The game instance.</param>
        /// <param name="renderDevice">The render device used for rendering.</param>
        public override void Init(Game game, IRenderDevice renderDevice)
        {
            base.Init(game, renderDevice);  
        }

        /// <summary>
        /// Renders the game element.
        /// </summary>
        /// <param name="game">The game instance.</param>
        /// <param name="renderDevice">The render device used for rendering.</param>
        public override void OnRender(Game game, IRenderDevice renderDevice)
        {
            base.OnRender(game, renderDevice);
            renderDevice.DrawBufferedSprite(this);
        }

        /// <summary>
        /// Updates the game element.
        /// </summary>
        /// <param name="game">The game instance.</param>
        /// <param name="renderDevice">The render device used for rendering.</param>
        public override void OnUpdate(Game game, IRenderDevice renderDevice)
        {
            base.OnUpdate(game, renderDevice);
        }
    }
}

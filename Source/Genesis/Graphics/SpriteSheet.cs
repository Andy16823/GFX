using BulletSharp.Math;
using Genesis.Core;
using Genesis.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Graphics
{
    public class SpriteSheet
    {
        /// <summary>
        /// Gets or sets the name of the sprite sheet.
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// Gets or sets the resolution of each tile in the sprite sheet.
        /// </summary>
        public Vec3 TileResolution { get; set; }

        /// <summary>
        /// Gets or sets the texture used for the sprite sheet.
        /// </summary>
        public Texture Texture { get; set; }

        /// <summary>
        /// Gets the UUID of the sprite sheet.
        /// </summary>
        public String UUID { get; set; }

        /// <summary>
        /// Gets the number of rows in the sprite sheet.
        /// </summary>
        public int Rows { get; set; }

        /// <summary>
        /// Gets the number of columns in the sprite sheet.
        /// </summary>
        public int Columns { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SpriteSheet"/> class.
        /// </summary>
        /// <param name="name">The name of the sprite sheet.</param>
        /// <param name="tileResolution">The resolution of each tile in the sprite sheet.</param>
        /// <param name="texture">The texture used for the sprite sheet.</param>
        public SpriteSheet(String name, Vec3 tileResolution, Texture texture)
        {
            this.Name = name;
            this.TileResolution = tileResolution;
            this.Texture = texture;
            this.UUID = System.Guid.NewGuid().ToString();
            this.Rows = (int)(texture.Bitnmap.Height / tileResolution.Y);
            this.Columns = (int)(texture.Bitnmap.Width / tileResolution.X);
        }

        /// <summary>
        /// Retrieves the texture coordinates for the specified tile in the sprite sheet.
        /// </summary>
        /// <param name="col">Column index of the tile (starting from 0).</param>
        /// <param name="row">Row index of the tile (starting from 0).</param>
        /// <returns>The texture coordinates of the specified tile.</returns>
        public TexCoords GetSprite(float col, float row)
        {
            float colFactor = 1.0f / (float)Columns;
            float rowFactor = 1.0f / (float)Rows;

            TexCoords texCoords = new TexCoords();
            texCoords.TopLeft = new Vec3(colFactor * col, rowFactor * row);
            texCoords.TopRight = new Vec3(colFactor * (col + 1f), rowFactor * row);
            texCoords.BottomRight = new Vec3(colFactor * (col + 1f), rowFactor * (row + 1f));
            texCoords.BottomLeft = new Vec3(colFactor * col, rowFactor * (row + 1f));
            return texCoords;
        }

        public Rect GetSpriteBounds(int col, int row)
        {
            var textureWidth = (float)Texture.Bitnmap.Width;
            var textureHeight = (float)Texture.Bitnmap.Height;

            var cellWidth = textureWidth / (float)this.Columns;
            var cellHeight = textureHeight / (float)this.Rows;
            var x = (float)col * cellWidth;
            var y = (float)row * textureHeight;

            return new Rect(x, y, cellWidth, cellHeight);
        }

        public Vec4 CalculateUVTransform(int col, int row)
        {
            var textureWidth = (float)Texture.Bitnmap.Width;
            var textureHeight = (float)Texture.Bitnmap.Height;
            var spriteBounds = GetSpriteBounds(col, row);
            return Utils.CalculateUVTransform(textureWidth, textureHeight, spriteBounds.X, spriteBounds.Y, spriteBounds.Width, spriteBounds.Height);
        }
    }
}

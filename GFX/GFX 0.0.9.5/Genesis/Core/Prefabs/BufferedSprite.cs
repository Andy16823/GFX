using Genesis.Graphics;
using Genesis.Math;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Core.Prefabs
{
    /// <summary>
    /// Creates an Sprite Buffer
    /// </summary>
    public class BufferedSprite : GameElement
    {
        public List<float> Verticies { get; set; }
        public List<float> Colors { get; set; }
        public List<float> TexCoords { get; set; }

        public Texture Texture { get; set; }

        /// <summary>
        /// Creates a new sprite buffer
        /// </summary>
        /// <param name="name">Name of the game element</param>
        /// <param name="location">Location of the game element</param>
        /// <param name="texture">Texture for the sprites</param>
        public BufferedSprite(String name, Vec3 location, Texture texture)
        {
            this.Name = name;
            this.Location = location;
            this.Texture = texture;
            this.Verticies = new List<float>();
            this.Colors = new List<float>();
            this.TexCoords = new List<float>();
        }

        /// <summary>
        /// Adds a new shape at the given location and with the given size
        /// </summary>
        /// <param name="location">The location for the sprite</param>
        /// <param name="size">The size for the sprite</param>
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

        }

        /// <summary>
        /// Init the game element
        /// </summary>
        /// <param name="game"></param>
        /// <param name="renderDevice"></param>
        public override void Init(Game game, IRenderDevice renderDevice)
        {
            base.Init(game, renderDevice);  
        }

        /// <summary>
        /// Renders the game element
        /// </summary>
        /// <param name="game"></param>
        /// <param name="renderDevice"></param>
        public override void OnRender(Game game, IRenderDevice renderDevice)
        {
            base.OnRender(game, renderDevice);
            renderDevice.DrawBufferedSprite(this);
        }

        /// <summary>
        /// Updates the game element
        /// </summary>
        /// <param name="game"></param>
        /// <param name="renderDevice"></param>
        public override void OnUpdate(Game game, IRenderDevice renderDevice)
        {
            base.OnUpdate(game, renderDevice);
        }
    }
}

using Genesis.Graphics;
using Genesis.Math;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Core
{
    /// <summary>
    /// Represents a 2D light element in the game.
    /// </summary>
    public class Light2D : GameElement
    {
        /// <summary>
        /// Gets or sets the shape of the light.
        /// </summary>
        public Texture LightShape { get; set; }

        /// <summary>
        /// Gets or sets the color of the light.
        /// </summary>
        public Color LightColor { get; set; } = Color.White;

        /// <summary>
        /// Initializes a new instance of the <see cref="Light2D"/> class with the specified location and size.
        /// </summary>
        /// <param name="location">The location of the light.</param>
        /// <param name="size">The size of the light.</param>
        public Light2D(Vec3 location, Vec3 size)
        {
            this.Location = location;
            this.Size = size;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Light2D"/> class with the specified name, location, size, and texture.
        /// </summary>
        /// <param name="name">The name of the light.</param>
        /// <param name="location">The location of the light.</param>
        /// <param name="size">The size of the light.</param>
        /// <param name="texture">The texture of the light.</param>
        public Light2D(String name, Vec3 location, Vec3 size, Texture texture)
        {
            this.Name = name;
            this.Location = location;
            this.Size = size;
            this.LightShape = texture;
        }

        /// <summary>
        /// Initializes the light element.
        /// </summary>
        /// <param name="game">The game instance.</param>
        /// <param name="renderDevice">The render device.</param>
        public override void Init(Game game, IRenderDevice renderDevice)
        {
            base.Init(game, renderDevice);
        }

        /// <summary>
        /// Renders the light element.
        /// </summary>
        /// <param name="game">The game instance.</param>
        /// <param name="renderDevice">The render device.</param>
        public override void OnRender(Game game, IRenderDevice renderDevice)
        {
            renderDevice.DrawGameElement(this);
            base.OnRender(game, renderDevice);
        }

        /// <summary>
        /// Destroys the light element.
        /// </summary>
        /// <param name="game">The game instance.</param>
        public override void OnDestroy(Game game)
        {
            base.OnDestroy(game);
            game.RenderDevice.DisposeElement(this);
        }

        /// <summary>
        /// Gets the color of the light.
        /// </summary>
        /// <returns>The color of the light as a vector.</returns>
        public Vec3 GetColor()
        {
            var colors = Utils.ConvertColor(LightColor);
            return new Vec3(colors[0], colors[1], colors[2]);
        }

        /// <summary>
        /// Gets the vertices for the light shape.
        /// </summary>
        /// <returns>An array of vertices defining the shape of the light.</returns>
        public static float[] GetVericies()
        {
            float[] vericies =
            {
                -0.5f, -0.5f, 0.0f,
                -0.5f, 0.5f, 0.0f,
                0.5f, 0.5f, 0.0f,

                -0.5f, -0.5f, 0.0f,
                0.5f, 0.5f, 0.0f,
                0.5f, -0.5f, 0.0f,
            };
            return vericies;
        }

        /// <summary>
        /// Gets the texture coordinates for the light shape.
        /// </summary>
        /// <returns>An array of texture coordinates corresponding to the light shape.</returns>
        public static float[] GetTexCoords()
        {
            float[] texcoords =
            {
                 0.0f, 0.0f,
                0.0f, 1.0f,
                1.0f, 1.0f,

                0.0f, 0.0f,
                1.0f, 1.0f,
                1.0f, 0.0f
            };
            return texcoords;
        }

        /// <summary>
        /// Tests the Light2D functionality.
        /// </summary>
        public void TestLight2D()
        {
            Console.WriteLine("Test");
        }
    }
}

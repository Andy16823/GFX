using Genesis.Graphics;
using Genesis.Graphics.Shaders.OpenGL;
using Genesis.Graphics.Shapes;
using Genesis.Math;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Core.GameElements
{
    /// <summary>
    /// Represents a cube-shaped game element with customizable appearance.
    /// </summary>
    public class Qube : GameElement
    {
        /// <summary>
        /// Gets or sets the shader program associated with this cube.
        /// </summary>
        public ShaderProgram Shader { get; set; }

        /// <summary>
        /// Gets or sets the material for this cube
        /// </summary>
        public Material Material { get; set; }

        /// <summary>
        /// Gets or sets the cube shape definition.
        /// </summary>
        public QubeShape Shape { get; set; }

        /// <summary>
        /// Gets or sets the color of the cube.
        /// </summary>
        public Color Color {
            get => Material.DiffuseColor;
            set => Material.DiffuseColor = value;
        }

        /// <summary>
        /// Initializes a new instance of the cube class with default settings.
        /// </summary>
        /// <param name="name">The name of the cube.</param>
        /// <param name="location">The initial location of the cube.</param>
        public Qube(String name, Vec3 location)
        {
            this.Name = name;
            this.Location = location;
            this.Size = new Vec3(1f);
            this.Rotation = Vec3.Zero();
            this.Shape = new QubeShape();
            this.Shader = new DiffuseSolidShader();
            this.Material = new Material();
        }

        /// <summary>
        /// Initializes a new instance of the cube class with specified size.
        /// </summary>
        /// <param name="name">The name of the cube.</param>
        /// <param name="location">The initial location of the cube.</param>
        /// <param name="size">The size of the cube.</param>
        public Qube(String Name, Vec3 location, Vec3 size)
        {
            this.Name = Name;
            this.Location = location;
            this.Size = size;
            this.Rotation = Vec3.Zero();
            this.Shape = new QubeShape();
            this.Shader = new DiffuseSolidShader();
            this.Material = new Material();
        }

        /// <summary>
        /// Initializes a new instance of the cube class with specified size and rotation.
        /// </summary>
        /// <param name="name">The name of the cube.</param>
        /// <param name="location">The initial location of the cube.</param>
        /// <param name="size">The size of the cube.</param>
        /// <param name="rotation">The initial rotation of the cube.</param>
        public Qube(String name, Vec3 location, Vec3 size, Vec3 rotation)
        {
            this.Name = name;
            this.Location = location;
            this.Size = size;
            this.Rotation = rotation;
            this.Shape = new QubeShape();
            this.Shader = new DiffuseSolidShader();
            this.Material = new Material();
        }

        /// <summary>
        /// Gets an array of color values based on the specified color.
        /// </summary>
        /// <param name="color">The color for the cube faces.</param>
        /// <returns>An array of color values for the cube faces.</returns>
        public static float[] GetColors(Color color)
        {
            float r = (float)color.R / 255;
            float g = (float)color.G / 255;
            float b = (float)color.B / 255;

            float[] colorArray =
            {
                // Front
                r, g, b,
                r, g, b,
                r, g, b,
                r, g, b,
                r, g, b,
                r, g, b,

                //Back
                r, g, b,
                r, g, b,
                r, g, b,
                r, g, b,
                r, g, b,
                r, g, b,

                //Right
                r, g, b,
                r, g, b,
                r, g, b,
                r, g, b,
                r, g, b,
                r, g, b,

                //Left
                r, g, b,
                r, g, b,
                r, g, b,
                r, g, b,
                r, g, b,
                r, g, b,

                //Top
                r, g, b,
                r, g, b,
                r, g, b,
                r, g, b,
                r, g, b,
                r, g, b,

                //Bottom
                r, g, b,
                r, g, b,
                r, g, b,
                r, g, b,
                r, g, b,
                r, g, b
            };
            return colorArray;
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
            renderDevice.DrawGameElement(this);
        }

        public override void OnDestroy(Game game)
        {
            base.OnDestroy(game);
            game.RenderDevice.DisposeElement(this);
        }
    }
}

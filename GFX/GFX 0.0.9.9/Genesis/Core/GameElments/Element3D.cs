using BulletSharp;
using BulletSharp.SoftBody;
using Genesis.Graphics;
using Genesis.Math;
using GlmSharp;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BulletSharp.Math;
using System.Xml.Linq;

namespace Genesis.Core.GameElements
{
    /// <summary>
    /// Represents a 3D element in the game world, such as a 3D model with shaders.
    /// </summary>
    public class Element3D : GameElement
    {
        /// <summary>
        /// Gets or sets the shader program associated with this 3D element.
        /// </summary>
        public ShaderProgram Shader { get; set; }

        /// <summary>
        /// Gets or sets the 3D model associated with this element.
        /// </summary>
        public OpenObjectLoader.Model Model { get; set; }

        /// <summary>
        /// Initializes a new instance of the Element3D class with specified parameters.
        /// </summary>
        /// <param name="name">The name of the 3D element.</param>
        /// <param name="path">The file path to the 3D model.</param>
        /// <param name="location">The initial location of the 3D element.</param>
        /// <param name="rotation">The initial rotation of the 3D element.</param>
        /// <param name="scale">The initial scale of the 3D element.</param>
        public Element3D(String name, String path, Vec3 location, Vec3 rotation, Vec3 scale)
        {
            this.Name = name;
            this.Location = location;
            this.Rotation = rotation;
            this.Size = scale;
            this.Model = new OpenObjectLoader.WavefrontLoader().LoadModel(path);
            this.Model.Propertys.Add("path", new System.IO.FileInfo(path).DirectoryName);
        }

        /// <summary>
        /// Called when the game is being updated. Override to provide custom update logic.
        /// </summary>
        /// <param name="game">The game instance.</param>
        /// <param name="renderDevice">The render device used for rendering.</param>
        public override void OnUpdate(Game game, IRenderDevice renderDevice)
        {
            base.OnUpdate(game, renderDevice);
        }

        /// <summary>
        /// Called when the game is being initialized. Override to provide custom initialization logic.
        /// </summary>
        /// <param name="game">The game instance.</param>
        /// <param name="renderDevice">The render device used for rendering.</param>
        public override void Init(Game game, IRenderDevice renderDevice)
        {
            base.Init(game, renderDevice);
            renderDevice.InitElement3D(this);
        }

        /// <summary>
        /// Called when the game is being rendered. Override to provide custom rendering logic.
        /// </summary>
        /// <param name="game">The game instance.</param>
        /// <param name="renderDevice">The render device used for rendering.</param>
        public override void OnRender(Game game, IRenderDevice renderDevice)
        {
            base.OnRender(game, renderDevice);
            renderDevice.DrawElement3D(this);
        }

        /// <summary>
        /// Called when the game element is being destroyed. Override to provide custom cleanup logic.
        /// </summary>
        /// <param name="game">The game instance.</param>
        public override void OnDestroy(Game game)
        {
            base.OnDestroy(game);
            game.RenderDevice.DisposeElement3D(this);
        }

    }
}

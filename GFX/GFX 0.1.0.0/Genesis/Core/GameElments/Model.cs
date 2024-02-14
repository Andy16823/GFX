using Genesis.Graphics;
using Genesis.Math;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Core.GameElements
{
    /// <summary>
    /// Represents a game element with a 3D model mesh, now deprecated in favor of the Element3D class.
    /// </summary>
    public class Model : GameElement
    {
        /// <summary>
        /// Gets or sets the mesh associated with this model.
        /// </summary>
        public Mesh Mesh { get; set; }

        /// <summary>
        /// Initializes a new instance of the Model class with the specified mesh.
        /// </summary>
        /// <param name="mesh">The mesh associated with the model.</param>
        public Model(Mesh mesh) { 
            this.Mesh = mesh;
        }

        /// <summary>
        /// Initializes the game element and its associated mesh.
        /// </summary>
        /// <param name="game">The game instance.</param>
        /// <param name="renderDevice">The render device used for rendering.</param>
        public override void Init(Game game, IRenderDevice renderDevice)
        {
            base.Init(game, renderDevice);
            this.Mesh.InitMesh(renderDevice);
        }

        /// <summary>
        /// Renders the game element with its associated mesh.
        /// </summary>
        /// <param name="game">The game instance.</param>
        /// <param name="renderDevice">The render device used for rendering.</param>
        public override void OnRender(Game game, IRenderDevice renderDevice)
        {
            base.OnRender(game, renderDevice);
            float tX = Location.X;
            float tY = Location.Y;

            renderDevice.ModelViewMatrix();
            renderDevice.PushMatrix();
            renderDevice.Translate(tX, tY, 0.0f);
            //renderDevice.Rotate(0f, new Vec3(0f, 0f, 200f));
            //renderDevice.Translate(-tX, -tY, 0.0f);
            renderDevice.DrawMesh(this.Mesh, Color.White);
            renderDevice.PopMatrix();
        }
    }
}

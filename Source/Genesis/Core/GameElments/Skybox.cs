using Genesis.Core.GameElements;
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
    /// Represents a skybox element in a 3D environment.
    /// </summary>
    public class Skybox : Element3D
    {
        /// <summary>
        /// Initializes a new instance of the Skybox class with specified name, texture path, location, rotation, and scale.
        /// </summary>
        /// <param name="name">The name of the skybox.</param>
        /// <param name="path">The path to the texture for the skybox.</param>
        /// <param name="location">The initial location of the skybox.</param>
        /// <param name="rotation">The initial rotation of the skybox.</param>
        /// <param name="scale">The initial scale of the skybox.</param>
        public Skybox(string name, string path, Vec3 location, Vec3 rotation, Vec3 scale) 
            : base(name, path, location, rotation, scale)
        {

        }

        /// <summary>
        /// Updates the skybox position based on the selected scene's camera location.
        /// </summary>
        /// <param name="game">The game instance.</param>
        /// <param name="renderDevice">The render device used for rendering.</param>
        public override void OnUpdate(Game game, IRenderDevice renderDevice)
        {
            base.OnUpdate(game, renderDevice);
            if(game.SelectedScene != null) { 
                if(game.SelectedScene.Camera != null)
                {
                    this.Location = game.SelectedScene.Camera.Location;
                }
            }
        }

    }
}

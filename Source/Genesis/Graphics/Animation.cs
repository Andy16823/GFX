using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Graphics
{
    /// <summary>
    /// Represents an animation definition with details such as name, starting cell, row, and number of frames.
    /// </summary>
    public class Animation
    {
        /// <summary>
        /// Gets or sets the name of the animation.
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// Gets or sets the starting cell index of the animation.
        /// </summary>
        public int Cell { get; set; }

        /// <summary>
        /// Gets or sets the row index in the animation sheet.
        /// </summary>
        public int Row { get; set; }

        /// <summary>
        /// Gets or sets the number of frames in the animation.
        /// </summary>
        public int Frames { get; set; }

        /// <summary>
        /// Initializes a new instance of the Animation class.
        /// </summary>
        public Animation()
        {

        }

        /// <summary>
        /// Initializes a new instance of the Animation class with specified parameters.
        /// </summary>
        /// <param name="name">The name of the animation.</param>
        /// <param name="cell">The starting cell index of the animation.</param>
        /// <param name="row">The row index in the animation sheet.</param>
        /// <param name="frames">The number of frames in the animation.</param>
        public Animation(String name, int cell, int row, int frames)
        {
            this.Name = name;
            this.Cell= cell;
            this.Row = row; 
            this.Frames = frames;   
        }
    }
}

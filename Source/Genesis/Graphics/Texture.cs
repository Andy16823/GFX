using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Graphics
{
    /// <summary>
    /// Represents a texture used in graphics rendering.
    /// </summary>
    public class Texture
    {
        /// <summary>
        /// Gets or sets the name of the texture.
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// Gets or sets the render ID associated with the texture.
        /// </summary>
        public int RenderID { get; set; }

        /// <summary>
        /// Gets or sets the Bitmap representing the texture.
        /// </summary>
        public Bitmap Bitnmap { get; set; }

        /// <summary>
        /// Constructor for the Texture class that initializes the texture with a Bitmap.
        /// </summary>
        /// <param name="bitmap">The Bitmap representing the texture.</param>
        public Texture(Bitmap bitmap)
        {
            this.Bitnmap = bitmap;
        }

        /// <summary>
        /// Constructor for the Texture class that initializes the texture with a name and a Bitmap.
        /// </summary>
        /// <param name="name">The name of the texture.</param>
        /// <param name="bitmap">The Bitmap representing the texture.</param>
        public Texture(string name, Bitmap bitnmap)
        {
            Name = name;
            Bitnmap = bitnmap;
        }

        /// <summary>
        /// Constructor for the Texture class that initializes the texture with a render ID.
        /// </summary>
        /// <param name="renderID">The render ID associated with the texture.</param>
        public Texture(int RenderID)
        {
            this.RenderID = RenderID;  
        }
    }
}

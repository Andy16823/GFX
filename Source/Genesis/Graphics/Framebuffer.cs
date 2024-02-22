using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Graphics
{
    /// <summary>
    /// Represents a framebuffer object in graphics rendering.
    /// </summary>
    public class Framebuffer
    {
        /// <summary>
        /// Gets or sets the ID of the framebuffer.
        /// </summary>
        public int FramebufferID { get; set; }

        /// <summary>
        /// Gets or sets the ID of the render buffer associated with the framebuffer.
        /// </summary>
        public int RenderBuffer { get; set; }

        /// <summary>
        /// Gets or sets the ID of the texture associated with the framebuffer.
        /// </summary>
        public int Texture { get; set; }

        /// <summary>
        /// Converts the framebuffer to a texture.
        /// </summary>
        /// <returns>The converted texture.</returns>
        public Texture ToTexture()
        {
            var texture = new Texture(this.Texture);
            return texture;
        }
    }
}

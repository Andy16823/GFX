using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Graphics
{
    public class Framebuffer
    {
        public int FramebufferID { get; set; }
        public int RenderBuffer { get; set; }
        public int Texture { get; set; }

        public Texture ToTexture()
        {
            var texture = new Texture(this.Texture);
            return texture;
        }
    }
}

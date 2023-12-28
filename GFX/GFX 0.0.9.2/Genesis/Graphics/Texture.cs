using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Graphics
{
    public class Texture
    {
        public String Name { get; set; }
        public int RenderID { get; set; }
        public Bitmap Bitnmap { get; set; }

        public Texture(Bitmap bitmap)
        {
            this.Bitnmap = bitmap;
        }

        public Texture(string name, Bitmap bitnmap)
        {
            Name = name;
            Bitnmap = bitnmap;
        }

        public Texture(int RenderID)
        {
            this.RenderID = RenderID;  
        }
    }
}

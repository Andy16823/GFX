using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Graphics
{
    public struct MaterialBuffer
    {
        public bool HasData;
        public float[] Verticies;
        public float[] Normals;
        public float[] Texcords;
    }

    public class Material
    {
        public Color DiffuseColor { get; set; }
        public String DiffuseTexture { get; set; }
        public String NormalTexture { get; set; }
        public Dictionary<String, Object> Propeterys { get; set; }

        public Material()
        {
            this.Propeterys = new Dictionary<String, Object>();
        }
    }
}

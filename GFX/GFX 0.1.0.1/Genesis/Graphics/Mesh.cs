using Genesis.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Graphics
{
    public class Mesh
    {
        public int MaterialIndex { get; set; }
        public List<int> Indicies { get; set; }
        public List<float> Faces { get; set; }
        public List<float> Vericies { get; set; }
        public List<float> Normals { get; set; }
        public List<float> TextureCords { get; set; }
        public Dictionary<String, Object> Propeterys { get; set; }

        public Mesh()
        {
            this.Propeterys = new Dictionary<string, object>();
            this.Faces = new List<float>();
            this.Vericies = new List<float>();
            this.Normals = new List<float>();
            this.TextureCords = new List<float>();
            this.Indicies = new List<int>();
        }
    }
}

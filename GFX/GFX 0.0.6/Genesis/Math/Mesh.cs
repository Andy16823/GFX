using Genesis.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Math
{
    public class Mesh
    {
        public List<Face> Faces { get; set; }
        public Dictionary<String, Object> Propertys { get; set; }

        public Mesh()
        {
            this.Propertys = new Dictionary<string, object>();
            this.Faces = new List<Face>();
        }

        public void InitMesh(IRenderDevice renderer)
        {
            foreach (var face in this.Faces)
            {
                face.InitFace(renderer);
            }
        }
    }
}

using Genesis.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Graphics
{
    public class Face
    {
        public Texture Texture { get; set; }
        public List<Vec3> Vertices { get; set; }
        public List<Vec3> TexCords { get; set; } 
        public Dictionary<String, Object> Propertys { get; set; }

        public Face()
        {
            this.Propertys = new Dictionary<String, Object>();
            this.Vertices = new List<Vec3>();
            this.TexCords = new List<Vec3>();
        }

        public Face(Texture texture)
        {
            this.Propertys = new Dictionary<String, Object>();
            this.Vertices = new List<Vec3>();
            this.TexCords = new List<Vec3>();
            this.Texture = texture;
        }

        public Face(Vec3[] vecs)
        {
            this.Propertys = new Dictionary<String, Object>();
            this.TexCords = new List<Vec3>();
            this.Vertices = vecs.ToList<Vec3>();
        }

        public Face(Vec3[] vecs, Vec3[] texCords, Texture texture)
        {
            this.Propertys = new Dictionary<String, Object>();
            this.Vertices = vecs.ToList<Vec3>();
            this.TexCords = texCords.ToList<Vec3>();
            this.Texture = texture;
        }

        public void InitFace(IRenderDevice renderer)
        {
            if(Texture != null)
            {
                if(Texture.RenderID == 0)
                {
                    renderer.LoadTexture(Texture);
                }
            }
        }
    }
}

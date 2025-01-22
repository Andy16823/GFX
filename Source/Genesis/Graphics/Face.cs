using Genesis.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Graphics
{
    /// <summary>
    /// Represents a face in a 3D model with vertices, texture coordinates, and optional properties.
    /// </summary>
    public class Face
    {
        /// <summary>
        /// Gets or sets the texture associated with the face.
        /// </summary>
        public Texture Texture { get; set; }

        /// <summary>
        /// Gets or sets the list of vertices defining the face.
        /// </summary>
        public List<Vec3> Vertices { get; set; }

        /// <summary>
        /// Gets or sets the list of texture coordinates associated with the face vertices.
        /// </summary>
        public List<Vec3> TexCords { get; set; }

        /// <summary>
        /// Gets or sets additional properties associated with the face.
        /// </summary>
        public Dictionary<String, Object> Propertys { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Face"/> class.
        /// </summary>
        public Face()
        {
            this.Propertys = new Dictionary<String, Object>();
            this.Vertices = new List<Vec3>();
            this.TexCords = new List<Vec3>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Face"/> class with a specified texture.
        /// </summary>
        /// <param name="texture">The texture associated with the face.</param>
        public Face(Texture texture)
        {
            this.Propertys = new Dictionary<String, Object>();
            this.Vertices = new List<Vec3>();
            this.TexCords = new List<Vec3>();
            this.Texture = texture;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Face"/> class with specified vertices.
        /// </summary>
        /// <param name="vecs">An array of vertices defining the face.</param>
        public Face(Vec3[] vecs)
        {
            this.Propertys = new Dictionary<String, Object>();
            this.TexCords = new List<Vec3>();
            this.Vertices = vecs.ToList<Vec3>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Face"/> class with specified vertices, texture coordinates, and a texture.
        /// </summary>
        /// <param name="vecs">An array of vertices defining the face.</param>
        /// <param name="texCords">An array of texture coordinates associated with the face vertices.</param>
        /// <param name="texture">The texture associated with the face.</param>
        public Face(Vec3[] vecs, Vec3[] texCords, Texture texture)
        {
            this.Propertys = new Dictionary<String, Object>();
            this.Vertices = vecs.ToList<Vec3>();
            this.TexCords = texCords.ToList<Vec3>();
            this.Texture = texture;
        }

        /// <summary>
        /// Initializes the face by loading its associated texture using the specified renderer.
        /// </summary>
        /// <param name="renderer">The renderer responsible for loading the texture.</param>
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

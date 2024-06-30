using Genesis.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Graphics
{
    /// <summary>
    /// Class representing a 3D mesh, including its geometry and material information.
    /// </summary>
    public class Mesh
    {
        /// <summary>
        /// Index of the material associated with the mesh.
        /// </summary>
        public Material Material { get; set; }

        /// <summary>
        /// List of indices defining the mesh faces.
        /// </summary>
        public List<int> Indicies { get; set; }

        /// <summary>
        /// List of data representing the faces of the mesh.
        /// </summary>
        public List<float> Faces { get; set; }

        /// <summary>
        /// List of vertex coordinates for the mesh.
        /// </summary>
        public List<float> Vericies { get; set; }

        /// <summary>
        /// List of normal vectors for the mesh vertices.
        /// </summary>
        public List<float> Normals { get; set; }

        /// <summary>
        /// List of texture coordinates for the mesh vertices.
        /// </summary>
        public List<float> TextureCords { get; set; }

        /// <summary>
        /// A collection of user-defined properties associated with the mesh.
        /// </summary>
        public Dictionary<String, Object> Propeterys { get; set; }

        /// <summary>
        /// Constructor for the Mesh class. Initializes properties and collections.
        /// </summary>
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

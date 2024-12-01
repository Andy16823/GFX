using GlmSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Graphics.Animation3D
{
    /// <summary>
    /// Represents a vertex in a 3D model mesh.
    /// </summary>
    public struct vertex
    {
        public vec3 position;
        public vec2 textcoords;
        public ivec4 BoneIDs;
        public vec4 BoneWeights;
    }

    /// <summary>
    /// Contains information about a bone.
    /// </summary>
    public struct boneinfo
    {
        public int id;
        public mat4 offset;
    }

    /// <summary>
    /// Represents a mesh of a 3D model.
    /// </summary>
    public class ModelMesh
    {
        /// <summary>
        /// Maximum number of bones influencing a vertex.
        /// </summary>
        public const int MaxBoneInfluence = 4;

        /// <summary>
        /// Name of the mesh.
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// List of vertices in the mesh.
        /// </summary>
        public List<vertex> Vertices { get; set; }

        /// <summary>
        /// List of indices defining the geometry of the mesh.
        /// </summary>
        public List<int> Indices { get; set; }

        /// <summary>
        /// Material applied to the mesh.
        /// </summary>
        public Material Material { get; set; }

        /// <summary>
        /// Additional properties of the mesh.
        /// </summary>
        public Dictionary<String, object> Propertys { get; set; }

        /// <summary>
        /// Initializes a new instance of the ModelMesh class.
        /// </summary>
        public ModelMesh()
        {
            Propertys = new Dictionary<string, object>();
            Vertices = new List<vertex>();
            Indices = new List<int>();
        }
    }
}

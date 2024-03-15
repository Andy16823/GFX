using GlmSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Graphics.Animation3D
{
    public struct vertex
    {
        public vec3 position;
        public vec2 textcoords;
        public ivec4 BoneIDs;
        public vec4 BoneWeights;
    }

    public struct boneinfo
    {
        public int id;
        public mat4 offset;
    }

    public class ModelMesh
    {
        public const int MaxBoneInfluence = 4;

        public String Name { get; set; }
        public List<vertex> Vertices { get; set; }
        public List<int> Indices { get; set; }
        public Material Material { get; set; }
        public Dictionary<String, object> Propertys { get; set; }

        public ModelMesh()
        {
            Propertys = new Dictionary<string, object>();
            Vertices = new List<vertex>();
            Indices = new List<int>();
        }
    }
}

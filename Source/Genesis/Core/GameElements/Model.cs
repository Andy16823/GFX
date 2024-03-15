using Genesis.Graphics.Animation3D;
using Genesis.Graphics;
using GlmSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Genesis.Math;
using Genesis.Graphics.Shaders.OpenGL;

namespace Genesis.Core.GameElements
{
    public class Model : GameElement
    {
        public ShaderProgram Shader { get; set; } = new AnimatedModelShader();
        public List<Graphics.Material> Materials { get; set; }
        public Dictionary<String, boneinfo> BoneInfoMap { get; set; }
        public List<Graphics.Animation3D.Animation> Animations { get; set; }
        public int BoneCounter { get; set; }
        public List<ModelMesh> Meshes { get; set; }
        public String FileDirectory { get; set; }
        public String FileName { get; set; }
        public float AnimationSpeed { get; set; } = 0.01f;
        public Animator Animator { get; set; }


        public override void Init(Game game, IRenderDevice renderDevice)
        {
            base.Init(game, renderDevice);
        }

        public override void OnRender(Game game, IRenderDevice renderDevice)
        {
            renderDevice.DrawGameElement(this);
            base.OnRender(game, renderDevice);
        }

        public override void OnUpdate(Game game, IRenderDevice renderDevice)
        {
            base.OnUpdate(game, renderDevice);
            Animator.UpdateAnimation(AnimationSpeed);
        }

        public override void OnDestroy(Game game)
        {
            game.RenderDevice.DisposeElement(this);
            base.OnDestroy(game);
        }

        public Model(String name, Vec3 location, String filename)
        {
            this.Name = name;
            this.Location = location;
            this.Rotation = new Vec3(0f);
            this.Size = new Vec3(1f);
            BoneInfoMap = new Dictionary<string, boneinfo>();
            FileInfo fileInfo = new FileInfo(filename);
            this.FileDirectory = fileInfo.DirectoryName;
            this.FileName = fileInfo.Name;

            Assimp.Scene model;
            Assimp.AssimpContext importer = new Assimp.AssimpContext();
            importer.SetConfig(new Assimp.Configs.NormalSmoothingAngleConfig(66.0f));
            model = importer.ImportFile(filename, Assimp.PostProcessSteps.Triangulate | Assimp.PostProcessSteps.CalculateTangentSpace | Assimp.PostProcessSteps.JoinIdenticalVertices);

            this.ExtractMaterials(model);
            this.ExtractMeshes(model);
            this.ExtractAnimations(model);
        }

        public void ExtractMaterials(Assimp.Scene scene)
        {
            this.Materials = new List<Graphics.Material>();
            foreach (var aiMaterial in scene.Materials)
            {
                var material = new Graphics.Material();
                material.Name = aiMaterial.Name;
                material.DiffuseTexture = this.FileDirectory + "\\" + aiMaterial.TextureDiffuse.FilePath;
                this.Materials.Add(material);
            }
        }

        public void ExtractMeshes(Assimp.Scene scene)
        {
            this.Meshes = new List<ModelMesh>();
            foreach (var aiMesh in scene.Meshes)
            {
                var mesh = new ModelMesh();
                mesh.Name = aiMesh.Name;
                mesh.Material = this.Materials[aiMesh.MaterialIndex];
                mesh.Indices.AddRange(aiMesh.GetIndices());
                for (int i = 0; i < aiMesh.VertexCount; i++)
                {
                    var vertex = new vertex();
                    vertex.position = new GlmSharp.vec3(aiMesh.Vertices[i].X, aiMesh.Vertices[i].Y, aiMesh.Vertices[i].Z);
                    SetVertexBoneDataToDefault(ref vertex);
                    if (aiMesh.TextureCoordinateChannels[0] != null)
                    {
                        vertex.textcoords = new vec2(aiMesh.TextureCoordinateChannels[0][i].X, aiMesh.TextureCoordinateChannels[0][i].Y);
                    }
                    else
                    {
                        vertex.textcoords = new vec2(0f);
                    }
                    mesh.Vertices.Add(vertex);
                }
                ExtractBoneWeightForVertices(aiMesh, scene, mesh);
                this.Meshes.Add(mesh);
            }
        }

        public void ExtractAnimations(Assimp.Scene scene)
        {
            Animations = new List<Graphics.Animation3D.Animation>();
            for(int i = 0; i < scene.AnimationCount; i++)
            {
                var animation = new Graphics.Animation3D.Animation(scene, this, i);
                this.Animations.Add(animation);
            }
            
            this.Animator = new Graphics.Animation3D.Animator(this.Animations[0]);
        }

        private void SetVertexBoneDataToDefault(ref vertex vertex)
        {
            vertex.BoneIDs = new ivec4(-1);
            vertex.BoneWeights = new vec4(0.0f);
        }

        private void SetVertexBoneData(ref vertex v, int boneId, float weight)
        {
            for (int i = 0; i < 4; ++i)
            {
                if (v.BoneIDs[i] < 0)
                {
                    v.BoneWeights[i] = weight;
                    v.BoneIDs[i] = boneId;
                    break;
                }
            }
        }

        private void ExtractBoneWeightForVertices(Assimp.Mesh mesh, Assimp.Scene scene, ModelMesh gmesh)
        {
            for (int boneIndex = 0; boneIndex < mesh.BoneCount; boneIndex++)
            {
                int boneId = -1;
                var boneName = mesh.Bones[boneIndex].Name;
                if (!BoneInfoMap.ContainsKey(boneName))
                {
                    var boneInfo = new boneinfo();
                    boneInfo.id = BoneCounter;
                    boneInfo.offset = Utils.ConvertToGlmMat4(mesh.Bones[boneIndex].OffsetMatrix);
                    BoneInfoMap.Add(boneName, boneInfo);
                    boneId = BoneCounter;
                    BoneCounter++;
                }
                else
                {
                    boneId = BoneInfoMap[boneName].id;
                }

                var weights = mesh.Bones[boneIndex].VertexWeights;
                var numWeights = mesh.Bones[boneIndex].VertexWeightCount;
                for (int weigthIndex = 0; weigthIndex < numWeights; weigthIndex++)
                {
                    int vertexId = weights[weigthIndex].VertexID;
                    float weight = weights[weigthIndex].Weight;
                    Debug.Assert(vertexId <= gmesh.Indices.Count);
                    var vertex = gmesh.Vertices[vertexId];
                    SetVertexBoneData(ref vertex, boneId, weight);
                    gmesh.Vertices[vertexId] = vertex;
                }
            }
        }

        public void PlayAnimation(String name)
        {
            var animation = this.FindAnimation(name);
            if(animation != null)
            {
                this.Animator.LoadAnimation(animation);
            }
        }

        public Genesis.Graphics.Animation3D.Animation FindAnimation(String name)
        {
            var animation = Animations.FirstOrDefault(a => a.Name == name);
            if (animation != null)
            {
                return animation;
            }
            return null;
        }
    }
}

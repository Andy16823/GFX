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
    /// <summary>
    /// Represents a 3D model in the game.
    /// </summary>
    public class Model : GameElement
    {
        /// <summary>
        /// Gets or sets the shader program used for rendering the model.
        /// </summary>
        public ShaderProgram Shader { get; set; } = new AnimatedModelShader();

        /// <summary>
        /// Gets or sets the list of materials used by the model.
        /// </summary>
        public List<Graphics.Material> Materials { get; set; }

        /// <summary>
        /// Gets or sets the mapping of bone names to bone information.
        /// </summary>
        public Dictionary<String, boneinfo> BoneInfoMap { get; set; }

        /// <summary>
        /// Gets or sets the list of animations associated with the model.
        /// </summary>
        public List<Graphics.Animation3D.Animation> Animations { get; set; }

        /// <summary>
        /// Gets or sets the counter for bones in the model.
        /// </summary>
        public int BoneCounter { get; set; }

        /// <summary>
        /// Gets or sets the list of meshes composing the model.
        /// </summary>
        public List<ModelMesh> Meshes { get; set; }

        /// <summary>
        /// Gets or sets the directory of the model file.
        /// </summary>
        public String FileDirectory { get; set; }

        /// <summary>
        /// Gets or sets the name of the model file.
        /// </summary>
        public String FileName { get; set; }

        /// <summary>
        /// Gets or sets the speed of animation playback.
        /// </summary>
        public float AnimationSpeed { get; set; } = 0.01f;

        /// <summary>
        /// Gets or sets the animator responsible for controlling animations.
        /// </summary>
        public Animator Animator { get; set; }

        /// <summary>
        /// Initializes the model within the game environment.
        /// </summary>
        public override void Init(Game game, IRenderDevice renderDevice)
        {
            base.Init(game, renderDevice);
        }

        /// <summary>
        /// Renders the model within the game environment.
        /// </summary>
        public override void OnRender(Game game, IRenderDevice renderDevice)
        {
            renderDevice.DrawGameElement(this);
            base.OnRender(game, renderDevice);
        }

        /// <summary>
        /// Updates the model within the game environment.
        /// </summary>
        public override void OnUpdate(Game game, IRenderDevice renderDevice)
        {
            base.OnUpdate(game, renderDevice);
            Animator.UpdateAnimation(AnimationSpeed);
        }

        /// <summary>
        /// Cleans up resources associated with the model when destroyed.
        /// </summary>
        public override void OnDestroy(Game game)
        {
            game.RenderDevice.DisposeElement(this);
            base.OnDestroy(game);
        }

        /// <summary>
        /// Constructs a new instance of the Model class.
        /// </summary>
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

            if(model.HasAnimations)
            {
                this.ExtractAnimations(model);
            }
            else
            {
                this.Animator = new Animator();
            }
        }

        /// <summary>
        /// Extracts materials information from the imported model.
        /// </summary>
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

        /// <summary>
        /// Extracts mesh data from the imported model.
        /// </summary>
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

        /// <summary>
        /// Extracts animations from the imported model.
        /// </summary>
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

        /// <summary>
        /// Sets default bone data for a vertex.
        /// </summary>
        private void SetVertexBoneDataToDefault(ref vertex vertex)
        {
            vertex.BoneIDs = new ivec4(-1);
            vertex.BoneWeights = new vec4(0.0f);
        }

        /// <summary>
        /// Sets bone data for a vertex.
        /// </summary>
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

        /// <summary>
        /// Extracts bone weight information for vertices of a mesh.
        /// </summary>
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

        /// <summary>
        /// Plays the specified animation on the model.
        /// </summary>
        public void PlayAnimation(String name)
        {
            var animation = this.FindAnimation(name);
            if(animation != null)
            {
                this.Animator.LoadAnimation(animation);
            }
        }

        /// <summary>
        /// Finds an animation with the specified name.
        /// </summary>
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

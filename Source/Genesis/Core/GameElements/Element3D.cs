using BulletSharp;
using BulletSharp.SoftBody;
using Genesis.Graphics;
using Genesis.Math;
using GlmSharp;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BulletSharp.Math;
using System.Xml.Linq;
using System.Drawing;
using System.Reflection;
using Assimp.Unmanaged;
using Genesis.Graphics.Animation3D;

namespace Genesis.Core.GameElements
{
    /// <summary>
    /// Represents a 3D element in the game world, such as a 3D model with shaders.
    /// </summary>
    public class Element3D : GameElement
    {
        /// <summary>
        /// Gets or sets the shader program associated with this 3D element.
        /// </summary>
        public ShaderProgram Shader { get; set; }

        /// <summary>
        /// Gets or sets the meshes from the model
        /// </summary>
        public List<Mesh> Meshes { get; set; }

        /// <summary>
        /// Gets or sets the materials from the model
        /// </summary>
        public List<Genesis.Graphics.Material> Materials { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the object has transparency.
        /// </summary>
        public bool HasTransparancy { get; set; }

        /// <summary>
        /// Initializes a new instance of the Element3D class with specified parameters.
        /// </summary>
        /// <param name="name">The name of the 3D element.</param>
        /// <param name="path">The file path to the 3D model.</param>
        /// <param name="location">The initial location of the 3D element.</param>
        /// <param name="rotation">The initial rotation of the 3D element.</param>
        /// <param name="scale">The initial scale of the 3D element.</param>
        public Element3D(String name, String path, Vec3 location, Vec3 rotation, Vec3 scale)
        {
            this.Name = name;
            this.Location = location;
            this.Rotation = rotation;
            this.Size = scale;
            this.Materials = new List<Graphics.Material>();
            this.Meshes = new List<Mesh>();

            var directory = new System.IO.FileInfo(path).DirectoryName;

            Assimp.AssimpContext importer = new Assimp.AssimpContext();
            //importer.SetConfig(new Assimp.Configs.NormalSmoothingAngleConfig(66.0f));
            var model = importer.ImportFile(path, Assimp.PostProcessPreset.TargetRealTimeQuality | Assimp.PostProcessSteps.PreTransformVertices);

            foreach (var material in model.Materials)
            {
                var gMaterial = new Genesis.Graphics.Material();
                gMaterial.Opacity = material.Opacity;
                gMaterial.DiffuseColor = Utils.ConvertDrawingColor(material.ColorDiffuse.A, material.ColorDiffuse.R, material.ColorDiffuse.G, material.ColorDiffuse.B);
                gMaterial.DiffuseTexture = material.TextureDiffuse.FilePath;
                if(material.HasTextureNormal)
                {
                    gMaterial.NormalTexture = material.TextureNormal.FilePath;
                }
                else if(material.HasTextureHeight)
                {
                    gMaterial.NormalTexture = material.TextureHeight.FilePath;
                }
                this.Materials.Add(gMaterial);
            }

            foreach (var mesh in model.Meshes)
            {
                var gMesh = new Genesis.Graphics.Mesh();
                gMesh.Name = mesh.Name;
                gMesh.Material = this.Materials[mesh.MaterialIndex];
                gMesh.Indicies.AddRange(mesh.GetIndices());
                foreach (var face in mesh.Faces)
                {
                    foreach(var faceindices in face.Indices)
                    {
                        gMesh.Faces.Add(mesh.Vertices[faceindices].X);
                        gMesh.Faces.Add(mesh.Vertices[faceindices].Y);
                        gMesh.Faces.Add(mesh.Vertices[faceindices].Z);
                    }
                }

                foreach (int index in mesh.GetIndices())
                {
                    gMesh.Vericies.Add(mesh.Vertices[index].X);
                    gMesh.Vericies.Add(mesh.Vertices[index].Y);
                    gMesh.Vericies.Add(mesh.Vertices[index].Z);

                    if (mesh.TextureCoordinateChannels[0] != null)
                    {
                        if (mesh.TextureCoordinateChannels[0].Count > index)
                        {
                            gMesh.TextureCords.Add(mesh.TextureCoordinateChannels[0][index].X);
                            gMesh.TextureCords.Add(mesh.TextureCoordinateChannels[0][index].Y);
                        }
                        else
                        {
                            gMesh.TextureCords.Add(0f);
                            gMesh.TextureCords.Add(0f);
                        }
                    }
                    else
                    {
                        gMesh.TextureCords.Add(0f);
                        gMesh.TextureCords.Add(0f);
                    }

                    gMesh.Normals.Add(mesh.Normals[index].X);
                    gMesh.Normals.Add(mesh.Normals[index].Y);
                    gMesh.Normals.Add(mesh.Normals[index].Z);
                }
                this.Meshes.Add(gMesh);
            }

            this.Materials = this.Materials.OrderBy(m => m.Opacity).ToList();
            this.Propertys.Add("path", directory);
        }

        /// <summary>
        /// Called when the game is being updated. Override to provide custom update logic.
        /// </summary>
        /// <param name="game">The game instance.</param>
        /// <param name="renderDevice">The render device used for rendering.</param>
        public override void OnUpdate(Game game, IRenderDevice renderDevice)
        {
            base.OnUpdate(game, renderDevice);
        }

        /// <summary>
        /// Called when the game is being initialized. Override to provide custom initialization logic.
        /// </summary>
        /// <param name="game">The game instance.</param>
        /// <param name="renderDevice">The render device used for rendering.</param>
        public override void Init(Game game, IRenderDevice renderDevice)
        {
            renderDevice.InitElement3D(this);
            foreach (var item in this.Behaviors)
            {
                item.OnInit(game, this);
            }
            foreach (var element in this.Children)
            {
                element.Init(game, renderDevice);
            }
            //base.Init(game, renderDevice);
        }

        /// <summary>
        /// Called when the game is being rendered. Override to provide custom rendering logic.
        /// </summary>
        /// <param name="game">The game instance.</param>
        /// <param name="renderDevice">The render device used for rendering.</param>
        public override void OnRender(Game game, IRenderDevice renderDevice)
        {
            base.OnRender(game, renderDevice);
            renderDevice.DrawElement3D(this);
        }

        /// <summary>
        /// Called when the game element is being destroyed. Override to provide custom cleanup logic.
        /// </summary>
        /// <param name="game">The game instance.</param>
        public override void OnDestroy(Game game)
        {
            base.OnDestroy(game);
            game.RenderDevice.DisposeElement3D(this);
        }

        /// <summary>
        /// Gets the shape data of the 3D element.
        /// </summary>
        /// <returns>An array representing the shape data.</returns>
        public float[] GetShape()
        {
            List<float> shape = new List<float>();
            foreach (var mesh in this.Meshes)
            {
                shape.AddRange(mesh.Faces);
            }
            return shape.ToArray();
        }

        /// <summary>
        /// Gets the material buffers for a specific material index.
        /// </summary>
        /// <param name="material">The material index.</param>
        /// <returns>A MaterialBuffer containing vertex, normal, and texture coordinate data.</returns>
        public MaterialBuffer GetMaterialBuffers(Graphics.Material material)
        {
            List<float> verticies = new List<float>();
            List<float> normals = new List<float>();
            List<float> texCords = new List<float>();

            foreach (var mesh in this.Meshes)
            {
                if(mesh.Material.Equals(material))
                {
                    verticies.AddRange(mesh.Vericies);
                    normals.AddRange(mesh.Normals);
                    texCords.AddRange(mesh.TextureCords);
                }
            }

            var buffer = new MaterialBuffer();
            if(verticies.Count > 0) {
                buffer.HasData = true;
            }
            else
            {
                buffer.HasData = false;
            }
            buffer.Verticies = verticies.ToArray();
            buffer.Normals = normals.ToArray();
            buffer.Texcords = texCords.ToArray();

            return buffer;
        }

        public RenderInstanceContainer ToRenderInstance()
        {
            return Element3D.CreateInstanceContainer(this);
        }

        public static RenderInstanceContainer CreateInstanceContainer(Element3D element)
        {
            var renderInstance = new RenderInstanceContainer();
            foreach (var material in element.Materials)
            {
                var buffers = element.GetMaterialBuffers(material);
                InstancedMesh mesh = new InstancedMesh();
                mesh.Vertices = buffers.Verticies;
                mesh.VertexColors = Utils.CreateVertexColors(mesh.Vertices.Length / 3, Color.White);
                mesh.TextureCords = buffers.Texcords;
                mesh.Normals = buffers.Normals;
                mesh.Material = material;
                material.Propeterys.Add("Path", element.Propertys["path"]);

                renderInstance.Meshes.Add(mesh);
            }
            return renderInstance;
        }
    }
}

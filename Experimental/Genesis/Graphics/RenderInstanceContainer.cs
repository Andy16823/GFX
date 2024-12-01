using Genesis.Core;
using Genesis.Math;
using GlmSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Graphics
{
    /// <summary>
    /// Represents an instanced mesh containing material, vertex data, and other properties.
    /// </summary>
    public class InstancedMesh
    {
        /// <summary>
        /// Gets or sets the material of the mesh.
        /// </summary>
        public Material Material { get; set; }

        /// <summary>
        /// Gets or sets the vertex data of the mesh.
        /// </summary>
        public float[] Vertices { get; set; }

        /// <summary>
        /// Gets or sets the texture coordinates of the mesh.
        /// </summary>
        public float[] TextureCords { get; set; }

        /// <summary>
        /// Gets or sets the vertex color data of the mesh.
        /// </summary>
        public float[] VertexColors { get; set; }

        /// <summary>
        /// Gets or sets the normals of the mesh.
        /// </summary>
        public float[] Normals { get; set; }

        /// <summary>
        /// Gets or sets additional properties for the mesh.
        /// </summary>
        public Dictionary<String, Object> Propertys { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="InstancedMesh"/> class.
        /// </summary>
        public InstancedMesh()
        {
            this.Propertys = new Dictionary<string, object>();
        }
    }

    /// <summary>
    /// Represents a container for managing and rendering instanced meshes.
    /// </summary>
    public class RenderInstanceContainer : GameElement
    {
        /// <summary>
        /// Gets or sets the collection of instanced meshes.
        /// </summary>
        public List<InstancedMesh> Meshes { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the instances need to be updated.
        /// </summary>
        public bool UpdateInstances { get; set; } = false;

        /// <summary>
        /// Gets or sets the shader program used for rendering the instances.
        /// </summary>
        public ShaderProgram Shader { get; set; }

        private IRenderDevice renderer;

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderInstanceContainer"/> class with a specified shader.
        /// </summary>
        /// <param name="shader">The shader program to use for rendering.</param>
        public RenderInstanceContainer(ShaderProgram shader)
        {
            this.Meshes = new List<InstancedMesh>();
            Shader = shader;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderInstanceContainer"/> class with a specified mesh and shader.
        /// </summary>
        /// <param name="mesh">The instanced mesh to add.</param>
        /// <param name="shader">The shader program to use for rendering.</param>
        public RenderInstanceContainer(InstancedMesh mesh, ShaderProgram shader) : base()
        {
            this.Meshes = new List<InstancedMesh>();
            this.Meshes.Add(mesh);
            this.Shader = shader;
        }

        /// <summary>
        /// Adds a new instance to the container with specified position, size, and optional rotation.
        /// </summary>
        /// <param name="position">The position of the instance.</param>
        /// <param name="size">The size of the instance.</param>
        /// <param name="rotation">The rotation of the instance. Default is no rotation.</param>
        /// <returns>The created <see cref="InstancedElement"/>.</returns>
        public InstancedElement AddInstance(Vec3 position, Vec3 size, Vec3 rotation = default)
        {
            InstancedElement instancedElement = new InstancedElement();
            instancedElement.Location = position;
            instancedElement.Size = size;
            instancedElement.Rotation = rotation;
            this.AddChild(instancedElement);
            instancedElement.InstanceID = this.Children.Count -1;
            return instancedElement;
        }

        /// <summary>
        /// Retrieves an array of transformation matrices for all instances.
        /// </summary>
        /// <returns>An array of transformation matrices as floats.</returns>
        public float[] GetMatrices()
        {
            List<float> matrices = new List<float>();
            foreach (var item in this.Children)
            {
                var instance = (InstancedElement)item;
                var matrix = instance.GetModelViewMatrix();
                matrices.AddRange(matrix.ToArray());
            }
            return matrices.ToArray();
        }

        /// <summary>
        /// Initializes the container with the specified game and render device.
        /// </summary>
        /// <param name="game">The game instance.</param>
        /// <param name="renderDevice">The render device used for rendering.</param>
        public override void Init(Game game, IRenderDevice renderDevice)
        {
            renderDevice.InitInstance(this);
            this.renderer = renderDevice;
        }

        /// <summary>
        /// Renders the container using the specified game and render device.
        /// </summary>
        /// <param name="game">The game instance.</param>
        /// <param name="renderDevice">The render device used for rendering.</param>
        public override void OnRender(Game game, IRenderDevice renderDevice)
        {
            renderDevice.DrawInstance(this);
        }

        /// <summary>
        /// Cleans up resources associated with the container when it is destroyed.
        /// </summary>
        /// <param name="game">The game instance.</param>
        public override void OnDestroy(Game game)
        {
            game.RenderDevice.DisposeInstance(this);
            foreach (var instance in this.Children)
            {
                foreach (var behavior in instance.Behaviors)
                {
                    behavior.OnDestroy(game, this);
                }
            }

            foreach (var behavior in this.Behaviors)
            {
                behavior.OnDestroy(game, this);
            }
        }

        /// <summary>
        /// Updates the instances in the container if <see cref="UpdateInstances"/> is set to true.
        /// </summary>
        /// <param name="game">The game instance.</param>
        /// <param name="renderDevice">The render device used for rendering.</param>
        public override void OnUpdate(Game game, IRenderDevice renderDevice)
        {
            if (this.UpdateInstances)
            {
                foreach (var item in this.Children)
                {
                    item.OnUpdate(game, renderDevice);
                }
            }
        }

        /// <summary>
        /// Updates the transformation matrix for a specific instance.
        /// </summary>
        /// <param name="instanceId">The ID of the instance to update.</param>
        /// <param name="data">The new transformation matrix data.</param>
        public void UpdateInstanceMatrix(int instanceId, float[] data)
        {
            if(this.renderer != null)
            {
                int offsetSize = 16 * sizeof(float);
                renderer.EditBufferSubData((int)this.Propertys["mbo"], instanceId * offsetSize, data);
            }
        }
    }
}

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
    public class InstancedMesh
    {
        public Material Material { get; set; }
        public float[] Vertices { get; set; }
        public float[] TextureCords { get; set; }
        public float[] VertexColors { get; set; }
        public float[] Normals { get; set; }
        public Dictionary<String, Object> Propertys { get; set; }

        public InstancedMesh()
        {
            this.Propertys = new Dictionary<string, object>();
        }
    }

    public class RenderInstanceContainer : GameElement
    {
        public List<InstancedMesh> Meshes { get; set; }
        public bool UpdateInstances { get; set; } = false;

        private IRenderDevice renderer;

        public RenderInstanceContainer(InstancedMesh mesh) : base()
        {
            this.Meshes = new List<InstancedMesh>();
            this.Meshes.Add(mesh);
        }

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

        public override void Init(Game game, IRenderDevice renderDevice)
        {
            renderDevice.InitInstance(this);
            this.renderer = renderDevice;
        }

        public override void OnRender(Game game, IRenderDevice renderDevice)
        {
            renderDevice.DrawInstance(this);
        }

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

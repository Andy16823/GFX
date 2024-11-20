using Genesis.Core;
using Genesis.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Graphics
{
    public class RenderInstanceContainer : GameElement
    {
        public String InstanceID { get; set; }
        public float[] Vertices { get; set; }
        public float[] TextureCords { get; set; }
        public float[] VertexColors { get; set; }
        public Material Material { get; set; }
        public int Instances { get; set; }

        public RenderInstanceContainer(String instanceId, Material material) : base()
        {
            this.InstanceID = instanceId;
            this.Material = material;
        }

        public InstancedElement AddInstance(Vec3 position, Vec3 size, Vec3 rotation = default)
        {
            InstancedElement instancedElement = new InstancedElement();
            instancedElement.Location = position;
            instancedElement.Size = size;
            instancedElement.Rotation = rotation;
            this.AddChild(instancedElement);
            instancedElement.InstanceID = this.Children.Count;
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
        }

        public override void OnRender(Game game, IRenderDevice renderDevice)
        {
            renderDevice.DrawInstance(this);
        }

    }
}

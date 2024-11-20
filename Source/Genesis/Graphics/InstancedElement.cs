using Genesis.Core;
using Genesis.Math;
using GlmSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Genesis.Core.WindowUtilities;

namespace Genesis.Graphics
{
    public class InstancedElement : GameElement
    {
        private Vec3 m_location = new Vec3();
        private Vec3 m_rotation = new Vec3();
        private Vec3 m_size = new Vec3();

        public override Vec3 Location
        {
            get => m_location;
            set
            {
                m_location = value;
                if (this.Parent != null)
                {
                    var instanceContainer = (RenderInstanceContainer)this.Parent;
                    instanceContainer.UpdateInstanceMatrix(this.InstanceID, this.GetModelViewMatrix().ToArray());
                }
            }
        }

        public override Vec3 Rotation
        {
            get => m_rotation;
            set
            {
                m_rotation = value;
                if (this.Parent != null)
                {
                    var instanceContainer = (RenderInstanceContainer)this.Parent;
                    instanceContainer.UpdateInstanceMatrix(this.InstanceID, this.GetModelViewMatrix().ToArray());
                }
            }
        }

        public override Vec3 Size
        {
            get => m_size;
            set
            {
                m_size = value;
                if (this.Parent != null)
                {
                    var instanceContainer =(RenderInstanceContainer) this.Parent;
                    instanceContainer.UpdateInstanceMatrix(this.InstanceID, this.GetModelViewMatrix().ToArray());
                }
            }
        }

        public int InstanceID { get; set; }
        public bool Initialized { get; set; }

        public InstancedElement()
        {
        }


        public void UpdatePosition(Vec3 position)
        {
        }

        public mat4 GetModelViewMatrix()
        {
            mat4 mt_mat = mat4.Translate(this.Location.X, this.Location.Y, this.Location.Z);
            mat4 mr_mat = mat4.RotateX(Utils.ToRadians(this.Rotation.X)) * mat4.RotateY(Utils.ToRadians(this.Rotation.Y)) * mat4.RotateZ(Utils.ToRadians(this.Rotation.Z));
            mat4 ms_mat = mat4.Scale(this.Size.X, this.Size.Y, this.Size.Z);
            mat4 m_mat = mt_mat * mr_mat * ms_mat;
            return m_mat;
        }
    }
}

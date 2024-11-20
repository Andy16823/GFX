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
            mat4 mr_mat = mat4.RotateX(this.Rotation.X) * mat4.RotateY(this.Rotation.Y) * mat4.RotateZ(this.Rotation.Z);
            mat4 ms_mat = mat4.Scale(this.Size.X, this.Size.Y, this.Size.Z);
            mat4 m_mat = mt_mat * mr_mat * ms_mat;
            return m_mat;
        }
    }
}

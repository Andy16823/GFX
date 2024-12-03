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
    /// <summary>
    /// Represents an instanced element within a scene, providing position, rotation, and size transformations.
    /// </summary>
    public class InstancedElement : GameElement
    {
        private Vec3 m_location = new Vec3();
        private Vec3 m_rotation = new Vec3();
        private Vec3 m_size = new Vec3();

        /// <summary>
        /// Gets or sets the location of the instance in the world space.
        /// Updates the instance matrix in the parent container when changed.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the rotation of the instance in degrees (X, Y, Z).
        /// Updates the instance matrix in the parent container when changed.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the size (scale) of the instance in the world space.
        /// Updates the instance matrix in the parent container when changed.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the unique identifier for the instance within its container.
        /// </summary>
        public int InstanceID { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the instance has been initialized.
        /// </summary>
        public bool Initialized { get; set; }

        /// <summary>
        /// Computes the model-view transformation matrix for the instance.
        /// </summary>
        /// <returns>
        /// A <see cref="mat4"/> representing the combined transformations: translation, rotation, and scaling.
        /// </returns>
        public mat4 GetModelViewMatrix()
        {
            quat quat = new quat(new vec3(Utils.ToRadians(this.Rotation.X), Utils.ToRadians(this.Rotation.Y), Utils.ToRadians(this.Rotation.Z)));
            mat4 mt_mat = mat4.Translate(this.Location.X, this.Location.Y, this.Location.Z);
            mat4 mr_mat = new mat4(quat);
            mat4 ms_mat = mat4.Scale(this.Size.X, this.Size.Y, this.Size.Z);
            mat4 m_mat = mt_mat * mr_mat * ms_mat;
            return m_mat;
        }
    }
}

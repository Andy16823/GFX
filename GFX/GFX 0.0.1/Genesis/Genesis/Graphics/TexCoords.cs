using Genesis.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Graphics
{
    /// <summary>
    /// Values for texture coords
    /// </summary>
    public class TexCoords
    {
        public Vec3 TopLeft { get; set; } = new Vec3(0f, 0f);
        public Vec3 TopRight { get; set; } = new Vec3(1f, 0f);
        public Vec3 BottomRight { get; set; } = new Vec3(1f, 1f);
        public Vec3 BottomLeft { get; set; } = new Vec3(0f, 1f);

        public TexCoords()
        {

        }

        public TexCoords(Vec3 topLeft, Vec3 topRight, Vec3 bottomRight, Vec3 bottomLeft) 
        {
            this.TopLeft = topLeft;
            this.TopRight = topRight;
            this.BottomRight = bottomRight;
            this.BottomLeft = bottomLeft;
        }
    }
}

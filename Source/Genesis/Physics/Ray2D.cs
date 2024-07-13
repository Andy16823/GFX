using Genesis.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Physics
{
    public class Ray2D
    {
        public Vec3 Origin { get; set; }
        public Vec3 Direction { get; set; }

        public Ray2D(Vec3 origin, Vec3 direction)
        {
            this.Origin = origin;
            this.Direction = direction;
        }
    }
}

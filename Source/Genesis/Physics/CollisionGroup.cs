using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Physics
{
    public class CollisionGroup
    {
        public short Group { get; set; }

        public CollisionGroup(short group)
        {
            this.Group = group;
        }
    }
}

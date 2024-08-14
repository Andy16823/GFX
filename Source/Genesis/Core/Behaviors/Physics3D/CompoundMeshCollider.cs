using Genesis.Core.GameElements;
using Genesis.Physics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Core.Behaviors.Physics3D
{
    public class CompoundMeshCollider : ColliderBehavior3D
    {
        public CompoundMeshCollider(PhysicHandler physicHandler) : base(physicHandler)
        {
        }

        public override void CreateCollider(int collisionGroup = -1, int collisionMask = -1)
        {
            var element = (Element3D)this.Parent;
        }
    }
}

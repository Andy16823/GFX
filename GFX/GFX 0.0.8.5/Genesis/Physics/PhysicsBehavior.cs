using Genesis.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Physics
{
    public delegate void PhysicsBehaviorEvent(Scene scene, Game game, object collision);

    public abstract class PhysicsBehavior : IGameBehavior
    {
        public PhysicsBehaviorEvent OnCollide;

        public virtual void Collide(Scene scene, Game game, BulletSharp.RigidBody collisionObject)
        {
            if(this.OnCollide != null)
            {
                this.OnCollide(scene, game, collisionObject);
            }
        }

        public abstract object GetPhysicsObject();

        public abstract T GetPhysicsObject<T>();
    }
}

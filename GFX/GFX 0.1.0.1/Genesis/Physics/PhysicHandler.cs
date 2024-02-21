using Genesis.Core;
using Genesis.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Physics
{
    public delegate void PhysicHandlerEvent(Scene scene, Game game, object element);

    public struct PhysicPropeterys
    {
        public float gravityX;
        public float gravityY;
        public float gravityZ;
    }

    public abstract class PhysicHandler
    {
        public PhysicHandler()
        {
            this.Callbacks = new Dictionary<object, PhysicHandlerEvent>();
        }

        public Dictionary<object, PhysicHandlerEvent>  Callbacks { get; set; }

        public abstract void Process(Scene scene, Game game);

        public virtual void ManageElement(PhysicsBehavior physicsBehavior)
        {
            this.Callbacks.Add(physicsBehavior.GetPhysicsObject(), (scene, game, cObj) =>
            {
                physicsBehavior.Collide(scene, game, (BulletSharp.RigidBody)cObj);
            });
        }
    }
}

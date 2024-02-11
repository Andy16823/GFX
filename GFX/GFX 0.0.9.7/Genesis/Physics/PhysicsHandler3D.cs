using BulletSharp;
using BulletSharp.Math;
using Genesis.Core;
using OpenObjectLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Physics
{
    public class PhysicsHandler3D : PhysicHandler
    {
        public DiscreteDynamicsWorld PhysicsWorld { get; set; }
        public bool ProcessPhysics { get; set; } = true;


        public PhysicsHandler3D(PhysicPropeterys propeterys)
        {
            var CollisionConfiguration = new DefaultCollisionConfiguration();
            var Dispatcher = new CollisionDispatcher(CollisionConfiguration);
            var Broadphase = new DbvtBroadphase();
            this.PhysicsWorld = new DiscreteDynamicsWorld(Dispatcher, Broadphase, null, CollisionConfiguration);
            this.PhysicsWorld.Gravity = new BulletSharp.Math.Vector3(propeterys.gravityX, propeterys.gravityY, propeterys.gravityZ);
        }

        public PhysicsHandler3D(float gravityX, float gravityY, float gravityZ)
        {
            var CollisionConfiguration = new DefaultCollisionConfiguration();
            var Dispatcher = new CollisionDispatcher(CollisionConfiguration);
            var Broadphase = new DbvtBroadphase();
            this.PhysicsWorld = new DiscreteDynamicsWorld(Dispatcher, Broadphase, null, CollisionConfiguration);
            this.PhysicsWorld.Gravity = new BulletSharp.Math.Vector3(gravityX, gravityY, gravityZ);
        }

        public override void Process(Scene scene, Game game)
        {
            if (this.ProcessPhysics && this.PhysicsWorld != null)
            {
                this.PhysicsWorld.StepSimulation((float)(game.DeltaTime / 1000));

                int numManifolds = PhysicsWorld.Dispatcher.NumManifolds;
                for (int i = 0; i < numManifolds; i++)
                {
                    PersistentManifold contactManifold = PhysicsWorld.Dispatcher.GetManifoldByIndexInternal(i);
                    CollisionObject obA = contactManifold.Body0 as CollisionObject;
                    CollisionObject obB = contactManifold.Body1 as CollisionObject;

                    if(Callbacks.ContainsKey(obA))
                    {
                        Callbacks[obA](scene, game, obB);
                    }
                }
            }
        }

        public override void ManageElement(PhysicsBehavior rigidBody)
        {
            base.ManageElement(rigidBody);
            PhysicsWorld.AddRigidBody((RigidBody) rigidBody.GetPhysicsObject());
        }
    }
}

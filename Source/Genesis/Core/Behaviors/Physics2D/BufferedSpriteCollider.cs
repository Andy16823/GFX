using BulletSharp;
using BulletSharp.Math;
using Genesis.Core.GameElements;
using Genesis.Graphics.Shapes;
using Genesis.Math;
using Genesis.Physics;
using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Core.Behaviors.Physics2D
{
    /// <summary>
    /// Represents a behavior for creating a physics collider for 2D sprites using BulletSharp.
    /// </summary>
    public class BufferedSpriteCollider : Physics.PhysicsBehavior
    {
        /// <summary>
        /// Gets or sets the rigid body associated with the collider.
        /// </summary>
        public RigidBody RigidBody { get; set; }

        /// <summary>
        /// Gets or sets the linear factor for the RigidBody's motion.
        /// </summary>
        public Vec3 LinearFactor { get; set; } = new Vec3(0, 0, 0);

        /// <summary>
        /// Gets or sets the angular factor for the RigidBody's rotation.
        /// </summary>
        public Vec3 AngularFactor { get; set; } = new Vec3(0, 0, 0);

        /// <summary>
        /// Gets the physics object associated with the collider.
        /// </summary>
        /// <returns>The rigid body associated with the collider.</returns>
        public override object GetPhysicsObject()
        {
            return RigidBody;
        }

        /// <summary>
        /// Gets the typed physics object associated with the collider.
        /// </summary>
        /// <typeparam name="T">Type of the physics object.</typeparam>
        /// <returns>The typed rigid body associated with the collider.</returns>
        public override T GetPhysicsObject<T>()
        {
            return (T)(object)RigidBody;
        }

        /// <summary>
        /// Creates a rigid body for the collider.
        /// </summary>
        /// <param name="mass">The mass of the rigid body.</param>
        /// <param name="handler">The physics handler for managing the collider.</param>
        public void CreateRigidBody(float mass, PhysicHandler handler)
        {
            if (this.Parent.GetType() == typeof(BufferedSprite))
            {
                var bufferedSprite = (BufferedSprite)this.Parent;
                CompoundShape compoundShape = new CompoundShape(true);

                foreach(var deffinition in bufferedSprite.ShapeDeffinitions)
                {
                    Box2DShape box2DShape = new Box2DShape(new Vector3(deffinition.sizeX, deffinition.sizeY, 0f) / 2);
                    BulletSharp.Math.Matrix boxtransform = BulletSharp.Math.Matrix.Translation(new Vector3(deffinition.locX, deffinition.locY, 0f));
                    compoundShape.AddChildShape(boxtransform, box2DShape);
                }
                RigidBodyConstructionInfo info = new RigidBodyConstructionInfo(mass, null, compoundShape, compoundShape.CalculateLocalInertia(mass));
                BulletSharp.Math.Matrix transform = BulletSharp.Math.Matrix.Translation(bufferedSprite.Location.ToBulletVec3());
                Matrix startTransform = transform;
                info.MotionState = new DefaultMotionState(startTransform);

                RigidBody = new BulletSharp.RigidBody(info);
                RigidBody.UserObject = this.Parent;
                RigidBody.LinearFactor = this.LinearFactor.ToBulletVec3();
                RigidBody.AngularFactor = this.AngularFactor.ToBulletVec3();
                //RigidBody.CollisionFlags = CollisionFlags.StaticObject;

                handler.ManageElement(this);
            }
            else
            {
                throw new InvalidOperationException("Invalid element for this Behavior");
            }
        }

        /// <summary>
        /// Placeholder for OnDestroy event. No implementation.
        /// </summary>
        public override void OnDestroy(Game game, GameElement parent)
        {
            
        }

        /// <summary>
        /// Placeholder for OnInit event. No implementation.
        /// </summary>
        public override void OnInit(Game game, GameElement parent)
        {
            
        }

        /// <summary>
        /// Placeholder for OnRender event. No implementation.
        /// </summary>
        public override void OnRender(Game game, GameElement parent)
        {
            
        }

        /// <summary>
        /// Placeholder for OnUpdate event. No implementation.
        /// </summary>
        public override void OnUpdate(Game game, GameElement parent)
        {
            Matrix transform = BulletSharp.Math.Matrix.Translation(Parent.Location.ToBulletVec3());
            RigidBody.MotionState = new DefaultMotionState(transform);
            RigidBody.Activate(true);
        }
    }
}

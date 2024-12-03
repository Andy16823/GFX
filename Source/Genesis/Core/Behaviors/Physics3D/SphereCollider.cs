using BulletSharp;
using Genesis.Math;
using Genesis.Physics;
using GlmSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Core.Behaviors.Physics3D
{
    /// <summary>
    /// Represents a Sphere Collider behavior for 3D physics.
    /// </summary>
    public class SphereCollider : ColliderBehavior3D
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SphereCollider"/> class with the specified physics handler.
        /// </summary>
        /// <param name="physicHandler">The physics handler to associate with this sphere collider.</param>
        public SphereCollider(PhysicHandler physicHandler) : base(physicHandler)
        {
        }

        /// <summary>
        /// Creates a collider with a sphere shape using the default radius (half of the parent's size).
        /// </summary>
        /// <param name="physicHandler">The physics handler to manage this element.</param>
        public override void CreateCollider(int collisionGroup = -1, int collisionMask = -1)
        {
            this.CreateCollider(this.Parent.Size.X / 2, collisionGroup, collisionMask);
        }

        /// <summary>
        /// Creates a collider with a sphere shape at the origin.
        /// </summary>
        /// <param name="handler">The physics handler to manage this element.</param>
        /// <param name="radius">The radius of the sphere.</param>
        public void CreateCollider(float radius, int collisionGroup = -1, int collisionMask = -1)
        {
            this.CreateCollider(Vec3.Zero(), radius, collisionGroup, collisionMask);
        }

        /// <summary>
        /// Creates a collider with a sphere shape at the specified offset.
        /// </summary>
        /// <param name="handler">The physics handler to manage this element.</param>
        /// <param name="offset">The offset from the parent element's location.</param>
        /// <param name="radius">The radius of the sphere.</param>
        public void CreateCollider(Vec3 offset, float radius, int collisionGroup = -1, int collisionMask = -1)
        {
            this.Offset = offset;

            var element = this.Parent;
            SphereShape sphereShape = new SphereShape(radius);
            var btStartTransform = Utils.GetBtTransform(element, Offset);

            Collider = new CollisionObject();
            Collider.UserObject = element;
            Collider.CollisionShape = sphereShape;
            Collider.WorldTransform = btStartTransform;
            PhysicHandler.ManageElement(this, collisionGroup, collisionMask);
        }
    }
}

using BulletSharp;
using Genesis.Core.GameElements;
using Genesis.Math;
using Genesis.Physics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Core.Behaviors.Physics3D
{
    /// <summary>
    /// Represents a 3D convex hull collider behavior for physics interactions.
    /// </summary>
    /// <remarks>
    /// Provides functionality to create and manage a convex hull-shaped collider for detecting collisions in a 3D physics world.
    /// </remarks>
    public class ConvexHullCollider : ColliderBehavior3D
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConvexHullCollider"/> class with the specified physics handler.
        /// </summary>
        /// <param name="physicHandler">The physics handler to associate with this convex hull collider.</param>
        public ConvexHullCollider(PhysicHandler physicHandler) : base(physicHandler)
        {
        }

        /// <summary>
        /// Creates the convex hull collider based on the parent's shape.
        /// </summary>
        public override void CreateCollider() 
        {
            if (this.Parent.GetType() == typeof(Element3D))
            {
                //Create the shape 
                Element3D element = (Element3D)this.Parent;
                ConvexHullShape convexHull = new ConvexHullShape(element.GetShape());

                //Create the start matrix
                Vec3 location = Utils.GetElementWorldLocation(element);
                Vec3 rotation = Utils.GetElementWorldRotation(element);
                Vec3 scale = Utils.GetElementWorldScale(element);

                BulletSharp.Math.Matrix btworldTransform = Utils.BuildPhysicsMatrix(location.ToBulletVec3(), rotation.ToBulletVec3(), scale.ToBulletVec3());

                this.Collider = new CollisionObject();
                this.Collider.CollisionShape = convexHull;
                this.Collider.UserObject = this.Parent;
                this.Collider.WorldTransform = btworldTransform;
                this.Collider.CollisionShape.LocalScaling = scale.ToBulletVec3();

                PhysicHandler.ManageElement(this);
            }
            else
            {
                throw new InvalidOperationException("Invalid element for this Behavior");
            }
        }
    }
}

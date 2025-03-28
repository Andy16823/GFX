﻿using BulletSharp;
using BulletSharp.SoftBody;
using Genesis.Math;
using Genesis.Physics;
using GlmSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Core.Behaviors.Physics3D
{
    /// <summary>
    /// Defines a box collider behavior for 3D physics simulations.
    /// </summary>

    public class BoxCollider : ColliderBehavior3D
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BoxCollider"/> class with the specified physics handler.
        /// </summary>
        /// <param name="physicHandler">The physics handler to associate with this box collider.</param>
        public BoxCollider(PhysicHandler physicHandler) : base(physicHandler)
        {
        }

        /// <summary>
        /// Creates a box collider with default half extends.
        /// </summary>
        /// <param name="handler">The physics handler to manage this collider.</param>
        public override void CreateCollider(int collisionGroup = -1, int collisionMask = -1)
        {
            this.CreateCollider(new Vec3(0.5f, 0.5f, 0.5f), collisionGroup, collisionMask);
        }

        /// <summary>
        /// Creates a box collider with specified half extends.
        /// </summary>
        /// <param name="boxHalfExtends">Half extends of the box collider.</param>
        public void CreateCollider(Vec3 boxHalfExtends, int collisionGroup = -1, int collisionMask = -1)
        {
            var element = this.Parent;
            BoxShape boxShape = new BoxShape(boxHalfExtends.ToBulletVec3());
            var btStartTransform = Utils.GetBtTransform(element, Offset);

            Collider = new CollisionObject();
            Collider.UserObject = element;
            Collider.CollisionShape = boxShape;
            Collider.WorldTransform = btStartTransform;
            Collider.CollisionShape.LocalScaling = element.Size.ToBulletVec3();

            PhysicHandler.ManageElement(this, collisionGroup, collisionMask);
        }
    }
}
